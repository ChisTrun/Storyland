namespace ExporterEPUB.Helpers
{
    public static class StringExtension
    {
        public static string InverseSlash(this string str)
        {
            return str.Replace('\\', '/');
        }
    }
}
