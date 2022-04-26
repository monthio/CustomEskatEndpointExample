using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace EskatServer
{
  public interface IEncryptionProvider
  {
    public string Encrypt(string data);
  }
  public class EncryptionProvider : IEncryptionProvider
  {
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly EndpointOptions _endpointOptions;
    private readonly IMonthioPublicKeyProvider _publicKeyProvider;
    public EncryptionProvider(
      IMonthioPublicKeyProvider publicKeyProvider,
      IOptions<EndpointOptions> endpointOptions
    )
    {
      _publicKeyProvider = publicKeyProvider;
      _endpointOptions = endpointOptions.Value;
    }

    public string Encrypt(string data)
    {
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Audience = "monthio",
        Issuer = _endpointOptions.EndpointHost,
        Subject = new ClaimsIdentity(new List<Claim> { new Claim("sub", "monthio") }),
        Claims = new Dictionary<string, object> { { "xml", data } },
        EncryptingCredentials = new X509EncryptingCredentials(_publicKeyProvider.PublicKey())
      };
      return _tokenHandler.CreateEncodedJwt(tokenDescriptor);
    }
  }
}
