namespace Dakoq.Repository.Exceptions
{
    public sealed class SqlRowNotFoundException : RepositoryException
    {
        public SqlRowNotFoundException() : base() { }
        public SqlRowNotFoundException(string? message) : base(message) { }
        public SqlRowNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
