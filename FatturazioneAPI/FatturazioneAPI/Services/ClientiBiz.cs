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

            string query = $@"select szFirstBusinessName business_name,isnull(c.szTaxNmbr,c.szITFiscalCode) cf_piva, szLastName surname, szFirstName name, szITEmail email from dbo.RECEIPT_CLIENT c 
                                where c.szFirstBusinessName like '{request.business_name}%' 
                                and isnull(c.szTaxNmbr,c.szITFiscalCode) like '{request.cf_piva}%' 
                                and c.szLastName like '{request.surname}%'
                                and c.szFirstName like '{request.name}%'
                                and c.szITEmail like '{request.email}%'
                                order by szFirstBusinessName";
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
                        result.Add(new ClientiModel(reader["business_name"].ToString()!,
                            reader["cf_piva"].ToString()!,
                            reader["surname"].ToString()!,
                            reader["name"].ToString()!,
                            reader["email"].ToString()!

                            ));

                    }
                }
            }
            return result;
        }
    }
}
