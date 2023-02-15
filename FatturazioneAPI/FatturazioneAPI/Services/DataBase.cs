using System.Text;
using RicevutaAPI.Models;
using Microsoft.Data.SqlClient;

namespace RicevutaAPI.Services
{
    public class DataBase
    {
        readonly IConfiguration configuration;
        private SqlConnection con { get; set; }


        public DataBase(IConfiguration configuration)
        {
            this.configuration = configuration;
            con = new SqlConnection(configuration.GetSection("appsettings").GetValue<string>("RicevuteDB"));
            con.Open();

        }

        public RicevutaModel GetRicevuta_DB(string nomeFile)
        {

            RicevutaModel result = new RicevutaModel(nomeFile);
            string query = $"select a.desc_articolo,a.prezzo,a.quantita,a.flg_isDiscount from TEST1_RICEVUTA r join TEST1_ARTICOLO a on r.[ricevuta_id] = a.[ricevuta_id]  WHERE nome_ricevuta = '{nomeFile}'";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    while (reader.Read())
                    {
                        result.articoli.Add(new ArticoloModel(reader["desc_articolo"].ToString(), 
                                                                decimal.Parse(reader["prezzo"].ToString()), 
                                                                int.Parse(reader["quantita"].ToString()),
                                                                bool.Parse(reader["flg_isDiscount"].ToString())
                                                                
                                                                ));
                    }

                }
            }
            return result;
        }

        public List<RicevutaModel> GetRicevute()
        {
            List<RicevutaModel> result = new List<RicevutaModel>();
          
            string query = @"select nome_ricevuta from TEST1_RICEVUTA ";
            List<string> resultDB = new List<string>();
            using (SqlCommand cmd = new SqlCommand(query, con))
            {

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    while (reader.Read())
                    {
                        resultDB.Add(reader["nome_ricevuta"].ToString());
                    }

                }
            }
            foreach(string nome_ricevuta in resultDB)
            {
                result.Add(GetRicevuta_DB(nome_ricevuta));
            }
            return result;
        }



    }
}
