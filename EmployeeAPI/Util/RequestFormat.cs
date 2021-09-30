using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;

namespace EmployeeAPI.Util
{
    public static class RequestFormat
    {
        public static JsonMediaTypeFormatter JsonFormaterString()
        {
            var formatter = new JsonMediaTypeFormatter();
            return formatter;
        }
    }
}