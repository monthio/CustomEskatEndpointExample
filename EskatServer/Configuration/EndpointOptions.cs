#nullable disable
namespace EskatServer
{
  public class EndpointOptions
  {
    public string Audience { get; set; }
    public string EndpointHost { get; set; }
    public string MonthioHost { get; set; }
    public bool UseEncryption { get; set; }
  }
}