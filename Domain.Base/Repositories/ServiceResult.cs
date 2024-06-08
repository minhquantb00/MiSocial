using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base.Repositories
{
    public class ServiceResult
    {
        private static readonly ServiceResult _success = new ServiceResult(true);

        public ServiceResult(params string[] errors) : this((IEnumerable<string>)errors)
        {
        }

        public ServiceResult(IEnumerable<string> errors)
        {
            if (errors == null)
            {
                errors = new[] { "Default result error message" };
            }
            Succeeded = false;
            Errors = errors;
        }

        protected ServiceResult(bool success)
        {
            Succeeded = success;
            Errors = new string[0];
        }

        public bool Succeeded { get; private set; }

        public IEnumerable<string> Errors { get; private set; }

        public object DataResult { get; set; }

        public static ServiceResult Success
        {
            get { return _success; }
        }

        public static ServiceResult Failed(params string[] errors)
        {
            return new ServiceResult(errors);
        }

        public override string ToString()
        {
            if (Succeeded)
                return "Succeeded";
            else
            {
                return string.Join(", ", Errors);
            }
        }
    }
}
