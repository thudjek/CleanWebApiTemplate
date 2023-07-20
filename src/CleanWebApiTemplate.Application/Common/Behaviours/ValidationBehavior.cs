using FluentValidation;
using MediatR;

namespace CleanWebApiTemplate.Application.Common.Behaviours;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : notnull
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var validationErrors = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToList());

            if (validationErrors.Any())
            {
                var error = new Error(validationErrors);
                var failResultMethod = typeof(TResponse).GetMethod(nameof(Result.Fail), new[] { typeof(Error) });
                return failResultMethod.Invoke(null, new object[] { error }) as TResponse;
            }
        }

        return await next();
    }
}