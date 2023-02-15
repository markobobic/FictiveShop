using FictiveShop.Core.Exceptions;
using FluentValidation;
using MediatR;

namespace FictiveShop.Core.Pipelines
{
    public class ValidatorPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : MediatR.IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidatorPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators == null) return next();
            if (!_validators.Any()) return next();

            // Invoke the validators
            var failures = _validators
                .Select(validator => validator.Validate(request))
                .SelectMany(result => result.Errors)
                .ToArray();

            if (failures.Any())
                throw new ValidatingException(
                    failures.Select(x => new ValidatingException.Error(x.PropertyName, x.ErrorMessage)));

            return next();
        }
    }
}