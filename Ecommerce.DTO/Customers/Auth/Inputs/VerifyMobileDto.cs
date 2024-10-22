namespace Ecommerce.DTO.Customers.Auth.Inputs
{
    public class VerifyMobileDto
    {
        public int MobileId { get; set; }

        public string Code { get; set; }

        public bool IsPrimaryMobile { get; set; }

        /// <summary>
        /// Determine calling this endpoint from old steps registration or from landing page registration.
        /// Default: False.
        /// </summary>
        public bool FromLandingPage { get; set; } = false;
    }
}
