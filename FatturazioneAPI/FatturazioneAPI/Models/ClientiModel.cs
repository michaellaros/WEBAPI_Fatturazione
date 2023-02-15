namespace RicevutaAPI.Models
{
    public class ClientiModel
    {
        public List<ClientiModel> clienti { get; set; }
        public string clientSurname { get; set; }
        public string clientName { get; set; }
        
        public DateOnly birthDate { get; set; }
        public string clientAdress { get; set;}
        public string client_Email { get; set; }
        public ClientiModel(string clientSurname, string clientName,DateOnly birthDate,string clientAddress,string client_Email) {
            this.clientSurname = clientSurname;
            this.clientName = clientName;
            this.birthDate = birthDate;
            this.clientAdress = clientAddress;
            this.client_Email = client_Email;
            clienti = new List<ClientiModel>();
        }
    }
}
