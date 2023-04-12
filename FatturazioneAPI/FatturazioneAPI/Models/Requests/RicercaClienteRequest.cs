namespace FatturazioneAPI.Models.Requests
{
    public class RicercaClienteRequest
    {
        public string? business_name { get; set; }
        public string? cf_piva { get; set; }
        public string? surname { get; set; }
        public string? name { get; set; }

        public string? email { get; set; }

        public bool IsEmpty()
        {
            return business_name == null && cf_piva == null && surname == null && name == null && email == null;
        }
    }
}
