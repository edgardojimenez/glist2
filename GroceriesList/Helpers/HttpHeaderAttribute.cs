using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroceriesList.Helpers {
    public class HttpHeaderAttribute : ActionFilterAttribute {
        public string Name { get; set; }

        public string Value { get; set; }

        public HttpHeaderAttribute(string name, string value) {
            Name = name;
            Value = value;
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext) {
            filterContext.HttpContext.Response.AppendHeader(Name, Value);
            base.OnResultExecuted(filterContext);
        }
    }
}