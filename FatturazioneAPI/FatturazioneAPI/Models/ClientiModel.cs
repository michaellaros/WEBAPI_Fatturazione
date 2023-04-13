using System.Text.Json.Serialization;

namespace FatturazioneAPI.Models
{
    public class ClientiModel
    {
        public int id { get; set; }
        public string business_name { get; set; }
        public string cf_piva { get; set; }
        public string surname { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        [JsonIgnore]
        public string address { get; set; }
        [JsonIgnore]
        public string city { get; set; }
        [JsonIgnore]
        public string zipcode { get; set; }
        [JsonIgnore]
        public string district_code { get; set; }
        [JsonIgnore]
        public string country_code { get; set; }

        public ClientiModel(int id, string business_name, string cf_piva, string surname, string name, string email, string address, string city, string zipcode, string district_code, string country_code)
        {
            this.id = id;
            this.business_name = business_name;
            this.cf_piva = cf_piva;
            this.surname = surname;
            this.name = name;
            this.email = email;
            this.address = address;
            this.city = city;
            this.zipcode = zipcode;
            this.district_code = district_code;
            this.country_code = country_code;
        }

    }
}
