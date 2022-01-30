using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UserManagement.Domain.Validator;

namespace UserManagement.Extensions
{
    public static class HTMLHelperExtensions
    {
        //IHtmlContent ValidationMessageFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, string message, object htmlAttributes);
        //public static IHtmlContent BulkValidationError<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, int index, IList<BulkInsertValidationFailure> Errors)
        //{
        //    //return String.Format("<label for='{0}'>{1}</label>", target, text);
        //}
        public static IHtmlContent BulkValidationError(this IHtmlHelper htmlHelper, string expression, int index, IList<BulkInsertValidationFailure> Errors)
        {
            var error = Errors.FirstOrDefault(t => t.Index == index && t.PropertyName == expression)?.ErrorMessage;
            var hcb = new HtmlContentBuilder();        
            if (!string.IsNullOrEmpty(error))
            {
                return new HtmlString(String.Format("<span class='field-validation-error text-danger' data-valmsg-for='[{0}].{1}' data-valmsg-replace='true'>{2}</span>", index, expression, error));
            }
            return new HtmlString(string.Empty);
        }
    }
}
