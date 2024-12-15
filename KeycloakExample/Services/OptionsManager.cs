namespace KeycloakExample.Services;

public sealed class OptionsManager
{
    private readonly IdentityServerOption _identityOptions;

    public OptionsManager(IConfiguration configuration)
    {
        var section = configuration.GetSection("IdentityServer");

        _identityOptions = new IdentityServerOption
        {
            HostName = section["HostName"] ?? throw new Exception("HostName is required."),
            RealmName = section["RealmName"] ?? throw new Exception("RealmName is required."),
            ClientName = section["ClientName"] ?? throw new Exception("ClientName is required."),
            ClientSecret = section["ClientSecret"] ?? throw new Exception("ClientSecret is required."),
            ClientUUID = section["ClientUUID"] ?? throw new Exception("ClientUUID is required."),
        };
    }

    public IdentityServerOption GetIdentityServer()
    {
        return _identityOptions;
    }

    public string GetClientName => _identityOptions.ClientName;
}

public class IdentityServerOption
{
    public string HostName { get; set; } = null!;
    public string RealmName { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string ClientUUID { get; set; } = null!;
}

//public sealed class OptionsManager
//{
//    private static IConfiguration? _configuration;
//    private IdentityServerOption? _identityOptions;
//    private static readonly object _lock = new object(); // for thread safe

//    // Singleton Instance
//    private OptionsManager() { }
//    private static readonly Lazy<OptionsManager> _instance = new Lazy<OptionsManager>(() => new OptionsManager());
//    public static OptionsManager Instance => _instance.Value;

//    public static void Initialize(IConfiguration configuration)
//    {
//        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
//    }

//    public IdentityServerOption GetIdentityServer()
//    {
//        if (_identityOptions != null)
//        {
//            return _identityOptions;
//        }

//        if (_configuration == null)
//        {
//            throw new InvalidOperationException("OptionsManager is not initialized. Call Initialize() first.");
//        }

//        lock (_lock)
//        {
//            if (_identityOptions == null)
//            {
//                var section = _configuration.GetSection("IdentityServer");
//                _identityOptions = new IdentityServerOption
//                {
//                    HostName = section["HostName"] ?? throw new Exception("HostName is required."),
//                    RealmName = section["RealmName"] ?? throw new Exception("RealmName is required."),
//                    ClientName = section["ClientName"] ?? throw new Exception("ClientName is required."),
//                    ClientSecret = section["ClientSecret"] ?? throw new Exception("ClientSecret is required."),
//                    ClientUUID = section["ClientUUID"] ?? throw new Exception("ClientUUID is required."),
//                };
//            }
//        }

//        return _identityOptions;
//    }
//}