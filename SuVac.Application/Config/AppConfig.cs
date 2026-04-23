namespace SuVac.Application.Config;

public class AppConfig
{
    public Crypto Crypto { get; set; } = default!;
}

public class Crypto
{
    public string Secret { get; set; } = default!;
}
