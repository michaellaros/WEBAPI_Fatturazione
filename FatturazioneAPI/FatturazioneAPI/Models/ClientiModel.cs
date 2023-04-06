using System.Text.Json.Serialization;

namespace FatturazioneAPI.Models
{
    public class ClientiModel
    {
        public string surname { get; set; }
        public string name { get; set; }
        public string cf_piva { get; set; }
        public string email { get; set; }

        [JsonIgnore]
        public DateTime birthdayDt { get; set; }
        public string birthday { get { return birthdayDt.ToString("MM/dd/yyyy"); } }
        public string address { get; set; }

        public ClientiModel() { }
        public ClientiModel(string surname, string name, string cf_piva, string email, DateTime birthday, string address)
        {
            this.surname = surname;
            this.name = name;
            this.cf_piva = cf_piva;
            this.email = email;
            this.birthdayDt = birthday;
            this.address = address;

        }

    }
}
