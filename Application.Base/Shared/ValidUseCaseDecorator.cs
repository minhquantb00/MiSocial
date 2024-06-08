using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base.Shared
{
    public class ValidUseCaseDecorator<TUseCaseInput, TUseCaseOutput> : IUseCase<TUseCaseInput, TUseCaseOutput>
        where TUseCaseInput : class
        where TUseCaseOutput : UseCaseOutputBase, new()
    {
        private readonly IValidator<TUseCaseInput> _validator;
        private readonly IUseCase<TUseCaseInput, TUseCaseOutput> _inner;

        public ValidUseCaseDecorator(IValidator<TUseCaseInput> validator, IUseCase<TUseCaseInput, TUseCaseOutput> inner)
        {
            _validator = validator;
            _inner = inner;
        }

        public async Task<TUseCaseOutput> ExecuteAsync(TUseCaseInput request)
        {
            var validationResult = await _validator.HandleValidation(request);
            if (!validationResult.IsValid)
            {
                var useCaseOutput = new TUseCaseOutput();
                useCaseOutput.Succeeded = false;
                var errors = validationResult.Errors.Select(x => x.ErrorMessage);
                useCaseOutput.Errors = useCaseOutput.Errors.Concat(errors);
                return useCaseOutput;
            }
            return await _inner.ExecuteAsync(request);
        }
    }
}
