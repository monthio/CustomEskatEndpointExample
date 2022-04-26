using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace EskatServer
{
  public class ConfigureTokenValidationParameters : IPostConfigureOptions<JwtBearerOptions>
  {
    private readonly IMonthioPublicKeyProvider _publicKeyProvider;
    public ConfigureTokenValidationParameters(IMonthioPublicKeyProvider publicKeyProvider)
    {
      _publicKeyProvider = publicKeyProvider;
    }

    public void PostConfigure(string name, JwtBearerOptions options)
    {
      options.TokenValidationParameters.IssuerSigningKeyResolver = _publicKeyProvider.GetKeys;
    }

  }
}