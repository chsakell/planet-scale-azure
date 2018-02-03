using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online.Store.Core
{
    public static class ModelStateExtensions
    {
        public static IEnumerable<Error> AllErrors(this ModelStateDictionary modelState)
        {
            var result = new List<Error>();
            var erroneousFields = modelState.Where(ms => ms.Value.Errors.Any())
                                            .Select(x => new { x.Key, x.Value.Errors });

            foreach (var erroneousField in erroneousFields)
            {
                var fieldKey = erroneousField.Key;
                var fieldErrors = erroneousField.Errors
                                   .Select(error => new Error(fieldKey, error.ErrorMessage));
                result.AddRange(fieldErrors);
            }

            return result;
        }

        public static string ListMessages(this ModelStateDictionary modelState)
        {
            string message = "<ul>";

            var errors = modelState.AllErrors();

            foreach(var error in errors)
            {
                message+= "<li>" + error.Message + " </li>";
            }

            message += "</ul>";

            return message;
        }
    }

    public class Error
    {
        public Error(string key, string message)
        {
            Key = key;
            Message = message;
        }

        public string Key { get; set; }
        public string Message { get; set; }
    }
}
