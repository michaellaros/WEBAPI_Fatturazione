using Microsoft.Data.SqlClient;
using FatturazioneAPI.Models;
using FatturazioneAPI.Models.Requests;

namespace FatturazioneAPI.Services
{
    public class ClientiBiz
    {
        private readonly IConfiguration configuration;
        private SqlConnection con { get; set; }
        public ClientiBiz(IConfiguration configuration)
        {
            this.configuration = configuration;
            con = new SqlConnection(configuration.GetSection("appsettings").GetValue<string>("connectionstring"));
            con.Open();

        }
        public List<ClientiModel> GetClienti(RicercaClienteRequest request)
        {

            List<ClientiModel> result = new List<ClientiModel>();

            string query = $@"select cognome,nome,cf_piva,email,data_nascita,indirizzo from TEST1_CLIENTE 
                                where cognome like '%{request.clientSurname}%' 
                                and nome like '%{request.clientName}%' 
                                and indirizzo like '%{request.clientAddress}%' 
                                and (data_nascita = '{request.birthDateString}' or isnull('{request.birthDateString}','') = '')
                                and cf_piva like '%{request.cf_piva}%'
                                and email like '%{request.email}%'";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return result;
                    }

                    while (reader.Read())
                    {
                        result.Add(new ClientiModel(reader["cognome"].ToString()!,
                            reader["nome"].ToString()!,
                            reader["cf_piva"].ToString()!,
                            reader["email"].ToString()!,
                            DateTime.Parse(reader["data_nascita"].ToString()!),
                            reader["indirizzo"].ToString()!

                            ));

                    }
                }
            }
            return result;
        }
    }
}
