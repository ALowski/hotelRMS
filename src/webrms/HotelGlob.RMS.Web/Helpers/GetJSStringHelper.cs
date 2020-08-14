using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelGlob.RMS.Web.Helpers
{
    public static class GetJSStringHelper
    {
        public static MvcHtmlString GetJSString(this HtmlHelper helper, string target)
        {
            return MvcHtmlString.Create(target.Replace("\"", "\'").Replace(System.Environment.NewLine, ""));
        }
    }
}