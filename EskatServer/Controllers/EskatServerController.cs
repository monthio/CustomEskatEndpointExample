using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EskatServer.Controllers;

[ApiController]
[Route("")]
public class EskatServerController : ControllerBase
{
  private readonly IEskatDataProvider _eskatDataProvider;
  private readonly IEncryptionProvider _encryptionProvider;
  private readonly EndpointOptions _endpointOptions;
  public EskatServerController(
    IEskatDataProvider eskatDataProvider,
    IEncryptionProvider encryptionProvider,
    IOptions<EndpointOptions> endpointOptions
  )
  {
    _eskatDataProvider = eskatDataProvider;
    _encryptionProvider = encryptionProvider;
    _endpointOptions = endpointOptions.Value;
  }

  [HttpGet("")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public ActionResult<string> Get()
  {
    var applicantId = User.FindFirst("sub")?.Value;
    if (applicantId == null)
    {
      return BadRequest();
    }
    var rawXml = _eskatDataProvider.GetData(applicantId);
    if (rawXml == null)
    {
      return NotFound();
    }

    if (_endpointOptions.UseEncryption)
    {
      return new ContentResult
      {
        ContentType = "text/plain",
        Content = _encryptionProvider.Encrypt(rawXml),
        StatusCode = 200
      };
    }
    else
    {
      return new ContentResult
      {
        ContentType = "application/xml",
        Content = rawXml,
        StatusCode = 200
      };
    }
  }
}