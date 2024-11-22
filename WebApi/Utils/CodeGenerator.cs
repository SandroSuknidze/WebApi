namespace WebApi.Utils
{
    public static class CodeGenerator
    {
        public static int GenerateVerificationCode()
        {
            Random random = new();
            return random.Next(100000, 999999);
        }
    }
}
