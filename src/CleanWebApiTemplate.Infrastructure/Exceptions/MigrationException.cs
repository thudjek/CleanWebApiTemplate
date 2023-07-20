namespace CleanWebApiTemplate.Infrastructure.Exceptions;
public class MigrationException : Exception
{
    public MigrationException() : base("Error with migration")
    {

    }
    public MigrationException(string message) : base(message)
    {
    }
}