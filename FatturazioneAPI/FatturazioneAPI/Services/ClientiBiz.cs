using Microsoft.Data.SqlClient;
using FatturazioneAPI.Models;
using System;
using FatturazioneAPI.Models.Requests;

namespace FatturazioneAPI.Services
{
    public class ClientiBiz
    {
        readonly IConfiguration configuration;
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

            string query = $"select cognome,nome,data_nascita,indirizzo,email from TEST1_CLIENTE where nome ='{request.clientSurname}'" ;
            using (SqlCommand cmd = new SqlCommand(query, con))
            {

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    while (reader.Read())
                    {
                        result.Add(new ClientiModel(reader["cognome"].ToString(),
                            reader["nome"].ToString(),
                            DateTime.Parse(reader["data_nascita"].ToString()),
                            reader["indirizzo"].ToString(),
                            reader["email"].ToString()
                            ));
                        Console.WriteLine(result.Count);
                    }

                }
            }
            return result;
        }
    }
}
