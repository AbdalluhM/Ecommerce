using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class FileType:EntityBase
    {
        public FileType()
        {
            FileStorages = new HashSet<FileStorage>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<FileStorage> FileStorages { get; set; }
    }
}
