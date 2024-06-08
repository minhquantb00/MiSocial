using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Shared
{
    public interface IUseCaseOutputBase
    {
        public IEnumerable<string> Errors { get; set; }
    }
    public class UseCaseOutputBase : IUseCaseOutputBase
    {
        private static readonly UseCaseOutputBase _success = new UseCaseOutputBase(true);

        public UseCaseOutputBase(params string[] errors) : this((IEnumerable<string>)errors)
        {
        }

        public UseCaseOutputBase(IEnumerable<string> errors)
        {
            if (errors == null)
            {
                errors = new[] { "Default result error message" };
            }
            Succeeded = false;
            Errors = errors;
        }

        protected UseCaseOutputBase(bool success)
        {
            Succeeded = success;
            Errors = new string[0];
        }

        public bool Succeeded { get; set; } = true;

        public IEnumerable<string> Errors { get; set; }

        public static UseCaseOutputBase Success
        {
            get { return _success; }
        }

        public static UseCaseOutputBase Failed(params string[] errors)
        {
            return new UseCaseOutputBase(errors);
        }

        public static UseCaseOutputBase Failed(IEnumerable<string> errors)
        {
            return new UseCaseOutputBase(errors);
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
