using System.Diagnostics.CodeAnalysis;

namespace Dakoq.Domain.Models
{
    public static class Optional
    {
        public static Optional<T> Create<T>(T value)
        {
            return new Optional<T> { HasValue = true, Value = value };
        }

        public static Optional<T> CreateNoValue<T>()
        {
            return new Optional<T> { HasValue = false };
        }
    }

    public readonly struct Optional<T>
    {
        [MemberNotNullWhen(true, nameof(Value))]
        public bool HasValue { get; internal init; }

        public T Value { get; internal init; }
    }
}
