using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Shared
{
    public class ValidationException : Exception
    {
        public ValidationException(ValidationResultModel validationResultModel)
        {
            ValidationResultModel = validationResultModel;
        }

        public ValidationResultModel ValidationResultModel { get; }
    }
}
