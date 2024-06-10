namespace backend.Application.Plugins.DLLScanner.Utilis;

public class UUID
{
    public static string GenerateUUID()
    {
        Guid uuid = Guid.NewGuid();
        string uuidStr = uuid.ToString();
        return uuidStr;
    }
}
