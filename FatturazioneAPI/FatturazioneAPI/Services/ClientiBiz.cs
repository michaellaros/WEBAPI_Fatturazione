using Microsoft.Data.SqlClient;
using RicevutaAPI.Models;
using System;
using RicevutaAPI.Models.Requests;

namespace RicevutaAPI.Services
{
    public class ClientiBiz
    {
        readonly IConfiguration configuration;
        private SqlConnection con { get; set; }
        public ClientiBiz(IConfiguration configuration)
        {
            this.configuration = configuration;
            con = new SqlConnection(configuration.GetSection("appsettings").GetValue<string>("RicevuteDB"));
            con.Open();

        }
        public List<ClientiModel> GetClienti(RicercaClienteRequest request)
        {
            List<ClientiModel> result = new List<ClientiModel>();

            string query = $"select * from TEST1_CLIENTE where nome ='{request.clientName}'" ;
            List<string> resultDB = new List<string>();
            using (SqlCommand cmd = new SqlCommand(query, con))
            {

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    while (reader.Read())
                    {
                        result.Add(new ClientiModel(reader["nome"].ToString(),
                            reader["cognome"].ToString(),
                            DateOnly.Parse(reader["data_nascita"].ToString()),
                            reader["indirizzo"].ToString(),
                            reader["email"].ToString()
                            ));
                    }

                }
            }
            return result;
        }
    }
}
