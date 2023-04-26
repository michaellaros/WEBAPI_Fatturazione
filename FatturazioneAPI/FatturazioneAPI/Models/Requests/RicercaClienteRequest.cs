namespace FatturazioneAPI.Models.Requests
{
    public class RicercaClienteRequest
    {
        public string? business_name { get; set; }
        public string? cf_piva_passport { get; set; }
        public string? surname { get; set; }
        public string? name { get; set; }

        public string? email { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(business_name) && string.IsNullOrEmpty(cf_piva_passport) && string.IsNullOrEmpty(surname) && string.IsNullOrEmpty(name) && string.IsNullOrEmpty(email);
        }
    }
}
