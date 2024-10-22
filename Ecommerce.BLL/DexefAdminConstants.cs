using System.Collections.Generic;
using System.ComponentModel;

namespace Ecommerce.BLL
{
    public class DXConstants
    {
        public static class General
        {
            public const int MaxVersionsCount = 3;
            public const string DexefServerIp = "185.44.64.217,1437";
            public const string BackupDbPath = @"C:\DB\DbUpdate.mtm";
            public const string BackupDbFilePath = @"C:\DB\DbUpdate_Files.mtm";
            //public const int MaxPaymentMethodCount = 3;
        }

        public static class DbSchemas
        {
            public const string Admin = "Admin";
            public const string LookUp = "LookUp";
            public const string Customer = "Customer";
            public static string ReferenceTableName = "[{0}].[{1}]";
        }

        public static class SupportedLanguage
        {
            public const string RequestHeader = "Accept-Language";
            public const string EN = "en";
            public const string AR = "ar";
        }
        public static class SupportedLanguageCulture
        {

            public const string EN = "en-US";
            public const string AR = "ar-EG";
        }
        public static class PasswordValidations
        {
            public const int MinLength = 8;
            public const int MaxLength = 500;
            public const string RegexContainsOneNumberOrMore = @".*\d{1}";
            public const string RegexContainsAlbhabeticCharacter = @"[a-zA-Z]+";
            public const string RegexContainsSpecialCharacter = @"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-^£# “”]"; //@"[-._!""`'#%&,:;<>=@{}~\$\(\)\*\+\/\\\?\[\]\^\|]+"; //@"[\_\.\$\~\&\@\!\#\%\*\~\^\?\+\-\=\=\;\'\{\}]";
        }

        public static class Regex
        {
            public const string Password = @"^(?=.*[!@#$%^&*()\-_=+`~\[\]{}?|])(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8,20}$";//from 8 to 20 characters
            public const string HttpUrl = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
        }
        public static class Images
        {
            #region Image Files and Their MIME Types
            //Here's a list of image files, their MIME types, and their file extensions.
            //Application MIME Type File Extension
            //Bitmap  image/bmp bmp
            //compiled source code image/cis-cod cod
            //graphic interchange format image/gif gif
            //image file  image/ief ief
            //JPEG image  image/jpeg jpe
            //JPEG image  image/jpeg jpeg
            //JPEG image  image/jpeg jpg
            //JPEG file interchange format    image/pipeg jfif
            //scalable vector graphic image/svg+xml svg
            //TIF image   image/tiff tif
            //TIF image   image/tiff tiff
            //Sun raster graphic image/x-cmu-raster ras
            //Corel metafile exchange image file image/x-cmx cmx
            //icon image/x-icon ico
            //portable any map image  image/x-portable-anymap pnm
            //portable bitmap image image/x-portable-bitmap pbm
            //portable graymap image image/x-portable-graymap pgm
            //portable pixmap image image/x-portable-pixmap ppm
            //RGB bitmap  image/x-rgb rgb
            //X11 bitmap  image/x-xbitmap xbm
            //X11 pixmap  image/x-xpixmap xpm
            //X-Windows dump image image/x-xwindowdump xwd
            #endregion
            public static List<string> SupportedImageFiles = new List<string>
            {"BMP", "cod","IEF","JPE", "JPG", "JPEG", "JFIF", "PNG", "GIF","SVG", "TIF", "TIFF", "RAS", "CMX", "PNM", "PBM", "PGM", "PPM","RGB", "XBM", "XPM", "XWD" ,"AVIF","WEBP"};
            public static List<string> SupportedContentTypes = new List<string> { "IMAGE" };
        }

        public enum MessageCodes

        {
            [Description("Success")]
            Success = 1000,
            [Description("Internal Server Error")]
            Failed = 2000,
            [Description("Failed To Fetch Data")]
            FailedToFetchData = 2001,
            [Description("There is NoPermission to Perform this Action")]
            UnAuthorizedAccess = 4000,
            //Exception
            [Description("Some Error Occurred")]
            Exception = 5000,

            //InputValidation
            [Description("Failed : Input Validation Error")]
            InputValidationError = 6000,
            [Description("Failed : {0} Is Required")]
            Required = 6001,
            [Description("Failed: {0}  Must Be Greater Than Zero")]
            GreaterThanZero = 6002,
            [Description("Length Validation Error")]
            LengthValidationError = 6003,
            [Description("Failed : {2} Must Be Between {0} And {1}")]
            InbetweenValue = 6004,
            [Description("Failed : {1} Must Be GreaterThan {0}")]
            InvalidMinLength = 6005,
            [Description("Failed : {1} Must Be LessThan {0}")]
            InvalidMaxLength = 6006,
            [Description("Failed : Invalid Email")]
            InvalidEmail = 6007,
            [Description("Failed :Invalid Items Count")]
            InvalidItemsCount = 6008,
            [Description("Failed :Invalid Logo")]
            InvalidLogo = 6009,
            [Description("Failed :Invalid Json")]
            InvalidJson = 6010,
            [Description("Failed :Invalid Json Empty Value")]
            InvalidJsonEmptyValue = 6011,
            [Description("Failed :Failed To Deserialize")]
            FailedToDeserialize = 6012,
            [Description("Failed :Missing Default Value")]
            MissingDefaultValue = 6013,
            [Description("Failed :Missing Arabic Value")]
            MissingArabicValue = 6014,
            [Description("Failed :Password should contain at least 1 digit")]
            MissingPasswordDigits = 6015,
            [Description("Failed :Password should contain at least one alphabetic character")]
            MissingPasswordAlphabetic = 6016,
            [Description("Failed :Password should contain at least one special characters Like { ., $, ~ ,&}")]
            MissingPasswordSpecialCharacters = 6017,
            [Description("Failed :Invalid Https Url")]
            InvalidHttpsUrl = 6018,
            [Description("Failed :Invalid File Type")]
            InvalidFileType = 6019,
            [Description("Failed :Invalid File Content Type")]
            InvalidFileContentType = 6020,
            [Description("Failed :Invalid File Size,, Must be less than 2 MB")]
            InvalidFileSize = 6021,
            [Description("Failed :Invalid Rate, Must be within 1 to 5")]
            InvalidRate = 6022,
            [Description("Failed :You Must select one at least")]
            InvalidItemsSelect = 6023,
            //Business Validation
            [Description("Failed : Business Validation Error")]
            BusinessValidationError = 7000,
            [Description("Failed : {0} Already Exists")]
            AlreadyExists = 7001,
            [Description("Failed : {0} Not Found")]
            NotFound = 7002,
            [Description("Failed : {0} Is DefaultForOther")]
            DefaultForOther = 7003,
            [Description("There're related data to this item")]
            RelatedDataExist = 7004,
            [Description("File type Is Not supported")]
            FileTypeNotSupported = 7005,
            [Description("Failed : Name Already Exists")]
            NameAlreadyExists = 7006,
            [Description("Failed : UserName Already Exists")]
            UserNameAlreadyExists = 7007,
            [Description("Failed : Email Already Exists")]
            EmailAlreadyExists = 7008,
            [Description("Failed : Can't Delete Admin User ")]
            CanNotDeleteAdminUser = 7009,
            [Description("Failed : English {0}  Already Exists")]
            AlreadyExistsEn = 7010,
            [Description("Failed : Arabic {0}  Already Exists")]
            AlreadyExistsAr = 7011,
            [Description("Failed : Invalid UserName or Password")]
            InvalidUserNameOrPassword = 7012,
            [Description("Failed : Invalid Password")]
            InvalidPassword = 7013,
            [Description("Failed :Employee Is InActive .. Activate then try again")]
            EmployeeIsInActive = 7014,
            [Description("Failed : {0} Is HiglightedVersion")]
            HiglightedVersion = 7015,
            [Description("Failed :{0} Is InActive .. Activate then try again")]
            InActiveEntity = 7016,
            [Description("Failed :{0} Is Default.. Not allowed delete")]
            DefaultEntity = 7017,
            [Description("Failed :You Already have 3 active versions,Please deactive one")]
            MaxActiveVersionsCountLimitExceeded = 7018,
            [Description("Failed :DownloadUrl or ReleaseNumber is already exisit")]
            VersionReleaseExisit = 7019,
            [Description("Failed :Mobile already verified")]
            MobileAlreadyVerified = 7020,
            [Description("Failed :Email already verified")]
            EmailAlreadyVerified = 7021,
            [Description("Failed :Send count exceeded")]
            SendCountExceeded = 7022,
            [Description("Failed :Phone code expired")]
            PhoneCodeExpired = 7023,
            [Description("Failed :Verification link expired")]
            VerificationLinkExpired = 7024,
            [Description("Failed :Invalid verification link")]
            InvalidVerificationLink = 7025,
            [Description("Failed :Invalid login credentials")]
            InvalidLoginCredentials = 7026,
            [Description("Failed :Invalid token")]
            InvalidToken = 7027,
            [Description("Failed :Token not expired yet")]
            TokenNotExpiredYet = 7028,
            [Description("Failed :Token and refresh token don't match")]
            TokensDoNotMatch = 7029,
            [Description("Failed :Email not verified")]
            EmailNotVerified = 7030,
            [Description("Failed :Password already defined for you before")]
            NewPasswordAlreadyDefined = 7031,
            [Description("Failed :Email is already primary")]
            EmailIsAlreadyPrimary = 7032,
            [Description("Failed :{0} Is InActive .. You must activate one at least")]
            ActiveEntityCount = 7033,
            [Description("Failed :{0} Is Default .. You must select another one to be default first")]
            DefaultEntityCount = 7034,
            [Description("Failed : Invalid password or link")]
            InvalidPasswordOrLink = 7035,
            [Description("Failed :{0} has unpaid invoices")]
            HasUnPaidInvoicesCount = 7036,
            [Description("Failed :There is an Unpaid {0} ")]
            HasUnPaidInvoices = 7037,
            [Description("Failed :Please, correct device serial")]
            OldEqualNewDevice = 7038,
            [Description("Failed : licenses limit has been exceeded")]
            ExceededLicensesLimit = 7039,
            [Description("Failed :You have exceeded the validity period")]
            ExceededPeriod = 7040,
            [Description("Failed : {0} status is Invalid")]
            InvalidInvoiceStatus = 7041,
            [Description("Failed : {0} Not Available in your Country")]
            NotAvailableInYourCountry = 7042,
            [Description("Failed : {0} Is Not Generated Yet")]
            IsNotGenerated = 7043,
            [Description("Failed : New Password not identical to Confirm Password")]
            MismatchNewConfirmPassword = 7044,
            [Description("Failed : license is not expired yet")]
            LicenseNotExpiredYet = 7045,
            [Description("Failed : There's no paid invoice.")]
            NoPaidInvoice = 7046,
            [Description("Failed : Must Purchase Version First.")]
            MustPurchaseVersionFirst = 7047,
            [Description("Failed : has more than one invoice")]
            HasMoreThanOneInvoice = 7048,
            [Description("Failed : Sorry the selected role is used.")]
            RoleAssign = 7049,
            [Description("Failed : request is already refunded.")]
            RequestAlreadyRefunded = 7050,
            [Description("Failed : request has been rejected.")]
            RequestAlreadyRejected = 7051,
            [Description("Failed : EndDate must be greater than StartDate.")]
            DatePeriod = 7052,
            [Description("Failed : Payment Failed.")]
            PaymentFailed = 7053,
            [Description("Failed : There're related addons have not been refunded yet.")]
            RelatedAddonNotRefunded = 7054,
            [Description("Failed : Invalid payment method.")]
            InvalidPaymentMethod = 7055,
            [Description("Failed : Can't Deactivate Featured Tag.")]
            DeactivateFeaturedTag = 7056,
            [Description("Failed :{0} Not Authroize show this page .")]
            PageNotAllowed = 7057,
            [Description("Failed :Page or Action not selected .")]
            PageNotSelected = 7058,
            [Description("Failed :You Must Select At least one country .")]
            CountrySelected = 7059,
            [Description("Failed :Email belongs to other mobile.")]
            EmailBelongsToOtherMobile = 7060,
            [Description("Failed :Mobile belongs to other email.")]
            MobileBelongsToOtherEmail = 7061,
            [Description("Failed :You must select paid versionSupscription.")]
            PaidVersionsubscription = 7062,
            [Description("Failed : This mobile is not verified.")]
            MobileNotVerified = 7063,
            [Description("Failed : The registration process is not completed.")]
            RegistrationProcessIncompleted = 7064,
            [Description("Failed : Invalid code.")]
            InvalidCode = 7065,
            [Description("Failed : Invalid source.")]
            InvalidSource = 7066,
            [Description("Failed : This customer is not verified.")]
            CustomerIsNotVerified = 7067,
            [Description("Failed : This action is not allowed.")]
            NotAllowed = 7068,
            [Description("Failed : This Old Mobile ID {0} is not Match With the Customer.")]
            InvalidOldMobileId = 7069,
            [Description("Failed : This email {0} is previously signed with Google.")]
            GoogleExist = 7070,
            [Description("Failed : This email {0} is previously signed with Apple.")]
            AppleExist = 7071,
            [Description("Failed : You have already workspace {0} .")]
            WorkSpaceExist = 7072,
        }
    }
}
