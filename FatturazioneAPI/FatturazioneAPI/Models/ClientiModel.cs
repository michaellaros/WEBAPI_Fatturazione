namespace FatturazioneAPI.Models
{
    public class ClientiModel
    {

        public string business_name { get; set; }
        public string cf_piva { get; set; }
        public string surname { get; set; }
        public string name { get; set; }
        public string email { get; set; }

        public ClientiModel(string business_name, string cf_piva, string surname, string name, string email)
        {
            this.business_name = business_name;
            this.cf_piva = cf_piva;
            this.surname = surname;
            this.name = name;
            this.email = email;
        }

    }
}
