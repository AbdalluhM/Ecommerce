using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Entities
{
    public partial class TranslationDetail:EntityBase
    {
        public int Id { get; set; }
        public int TranslationHeaderId { get; set; }
        public int? LanguageId { get; set; }
        public string Translate { get; set; }
        public string FieldName { get; set; }

        public virtual Language Language { get; set; }
        public virtual TranslationHeader TranslationHeader { get; set; }
    }
}
