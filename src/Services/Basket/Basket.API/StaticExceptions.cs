namespace Basket.API;
public class StaticExceptions
{
    public const string GetException = "Failed to get records";
    public const string UpdateException = "Failed to update record";
    public const string DeleteException = "Failed to delete record";
    public const string CreateException = "Failed to create record";

}

public class StaticExceptions<TEntity>
{
    public static string GetException => string.Format("Failed to get {Entity} records", nameof(TEntity));
    public static string UpdateException => string.Format("Failed to update {Entity} record", nameof(TEntity));
    public static string DeleteException => string.Format("Failed to delete {Entity} record", nameof(TEntity));
    public static string CreateException => string.Format("Failed to create {Entity} record", nameof(TEntity));

}
