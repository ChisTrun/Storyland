namespace backend.Domain.Utils
{
    public delegate void VoidDelegate();

    public class TryCatch
    {
        public static void Try(VoidDelegate v)
        {
            try
            {
                v();
            }
            catch { }
        }
    }
}
