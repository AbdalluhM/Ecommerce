using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class SimpleDatabas:EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string DataBaseName { get; set; }
    }
}
