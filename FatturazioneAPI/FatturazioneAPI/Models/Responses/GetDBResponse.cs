namespace RicevutaAPI.Models.Responses
{
    public class GetDBResponse
    {
        public List<RicevutaModel> ricevute { get; set; } = new List<RicevutaModel>();
    }
}
