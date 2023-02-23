using Microsoft.Data.SqlClient;
using FatturazioneAPI.Models;
using System;
using FatturazioneAPI.Models.Requests;
using Microsoft.AspNetCore.Http.HttpResults;

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

            string query = $"select cognome,nome,data_nascita,indirizzo,email from TEST1_CLIENTE where cognome like '%{request.clientSurname}%' and nome like '%{request.clientName}%' and indirizzo like '%{request.clientAdress}%' and (data_nascita = '{request.birthDate?.AddHours(1)}' or isnull('{request.birthDate}','') = '')" ; //aggiunta un ora perche nel datepicker utilizza un fusorario diverso
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return result;
                    while (reader.Read())
                    {
                        result.Add(new ClientiModel(reader["cognome"].ToString(),
                            reader["nome"].ToString(),
                            DateTime.Parse(reader["data_nascita"].ToString()),
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
