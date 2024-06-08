using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Shared
{
    public static class ValidationModelExtensions
    {
        public static ValidationResultModel ToValidationResultModel(this ValidationResult validationResult)
        {
            return new ValidationResultModel(validationResult);
        }

        public static async Task<ValidationResult> HandleValidation<TRequest>(this IValidator<TRequest> validator, TRequest request)
        {
            var validationResult = await validator.ValidateAsync(request);
            return validationResult;
        }
    }
}
