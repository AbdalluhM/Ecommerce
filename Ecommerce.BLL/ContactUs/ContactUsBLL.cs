using AutoMapper;
using Dexef.Notification.Enums;
using Dexef.Notification.Models.Mailing;
using Microsoft.Extensions.Options;
using Ecommerce.BLL.Notifications;
using Ecommerce.BLL.Responses;
using Ecommerce.BLL.Validation.ContactUs;
using Ecommerce.DTO.ContactUs;
using Ecommerce.DTO.Settings.EmailTemplates;
using Ecommerce.DTO.Settings.Notifications.Mails;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static Ecommerce.BLL.DXConstants;

namespace Ecommerce.BLL.ContactUs
{
    public class ContactUsBLL : BaseBLL, IContactUsBLL
    {
        private readonly INotificationBLL _notificationBLL;
        private readonly IMapper _mapper;
        private readonly ContactUsConfig _contactUsConfig;
        private readonly EmailTemplateSetting _emailTemplateSetting;

        public ContactUsBLL(INotificationBLL notificationBLL,
                            IMapper mapper,
                            IOptions<ContactUsConfig> contactOptions,
                            IOptions<EmailTemplateSetting> emailTemplateSetting)
            : base(mapper)
        {
            _notificationBLL = notificationBLL;
            _mapper = mapper;
            _contactUsConfig = contactOptions.Value;
            _emailTemplateSetting = emailTemplateSetting.Value;
        }

        public async Task<IResponse<bool>> SendRequestAsync(NewContactRequestDto request)
        {
            var response = new Response<bool>();

            try
            {
                // => inputs validation.
                var validator = new NewContactRequestDtoValidator().Validate(request);

                if (!validator.IsValid)
                {
                    return response.CreateResponse(validator.Errors);
                }

                var emailResult = await _notificationBLL.SendEmailAsync(EmailProvider.Smtp,
                                                                        new MailContent
                                                                        {
                                                                            FromEmail = request.FromEmail,
                                                                            Subject = _contactUsConfig.Subject,
                                                                            DisplayName = _contactUsConfig.DisplayName,
                                                                            ToEmails = _contactUsConfig.ToEmails,
                                                                            Body = await GenerateMailBodyAsync(request)
                                                                        });

                if (emailResult.Errors.Any())
                {
                    response.CreateResponse(MessageCodes.Failed);
                    return response;
                }

                return response.CreateResponse(true);
            }
            catch (Exception ex)
            {
                return response.CreateResponse(ex);
            }
        }

        private async Task<string> GenerateMailBodyAsync(NewContactRequestDto request)
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var file = await File.ReadAllTextAsync(Path.Combine(assemblyFolder,
                                                                _emailTemplateSetting.BaseFilesPath,
                                                                _emailTemplateSetting.Folder.ContactUs.Name,
                                                                _emailTemplateSetting.Folder.ContactUs.HtmlFile));

            var replacedFile = file.Replace("%customerName%", request.FullName)
                                   .Replace("%customerHelp%", request.HelpMessage)
                                   .Replace("%customerCountry%", request.Country)
                                   .Replace("%customerMobile%", request.MobileNumber)
                                   .Replace("%customerEmail%", request.FromEmail)
                                   .Replace("%companyName%", request.CompanyName)
                                   .Replace("%companyIndustry%", request.Industry)
                                   .Replace("%companySize%", request.CompanySize)
                                   .Replace("%customerMessage%", request.Message)
                                   .Replace("%contactUsLink%",
                                        Path.Combine(_emailTemplateSetting.CLientBaseUrl,
                                                     _emailTemplateSetting.ClientMethod.ContactUs))
                                   .Replace("%dexefLogoImg%",
                                        Path.Combine(_emailTemplateSetting.ApiBaseUrl,
                                                     _emailTemplateSetting.BaseFilesPath,
                                                     _emailTemplateSetting.Folder.Image.Name,
                                                     _emailTemplateSetting.Folder.Image.Logo))
                                   .Replace("%tajawalBoldFont%",
                                        Path.Combine(_emailTemplateSetting.ApiBaseUrl,
                                                     _emailTemplateSetting.BaseFilesPath,
                                                     _emailTemplateSetting.Folder.Font.Name,
                                                     _emailTemplateSetting.Folder.Font.TajawalBold))
                                   .Replace("%tajawalRegularFont%",
                                        Path.Combine(_emailTemplateSetting.ApiBaseUrl,
                                                     _emailTemplateSetting.BaseFilesPath,
                                                     _emailTemplateSetting.Folder.Font.Name,
                                                     _emailTemplateSetting.Folder.Font.TajawalRegular));

            return replacedFile;
        }
    }
}
