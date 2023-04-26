using FatturazioneAPI.Models.Requests;
using FatturazioneAPI.Models;
using Microsoft.Data.SqlClient;

namespace FatturazioneAPI.Services
{
    public class DataBase
    {
        private SqlConnection con { get; set; }


        public DataBase(IConfiguration configuration)
        {
            con = new SqlConnection(configuration.GetSection("appsettings").GetValue<string>("connectionstring"));
            con.Open();

        }

        public List<ClientiModel> GetClienti(RicercaClienteRequest request)
        {

            List<ClientiModel> result = new List<ClientiModel>();

            string query = $@"select client_id,
                                    szFirstBusinessName business_name
                                    ,c.szTaxNmbr piva
                                    ,c.szITFiscalCode cf
                                    ,c.szITPassportNmbr passport_number
                                    , szLastName surname
                                    , szFirstName name
                                    , szITEmail email 
                                    , szStreetName address
                                    ,szCityName city
                                    ,szPostalLocationZipCode zipcode
                                    ,szITPostalLocationDistrictCode district_code
                                    ,szITPostalLocationCountryCode country_code
                                from dbo.ITInvoiceCustomerInfo c  
                                where isnull(c.szFirstBusinessName,'') like '{request.business_name}%' 
                                    and (c.szTaxNmbr like '{request.cf_piva_passport}%'
                                        or c.szITFiscalCode like '{request.cf_piva_passport}%'
                                        or c.szITPassportNmbr like '{request.cf_piva_passport}%') 
                                    and isnull(c.szLastName,'') like '{request.surname}%'
                                    and isnull(c.szFirstName,'') like '{request.name}%'
                                    and isnull(c.szITEmail,'') like '{request.email}%'
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
                        result.Add(new ClientiModel(int.Parse(reader["client_id"].ToString()!),
                            reader["business_name"].ToString()!,
                            reader["cf"].ToString()!,
                            reader["piva"].ToString()!,
                            reader["passport_number"].ToString()!,
                            reader["surname"].ToString()!,
                            reader["name"].ToString()!,
                            reader["email"].ToString()!,
                            reader["address"].ToString()!,
                            reader["city"].ToString()!,
                            reader["zipcode"].ToString()!,
                            reader["district_code"].ToString()!,
                            reader["country_code"].ToString()!

                            ));

                    }
                }
            }
            return result;
        }
        public ClientiModel GetCliente(int client_id)
        {


            string query = $@"select client_id,
                                    szFirstBusinessName business_name
                                    , c.szTaxNmbr piva
                                    , c.szITFiscalCode cf
                                    , c.szITPassportNmbr passport_number
                                    , szLastName surname
                                    , szFirstName name
                                    , szITEmail email 
                                    , szStreetName address
                                    ,szCityName city
                                    ,szPostalLocationZipCode zipcode
                                    ,szITPostalLocationDistrictCode district_code
                                    ,szITPostalLocationCountryCode country_code
                                from dbo.ITInvoiceCustomerInfo c  
                                where c.client_id = {client_id}";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return null;
                    }

                    while (reader.Read())
                    {
                        return new ClientiModel(int.Parse(reader["client_id"].ToString()!),
                            reader["business_name"].ToString()!,
                            reader["cf"].ToString()!,
                            reader["piva"].ToString()!,
                            reader["passport_number"].ToString()!,
                            reader["surname"].ToString()!,
                            reader["name"].ToString()!,
                            reader["email"].ToString()!,
                            reader["address"].ToString()!,
                            reader["city"].ToString()!,
                            reader["zipcode"].ToString()!,
                            reader["district_code"].ToString()!,
                            reader["country_code"].ToString()!

                            );
                    }
                }
            }
            return null;
        }

        public int GetReceiptNumber(int shopNumber)
        {
            string query = $@"select NEXT VALUE FOR dbo.GelMarket_Receipt_S{shopNumber} as receiptNumber";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return -1;
                    }

                    while (reader.Read())
                    {
                        return int.Parse(reader["receiptNumber"].ToString()!);

                    }
                }
            }
            return -1;
        }

        public void InsertFattura(RicevutaModel receipt, int client_id, string receiptNumber)

        {
            SqlTransaction sqlTransaction = con.BeginTransaction();
            try
            {
                string[] receiptNameSplit = receipt.nome_ricevuta.Split("_");
                int invoiceYear = DateTime.Now.Year;

                string insertITIvoiceFooter = $@"INSERT INTO [dbo].[ITInvoiceFooter]
                                                       ([lRetailStoreID]
                                                       ,[lInvoiceYear]
                                                       ,[szInvoiceNmbr]
                                                       ,[szUsageID]
                                                       ,[szDate]
                                                       ,[lInvoiceType]
                                                       ,[lOperatorID]
                                                       ,[szWorkstationID])
                                                 VALUES
                                                       ({receiptNameSplit[0]}
                                                           ,{invoiceYear}
                                                           ,'{receiptNumber}'
                                                           ,'IT_INVOICE'
                                                       ,'{DateTime.Now.ToString("yyyyMMdd")}'
                                                       ,0
                                                       ,{receipt.lOperatorID}
                                                       ,{GetStringOrNull(receipt.szWorkstationID)})";

                using (SqlCommand cmd = new SqlCommand(insertITIvoiceFooter, con, sqlTransaction))
                {




                    int resultITIvoiceFooter = cmd.ExecuteNonQuery();
                    if (resultITIvoiceFooter <= 0) { throw new Exception("Query didn't succeed"); }

                    string insertITInvoiceTransaction = $@"INSERT INTO [dbo].[ITInvoiceTransaction]
                                                       ([lRetailStoreID]
                                                       ,[lInvoiceYear]
                                                       ,[szInvoiceNmbr]
                                                       ,[szUsageID]
                                                       ,[lTxTaNmbr]
                                                       ,[lTxWorkstationNmbr]
                                                       ,[szTxDate]
                                                       ,[szTxTime]
                                                       ,[lFPtrReceiptNmbr])
                                                        values
                                                       ({receiptNameSplit[0]}
			                                            ,{invoiceYear}
			                                            ,'{receiptNumber}'
			                                            ,'IT_INVOICE'
                                                       ,{receiptNameSplit[2]}
                                                       ,{receiptNameSplit[1]}
                                                       ,{receiptNameSplit[3].Substring(0, 8)}
                                                       ,{receiptNameSplit[3].Substring(8, 6)}
                                                       ,'{receiptNumber}')"; //[lFPtrReceiptNmbr]) --todo_ora_szInvoiceNmbr 

                    cmd.CommandText = insertITInvoiceTransaction;
                    int resultITInvoiceTransaction = cmd.ExecuteNonQuery();
                    if (resultITInvoiceTransaction <= 0) { throw new Exception("Query didn't succeed"); }

                    string insertITIvoiceCustomer = $@"INSERT INTO [dbo].[ITInvoiceCustomer]
                                   ([lRetailStoreID]
                                   ,[lInvoiceYear]
                                   ,[szInvoiceNmbr]
                                   ,[szUsageID]
                                   ,[szCustomerID]
                                   ,[szFirstName]
                                   ,[szLastName]
                                   ,[szStreetName]
                                   ,[szPostalLocationZipCode]
                                   ,[szCityName]
                                   ,[szPhoneNmbr]
                                   ,[szTaxNmbr]
                                   ,[szFirstBusinessName]
                                   ,[szLanguageCode]
                                   ,[szITPostalLocationDistrictCode]
                                   ,[szITPostalLocationCountryCode]
                                   ,[szITFiscalCode]
                                   ,[szITPassportNmbr]
                                   ,[szExternalID]
                                   ,[szITCodDestinazione]
                                   ,[szITNumTel]
                                   ,[szITNote]
                                   ,[szITEmail])
		                        SELECT {receiptNameSplit[0]}
			                        ,{invoiceYear}
			                        ,'{receiptNumber}'
			                        ,'IT_INVOICE'
			                        ,ici.szCustomerID
			                        ,ici.szFirstName
			                        ,ici.szLastName
			                        ,ici.szStreetName
			                        ,ici.szPostalLocationZipCode
			                        ,ici.szCityName 
			                        ,ici.szPhoneNmbr
			                        ,ici.szTaxNmbr
			                        ,ici.szFirstBusinessName
			                        ,ici.szLanguageCode
			                        ,ici.szITPostalLocationDistrictCode
			                        ,ici.szITPostalLocationCountryCode
			                        ,ici.szITFiscalCode
			                        ,ici.szITPassportNmbr
			                        ,ici.szExternalID
			                        ,ici.szITCodDestinazione
			                        ,ici.szITNumTel
			                        ,ici.szITNote
			                        ,ici.szITEmail
		                        FROM ITInvoiceCustomerInfo ici WHERE ici.client_id = {client_id}";

                    cmd.CommandText = insertITIvoiceCustomer;
                    int resultITIvoiceCustomer = cmd.ExecuteNonQuery();
                    if (resultITIvoiceCustomer <= 0) { throw new Exception("Query didn't succeed"); }

                    string insertITInvoiceSaleLineItem = @$"INSERT INTO [dbo].[ITInvoiceSaleLineItem]
                                                           ([lRetailStoreID]
                                                           ,[lInvoiceYear]
                                                           ,[szInvoiceNmbr]
                                                           ,[szUsageID]
                                                           ,[lTxTaNmbr]
                                                           ,[lTxWorkstationNmbr]
                                                           ,[szTxDate]
                                                           ,[szTxTime]
                                                           ,[lTaSeqNmbr]
                                                           ,[szType]
                                                           ,[dTaPrice]
                                                           ,[dTaQty]
                                                           ,[dTaTotal]
                                                           ,[dTaDiscount]
                                                           ,[szItemID]
                                                           ,[szPOSItemID]
                                                           ,[szItemLookupCode]
                                                           ,[szDesc]
                                                           ,[dTurnover]
                                                           ,[szItemTaxGroupID])
                                                     VALUES";

                    string insertITInvoiceTax = @$"INSERT INTO [dbo].[ITInvoiceTax]
                                                            ([lRetailStoreID]
                                                            ,[lInvoiceYear]
                                                            ,[szInvoiceNmbr]
                                                            ,[szUsageID]
                                                            ,[szTaxGroupID]
                                                            ,[szItemTaxGroupID]
                                                            ,[szTaxGroupName]
                                                            ,[szTaxAuthorityID]
                                                            ,[szTaxAuthorityName]
                                                            ,[dPercent]
                                                            ,[szReceiptPrintCode]
                                                            ,[dIncludedTaxValue]
                                                            ,[dIncludedExactTaxValue]
                                                            ,[dTotalSale]
                                                            ,[dUsedTotalSale]
                                                            ,[dAmount]
                                                            ,[dFreeIncludedTaxValue]
                                                            ,[dFreeIncludedExactTaxValue])
                                                        VALUES";
                    /*
                     [dAmount]  --todo_0
                    ,[dFreeIncludedTaxValue] --todo_0
                    ,[dFreeIncludedExactTaxValue]) --todo_0
                     */



                    receipt.articoli.ForEach((article) =>
                    {
                        insertITInvoiceSaleLineItem += $@"({receiptNameSplit[0]}
                                                           ,{invoiceYear}
                                                           ,'{receiptNumber}'
                                                           ,'IT_INVOICE'
                                                           ,{receiptNameSplit[2]}
                                                           ,{receiptNameSplit[1]}
                                                           ,{receiptNameSplit[3].Substring(0, 8)}
                                                           ,{receiptNameSplit[3].Substring(8, 6)}
                                                           ,{article.lTaSeqNmbr}
                                                           ,'{receipt.szType}'
                                                           ,{article.dTaPrice}
                                                           ,{article.quantita}
                                                           ,{article.prezzo_totale_articolo}
                                                           ,{article.totalDiscount}
                                                           ,'{article.cod_articolo}'
                                                           ,'{article.szPOSItemID}'
                                                           ,'{article.szItemLookupCode}'
                                                           ,{GetStringOrNull(article.desc_articolo)}
                                                           ,{article.prezzo_totale_articolo - article.totalDiscount}
                                                           ,{article.ivaArticolo.groupId}),";

                    });

                    insertITInvoiceSaleLineItem = insertITInvoiceSaleLineItem.Substring(0, insertITInvoiceSaleLineItem.Length - 1);

                    cmd.CommandText = insertITInvoiceSaleLineItem;
                    int resultITITInvoiceSaleLineItem = cmd.ExecuteNonQuery();
                    if (resultITITInvoiceSaleLineItem <= 0) { throw new Exception("Query didn't succeed"); }

                    receipt.riepilogoIva.ForEach(iva =>
                    {
                        insertITInvoiceTax += $@"({receiptNameSplit[0]}
                                                           ,{invoiceYear}
                                                           ,'{receiptNumber}'
                                                           ,'IT_INVOICE'
                                                           ,'{iva.groupId}'
                                                           ,'{iva.groupId}'
                                                           ,{GetStringOrNull(iva.ivaGroup)}
                                                           ,{GetStringOrNull(iva.szTaxAuthorityID)}
                                                           ,{GetStringOrNull(iva.szTaxAuthorityName)}
                                                           ,'{iva.ivaPercent}'
                                                           ,{GetStringOrNull(iva.szReceiptPrintCode)}
                                                           ,{iva.ivaPrice}
                                                           ,{iva.dIncludedExactTaxValue}
                                                           ,{iva.dTotalSale}
                                                           ,{iva.dUsedTotalSale}
                                                           ,0
                                                           ,0
                                                           ,0),";
                    });

                    insertITInvoiceTax = insertITInvoiceTax.Substring(0, insertITInvoiceTax.Length - 1);
                    cmd.CommandText = insertITInvoiceTax;
                    int resultITInvoiceTax = cmd.ExecuteNonQuery();
                    if (resultITInvoiceTax <= 0) { throw new Exception("Query didn't succeed"); }



                    string insertITInvoiceTxJournal = $@"INSERT INTO [dbo].[ITInvoiceTxJournal]
                                                       ([lRetailStoreID]
                                                       ,[lTaNmbr]
                                                       ,[lWorkstationNmbr]
                                                       ,[szDate]
                                                       ,[szTime]
                                                       ,[lInvoiceYear]
                                                       ,[szInvoiceNmbr]
                                                       ,[szUsageID]
                                                       ,[lTimeStamp])
                                                 VALUES
                                                       ({receiptNameSplit[0]}
                                                       ,{receiptNameSplit[2]}
                                                       ,{receiptNameSplit[1]}
                                                       ,'{DateTime.Now.ToString("yyyyMMdd")}'
                                                       ,'{DateTime.Now.ToString("hhmmss")}'
                                                       ,{invoiceYear}
                                                       ,'{receiptNumber}'
                                                       ,'IT_INVOICE'
                                                       ,{DateTime.Now.ToString("yyyyMMddhhmmss")})";
                    cmd.CommandText = insertITInvoiceTxJournal;
                    int resultITInvoiceTxJournal = cmd.ExecuteNonQuery();
                    if (resultITInvoiceTxJournal <= 0) { throw new Exception("Query didn't succeed"); }

                    sqlTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                sqlTransaction.Rollback();
                throw ex;
            }


        }

        public int InsertClient(ClientiModel client)
        {
            string queryInsert = $@"INSERT INTO [dbo].[ITInvoiceCustomerInfo]
                                       ([szFirstName]
                                       ,[szLastName]
                                       ,[szFirstBusinessName]
                                       ,[szITFiscalCode]
                                       ,[szTaxNmbr]
                                       ,[szITPassportNmbr]
                                       ,[szCustomerID]
                                       ,[szPhoneNmbr]
                                       ,[szITNumTel]
                                       ,[szITEmail]
                                       ,[szStreetName]
                                       ,[szCityName]
                                       ,[szPostalLocationZipCode]
                                       ,[szLanguageCode]
                                       ,[szITPostalLocationDistrictCode]
                                       ,[szITPostalLocationCountryCode]
                                       ,[szITNote])
                                 VALUES
                                       ({GetStringOrNull(client.name)}
                                       ,{GetStringOrNull(client.surname)}
                                       ,{GetStringOrNull(client.business_name)}
                                       ,{GetStringOrNull(client.cf)}
                                       ,{GetStringOrNull(client.piva)}
                                       ,{GetStringOrNull(client.passport_number)}
                                       ,{GetStringOrNull(client.client_id)}
                                       ,{GetStringOrNull(client.cel_number)}
                                       ,{GetStringOrNull(client.tel_number)}
                                       ,{GetStringOrNull(client.email)}
                                       ,{GetStringOrNull(client.address)}
                                       ,{GetStringOrNull(client.city)}
                                       ,{GetStringOrNull(client.zipcode)}
                                       ,{GetStringOrNull(client.lang_code)}
                                       ,{GetStringOrNull(client.district_code)}
                                       ,{GetStringOrNull(client.country_code)}
                                       ,{GetStringOrNull(client.note)})";
            SqlTransaction transaction = con.BeginTransaction();
            int client_id;
            try
            {
                using (SqlCommand cmd = new SqlCommand(queryInsert, con, transaction))
                {
                    int result = cmd.ExecuteNonQuery();
                    if (result <= 0) { throw new Exception("Query didn't succeed"); }
                    cmd.CommandText = "Select SCOPE_IDENTITY() as client_id";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            client_id = int.Parse(reader["client_id"].ToString());
                        }
                        else
                        {
                            throw new Exception("Error after creating the client");
                        }
                    }
                }
                transaction.Commit();
                return client_id;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }


        }

        public void UpdateClient(ClientiModel client)
        {
            string queryUpdate = $@"UPDATE [dbo].[ITInvoiceCustomerInfo]
                                       SET [szFirstName] = {GetStringOrNull(client.name)}
                                          ,[szLastName] = {GetStringOrNull(client.surname)}
                                          ,[szFirstBusinessName] = {GetStringOrNull(client.business_name)}
                                          ,[szITFiscalCode]= {GetStringOrNull(client.cf)}
                                          ,[szTaxNmbr] = {GetStringOrNull(client.piva)}
                                          ,[szITPassportNmbr] = {GetStringOrNull(client.passport_number)}
                                          ,[szCustomerID] = {GetStringOrNull(client.client_id)}
                                          ,[szPhoneNmbr] = {GetStringOrNull(client.cel_number)}
                                          ,[szITNumTel] = {GetStringOrNull(client.tel_number)}
                                          ,[szITEmail] = {GetStringOrNull(client.email)}
                                          ,[szStreetName] = {GetStringOrNull(client.address)}
                                          ,[szCityName] = {GetStringOrNull(client.city)}
                                          ,[szPostalLocationZipCode] = {GetStringOrNull(client.zipcode)}
                                          ,[szLanguageCode] = {GetStringOrNull(client.lang_code)}
                                          ,[szITPostalLocationDistrictCode] = {GetStringOrNull(client.district_code)}
                                          ,[szITPostalLocationCountryCode] = {GetStringOrNull(client.country_code)}
                                          ,[szITCodDestinazione] = {GetStringOrNull(client.note)}
                                          ,[Update_date] = GETDATE()
                                        WHERE client_id = {client.id}";

            SqlTransaction transaction = con.BeginTransaction();
            int client_id;
            try
            {
                using (SqlCommand cmd = new SqlCommand(queryUpdate, con, transaction))
                {
                    int result = cmd.ExecuteNonQuery();
                    if (result <= 0) { throw new Exception("Query didn't succeed"); }

                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }


        }

        private string GetStringOrNull(string? str)
        {
            return string.IsNullOrWhiteSpace(str) ? "null" : $"'{str.Replace("'", "''")}'";
        }

    }
}
