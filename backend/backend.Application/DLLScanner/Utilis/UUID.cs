namespace backend.Application.DLLScanner.Utilis;

public class UUID
{
    public static string GenerateUUID()
    {
        var uuid = Guid.NewGuid();
        var uuidStr = uuid.ToString();
        return uuidStr;
    }
}
