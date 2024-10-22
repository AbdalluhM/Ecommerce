using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Reports.Enums
{
    public enum FormatType
    {
        [Description("application/pdf")]
        pdf,
        [Description("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        docx,
        [Description("application/vnd.ms-excel")]
        xls,
        [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        xlsx,
       
        rtf,
        [Description("message/rfc822")]
        mht,
        [Description("text/html")]
        html,
        [Description("text/plain")]
        txt,
        [Description("text/plain")]
        csv,
        [Description("image/png")]
        png
    }
}
