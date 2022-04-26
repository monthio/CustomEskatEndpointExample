using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace EskatServer
{
  public interface IMonthioPublicKeyProvider
  {
    public X509Certificate2? PublicKey();
    public IEnumerable<SecurityKey> GetKeys(
        string token, SecurityToken securityToken,
        string kid, TokenValidationParameters validationParameters
    );
  }
  public class MonthioPublicKeyProvider : IMonthioPublicKeyProvider
  {
    private readonly EndpointOptions _endpointOptions;
    private X509SecurityKey? CachedKey { get; set; } = null;
    private readonly HttpClient _client;
    private X509Certificate2? _PublicKey { get; set; }
    public MonthioPublicKeyProvider(HttpClient client, IOptions<EndpointOptions> endpointOptions)
    {
      _client = client;
      _endpointOptions = endpointOptions.Value;
    }

    public IEnumerable<SecurityKey> GetKeys(
       string token, SecurityToken securityToken,
       string kid, TokenValidationParameters validationParameters
   )
    {
      if (CachedKey?.KeyId != kid || kid == null)
      {
        CachedKey = UpdateKey(new JwtSecurityToken(token)).GetAwaiter().GetResult();
      }
      return new List<SecurityKey>() { CachedKey };
    }

    public X509Certificate2? PublicKey()
    {
      return _PublicKey;
    }

    private async Task<X509SecurityKey> UpdateKey(JwtSecurityToken token)
    {
      var keyUrl = token.Header["x5u"].ToString() ?? "";
      var keyHost = new Uri(keyUrl).Host;
      if (keyHost != _endpointOptions.MonthioHost)
      {
        throw new SecurityTokenInvalidSigningKeyException("Not Monthio hosted key");
      }
      var fullCert = await _client.GetStringAsync(keyUrl);
      var certData = fullCert.Split('\n').Where(x => !x.Contains("CERTIFICATE")).First();
      _PublicKey = new X509Certificate2(Convert.FromBase64String(certData));
      return new X509SecurityKey(_PublicKey);
    }
  }
}
