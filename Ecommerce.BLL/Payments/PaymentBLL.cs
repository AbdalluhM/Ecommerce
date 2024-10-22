using AutoMapper;
using Dexef.Payment.FawryAPI;
using Dexef.Payment.Models;
using Dexef.Payment.Models.PAYFORT;
using Dexef.Payment.PAYFORTAPI;
using Dexef.Payment.PayPal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Customers.Invoices;
using Ecommerce.BLL.Responses;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums.Admins;
using Ecommerce.Core.Enums.Invoices;
using Ecommerce.Core.Enums.Json;
using Ecommerce.Core.Helpers.JsonModels.Payments;
using Ecommerce.Core.Infrastructure;
using Ecommerce.DTO.Customers.Cards;
using Ecommerce.DTO.Customers.Invoices;
using Ecommerce.DTO.Customers.Invoices.Outputs;
using Ecommerce.DTO.Settings.EmailTemplates;
using Ecommerce.Helper.String;
using Ecommerce.Repositroy.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;
using static Ecommerce.DTO.Customers.CustomerDto;
using static Ecommerce.DTO.PaymentSetup.PaymentSetupDto;
using PaymentMethod = Ecommerce.Core.Entities.PaymentMethod;

namespace Ecommerce.BLL.Payments
{
    public class PaymentBLL : BaseBLL, IPaymentBLL
    {
        #region Field

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentSetupBLL _paymentSetupBLL;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerCard> _customerCardRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<PaymentMethod> _paymentMethodRepository;
        private readonly IInvoiceHelperBLL _invoiceHelper;
        private readonly EmailTemplateSetting _emailTemplateSetting;
        private readonly PaymentGateWaySettings _paymentGateWaySettings;
        #endregion
        #region Constructor

        public PaymentBLL(IMapper mapper,
            IUnitOfWork unitOfWork,
            IPaymentSetupBLL paymentSetupBLL,
            IRepository<Customer> customerRepository,
            IRepository<CustomerCard> customerCardRepository,
            IRepository<Invoice> invoiceRepository,
            IRepository<PaymentMethod> paymentMethodRepository,
            IInvoiceHelperBLL invoiceHelper,
            IOptions<EmailTemplateSetting> emailTemplateSetting,
            IOptions<PaymentGateWaySettings> paymentGateWaySettings) : base(mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _paymentSetupBLL = paymentSetupBLL;
            _customerRepository = customerRepository;
            _customerCardRepository = customerCardRepository;
            _invoiceRepository = invoiceRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _invoiceHelper = invoiceHelper;
            _emailTemplateSetting = emailTemplateSetting.Value;
            _paymentGateWaySettings = paymentGateWaySettings.Value;
        }
        #endregion
        #region Payments   
        public async Task<InvoicePaymentInfoJson> Pay(ThirdPartyInvoiceInputDto paymentData, InvoiceDto invoice)
        {
            try
            {


                return paymentData.PaymentType.Id switch
                {

                    (int)PaymentTypesEnum.PayPal => await PayWithPayPal(paymentData, invoice),
                    (int)PaymentTypesEnum.Fawry => await PayWithFawry(paymentData, invoice),
                    (int)PaymentTypesEnum.Bank => await PayWithBank(paymentData, invoice),
                    (int)PaymentTypesEnum.PayMob => await PayWithPayMob(paymentData, invoice),

                    _ => throw new ArgumentOutOfRangeException(nameof(paymentData.PaymentMethodId), paymentData.PaymentMethodId, null)
                };
            }
            catch (Exception ex)
            {
                invoice.InvoiceStatusId = (int)InvoiceStatusEnum.Unpaid;
                return new InvoicePaymentInfoJson();
            }
        }
        public async Task<GetPaymentMethodsOutputDto> GetCustomerDefaultPaymentMethodAsnyc(int customerId)
        {
            var paymentMethodId = _customerRepository.GetById(customerId)?.CustomerCards?.FirstOrDefault(x => x.IsDefault)?.PaymentMethodId;
            var defaultPaymentMethod = await _paymentSetupBLL.GetPaymentMethodById(paymentMethodId.GetValueOrDefault(0));
            return defaultPaymentMethod;
        }
        public async Task<int> GetPaymentMethodId(int customerId)
        {
            var paymentMethod = await GetCustomerDefaultPaymentMethodAsnyc(customerId);
            //ToDo:Make PaymentMethodId Nullable in Invoice 
            return paymentMethod != null ? paymentMethod.Id : _paymentMethodRepository.GetAll().FirstOrDefault().Id;
        }

        public APIPayAndSubscribeInputDto MapedPaymentData(PaymentDetailsInputDto inputDto, APIPayAndSubscribeInputDto paymentDetails)
        {
            paymentDetails.InvoiceTypeId = inputDto.AdminInvoiceTypeId switch
            {
                (int)AdminInvoiceTypesEnum.Support => (int)InvoiceTypes.Support,
                var n when n == (int)AdminInvoiceTypesEnum.Product && inputDto.Discriminator == (int)DiscriminatorsEnum.Forever => (int)InvoiceTypes.ForeverSubscription,
                _ => (int)InvoiceTypes.Renewal
            };

            if (inputDto.AdminInvoiceTypeId == (int)AdminInvoiceTypesEnum.Support)
                paymentDetails.AddOnSubscriptionId = inputDto.AddOnSubscriptionId;

            paymentDetails.InvoiceStatusId = (int)InvoiceStatusEnum.Draft;
            paymentDetails.IsUnPaid = true;
            paymentDetails.StartDate = inputDto.StartDate;
            paymentDetails.EndDate = inputDto.EndDate;
            paymentDetails.Price = inputDto.Price;
            paymentDetails.PriceAfterDiscount = inputDto.Price;
            paymentDetails.TotalDiscountAmount = 0M;
            paymentDetails.InvoiceTitle = inputDto.InvoiceTitle;
            paymentDetails.CustomerId = inputDto.CustomerId;
            paymentDetails.VersionSubscriptionId = inputDto.VersionSubscriptionId;
            paymentDetails.CustomerSubscriptionId = inputDto.CustomerSubscriptionId;
            paymentDetails.Notes = inputDto.Notes;
            paymentDetails.IsAdminInvoice = inputDto.IsAdminInvoice;
            paymentDetails.InvoiceSerial = inputDto.InvoiceSerial;
            return paymentDetails;
        }
        public async Task<InvoicePaymentInfoJson> PayInvoiceAsync(PaymentDto payDto)
        {
            var Customer = _mapper.Map<GetCustomerOutputDto>(payDto.Invoice.CustomerSubscription.Customer);
            var mappedInvoice = _mapper.Map<InvoiceDto>(payDto.Invoice);
            //get Customer PaymentMethod to Update Payment Method and Type to be from Customer not Invoice

            var paymentMethod = await GetCustomerDefaultPaymentMethodAsnyc(Customer.Id);

            //var customerDefaultCurrency = payDto.Invoice.CustomerSubscription.Customer.Country.CountryCurrency.Currency.Code;
            var invoiceCurrency = payDto.Invoice.CustomerSubscription.CurrencyName;


            var paymentData = new ThirdPartyInvoiceInputDto
            {
                CurrencyCode = invoiceCurrency,
                Customer = Customer,
                PaymentMethodId = paymentMethod.Id,//payDto.Invoice.PaymentMethodId,
                PaymentType = paymentMethod.PaymentType,// _mapper.Map<PaymentTypeDto>(payDto.Invoice.PaymentMethod.PaymentType)
            };
            //validate via invoice payment method or customer default payment method
            //if ((int)PaymentTypesEnum.PayPal == payDto.Invoice.PaymentMethod.PaymentTypeId)
            if ((int)PaymentTypesEnum.PayPal == paymentMethod.PaymentType.Id)
            {
                paymentData.SuccessCallBackUrl = paymentData.FailCallBackUrl = _emailTemplateSetting.ApiBaseUrl + "/api/invoices/CapturePayPalOrder";

            }
            else if ((int)PaymentTypesEnum.Fawry == payDto.Invoice.PaymentMethod.PaymentTypeId)
            {
                paymentData.IsFawryCard = true;
                paymentData.CardToken = Customer.CustomerCards.FirstOrDefault(x => x.IsDefault)?.CardToken ?? string.Empty;
                paymentData.SuccessCallBackUrl = _emailTemplateSetting.ApiBaseUrl + "/api/invoices/Fawry/CallBackThreeDSecureUrl";
            }
            else //amazon // set callback=""
            {

            }


            var result = payDto.Invoice.PaymentMethod.PaymentTypeId switch
            {

                (int)PaymentTypesEnum.PayPal => await PayWithPayPal(paymentData, mappedInvoice),
                (int)PaymentTypesEnum.Fawry => await PayWithFawry(paymentData, mappedInvoice),
                (int)PaymentTypesEnum.Bank => await PayWithBank(paymentData, mappedInvoice),
                _ => throw new ArgumentOutOfRangeException(nameof(payDto.Invoice.PaymentMethodId), payDto.Invoice.PaymentMethodId, null)
            };
            _invoiceHelper.UpdateInvoicePaymentInfoForSearch(payDto.Invoice, result, true);
            return result;
        }

        public async Task<IResponse<string>> TryToPay(APIPayAndSubscribeInputDto inputDto, CustomerSubscription customerSubscription, Invoice invoice = null, bool commit = false)
        {
            var output = new Response<string>();
            bool isFirstInvoice = inputDto.CustomerSubscriptionId == 0;
            InvoiceDto invoiceDto = _mapper.Map<InvoiceDto>(invoice);
            //get payment type
            inputDto.PaymentType = await _paymentSetupBLL.GetPaymentTypeByPaymentMethod(inputDto.PaymentMethodId);

            if (!inputDto.IsUnPaid)
            {
                var paymentDto = _mapper.Map<ThirdPartyInvoiceInputDto>(inputDto);
                //paymentDto.CurrencyCode = customerSubscription.Customer.Country?.CountryCurrency?.Currency?.Code;
                paymentDto.CurrencyCode = customerSubscription.CurrencyName;
                invoiceDto = _mapper.Map<InvoiceDto>(inputDto);

                //pay invoice
                var paymentResult = await Pay(paymentDto, invoiceDto);
                invoice.PaymentMethodId = paymentDto.PaymentMethodId;
                //RollBack Subscription if payment failed
                if (isFirstInvoice && !paymentResult.IsSuccess)
                {
                    _invoiceHelper.RollBackSubscription(invoice.CustomerSubscriptionId);
                    return output.CreateResponse(MessageCodes.PaymentFailed);
                }
                else
                {
                    //Update Invoice Serial & Payment Info
                    inputDto.PaymentInfo = paymentResult.Serialize();
                    _invoiceHelper.UpdateInvoicePaymentInfoForSearch(invoice, paymentResult, false);

                }
            }


            //  UpdateInvoiceSerial(invoice, contract, false);
            if (commit)
                _unitOfWork.Commit();

            return output.CreateResponse(inputDto.PaymentInfo);
        }

        #region PayPal
        public async Task<InvoicePaymentInfoJson> PayWithPayPal(ThirdPartyInvoiceInputDto paymentData, InvoiceDto invoice)
        {

            //Convert Invoice Currency with Exchange Rate
            var paypalDefaultCurrencyCode = _paymentGateWaySettings.PayPal.BaseCurrency;
            var customerCurrencyCode = paymentData?.CurrencyCode ?? "";
            if (customerCurrencyCode != paypalDefaultCurrencyCode)
                invoice = await _invoiceHelper.UpdateInvoiceTotalsWithConvertedCurrencyAsync(invoice, customerCurrencyCode, paypalDefaultCurrencyCode);
            var merchantReference = invoice.Id.ConvertIntToGUIDString();

            var output = new InvoicePaymentInfoJson();
            try
            {
                var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.PayPal);
                PayPalClient client = new PayPalClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);

                var order = new OrderRequest()
                {
                    CheckoutPaymentIntent = "CAPTURE",
                    //Payer = new Payer
                    //{
                    //    TaxInfo = new TaxInfo
                    //    {
                    //        TaxId = paymentData.Customer.TaxRegistrationNumber,
                    //        TaxIdType = paymentData.Customer.Industry.Name
                    //    },
                    //    Email = paymentData.Customer.Email,
                    //    Name = new Name
                    //    {
                    //        FullName = paymentData.Customer.Name
                    //    },
                    //    AddressPortable = new AddressPortable
                    //    {
                    //        PostalCode = paymentData.Customer.PostalCode,
                    //        AddressLine1 = paymentData.Customer.FullAddress,
                    //    }

                    //},
                    PurchaseUnits = new List<PurchaseUnitRequest>()
                {

                    new PurchaseUnitRequest()
                    {
                        InvoiceId = merchantReference,
                        ReferenceId = merchantReference,
                        AmountWithBreakdown = new AmountWithBreakdown {

                            CurrencyCode = paypalDefaultCurrencyCode,
                            Value = Math.Round(invoice.Total , 2).ToString()
                        },
                            Description = "Invoice Serial : " + invoice.Serial  ?? string.Empty


                    }
                },
                    ApplicationContext = new ApplicationContext()
                    {
                        ReturnUrl = paymentData.SuccessCallBackUrl,
                        CancelUrl = paymentData.FailCallBackUrl
                    }
                };
                var orderResponse = await client.CreateOrder(order);
                invoice.PaymentInfo = orderResponse.Serialize();
                if (orderResponse.IsSuccess)
                    output = new InvoicePaymentInfoJson
                    {
                        IsSuccess = true,
                        Paypal = new Paypal
                        {
                            Account = paymentData.Customer.Email,
                            ApproveOrderLink = await client.GetOrderLink(orderResponse.Data, "approve"),
                            CaptureOrderLink = await client.GetOrderLink(orderResponse.Data, "capture"),
                            UpdateOrderLink = await client.GetOrderLink(orderResponse.Data, "update"),
                            GetOrderLink = await client.GetOrderLink(orderResponse.Data, "self"),
                        },
                        Bank = null,
                        Fawry = null
                    };

                return output;
            }
            catch (Exception e)
            {
                return output;

            }
        }

        public async Task<bool> UpdatePaypalCaptureId(int referenceId, string captureId)
        {
            try
            {

                if (referenceId <= 0)
                    return false;

                var invoice = await _invoiceRepository.GetByIdAsync(referenceId);
                if (invoice == null)
                    return false;
                var mappedResult = _mapper.Map<RetrieveInvoiceDto>(invoice);
                mappedResult.PaymentInfo.Paypal.CaptureId = captureId;
                invoice.PaymentInfo = mappedResult.PaymentInfo.Serialize();
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> UpdateBankFortIdAndCardData(int referenceId, string fortId, string paymentOption, string cardNumber)
        {
            try
            {

                if (referenceId <= 0)
                    return false;

                var invoice = await _invoiceRepository.GetByIdAsync(referenceId);
                if (invoice == null)
                    return false;
                var mappedResult = _mapper.Map<RetrieveInvoiceDto>(invoice);
                mappedResult.PaymentInfo.Bank.FortId = fortId;
                mappedResult.PaymentInfo.Bank.CardNumber = cardNumber;
                mappedResult.PaymentInfo.Bank.CardTypeId = paymentOption switch
                {
                    nameof(CardTypesEnum.MasterCard) => (int)CardTypesEnum.MasterCard,
                    nameof(CardTypesEnum.Visa) => (int)CardTypesEnum.Visa,
                    _ => (int)CardTypesEnum.Others,
                };

                invoice.PaymentInfo = mappedResult.PaymentInfo.Serialize();
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> UpdateFawryCardReferenceCodeAndCardData(int referenceId, string fawryReferenceId, string cardNumber)
        {
            try
            {

                if (referenceId <= 0)
                    return false;

                var invoice = await _invoiceRepository.GetByIdAsync(referenceId);
                if (invoice == null)
                    return false;
                var mappedResult = _mapper.Map<RetrieveInvoiceDto>(invoice);
                mappedResult.PaymentInfo.Fawry.ReferenceNumber = fawryReferenceId;
                mappedResult.PaymentInfo.Fawry.CardNumber = cardNumber;

                invoice.PaymentInfo = mappedResult.PaymentInfo.Serialize();
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> CallBackConfirmPayPalOrder(string token, string payerId)
        {
            //var output = new Response<bool>();
            var output = new InvoicePaymentInfoJson();

            try
            {
                var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.PayPal);
                PayPalClient client = new PayPalClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);
                try
                {
                    var result = client.CaptureOrder(token, payerId).GetAwaiter().GetResult();
                    if (result != null && result.IsSuccess)
                    {
                        int.TryParse(result?.Data?.PurchaseUnits?.FirstOrDefault()?.ReferenceId?.GetIntFromGUIDString(), out int invoiceId);
                        await UpdatePaypalCaptureId(invoiceId, result?.Data?.PurchaseUnits?.FirstOrDefault()?.Payments?.Captures?.FirstOrDefault()?.Id);

                        await _invoiceHelper.UpdateInvoiceStatusWithPaymentResponseFromPaymentCallBackAsync(invoiceId, PaymentTypesEnum.PayPal, result.IsSuccess ? (int)InvoiceStatusEnum.Paid : (int)InvoiceStatusEnum.Unpaid, JsonConvert.SerializeObject(result), true);
                        return true;

                    }
                    return false;

                }
                catch (Exception ex)
                {
                    return false;

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<RefundPaymentResponseDto> RefundPayPal(Invoice invoice)
        {
            var refundResultDto = new RefundPaymentResponseDto();

            if (invoice.PaymentMethod.PaymentTypeId != (int)PaymentTypesEnum.PayPal)
                return refundResultDto;

            try
            {
                var paypalDefaultCurrencyCode = _paymentGateWaySettings.PayPal.BaseCurrency;

                var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.PayPal);
                PayPalClient client = new PayPalClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);
                try
                {
                    var paymentInfoModel = !string.IsNullOrWhiteSpace(invoice.PaymentInfo) ? JsonConvert.DeserializeObject<InvoicePaymentInfoJson>(invoice.PaymentInfo) : new InvoicePaymentInfoJson();

                    string captureId = paymentInfoModel.Paypal?.CaptureId ?? string.Empty;
                    PayPalCheckoutSdk.Orders.Order orderResponseModel = new PayPalCheckoutSdk.Orders.Order();
                    if (!string.IsNullOrWhiteSpace(invoice.PaymentResponse))
                    {
                        var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse<PayPalCheckoutSdk.Orders.Order>>(invoice.PaymentResponse);
                        orderResponseModel = paymentResponse.Data;
                        var grossAmount = orderResponseModel.PurchaseUnits?.FirstOrDefault()?.Payments?.Captures?.FirstOrDefault()?.SellerReceivableBreakdown?.GrossAmount ?? new Money { Value = "0", CurrencyCode = paypalDefaultCurrencyCode };
                        var result = await client.RefundOrder(captureId, invoice.Id.ToString(), grossAmount.Value, grossAmount.CurrencyCode, invoice.CancelReason);
                        refundResultDto.IsSuccess = result.IsSuccess;
                        refundResultDto.Result = JsonConvert.SerializeObject(result);
                        return refundResultDto;
                    }
                    else
                    {
                        refundResultDto.IsSuccess = false;
                        refundResultDto.Result = JsonConvert.SerializeObject("No Order Data Found");
                        return refundResultDto;

                    }


                }
                catch (Exception ex)
                {
                    refundResultDto.Result = ex.ToString();
                    return refundResultDto;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
        #region Fawry
        public async Task<InvoicePaymentInfoJson> PayWithFawry(ThirdPartyInvoiceInputDto paymentData, InvoiceDto invoice)
        {
            //TODO:Check to remove or not
            //Convert Invoice Currency with Exchange Rate
            var fawryDefaultCurrencyCode = _paymentGateWaySettings.Fawry.BaseCurrency;
            var customerCurrencyCode = paymentData?.CurrencyCode ?? "";
            if (customerCurrencyCode != fawryDefaultCurrencyCode)
                invoice = await _invoiceHelper.UpdateInvoiceTotalsWithConvertedCurrencyAsync(invoice, customerCurrencyCode, fawryDefaultCurrencyCode);

            var output = new InvoicePaymentInfoJson();
            try
            {
                var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);
                FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey, paymentData.SuccessCallBackUrl ?? string.Empty);

                var merchantReference = invoice.Id.ConvertIntToGUIDString();
                //for fawry reference code
                if (!paymentData.IsFawryCard)
                {

                    var chargeResponse = await fawryClient.PayAsync(merchantReference, new CustomerInfo(paymentData.Customer.Id + "", paymentData.Customer.Phone, paymentData.Customer.Email),
                   new PaymentInfo(Dexef.Payment.FawryAPI.PaymentMethod.PayAtFawry, Convert.ToDouble(invoice.Total), invoice.Notes ?? string.Empty, paymentExpiryDateTime:
                  //TODO:Make
                  DateTime.Now.AddDays(_paymentGateWaySettings.Fawry.PaymentExpirationDays))
                   { CurrencyCode = paymentData.CurrencyCode },
                   new[]{
                         new ChargeItem
                          {
                            Description = "Invoice Serial : " + invoice.Serial  ?? string.Empty ,
                            Price = (float)invoice.Total,
                            ItemId = invoice.Details.FirstOrDefault()?.Id ?? 0,
                            Quantity =1
                         }
                   });
                    invoice.PaymentInfo = chargeResponse.Serialize();
                    if (chargeResponse.IsSuccess)
                        output = new InvoicePaymentInfoJson
                        {
                            IsSuccess = true,
                            Fawry = new Fawry
                            {
                                IsFawryCard = false,
                                Expiration = chargeResponse.ExpirationDateTime,
                                ReferenceNumber = chargeResponse.ReferenceNumber,
                                GeneratedMerchantNumber = merchantReference
                            },
                            Bank = null,
                            Paypal = null
                        };
                    var jObject = JObject.Parse(invoice.PaymentInfo);
                    var referenceNumber = jObject["ReferenceNumber"].ToString();

                    output.IsSuccess = true;
                    output.Fawry.ReferenceNumber = referenceNumber;
                    output.Fawry.Expiration = Convert.ToDateTime(jObject["ExpirationDateTime"].ToString());
                    ////TOD:Remove this after implement getnotification call back from fawry

                }
                //for fawry credit card
                else
                {
                    var chargeResponse = await fawryClient.PayWithCardAsync(merchantReference, new CustomerInfo(paymentData.Customer.Id + "", paymentData.Customer.Phone, paymentData.Customer.Email),
                   new PaymentInfo(Dexef.Payment.FawryAPI.PaymentMethod.Card, Convert.ToDouble(invoice.Total), invoice.Notes ?? string.Empty, cardToken: paymentData.CardToken, paymentExpiryDateTime:
                  //TODO:Make
                  DateTime.Now.AddDays(_paymentGateWaySettings.Fawry.PaymentExpirationDays))
                   { CurrencyCode = paymentData.CurrencyCode },
                   new[]{
                         new ChargeItem
                          {
                            Description = "Invoice Serial : " + invoice.Serial  ?? string.Empty ,
                            Price = (float)invoice.Total,
                            ItemId = invoice.Details.FirstOrDefault()?.Id ?? 0,
                            Quantity =1
                         }
                   });
                    invoice.PaymentInfo = chargeResponse.Serialize();
                    var jObject = JObject.Parse(invoice.PaymentInfo);

                    if (chargeResponse.IsSuccess)
                    {
                        output = new InvoicePaymentInfoJson
                        {
                            IsSuccess = true,
                            Fawry = new Fawry
                            {
                                IsFawryCard = true,
                                ReferenceNumber = !paymentData.IsFawryCard ? jObject["ReferenceNumber"].ToString() : string.Empty,
                                Expiration = !paymentData.IsFawryCard ? Convert.ToDateTime(jObject["ExpirationDateTime"].ToString()) : null,
                                ThreeDSecureUrl = paymentData.IsFawryCard ? (chargeResponse.NextAction?.RedirectUrl ?? string.Empty) : string.Empty,
                                Token = paymentData.IsFawryCard ? paymentData.CardToken : string.Empty,
                                GeneratedMerchantNumber = merchantReference

                            },
                            Bank = null,
                            Paypal = null
                        };
                    }

                }

                return output;
            }
            catch (Exception e)
            {
                //TODO:U could try remove unpaid invoice
                await _invoiceHelper.UpdateInvoiceStatusWithPaymentResponseFromPaymentCallBackAsync(invoice.Id, PaymentTypesEnum.Fawry, (int)InvoiceStatusEnum.Unpaid, paymentResponse: e.Data.Values.Serialize(), true);

                return output;

            }
        }
        public async Task<InvoicePaymentInfoJson> PayWithPayMob(ThirdPartyInvoiceInputDto paymentData, InvoiceDto invoice)
        {
            //TODO:Check to remove or not
            //Convert Invoice Currency with Exchange Rate
            var fawryDefaultCurrencyCode = _paymentGateWaySettings.Fawry.BaseCurrency;
            var customerCurrencyCode = paymentData?.CurrencyCode ?? "";
            if (customerCurrencyCode != fawryDefaultCurrencyCode)
                invoice = await _invoiceHelper.UpdateInvoiceTotalsWithConvertedCurrencyAsync(invoice, customerCurrencyCode, fawryDefaultCurrencyCode);

            var output = new InvoicePaymentInfoJson();
            try
            {
                var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);
                FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey, paymentData.SuccessCallBackUrl ?? string.Empty);

                var merchantReference = invoice.Id.ConvertIntToGUIDString();
                //for fawry reference code
                //if (!paymentData.IsFawryCard)
                //{

                //    var chargeResponse = await fawryClient.PayAsync(merchantReference, new CustomerInfo(paymentData.Customer.Id + "", paymentData.Customer.Phone, paymentData.Customer.Email),
                //   new PaymentInfo(Dexef.Payment.FawryAPI.PaymentMethod.PayAtFawry, Convert.ToDouble(invoice.Total), invoice.Notes ?? string.Empty, paymentExpiryDateTime:
                //  //TODO:Make
                //  DateTime.Now.AddDays(_paymentGateWaySettings.Fawry.PaymentExpirationDays))
                //   { CurrencyCode = paymentData.CurrencyCode },
                //   new[]{
                //         new ChargeItem
                //          {
                //            Description = "Invoice Serial : " + invoice.Serial  ?? string.Empty ,
                //            Price = (float)invoice.Total,
                //            ItemId = invoice.Details.FirstOrDefault()?.Id ?? 0,
                //            Quantity =1
                //         }
                //   });
                //    invoice.PaymentInfo = chargeResponse.Serialize();
                //    if (chargeResponse.IsSuccess)
                //        output = new InvoicePaymentInfoJson
                //        {
                //            IsSuccess = true,
                //            Fawry = new Fawry
                //            {
                //                IsFawryCard = false,
                //                Expiration = chargeResponse.ExpirationDateTime,
                //                ReferenceNumber = chargeResponse.ReferenceNumber,
                //                GeneratedMerchantNumber = merchantReference
                //            },
                //            Bank = null,
                //            Paypal = null
                //        };
                //    var jObject = JObject.Parse(invoice.PaymentInfo);
                //    var referenceNumber = jObject["ReferenceNumber"].ToString();

                //    output.IsSuccess = true;
                //    output.Fawry.ReferenceNumber = referenceNumber;
                //    output.Fawry.Expiration = Convert.ToDateTime(jObject["ExpirationDateTime"].ToString());
                //    ////TOD:Remove this after implement getnotification call back from fawry

                //}
                ////for fawry credit card
                //else
                //{
                //    var chargeResponse = await fawryClient.PayWithCardAsync(merchantReference, new CustomerInfo(paymentData.Customer.Id + "", paymentData.Customer.Phone, paymentData.Customer.Email),
                //   new PaymentInfo(Dexef.Payment.FawryAPI.PaymentMethod.Card, Convert.ToDouble(invoice.Total), invoice.Notes ?? string.Empty, cardToken: paymentData.CardToken, paymentExpiryDateTime:
                //  //TODO:Make
                //  DateTime.Now.AddDays(_paymentGateWaySettings.Fawry.PaymentExpirationDays))
                //   { CurrencyCode = paymentData.CurrencyCode },
                //   new[]{
                //         new ChargeItem
                //          {
                //            Description = "Invoice Serial : " + invoice.Serial  ?? string.Empty ,
                //            Price = (float)invoice.Total,
                //            ItemId = invoice.Details.FirstOrDefault()?.Id ?? 0,
                //            Quantity =1
                //         }
                //   });
                //    invoice.PaymentInfo = chargeResponse.Serialize();
                //    var jObject = JObject.Parse(invoice.PaymentInfo);

                //    if (chargeResponse.IsSuccess)
                //    {
                //        output = new InvoicePaymentInfoJson
                //        {
                //            IsSuccess = true,
                //            Fawry = new Fawry
                //            {
                //                IsFawryCard = true,
                //                ReferenceNumber = !paymentData.IsFawryCard ? jObject["ReferenceNumber"].ToString() : string.Empty,
                //                Expiration = !paymentData.IsFawryCard ? Convert.ToDateTime(jObject["ExpirationDateTime"].ToString()) : null,
                //                ThreeDSecureUrl = paymentData.IsFawryCard ? (chargeResponse.NextAction?.RedirectUrl ?? string.Empty) : string.Empty,
                //                Token = paymentData.IsFawryCard ? paymentData.CardToken : string.Empty,
                //                GeneratedMerchantNumber = merchantReference

                //            },
                //            Bank = null,
                //            Paypal = null
                //        };
                //    }

                //}
                output.IsSuccess = true;

                return output;
            }
            catch (Exception e)
            {
                //TODO:U could try remove unpaid invoice
                await _invoiceHelper.UpdateInvoiceStatusWithPaymentResponseFromPaymentCallBackAsync(invoice.Id, PaymentTypesEnum.Fawry, (int)InvoiceStatusEnum.Unpaid, paymentResponse: e.Data.Values.Serialize(), true);

                return output;

            }
        }
        public async Task<IResponse<string>> FawryCreateCardToken(FawryAPICreateTokenInputDto inputDto)
        {
            var output = new Response<string>();
            try
            {
                var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);
                var customer = _mapper.Map<GetSimplifiedCustomerOutputDto>(_customerRepository.GetById(inputDto.CustomerId));
                FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);

                //for fawry credit card
                var tokenRedirectUrl = await fawryClient.CreateCardTokenPluginUrl(new CustomerInfo(customer.Id + "", customer.Phone, customer.Email), inputDto.ReturnUrl);

                return output.CreateResponse(tokenRedirectUrl ?? string.Empty);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }
        //public async Task<IResponse<CardTokenResponse>> FawryDeleteCardToken( int CustomerId, string cardToken )
        //{
        //    var output = new Response<CardTokenResponse>();
        //    try
        //    {
        //        var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);
        //        FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);
        //        var response = await fawryClient.DeleteCustomerCardTokensAsync(CustomerId + "", cardToken);
        //        return output.CreateResponse(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return output.CreateResponse(ex);
        //    }
        //}
        public async Task<IResponse<bool>> FawryDeleteCardToken(int CustomerId, string cardToken)
        {
            var output = new Response<bool>();
            try
            {
                var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);
                FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);
                var response = await fawryClient.DeleteCustomerCardTokensAsync(CustomerId + "", cardToken);
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<ListCustomerTokensResponse>> FawryListCustomerCards(int CustomerId)
        {
            var output = new Response<ListCustomerTokensResponse>();
            ListCustomerTokensResponse cards = new ListCustomerTokensResponse();

            try
            {
                var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);
                FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);
                cards = await fawryClient.ListCustomerCardTokensAsync(CustomerId + "");
                return output.CreateResponse(cards);
            }
            catch (Exception ex)
            {
                output.IsSuccess = true;
                return output.CreateResponse(new ListCustomerTokensResponse { });
                //return output.CreateResponse(ex);
            }
        }
        public async Task FawryDeleteAllCardToken()
        {
            var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);
            FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);
            var customers = _customerRepository.GetAllList();
            foreach (var customer in customers)
            {
                var cardResponse = await fawryClient.ListCustomerCardTokensAsync(customer.Id + "");
                foreach (var card in cardResponse.Cards)
                {
                    try
                    {

                        await fawryClient.DeleteCustomerCardTokensAsync(customer.Id + "", card.Token);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }

        }
        public async Task FawryDeleteAllCustomerCardToken(int customerId)
        {
            var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);
            FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);
            var customer = _customerRepository.GetById(customerId);
            var cardResponse = await fawryClient.ListCustomerCardTokensAsync(customer.Id + "");
            foreach (var card in cardResponse.Cards)
            {
                try
                {

                    await fawryClient.DeleteCustomerCardTokensAsync(customer.Id + "", card.Token);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        public async Task DeleteCustomerTokens(int customerId)
        {
            var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);
            FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);
            ListCustomerTokensResponse cards = await fawryClient.ListCustomerCardTokensAsync(customerId + "");
            foreach (var card in cards.Cards)
            {
                await fawryClient.DeleteCustomerCardTokensAsync(customerId + "", card.Token);
            }

        }
        public async Task<IResponse<bool>> FawryVerifyNotificationCallBack(string requestCallBack)
        {
            var output = new Response<bool>();

            try
            {
                var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);
                FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);
                var paymentResponse = await fawryClient.VerifyNotificaion(requestCallBack);
                //bool isValid = paymentResponse.Verify(paymentMethod.Credential.SercretKey);
                //TODO:remove this

                paymentResponse = JsonConvert.DeserializeObject<FawryV2NotificationResponse>(requestCallBack);
                int paymentStatusId = (int)InvoiceStatusEnum.Unpaid;
                if (paymentResponse.OrderStatus == OrderStatus.New.ToString().ToUpper())
                    return output.CreateResponse(false);


                paymentStatusId = await UpdateFawryInvoiceAsync(paymentResponse.MerchantRefNumber, paymentResponse.OrderStatus, paymentResponse.Serialize(), true);
                return output.CreateResponse(paymentStatusId == (int)InvoiceStatusEnum.Paid ? true : false);
            }
            catch (Exception ex)
            {
                output.IsSuccess = false;
                return output.CreateResponse(ex);
            }
        }
        public async Task<PaymentStatusResponse> FawryGetPaymentStatus(string merchantRefNumber)
        {
            var output = new Response<bool>();

            try
            {
                var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);
                FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);
                var paymentResponse = await fawryClient.GetPaymentStatusAsync(merchantRefNumber);
                return paymentResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<PaymentStatusResponse> FawryTestGetPaymentStatus(string merchantRefNumber)
        {
            var output = new Response<bool>();

            try
            {
                var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);
                FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey);
                var paymentResponse = await fawryClient.GetPaymentStatusAsync(merchantRefNumber);
                return paymentResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IResponse<bool>> CallBackConfirmFawryOrder(string type, string referenceNumber, string merchantRefNumber, string orderAmount, string fawryFees, string orderStatus, string paymentMethod, string paymentTime, string customerMobile, string customerMail, string customerProfileId, string signature, string taxes, int statusCode, string statusDescription, bool basketPayment = false)
        {
            var output = new Response<bool>();
            try
            {
                //TODO
                // 1- verify Signature
                // 2- update invoice status & Payment Response
                var paymentResponse = new InvoicePaymentResponseJson
                {
                    IsSuccess = true,
                    Fawry = new FawryCardTokenResponse
                    {
                        basketPayment = basketPayment,
                        orderStatus = orderStatus,
                        customerMail = customerMail,
                        customerMobile = customerMobile,
                        customerProfileId = customerProfileId,
                        merchantRefNumber = merchantRefNumber,
                        referenceNumber = referenceNumber,
                        orderAmount = orderAmount,
                        fawryFees = fawryFees,
                        taxes = taxes,
                        paymentMethod = paymentMethod,
                        paymentTime = paymentTime,
                        signature = signature,
                        statusCode = statusCode,
                        statusDescription = statusDescription,
                        type = type,
                    }
                };
                int paymentStatusId = 0;
                if (statusCode == 200)
                {
                    paymentStatusId = await UpdateFawryInvoiceAsync(merchantRefNumber, orderStatus, paymentResponse.Serialize());
                    #region Refactored
                    //int.TryParse(merchantRefNumber.GetIntFromGUIDString(), out int invoiceId);

                    ////int paymentStatusId = orderStatus.ToUpper() switch
                    ////{
                    ////    //TODO:check with kareem
                    ////    Enum.GetName<FawryPaymentStatus>(FawryPaymentStatus.Paid).ToString().ToUpper() ?? string.Empty => (int)InvoiceStatusEnum.Paid,
                    ////    //FawryPaymentStatus.Unpaid.ToString().ToUpper() => (int)InvoiceStatusEnum.Unpaid,
                    ////    //FawryPaymentStatus.Expired => (int)InvoiceStatusEnum.WaitingPaymentConfirmation,
                    ////    //FawryPaymentStatus.Cancelled => (int)InvoiceStatusEnum.Cancelled,
                    ////    //FawryPaymentStatus.Refunded => (int)InvoiceStatusEnum.Refunded,
                    ////    //FawryPaymentStatus.Failed => (int)InvoiceStatusEnum.Unpaid,
                    ////    _ => (int)InvoiceStatusEnum.Unpaid
                    ////};

                    //if (orderStatus == FawryPaymentStatus.Paid.ToString().ToUpper() /*"PAID"*/)
                    //{
                    //    paymentStatusId = (int)InvoiceStatusEnum.Paid;
                    //}
                    //else
                    //{
                    //    paymentStatusId = (int)InvoiceStatusEnum.Unpaid;

                    //}

                    //// 2- update invoice status & Payment Response
                    //await UpdateInvoiceStatusWithPaymentResponse(invoiceId, PaymentTypesEnum.Fawry, paymentStatusId, paymentResponse: paymentResponse.Serialize(), true);
                    #endregion
                }
                return output.CreateResponse(paymentStatusId == (int)InvoiceStatusEnum.Paid ? true : false);

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<bool>> CallBackConfirmPayMobOrder(int merchantOrderNumber, bool success)
        {
            var output = new Response<bool>();

            try
            {
                var response = new PayMobResponse
                {
                    MerchantRefNumber = merchantOrderNumber
                };
                if (success)
                {
                    var result = await _invoiceHelper.UpdateInvoiceStatusWithPaymentResponseFromPaymentCallBackAsync(merchantOrderNumber, PaymentTypesEnum.PayMob, (int)InvoiceStatusEnum.Paid, response.Serialize(), true);

                    if (!result)
                        return output.CreateResponse(false);

                    return output.CreateResponse(true);
                }
                return output.CreateResponse(false);
            }
            catch (Exception e)
            {

                return output.CreateResponse(e);
            }



        }
        /// <summary>
        /// returns paymentStatusId
        /// </summary>
        /// <param name="merchantRefNumber"></param>
        /// <param name="orderStatus"></param>
        /// <param name="paymentResponse"></param>
        /// <returns></returns>
        public async Task<int> UpdateFawryInvoiceAsync(string merchantRefNumber, string orderStatus, string paymentResponse, bool commit = true)
        {
            int paymentStatusId = 0;
            int.TryParse(merchantRefNumber.GetIntFromGUIDString(), out int invoiceId);

            //int paymentStatusId = orderStatus.ToUpper() switch
            //{
            //    //TODO:check with kareem
            //    Enum.GetName<FawryPaymentStatus>(FawryPaymentStatus.Paid).ToString().ToUpper() ?? string.Empty => (int)InvoiceStatusEnum.Paid,
            //    //FawryPaymentStatus.Unpaid.ToString().ToUpper() => (int)InvoiceStatusEnum.Unpaid,
            //    //FawryPaymentStatus.Expired => (int)InvoiceStatusEnum.WaitingPaymentConfirmation,
            //    //FawryPaymentStatus.Cancelled => (int)InvoiceStatusEnum.Cancelled,
            //    //FawryPaymentStatus.Refunded => (int)InvoiceStatusEnum.Refunded,
            //    //FawryPaymentStatus.Failed => (int)InvoiceStatusEnum.Unpaid,
            //    _ => (int)InvoiceStatusEnum.Unpaid
            //};

            if (orderStatus == FawryPaymentStatus.Paid.ToString().ToUpper() /*"PAID"*/)
            {
                paymentStatusId = (int)InvoiceStatusEnum.Paid;
            }
            else
            {
                paymentStatusId = (int)InvoiceStatusEnum.Unpaid;

            }

            // 2- update invoice status & Payment Response
            await _invoiceHelper.UpdateInvoiceStatusWithPaymentResponseFromPaymentCallBackAsync(invoiceId, PaymentTypesEnum.Fawry, paymentStatusId, paymentResponse: paymentResponse.Serialize(), commit);
            return paymentStatusId;

        }
        //public async Task VerifyNotificaion( string merchantRefNumber )
        //{
        //    int paymentStatusId = 0;
        //    if (statusCode == 200)
        //    {
        //        int.TryParse(merchantRefNumber.GetIntFromGUIDString(), out int invoiceId);

        //        //int paymentStatusId = orderStatus.ToUpper() switch
        //        //{
        //        //    //TODO:check with kareem
        //        //    Enum.GetName<FawryPaymentStatus>(FawryPaymentStatus.Paid).ToString().ToUpper() ?? string.Empty => (int)InvoiceStatusEnum.Paid,
        //        //    //FawryPaymentStatus.Unpaid.ToString().ToUpper() => (int)InvoiceStatusEnum.Unpaid,
        //        //    //FawryPaymentStatus.Expired => (int)InvoiceStatusEnum.WaitingPaymentConfirmation,
        //        //    //FawryPaymentStatus.Cancelled => (int)InvoiceStatusEnum.Cancelled,
        //        //    //FawryPaymentStatus.Refunded => (int)InvoiceStatusEnum.Refunded,
        //        //    //FawryPaymentStatus.Failed => (int)InvoiceStatusEnum.Unpaid,
        //        //    _ => (int)InvoiceStatusEnum.Unpaid
        //        //};

        //        if (orderStatus == FawryPaymentStatus.Paid.ToString().ToUpper() /*"PAID"*/)
        //        {
        //            paymentStatusId = (int)InvoiceStatusEnum.Paid;
        //        }
        //        else
        //        {
        //            paymentStatusId = (int)InvoiceStatusEnum.Unpaid;

        //        }

        //        // 2- update invoice status & Payment Response
        //        await UpdateInvoiceStatusWithPaymentResponse(invoiceId, PaymentTypesEnum.Fawry, paymentStatusId, paymentResponse: paymentResponse.Serialize(), true);

        //    }
        //    return output.CreateResponse(paymentStatusId == (int)InvoiceStatusEnum.Paid ? true : false);
        //    ////TODO
        //    //// 1- update invoice status
        //    //// 2- Delete CustomerTokens
        //    //try
        //    //{
        //    //    bool canConvert = int.TryParse(merchantRefNumber, out int invoiceId);
        //    //    if (canConvert)
        //    //    {
        //    //        var invoice = _invoiceRepository.GetById(invoiceId);
        //    //        FawryClient fawryClient = new FawryClient(FawryInitializeDto.BaseUrl, FawryInitializeDto.MerchantCode, FawryInitializeDto.SecurityKey);
        //    //        var response = await fawryClient.VerifyNotificaion(merchantRefNumber);
        //    //        response.OrderStatus
        //    //        UpdateInvoiceStatus(invoice.Id,PaymentTypesEnum.Fawry,PaymentStatus.Paid)
        //    //        await DeleteCustomerTokens(invoice.CustomerSubscription.CustomerId);
        //    //    }

        //    //}
        //    //catch (Exception ex)
        //    //{

        //    //}
        //}
        public async Task<RefundPaymentResponseDto> RefundFawry(Invoice invoice)
        {
            var refundResultDto = new RefundPaymentResponseDto();

            if (invoice.PaymentMethod.PaymentTypeId != (int)PaymentTypesEnum.PayMob)
                return refundResultDto;

            try
            {
                var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Fawry);
                FawryClient fawryClient = new FawryClient(paymentMethod.Credential.BaseUrl, paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey, "");

                var paymentInfoModel = !string.IsNullOrWhiteSpace(invoice.PaymentInfo) ? JsonConvert.DeserializeObject<InvoicePaymentInfoJson>(invoice.PaymentInfo) : new InvoicePaymentInfoJson();
                var isFawryCard = paymentInfoModel.Fawry.IsFawryCard;

                if (!isFawryCard)
                {
                    refundResultDto.IsSuccess = true;
                    refundResultDto.Result = "Success for Fawry Reference Number ,, will be refunded Manaually";
                    return refundResultDto;
                }

                // for fawry Cards
                var fawryReferenceNumber = JsonConvert.DeserializeObject<InvoicePaymentResponseJson>(invoice.PaymentResponse);
                var response = await fawryClient.RefundAsync(fawryReferenceNumber.Fawry.referenceNumber, Convert.ToDouble(invoice.Total), invoice.CancelReason ?? string.Empty);
                //if (response.IsSuccess)
                //{
                //    await UpdateInvoiceStatusWithPaymentResponse(invoice.Id, PaymentTypesEnum.Fawry, (int)InvoiceStatusEnum.Refunded, fawryReferenceNumber.Serialize(), true);
                //    return true;

                //}
                //else
                //    return false;
                refundResultDto.IsSuccess = response.IsSuccess;
                refundResultDto.Result = fawryReferenceNumber.Serialize();
                return refundResultDto;
            }
            catch (Exception ex)
            {
                refundResultDto.Result = ex.ToString();
                return refundResultDto;
            }

        }



        #endregion
        #region Bank
        public async Task<InvoicePaymentInfoJson> PayWithBank(ThirdPartyInvoiceInputDto paymentData, InvoiceDto invoice)
        {
            //TODO:Check Convert Invoice Currency with Exchange Rate?
            var customerCurrencyCode = paymentData?.CurrencyCode ?? "";

            var output = new InvoicePaymentInfoJson();
            try
            {
                var generatedMerchantNumber = invoice.Id.ConvertIntToGUIDString();
                var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Bank);
                PayFortClient client = new PayFortClient(paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey, _paymentGateWaySettings.Language, _paymentGateWaySettings.Bank.SHARequestPhrase, _paymentGateWaySettings.Bank.SHAResponsePhrase, _paymentGateWaySettings.Bank.SHAType, _paymentGateWaySettings.IsSandBox);

                //Reference https://en.wikipedia.org/wiki/ISO_4217
                var multiplyFactorValue = _customerRepository.GetById(paymentData.Customer.Id)?.Country?.CountryCurrency?.Currency?.MultiplyFactor ?? 0;
                var multiplyFactor = Convert.ToDecimal(Math.Pow(10, multiplyFactorValue));
                var invoiceTotal = Convert.ToDecimal(invoice.Total * multiplyFactor);
                invoiceTotal = decimal.Round(invoiceTotal, 0, MidpointRounding.AwayFromZero);
                //TODO:Add Lookup to determine MultiplyFactor for each currency
                var template = await client.GetPurchaseTemplate(generatedMerchantNumber, invoiceTotal.ToString(), paymentData.CurrencyCode, paymentData.Customer.Email, "Invoice Serial : " + invoice.Serial /*+ " for Purchasing " + invoice.Details.FirstOrDefault().VersionName*/, _paymentGateWaySettings.Language, paymentData.SuccessCallBackUrl);

                var parameters = await client.GetPurchaseParameters(generatedMerchantNumber, invoiceTotal.ToString(), paymentData.CurrencyCode, paymentData.Customer.Email, "Invoice Serial : " + invoice.Serial/* + " for Purchasing " + invoice.Details.FirstOrDefault().VersionName*/, _paymentGateWaySettings.Language, paymentData.SuccessCallBackUrl);
                invoice.PaymentInfo = template.Serialize();
                output = new InvoicePaymentInfoJson
                {
                    IsSuccess = true,
                    Bank = new Bank
                    {
                        Template = template,
                        Parameters = parameters,
                        GeneratedMerchantNumber = generatedMerchantNumber,
                        CardNumber = "",
                        CardTypeId = 0,
                        FortId = ""
                    },
                    Fawry = null,
                    Paypal = null
                };
                return output;
            }
            catch (Exception e)
            {
                return output;

            }
        }

        public async Task<IResponse<PayFortPurchaseResponse>> CallBackConfirmBankOrder(IFormCollection formParameters)
        {

            var response = new Response<PayFortPurchaseResponse>();
            var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Bank);
            PayFortClient client = new PayFortClient(paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey, _paymentGateWaySettings.Language, _paymentGateWaySettings.Bank.SHARequestPhrase, _paymentGateWaySettings.Bank.SHAResponsePhrase, _paymentGateWaySettings.Bank.SHAType, _paymentGateWaySettings.IsSandBox);

            // //TODO:Recheck This
            //// Validate Signature
            //var isVerifiedResponse = client.IsVerifiedResponse(formParameters);
            //if (isVerifiedResponse)
            //{

            var payFortResponse = await client.GetResponseFromPurchaseCallBack(formParameters);

            if (payFortResponse != null)
            {
                int invoiceId = Convert.ToInt32(payFortResponse?.Data?.MerchantReference?.GetIntFromGUIDString() ?? "0");
                await UpdateBankFortIdAndCardData(invoiceId, payFortResponse?.Data?.FortId, payFortResponse?.Data?.PaymentOption, payFortResponse?.Data?.CardNumber);

                await _invoiceHelper.UpdateInvoiceStatusWithPaymentResponseFromPaymentCallBackAsync(invoiceId, PaymentTypesEnum.Bank, payFortResponse.IsSuccess ? (int)InvoiceStatusEnum.Paid : (int)InvoiceStatusEnum.Unpaid, paymentResponse: JsonConvert.SerializeObject(payFortResponse), true);


                return response.CreateResponse(payFortResponse.Data);


            }
            else
            {
                response.IsSuccess = false;
                return response.CreateResponse(MessageCodes.Failed);
            }




            //}
            //    else
            //        return response.CreateResponse(MessageCodes.Failed);
        }

        public async Task<RefundPaymentResponseDto> RefundBank(Invoice invoice)
        {
            var refundResultDto = new RefundPaymentResponseDto();

            try
            {
                if (invoice.PaymentMethod.PaymentTypeId != (int)PaymentTypesEnum.Bank)
                    return refundResultDto;

                var paymentMethod = await _paymentSetupBLL.GetPaymentMethodByPaymentType(PaymentTypesEnum.Bank);
                PayFortClient client = new PayFortClient(paymentMethod.Credential.ApiKey, paymentMethod.Credential.SercretKey, _paymentGateWaySettings.Language, _paymentGateWaySettings.Bank.SHARequestPhrase, _paymentGateWaySettings.Bank.SHAResponsePhrase, _paymentGateWaySettings.Bank.SHAType, _paymentGateWaySettings.IsSandBox);

                int invoiceId = invoice.Id;
                decimal amount = invoice.Total;
                var invoiceDto = JsonConvert.DeserializeObject<InvoicePaymentInfoJson>(invoice.PaymentInfo);
                var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse<PayFortPurchaseResponse>>(invoice.PaymentResponse);
                var grossAmount = paymentResponse?.Data?.Amount;
                var currency = paymentResponse?.Data?.Currency;

                //TODO:Add Lookup to determine MultiplyFactor for each currency
                var response = await client.RefundAsync(invoiceDto.Bank.FortId, currency, grossAmount);

                //if (response.IsSuccess)
                //{
                //    await UpdateInvoiceStatusWithPaymentResponse(invoiceId, PaymentTypesEnum.Bank, (int)InvoiceStatusEnum.Refunded, JsonConvert.SerializeObject(response), true);
                //    return true;

                //}
                //else
                //    return false;

                refundResultDto.IsSuccess = response.IsSuccess;
                refundResultDto.Result = JsonConvert.SerializeObject(response);
                return refundResultDto;
            }
            catch (Exception e)
            {
                refundResultDto.Result = e.ToString();
                return refundResultDto;
            }
        }
        #endregion
        #endregion
        #region CRUD
        public async Task<IResponse<bool>> CreateAsync(CardTokenDto inputDto)
        {
            var output = new Response<bool>();
            try
            {
                //Business Validation
                bool isCardTokenExists = _customerCardRepository.Any(x => x.CustomerId == inputDto.CustomerId && x.PaymentMethodId == inputDto.PaymentMethodId && x.CardToken == inputDto.CardToken);
                if (isCardTokenExists)
                    return output.CreateResponse(false);

                var result = _mapper.Map<CustomerCard>(inputDto);
                _customerCardRepository.Add(result);
                _unitOfWork.Commit();
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {

                return output.CreateResponse(ex);
            }

        }

        public async Task<IResponse<bool>> DeleteAsync(int customerId, string cardToken, int paymentMethodId, bool commit = false)
        {
            var output = new Response<bool>();
            try
            {
                //Input Validation

                var entity = _customerCardRepository.Where(x => x.CustomerId == customerId && x.CardToken == cardToken && x.PaymentMethodId == paymentMethodId).FirstOrDefault();
                //1-Check entity is exisit
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(CustomerCard));
                ////2- Check is Default
                //if (entity.IsDefault)
                //    return output.CreateResponse(MessageCodes.DefaultEntity);
                _customerCardRepository.Delete(entity);
                if (commit)
                    _unitOfWork.Commit();
                return output.CreateResponse(true);

            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<bool>> UpdateAsync(UpdateCardTokenDto inputDto)
        {
            var output = new Response<bool>();
            try
            {

                //BusinessValidation
                // 1- Check entity is exisit
                var entity = await _customerCardRepository.GetByIdAsync(inputDto.Id);

                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(CustomerCard));

                //make update for IsDefault only 
                //entity = _mapper.Map(inputDto, entity);
                entity.IsDefault = inputDto.IsDefault;
                entity.ModifiedDate = DateTime.UtcNow;
                if (inputDto.IsDefault == true)
                {
                    var otherCards = _customerCardRepository.Where(x => x.CustomerId == entity.CustomerId && x.PaymentMethodId == entity.PaymentMethodId && x.Id != entity.Id).ToList();
                    foreach (var card in otherCards)
                    {
                        card.IsDefault = false;
                    }
                }
                _customerCardRepository.Update(entity);
                _unitOfWork.Commit();
                var result = _mapper.Map<GetPaymentSetupOutputDto>(entity);
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public IResponse<bool> UpdateCardTypeAsync(UpdateCardTypeInputDto inputDto)
        {
            var output = new Response<bool>();
            try
            {

                //BusinessValidation
                // 1- Check entity is exisit
                var entity = _customerCardRepository.Where(x => x.CustomerId == inputDto.CustomerId && x.CardToken == inputDto.CardToken && x.PaymentMethodId == inputDto.PaymentMethodId).FirstOrDefault();

                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(CustomerCard));

                //make update for IsDefault only 
                //entity = _mapper.Map(inputDto, entity);
                entity.ExtraInfo = inputDto.CardType;
                _customerCardRepository.Update(entity);
                if (inputDto.Commit)
                    _unitOfWork.Commit();
                var result = _mapper.Map<GetPaymentSetupOutputDto>(entity);
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<List<CardTokenDto>>> GetAllAsync()
        {
            var ouput = new Response<List<CardTokenDto>>();
            try
            {
                var payments = await _customerCardRepository.GetAllListAsync();
                var result = _mapper.Map<List<CardTokenDto>>(payments);
                return ouput.CreateResponse(result);
            }
            catch (Exception ex)
            {
                return ouput.CreateResponse(ex);
            }
        }
        #endregion
        #region ALL
        public async Task<IResponse<string>> CreateCardToken(CreateCardTokenInputDto inputDto)
        {

            var output = new Response<string>();
            try
            {
                var paymentType = await _paymentSetupBLL.GetPaymentTypeByPaymentMethod(inputDto.PaymentMethodId);
                var result = paymentType.Id switch
                {

                    (int)PaymentTypesEnum.Fawry => await FawryCreateCardToken(new FawryAPICreateTokenInputDto
                    {
                        CustomerId = inputDto.CustomerId,
                        InvoiceId = 0,
                        ReturnUrl = inputDto.ReturnUrl
                    }),
                    //(int)PaymentTypesEnum.Bank => await BankListCreateCardToken(paymentData, invoice),

                    _ => throw new ArgumentOutOfRangeException("PaymentMethod", "Invalid PaymentMethod", null)
                };

                #region SyncCardTokens
                await SyncCardTokens(inputDto);
                #endregion

                return result;
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task SyncCardTokens(CreateCardTokenInputDto inputDto)
        {
            try
            {
                var paymentType = await _paymentSetupBLL.GetPaymentTypeByPaymentMethod(inputDto.PaymentMethodId);
                var savedCardsInPaymentGateWay = paymentType.Id switch
                {

                    (int)PaymentTypesEnum.Fawry => await FawryListCustomerCards(inputDto.CustomerId),
                    //(int)PaymentTypesEnum.Bank => ,
                    //(int)PaymentTypesEnum.PayPal => ,

                    _ => throw new ArgumentOutOfRangeException("PaymentMethod", "Invalid PaymentMethod", null)
                };

                var cardsDb = _customerCardRepository.Where(x => x.CustomerId == inputDto.CustomerId && x.PaymentMethodId == inputDto.PaymentMethodId).ToList();
                var savedCardsInDb = _mapper.Map<List<CardTokenDto>>(cardsDb);

                if (savedCardsInPaymentGateWay.Data.Cards != null)
                {
                    foreach (var item in savedCardsInPaymentGateWay.Data.Cards)
                    {
                        if (!savedCardsInDb.Select(x => x.CardToken).Contains(item.Token))
                        {
                            _customerCardRepository.Add(new CustomerCard
                            {
                                CardNumber = "**********" + (item.LastFourDigits ?? string.Empty),
                                CardToken = item.Token,
                                CreateDate = item.CreationDateTime,
                                IsDefault = savedCardsInDb.Any() ? false : true,
                                ExtraInfo = item.Brand,
                                PaymentMethodId = inputDto.PaymentMethodId,
                                CustomerId = inputDto.CustomerId
                            });
                        }
                    }
                    _unitOfWork.Commit();

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IResponse<bool>> CreateCardTokenCallBack(CreateCardTokenCallBackDto inputDto)
        {

            var output = new Response<bool>();
            try
            {
                var paymentType = await _paymentSetupBLL.GetPaymentTypeByPaymentMethod(inputDto.PaymentMethodId);
                var isFirstCard = !(_customerCardRepository.Any(x => x.PaymentMethodId == inputDto.PaymentMethodId && x.CustomerId == inputDto.CustomerId));

                //if(inputDto.StatusCode == "200")
                var result = paymentType.Id switch
                {
                    //(int)PaymentTypesEnum.PayPal =>
                    //(int)PaymentTypesEnum.Bank =>

                    (int)PaymentTypesEnum.Fawry =>
                    await CreateAsync(new CardTokenDto
                    {
                        CardNumber = (inputDto.FirstSixDigits ?? string.Empty) + "****" + (inputDto.LastFourDigits ?? string.Empty),
                        CardToken = inputDto.Token,
                        CreateDate = DateTime.UtcNow,
                        IsDefault = isFirstCard,
                        ExtraInfo = string.Empty,
                        PaymentMethodId = inputDto.PaymentMethodId,
                        CustomerId = inputDto.CustomerId
                    }),

                    _ => throw new ArgumentOutOfRangeException("PaymentMethod", "Invalid PaymentMethod", null)
                };

                #region Update Card Type from Fawry
                if (result.IsSuccess && result.Data == true && paymentType.Id == (int)PaymentTypesEnum.Fawry)
                {
                    var customerCards = await FawryListCustomerCards(inputDto.CustomerId);
                    if (customerCards.IsSuccess && customerCards.Data.Cards != null)
                    {
                        customerCards.Data.Cards.ToList().ForEach(x =>
                        {
                            UpdateCardTypeAsync(new UpdateCardTypeInputDto
                            {
                                CardToken = x.Token,
                                CardType = x.Brand,
                                CustomerId = inputDto.CustomerId,
                                PaymentMethodId = inputDto.PaymentMethodId,
                                Commit = false

                            });
                            _unitOfWork.Commit();
                        });
                    }
                }
                #endregion


                else
                {
                    return output.CreateResponse(false);

                }

                return result;
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }

        public async Task<IResponse<bool>> UpdateCardToken(UpdateCardTokenDto inputDto)
        {
            var output = new Response<bool>();
            try
            {
                var entity = await _customerCardRepository.GetByIdAsync(inputDto.Id);
                if (entity == null)
                    return output.CreateResponse(MessageCodes.NotFound, nameof(CustomerCard));

                await UpdateAsync(inputDto);
                return output.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return output.CreateResponse(ex);
            }
        }


        public async Task<IResponse<bool>> DeleteCardToken(int customerId, string cardToken, int paymentMethodId)
        {
            var output = new Response<bool>();
            try
            {

                var paymentType = await _paymentSetupBLL.GetPaymentTypeByPaymentMethod(paymentMethodId);
                var result = paymentType.Id switch
                {

                    //(int)PaymentTypesEnum.PayPal => await PayPalCustomerCards(paymentData, invoice),
                    (int)PaymentTypesEnum.Fawry => await FawryDeleteCardToken(customerId, cardToken),
                    //(int)PaymentTypesEnum.Bank => await BankListCustomerCards(paymentData, invoice),

                    _ => throw new ArgumentOutOfRangeException("PaymentMethod", "Invalid PaymentMethod", null)
                };

                if (result.IsSuccess && result.Data == true)
                {
                    return await DeleteAsync(customerId, cardToken, paymentMethodId, true);

                }
                else
                {
                    return output.CreateResponse(false);

                }
                return result;
            }
            catch (Exception ex)
            {
                return output.CreateResponse(false);
            }
        }
        public async Task<IResponse<List<CardTokenDto>>> ListCustomerCards(int customerId, int paymentMethodId)
        {
            var output = new Response<List<CardTokenDto>>();
            var cards = new List<CardTokenDto>();
            try
            {

                var result = _customerCardRepository
                    .WhereIf(x => x.PaymentMethodId == paymentMethodId, paymentMethodId > 0)
                    .Where(x => x.CustomerId == customerId).ToList();
                return output.CreateResponse(_mapper.Map<List<CardTokenDto>>(result));
            }
            catch (Exception ex)
            {
                return output.CreateResponse(cards);
            }
        }
        #endregion



    }
}
