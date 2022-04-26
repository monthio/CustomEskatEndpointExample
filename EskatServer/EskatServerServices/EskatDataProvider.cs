namespace EskatServer
{
  public interface IEskatDataProvider
  {
    public string? GetData(string applicantId);
  }
  public class EskatDataProvider : IEskatDataProvider
  {
    public string? GetData(string applicantId)
    {
      // TODO find the data for the applicantId and return the xml.
      // Return null if no data for the applicant exists.
      return File.ReadAllText("test-data/example.xml");
    }
  }
}
