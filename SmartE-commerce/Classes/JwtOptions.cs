namespace SmartE_commerce.Classes
{
    public class JwtOptions
    { 
            public string Issuer { get; set; }
            public string Audiencs { get; set; }
            public int LifeTime { get; set; }
            public string SiningKey { get; set; }
    }
}
