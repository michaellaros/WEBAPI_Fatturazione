using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RicevutaAPI.Models.Requests
{
    public class RicercaClienteRequest
    {
        public string? clientSurname { get; set; }
        public string? clientName { get; set; }

        public DateOnly? birthDate { get; set; }
        public string? clientAdress { get; set; }
        public bool IsEmpty()
        {
            return clientSurname == null && clientName == null && birthDate == null && clientAdress == null;
        }
    }
}
