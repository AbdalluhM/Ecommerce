﻿using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class ViewMissingVersionPrice:EntityBase
    {
        public string CountryName { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyShortCode { get; set; }
        public int CountryCurrencyId { get; set; }
        public int PriceLevelId { get; set; }
        public string PriceLevelName { get; set; }
        public int NumberOfLicenses { get; set; }
        public int VersionId { get; set; }
        public string VersionName { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public int SubscriptionTypeId { get; set; }
        public int Id { get; set; }
    }
}