using System.Text.Json.Serialization;

namespace FatturazioneAPI.Models
{
    public class ClientiModel
    {
        public string clientSurname { get; set; }
        public string clientName { get; set; }

        [JsonIgnore]
        public DateTime birthDateDt { get; set; }
        public string birthDate { get { return birthDateDt.ToString("dd/MM/yyyy"); } }
        public string clientAddress { get; set;}
        public string clientEmail { get; set; }
        public ClientiModel(string clientSurname, string clientName,DateTime birthDate,string clientAddress,string client_Email) {
            this.clientSurname = clientSurname;
            this.clientName = clientName;
            this.birthDateDt = birthDate;
            this.clientAddress = clientAddress;
            this.clientEmail = client_Email;
        }
    }
}
