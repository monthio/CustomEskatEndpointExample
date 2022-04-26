using Microsoft.AspNetCore.Authentication.JwtBearer;

using EskatServer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var endpointOptions = builder.Configuration.GetSection("EndpointConfig").Get<EndpointOptions>();

builder.Services.AddOptions<EndpointOptions>().Bind(builder.Configuration.GetSection("EndpointConfig"));

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IMonthioPublicKeyProvider, MonthioPublicKeyProvider>();
builder.Services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureTokenValidationParameters>();

builder.Services.AddTransient<IEncryptionProvider, EncryptionProvider>();
builder.Services.AddTransient<IEskatDataProvider, EskatDataProvider>();

builder.Services.AddAuthentication().AddJwtBearer(options =>
          {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
              ValidateAudience = true,
              ValidateIssuer = true,
              RequireExpirationTime = true,
              RequireSignedTokens = true,
              ValidateLifetime = true,
              ValidIssuer = $"https://{endpointOptions.MonthioHost}",
              ValidAudience = endpointOptions.EndpointHost
            };
          });


builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
