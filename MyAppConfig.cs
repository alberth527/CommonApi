namespace Comm
{
    public class MyAppConfig
    {
        public Jwtsettings JwtSettings { get; set; }
        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }
        public string Email { get; set; }
        public string ThemeColor { get; set; }
        public bool isRunSSL { get; set; }
        public int MaxNumber { get; set; }
    }

    public class Jwtsettings
    {
        public string Issuer { get; set; }
        public string SignKey { get; set; }
    }

    public class Logging
    {
        public Loglevel LogLevel { get; set; }
    }

    public class Loglevel
    {
        public string Default { get; set; }
    }

    public class Connectionstrings
    {

        public string Connsql { get; set; }


    }
}
