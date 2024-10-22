using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class File:EntityBase
    {
        public File()
        {
            AddOns = new HashSet<AddOn>();
            Features = new HashSet<Feature>();
        }

        public int Id { get; set; }
        public Guid Name { get; set; }
        public string RealName { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public decimal FileSize { get; set; }
        public int FileTypeId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }

        public virtual FileType FileType { get; set; }
        public virtual ICollection<AddOn> AddOns { get; set; }
        public virtual ICollection<Feature> Features { get; set; }
    }
}
