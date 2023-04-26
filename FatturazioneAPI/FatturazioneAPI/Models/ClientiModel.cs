using System.Text.Json.Serialization;

namespace FatturazioneAPI.Models
{
    public class ClientiModel
    {
        public int? id { get; set; }
        public string business_name { get; set; }
        public string? cf { get; set; }
        public string? piva { get; set; }
        public string? passport_number { get; set; }
        public string? surname { get; set; }
        public string? name { get; set; }
        public string? client_id { get; set; }
        public string? email { get; set; }
        public string? tel_number { get; set; }

        public string? cel_number { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public string? lang_code { get; set; }
        public string district_code { get; set; }
        public string country_code { get; set; }
        public string? note { get; set; }

        public ClientiModel(int id, string business_name, string cf, string piva, string passport_number, string surname, string name, string email, string address, string city, string zipcode, string district_code, string country_code)
        {
            this.id = id;
            this.business_name = business_name;
            this.cf = cf;
            this.piva = piva;
            this.passport_number = passport_number;
            this.surname = surname;
            this.name = name;
            this.email = email;
            this.address = address;
            this.city = city;
            this.zipcode = zipcode;
            this.district_code = district_code;
            this.country_code = country_code;
        }

        [JsonConstructor]
        public ClientiModel() { }

    }
}
