using System.Collections.Generic;

namespace Sanaclub.Application.Common.Exceptions;

public sealed class AppValidationException : Exception
{
    public AppValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("Uno o más campos no son válidos.")
    {
        Errors = errors;
    }

    public AppValidationException(string field, string error)
        : base("Uno o más campos no son válidos.")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { error } }
        };
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
