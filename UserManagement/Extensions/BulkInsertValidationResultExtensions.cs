using FormHelper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Domain.Validator;

namespace UserManagement.Extensions
{
    public static class BulkInsertValidationResultExtensions
    {
        public static void UpdateModelState(this BulkInsertValidationResult validationResult, ModelStateDictionary modelState)
        {
            //modelState.Clear();
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    modelState.TryAddModelError($"[{error.Index}].{error.PropertyName}", error.ErrorMessage);
                }
            }
        }
        public static FormResult ToFormResult(this BulkInsertValidationResult validationResult)
        {
            var formResult = new FormResult(FormResultStatus.Success);
            if (!validationResult.IsValid)
            {
                formResult = new FormResult(FormResultStatus.Error)
                {
                    ValidationErrors = new List<FormResultValidationError>()
                };
                foreach (var error in validationResult.Errors)
                {
                    var formError = new FormResultValidationError
                    {
                        PropertyName = $"[{error.Index}].{error.PropertyName}",
                        Message = error.ErrorMessage
                    };
                    formResult.ValidationErrors.Add(formError);
                }
            }
            return formResult;
        }
    }
}
