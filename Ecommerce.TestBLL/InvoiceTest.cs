using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDexef.BLL.Customers.Invoices;
using MyDexef.Core.Entities;
using MyDexef.Core.Enums.Admins;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDexef.TestBLL
{
    [TestClass]
    public class InvoiceTest : UnitTestBase
    {
        IInvoiceHelperBLL _invoiceHelperBLL;
        private const decimal DiscountForever = 30M; 
        private const decimal DiscountMonthly = 10M; 
        private const decimal DiscountYearly = 20M; 
        private const decimal TaxPercentage = 14M; 
        private const decimal ItemPriceYearly = 1500M; 
        private const decimal ItemPriceForever = 2000M; 
        private const decimal ItemPriceMonthly = 500M; 
        private const string AppTitle = "AppTitle";
        private const string TaxName = "TaxTest";
        private const int CreatedBy = 1; 
        private const int Id = 1;
        private const int CountryId = 66; 
        private const int MathRound = 2; 
        private readonly Tax  tax; 
        private readonly VersionPrice versionPrice; 
        private readonly AddOnPrice addonPrice; 
        public InvoiceTest()
        {
            _invoiceHelperBLL = serviceProvider.GetRequiredService<IInvoiceHelperBLL>();
             tax = new Tax
            {
                Name = TaxName,
                CountryId = CountryId,
                PriceIncludeTax = false,
                CreatedBy = CreatedBy,
                Id = Id,
                Percentage = TaxPercentage
            };
            versionPrice = new VersionPrice
            {
               
                ForeverPrice = ItemPriceForever,
                ForeverPrecentageDiscount = DiscountForever,

                MonthlyPrice = ItemPriceMonthly,
                MonthlyPrecentageDiscount = DiscountMonthly,

                YearlyPrice = ItemPriceYearly,
                YearlyPrecentageDiscount = DiscountYearly,

                Version = new MyDexef.Core.Entities.Version
                {
                    Title = AppTitle,
                }

            };
            addonPrice = new AddOnPrice
            {

                ForeverPrice = ItemPriceForever,
                ForeverPrecentageDiscount = DiscountForever,

                MonthlyPrice = ItemPriceMonthly,
                MonthlyPrecentageDiscount = DiscountMonthly,

                YearlyPrice = ItemPriceYearly,
                YearlyPrecentageDiscount = DiscountYearly,

                AddOn = new AddOn
                {
                    Title=AppTitle,
                }
            };
        }
        [TestMethod]
        public void CalculateInvoic_TakeVersionPriceForeverIncludeTax_ReturnInvoiceCalculted()
        {
            //IncludeTax
            tax.PriceIncludeTax = true;

            var result = _invoiceHelperBLL.CalculateInvoice(versionPrice, tax ,(int)DiscriminatorsEnum.Forever,true);

            decimal taxPercentage, priceWithoutTax, netPrice, discountValue, vatAmount, total;
            CalculateInvoicePrice(out taxPercentage, out priceWithoutTax, out netPrice, out discountValue, out vatAmount, out total, (int)DiscriminatorsEnum.Forever, true);

            result.ShouldNotBeNull();
            result.Price.ShouldBe(Math.Round(priceWithoutTax, MathRound));
            result.PriceAfterDiscount.ShouldBe(Math.Round(netPrice, MathRound));
            result.SubTotal.ShouldBe(Math.Round(netPrice, MathRound));
            result.TotalDiscountAmount.ShouldBe(discountValue);
            result.TotalVatAmount.ShouldBe(Math.Round(vatAmount, MathRound));
            result.VatPercentage.ShouldBe(taxPercentage * 100M);
            result.Total.ShouldBe(total);

        }

        [TestMethod]
        public void CalculateInvoic_TakeVersionPriceYearlyIncludeTax_ReturnInvoiceCalculted()
        {
            //IncludeTax
            tax.PriceIncludeTax = true;

            var result = _invoiceHelperBLL.CalculateInvoice(versionPrice, tax, (int)DiscriminatorsEnum.Yearly, true);

            decimal taxPercentage, priceWithoutTax, netPrice, discountValue, vatAmount, total;
            CalculateInvoicePrice(out taxPercentage, out priceWithoutTax, out netPrice, out discountValue, out vatAmount, out total, (int)DiscriminatorsEnum.Yearly, true);

            result.ShouldNotBeNull();
            result.Price.ShouldBe(Math.Round(priceWithoutTax, MathRound));
            result.PriceAfterDiscount.ShouldBe(Math.Round(netPrice, MathRound));
            result.SubTotal.ShouldBe(Math.Round(netPrice, MathRound));
            result.TotalDiscountAmount.ShouldBe(discountValue);
            result.TotalVatAmount.ShouldBe(Math.Round(vatAmount, MathRound));
            result.VatPercentage.ShouldBe(taxPercentage * 100M);
            result.Total.ShouldBe(total);

        }

        [TestMethod]
        public void CalculateInvoic_TakeVersionPriceMonthlyIncludeTax_ReturnInvoiceCalculted()
        {
            //IncludeTax
            tax.PriceIncludeTax = true;

            var result = _invoiceHelperBLL.CalculateInvoice(versionPrice, tax, (int)DiscriminatorsEnum.Monthly, true);

            decimal taxPercentage, priceWithoutTax, netPrice, discountValue, vatAmount, total;
            CalculateInvoicePrice(out taxPercentage, out priceWithoutTax, out netPrice, out discountValue, out vatAmount, out total, (int)DiscriminatorsEnum.Monthly, true);

            result.ShouldNotBeNull();
            result.Price.ShouldBe(Math.Round(priceWithoutTax, MathRound));
            result.PriceAfterDiscount.ShouldBe(Math.Round(netPrice, MathRound));
            result.SubTotal.ShouldBe(Math.Round(netPrice, MathRound));
            result.TotalDiscountAmount.ShouldBe(discountValue);
            result.TotalVatAmount.ShouldBe(Math.Round(vatAmount, MathRound));
            result.VatPercentage.ShouldBe(taxPercentage * 100M);
            result.Total.ShouldBe(total);

        }

        [TestMethod]
        public void CalculateInvoic_TakeAddOnnPriceForeverIncludeTax_ReturnInvoiceCalculted()
        {
            //IncludeTax
            tax.PriceIncludeTax = true;

            var result = _invoiceHelperBLL.CalculateInvoice(addonPrice, tax, (int)DiscriminatorsEnum.Forever, true);

            decimal taxPercentage, priceWithoutTax, netPrice, discountValue, vatAmount, total;
            CalculateInvoicePrice(out taxPercentage, out priceWithoutTax, out netPrice, out discountValue, out vatAmount, out total, (int)DiscriminatorsEnum.Forever, true);

            result.ShouldNotBeNull();
            result.Price.ShouldBe(Math.Round(priceWithoutTax, MathRound));
            result.PriceAfterDiscount.ShouldBe(Math.Round(netPrice, MathRound));
            result.SubTotal.ShouldBe(Math.Round(netPrice, MathRound));
            result.TotalDiscountAmount.ShouldBe(discountValue);
            result.TotalVatAmount.ShouldBe(Math.Round(vatAmount, MathRound));
            result.VatPercentage.ShouldBe(taxPercentage * 100M);
            result.Total.ShouldBe(total);

        }

        [TestMethod]
        public void CalculateInvoic_TakeAddOnPriceYearlyIncludeTax_ReturnInvoiceCalculted()
        {
            //IncludeTax
            tax.PriceIncludeTax = true;

            var result = _invoiceHelperBLL.CalculateInvoice(addonPrice, tax, (int)DiscriminatorsEnum.Yearly, true);

            decimal taxPercentage, priceWithoutTax, netPrice, discountValue, vatAmount, total;
            CalculateInvoicePrice(out taxPercentage, out priceWithoutTax, out netPrice, out discountValue, out vatAmount, out total, (int)DiscriminatorsEnum.Yearly, true);

            result.ShouldNotBeNull();
            result.Price.ShouldBe(Math.Round(priceWithoutTax, MathRound));
            result.PriceAfterDiscount.ShouldBe(Math.Round(netPrice, MathRound));
            result.SubTotal.ShouldBe(Math.Round(netPrice, MathRound));
            result.TotalDiscountAmount.ShouldBe(discountValue);
            result.TotalVatAmount.ShouldBe(Math.Round(vatAmount, MathRound));
            result.VatPercentage.ShouldBe(taxPercentage * 100M);
            result.Total.ShouldBe(total);

        }

        [TestMethod]
        public void CalculateInvoic_TakeAddOnPriceMonthlyIncludeTax_ReturnInvoiceCalculted()
        {
            //IncludeTax
            tax.PriceIncludeTax = true;

            var result = _invoiceHelperBLL.CalculateInvoice(addonPrice, tax, (int)DiscriminatorsEnum.Monthly, true);

            decimal taxPercentage, priceWithoutTax, netPrice, discountValue, vatAmount, total;
            CalculateInvoicePrice(out taxPercentage, out priceWithoutTax, out netPrice, out discountValue, out vatAmount, out total, (int)DiscriminatorsEnum.Monthly, true);

            result.ShouldNotBeNull();
            result.Price.ShouldBe(Math.Round(priceWithoutTax, MathRound));
            result.PriceAfterDiscount.ShouldBe(Math.Round(netPrice, MathRound));
            result.SubTotal.ShouldBe(Math.Round(netPrice, MathRound));
            result.TotalDiscountAmount.ShouldBe(discountValue);
            result.TotalVatAmount.ShouldBe(Math.Round(vatAmount, MathRound));
            result.VatPercentage.ShouldBe(taxPercentage * 100M);
            result.Total.ShouldBe(total);

        }


        [TestMethod]
        public  void CalculateVersionInvoice_TakeVersionPriceYearlyWitoutTax_ReturnInvoiceDetails()
        {
            var result = _invoiceHelperBLL.CalculateInvoice(versionPrice,tax, (int)DiscriminatorsEnum.Yearly, true);

            decimal taxPercentage, priceWithoutTax, netPrice, discountValue, vatAmount, total;
            CalculateInvoicePrice(out taxPercentage, out priceWithoutTax, out netPrice, out discountValue, out vatAmount, out total, (int)DiscriminatorsEnum.Yearly);

            result.ShouldNotBeNull();
            result.Price.ShouldBe(Math.Round(priceWithoutTax, MathRound));
            result.PriceAfterDiscount.ShouldBe(Math.Round(netPrice, MathRound));
            result.SubTotal.ShouldBe(Math.Round(netPrice, MathRound));
            result.TotalDiscountAmount.ShouldBe(discountValue);
            result.TotalVatAmount.ShouldBe(Math.Round(vatAmount, MathRound));
            result.VatPercentage.ShouldBe(taxPercentage * 100M);
            result.Total.ShouldBe(total);
        }

        [TestMethod]
        public void CalculateVersionInvoice_TakeVersionPriceMonthlyWitoutTax_ReturnInvoiceDetails()
        {
            var result = _invoiceHelperBLL.CalculateInvoice(versionPrice, tax, (int)DiscriminatorsEnum.Monthly, true);

            decimal taxPercentage, priceWithoutTax, netPrice, discountValue, vatAmount, total;
            CalculateInvoicePrice(out taxPercentage, out priceWithoutTax, out netPrice, out discountValue, out vatAmount, out total, (int)DiscriminatorsEnum.Monthly);

            result.ShouldNotBeNull();
            result.Price.ShouldBe(Math.Round(priceWithoutTax, MathRound));
            result.PriceAfterDiscount.ShouldBe(Math.Round(netPrice, MathRound));
            result.SubTotal.ShouldBe(Math.Round(netPrice, MathRound));
            result.TotalDiscountAmount.ShouldBe(discountValue);
            result.TotalVatAmount.ShouldBe(Math.Round(vatAmount, MathRound));
            result.VatPercentage.ShouldBe(taxPercentage * 100M);
            result.Total.ShouldBe(total);
        }

        [TestMethod]
        public void CalculateVersionInvoice_TakeVersionPriceForeverWitoutTax_ReturnInvoiceDetails()
        {
            var result = _invoiceHelperBLL.CalculateInvoice(versionPrice, tax, (int)DiscriminatorsEnum.Forever, true);

            decimal taxPercentage, priceWithoutTax, netPrice, discountValue, vatAmount, total;
            CalculateInvoicePrice(out taxPercentage, out priceWithoutTax, out netPrice, out discountValue, out vatAmount, out total, (int)DiscriminatorsEnum.Forever);

            result.ShouldNotBeNull();
            result.Price.ShouldBe(Math.Round(priceWithoutTax, MathRound));
            result.PriceAfterDiscount.ShouldBe(Math.Round(netPrice, MathRound));
            result.SubTotal.ShouldBe(Math.Round(netPrice, MathRound));
            result.TotalDiscountAmount.ShouldBe(discountValue);
            result.TotalVatAmount.ShouldBe(Math.Round(vatAmount, MathRound));
            result.VatPercentage.ShouldBe(taxPercentage * 100M);
            result.Total.ShouldBe(total);
        }

        [TestMethod]
        public void CalculateVersionInvoice_TakeAddonPriceYearlyWitoutTax_ReturnInvoiceDetails()
        {
            var result = _invoiceHelperBLL.CalculateInvoice(addonPrice, tax, (int)DiscriminatorsEnum.Yearly, true);

            decimal taxPercentage, priceWithoutTax, netPrice, discountValue, vatAmount, total;
            CalculateInvoicePrice(out taxPercentage, out priceWithoutTax, out netPrice, out discountValue, out vatAmount, out total, (int)DiscriminatorsEnum.Yearly);

            result.ShouldNotBeNull();
            result.Price.ShouldBe(Math.Round(priceWithoutTax, MathRound));
            result.PriceAfterDiscount.ShouldBe(Math.Round(netPrice, MathRound));
            result.SubTotal.ShouldBe(Math.Round(netPrice, MathRound));
            result.TotalDiscountAmount.ShouldBe(discountValue);
            result.TotalVatAmount.ShouldBe(Math.Round(vatAmount, MathRound));
            result.VatPercentage.ShouldBe(taxPercentage * 100M);
            result.Total.ShouldBe(total);
        }

        [TestMethod]
        public void CalculateVersionInvoice_TakeAddonPriceMonthlyWitoutTax_ReturnInvoiceDetails()
        {
            var result = _invoiceHelperBLL.CalculateInvoice(addonPrice, tax, (int)DiscriminatorsEnum.Monthly, true);

            decimal taxPercentage, priceWithoutTax, netPrice, discountValue, vatAmount, total;
            CalculateInvoicePrice(out taxPercentage, out priceWithoutTax, out netPrice, out discountValue, out vatAmount, out total, (int)DiscriminatorsEnum.Monthly);

            result.ShouldNotBeNull();
            result.Price.ShouldBe(Math.Round(priceWithoutTax, MathRound));
            result.PriceAfterDiscount.ShouldBe(Math.Round(netPrice, MathRound));
            result.SubTotal.ShouldBe(Math.Round(netPrice, MathRound));
            result.TotalDiscountAmount.ShouldBe(discountValue);
            result.TotalVatAmount.ShouldBe(Math.Round(vatAmount, MathRound));
            result.VatPercentage.ShouldBe(taxPercentage * 100M);
            result.Total.ShouldBe(total);
        }

        [TestMethod]
        public void CalculateVersionInvoice_TakeAddonPriceForeverWitoutTax_ReturnInvoiceDetails()
        {
            var result = _invoiceHelperBLL.CalculateInvoice(addonPrice, tax, (int)DiscriminatorsEnum.Forever, true);

            decimal taxPercentage, priceWithoutTax, netPrice, discountValue, vatAmount, total;
            CalculateInvoicePrice(out taxPercentage, out priceWithoutTax, out netPrice, out discountValue, out vatAmount, out total, (int)DiscriminatorsEnum.Forever);

            result.ShouldNotBeNull();
            result.Price.ShouldBe(Math.Round(priceWithoutTax, MathRound));
            result.PriceAfterDiscount.ShouldBe(Math.Round(netPrice, MathRound));
            result.SubTotal.ShouldBe(Math.Round(netPrice, MathRound));
            result.TotalDiscountAmount.ShouldBe(discountValue);
            result.TotalVatAmount.ShouldBe(Math.Round(vatAmount, MathRound));
            result.VatPercentage.ShouldBe(taxPercentage * 100M);
            result.Total.ShouldBe(total);
        }
        #region Helper
    

        private void CalculateInvoicePrice(out decimal taxPercentage, out decimal priceWithoutTax, out decimal netPrice, out decimal discountValue, out decimal vatAmount, out decimal total,int discriminator ,bool included = false)
        {
            taxPercentage = tax.Percentage / 100M;
            decimal discountPercentage = 0;
            priceWithoutTax = 0;

            if (discriminator == (int)DiscriminatorsEnum.Forever)
            {
                discountPercentage = DiscountForever / 100M;
                priceWithoutTax = included ? ItemPriceForever / (1M + taxPercentage) : ItemPriceForever;
            }
            if (discriminator == (int)DiscriminatorsEnum.Yearly)
            {
                discountPercentage = DiscountYearly / 100M;
                priceWithoutTax = included ? ItemPriceYearly / (1M + taxPercentage) : ItemPriceYearly;
            }
            if (discriminator == (int)DiscriminatorsEnum.Monthly)
            {
                discountPercentage = DiscountMonthly / 100M;
                priceWithoutTax = included ? ItemPriceMonthly / (1M + taxPercentage) : ItemPriceMonthly;
            }
            netPrice = priceWithoutTax * (1M - discountPercentage);
            discountValue = Math.Round(priceWithoutTax * discountPercentage, 2);
            vatAmount = netPrice * taxPercentage;
            total = Math.Round(netPrice + vatAmount, 2);
        }
       
        #endregion
    }



}
