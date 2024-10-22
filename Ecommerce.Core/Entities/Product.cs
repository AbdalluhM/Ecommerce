using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class Product:EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TranslationHeaderId { get; set; }

        public virtual TranslationHeader TranslationHeader { get; set; }
    }
}
