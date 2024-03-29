﻿using FatturazioneAPI.Models.Requests;
using FatturazioneAPI.Models;
using Microsoft.Data.SqlClient;
using FatturazioneAPI.Models.Responses;

namespace FatturazioneAPI.Services
{
    public class DataBase
    {
        private IConfiguration _configuration { get; set; }
        private string connectionstring => _configuration.GetSection("appsettings").GetValue<string>("connectionstring");

        public DataBase(IConfiguration configuration)
        {
            _configuration = configuration;

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
                                    , szCustomerID client_id
                                    , szITEmail email 
                                    , szITNumTel tel_number
                                    , szPhoneNmbr cel_number
                                    , szStreetName address
                                    , szCityName city
                                    , szPostalLocationZipCode zipcode
                                    , szITPostalLocationDistrictCode district_code
                                    , szITPostalLocationCountryCode country_code
                                    , szITCodDestinazione cod_destionazione
                                    , szITNote note
                                from dbo.ITInvoiceCustomerInfo c  
                                where isnull(c.szFirstBusinessName,'') like '%'+@business_name+'%' 
                                    and (c.szTaxNmbr like '%'+@cf_piva_passport+'%'
                                        or c.szITFiscalCode like '%'+@cf_piva_passport+'%'
                                        or c.szITPassportNmbr like '%'+@cf_piva_passport+'%') 
                                    and isnull(c.szLastName,'') like '%'+@surname+'%'
                                    and isnull(c.szFirstName,'') like  '%'+@name+'%'
                                    and isnull(c.szITEmail,'') like '%'+@email+'%'
                                order by szFirstBusinessName";
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@business_name", string.IsNullOrEmpty(request.business_name) ? "" : request.business_name);
                        cmd.Parameters.AddWithValue("@cf_piva_passport", string.IsNullOrEmpty(request.cf_piva_passport) ? "" : request.cf_piva_passport);
                        cmd.Parameters.AddWithValue("@surname", string.IsNullOrEmpty(request.surname) ? "" : request.surname);
                        cmd.Parameters.AddWithValue("@name", string.IsNullOrEmpty(request.name) ? "" : request.name);
                        cmd.Parameters.AddWithValue("@email", string.IsNullOrEmpty(request.email) ? "" : request.email);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {


                            while (reader.HasRows && reader.Read())
                            {
                                result.Add(new ClientiModel(
                                            int.TryParse(reader["client_id"].ToString(), out int clientId) ? clientId : 0,
                                            (reader["business_name"].ToString() == "NULL") ? null : reader["business_name"].ToString(),
                                            (reader["cf"].ToString() == "NULL") ? null : reader["cf"].ToString(),
                                            (reader["piva"].ToString() == "NULL") ? null : reader["piva"].ToString(),
                                            (reader["passport_number"].ToString() == "NULL") ? null : reader["passport_number"].ToString(),
                                            (reader["surname"].ToString() == "NULL") ? null : reader["surname"].ToString(),
                                            (reader["name"].ToString() == "NULL") ? null : reader["name"].ToString(),
                                            (reader["client_id"].ToString() == "NULL") ? null : reader["client_id"].ToString(),
                                            (reader["email"].ToString() == "NULL") ? null : reader["email"].ToString(),
                                            (reader["tel_number"].ToString() == "NULL") ? null : reader["tel_number"].ToString(),
                                            (reader["cel_number"].ToString() == "NULL") ? null : reader["cel_number"].ToString(),
                                            (reader["address"].ToString() == "NULL") ? null : reader["address"].ToString(),
                                            (reader["city"].ToString() == "NULL") ? null : reader["city"].ToString(),
                                            (reader["zipcode"].ToString() == "NULL") ? null : reader["zipcode"].ToString(),
                                            (reader["district_code"].ToString() == "NULL") ? null : reader["district_code"].ToString(),
                                            (reader["country_code"].ToString() == "NULL") ? null : reader["country_code"].ToString(),
                                            (reader["cod_destionazione"].ToString() == "NULL") ? null : reader["cod_destionazione"].ToString(),
                                            (reader["note"].ToString() == "NULL") ? null : reader["note"].ToString()
                                        ));

                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    con.Close();
                }
            }
            return result;
        }
        public ClientiModel GetCliente(int client_id)
        {


            string query = $@"select client_id,
                                    szFirstBusinessName business_name
                                    ,c.szTaxNmbr piva
                                    ,c.szITFiscalCode cf
                                    ,c.szITPassportNmbr passport_number
                                    , szLastName surname
                                    , szFirstName name
                                    , szCustomerID client_id
                                    , szITEmail email 
                                    , szITNumTel tel_number
                                    , szPhoneNmbr cel_number
                                    , szStreetName address
                                    , szCityName city
                                    , szPostalLocationZipCode zipcode
                                    , szITPostalLocationDistrictCode district_code
                                    , szITPostalLocationCountryCode country_code
                                    , szITCodDestinazione cod_destionazione
                                    , szITNote note
                                from dbo.ITInvoiceCustomerInfo c  
                                where c.client_id = @client_id";
            ClientiModel result = null;
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@client_id", client_id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {


                            while (reader.HasRows && reader.Read())
                            {
                                result = new ClientiModel(int.Parse(reader["client_id"].ToString()!),
                                    reader["business_name"].ToString()!,
                                    reader["cf"].ToString()!,
                                    reader["piva"].ToString()!,
                                    reader["passport_number"].ToString()!,
                                    reader["surname"].ToString()!,
                                    reader["name"].ToString()!,
                                    reader["client_id"].ToString()!,
                                    reader["email"].ToString()!,
                                    reader["tel_number"].ToString()!,
                                    reader["cel_number"].ToString()!,
                                    reader["address"].ToString()!,
                                    reader["city"].ToString()!,
                                    reader["zipcode"].ToString()!,
                                    reader["district_code"].ToString()!,
                                    reader["country_code"].ToString()!,
                                    reader["cod_destionazione"].ToString()!,
                                    reader["note"].ToString()!

                                    );
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    con.Close();
                }
            }
            return result;
        }

        public string InsertFattura(RicevutaModel receipt, int client_id, GetInfoTransazioneRequest? receiptInfo = null)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                try
                {

                    SqlTransaction sqlTransaction = con.BeginTransaction();
                    try
                    {
                        string[] receiptNameSplit = receipt.nome_ricevuta.Split("_");
                        int invoiceYear = DateTime.Now.Year;

                        string getReceiptNumber = $@"select NEXT VALUE FOR dbo.GelMarket_Receipt_S{receiptNameSplit[0]} as receiptNumber";
                        string receiptNumber;


                        using (SqlCommand cmd = new SqlCommand(getReceiptNumber, con, sqlTransaction))
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (!reader.HasRows)
                                {
                                    throw new Exception("Query didn't succeed");
                                }

                                if (!reader.Read())
                                {
                                    throw new Exception("Query didn't succeed");
                                }

                                receiptNumber = reader["receiptNumber"].ToString();
                                receiptNumber = "00000000".Substring(0, 8 - receiptNumber.Length) + receiptNumber;


                            }

                            string insertITIvoiceFooter = $@"INSERT INTO [dbo].[ITInvoiceFooter]
                                                       ([lRetailStoreID]
                                                       ,[lInvoiceYear]
                                                       ,[szInvoiceNmbr]
                                                       ,[szUsageID]
                                                       ,[szDate]
                                                       ,[lInvoiceType]
                                                       ,[lOperatorID]
                                                       ,[szWorkstationID]
                                {(receiptInfo != null ? @",[lParentRetailStoreID]
                                                        ,[lParentInvoiceYear]
                                                        ,[szParentInvoiceNmbr]
                                                            ,[szParentUsageID]" : "")})
                                                 VALUES
                                                       ({receiptNameSplit[0]}
                                                           ,{invoiceYear}
                                                           ,'{receiptNumber}'
                                                           ,'IT_INVOICE'
                                                       ,'{DateTime.Now.ToString("yyyyMMdd")}'
                                                       ,{(receiptInfo != null ? 1 : 0)/*SZTYPE 0 = FATTURA, 1 = NOTA DI CREDITO*/}
                                                       ,{receipt.lOperatorID}
                                                       ,{GetStringOrNull(receipt.szWorkstationID)}
                            {(receiptInfo != null ? @$",{receiptInfo.store_id}
                                                        ,{receiptInfo.receipt_year}
                                                        ,'{receiptInfo.receipt_number}'
                                                        ,'IT_INVOICE'" : "")})";



                            cmd.CommandText = insertITIvoiceFooter;
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
                                                       ,'{receipt.lFPtrReceiptNmbr}')";

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
                                                           ,null
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

                            if (receiptInfo == null)
                            {
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
                                                       ,'{receiptNameSplit[3].Substring(0, 8)}'
                                                       ,'{receiptNameSplit[3].Substring(8, 6)}'
                                                       ,{invoiceYear}
                                                       ,'{receiptNumber}'
                                                       ,'IT_INVOICE'
                                                       ,{DateTime.Now.ToString("yyyyMMddhhmmss")})";
                                cmd.CommandText = insertITInvoiceTxJournal;
                                int resultITInvoiceTxJournal = cmd.ExecuteNonQuery();
                                if (resultITInvoiceTxJournal <= 0) { throw new Exception("Query didn't succeed"); }
                            }



                            sqlTransaction.Commit();
                            return receiptNumber;
                        }
                    }
                    catch (Exception ex)
                    {
                        sqlTransaction.Rollback();
                        throw ex;
                    }


                }

                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    con.Close();
                }
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
                                       ,[szITCodDestinazione]
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
                                       ,{GetStringOrNull(client.cod_destinazione)}
                                       ,{GetStringOrNull(client.district_code)}
                                       ,{GetStringOrNull(client.country_code)}
                                       ,{GetStringOrNull(client.note)})";
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                try
                {
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

                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    con.Close();
                }
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
                                          ,[szITCodDestinazione] = {GetStringOrNull(client.cod_destinazione)}
                                          ,[szITPostalLocationDistrictCode] = {GetStringOrNull(client.district_code)}
                                          ,[szITPostalLocationCountryCode] = {GetStringOrNull(client.country_code)}
                                          ,[szITNote] = {GetStringOrNull(client.note)}
                                          ,[Update_date] = GETDATE()
                                        WHERE client_id = {client.id}";
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                try
                {
                    SqlTransaction transaction = con.BeginTransaction();
                    int client_id;
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(queryUpdate, con, transaction))
                        {
                            int result = cmd.ExecuteNonQuery();
                            if (result <= 0) { throw new Exception("UpdateClient didn't succeed"); }

                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public List<RicevutaStoricoModel> RicercaRicevutaStorico(RicercaRicevutaStoricoRequest request)
        {


            List<RicevutaStoricoModel> result = new List<RicevutaStoricoModel>();

            string query = $@"SELECT f.[szInvoiceNmbr] receipt_number
									  ,[if].szInvoiceNmbr storno_number
                                      ,f.[lInvoiceYear] receipt_year
                                      ,f.[lRetailStoreID] store_id
                                      ,f.[szDate] date
                                      ,f.[lInvoiceType] receipt_type
                                  FROM [dbo].[ITInvoiceFooter] f
								  LEFT JOIN ITInvoiceFooter [if] 
									ON f.lRetailStoreID = [if].lRetailStoreID
									AND f.lInvoiceYear = [if].lInvoiceYear
									AND f.szInvoiceNmbr = [IF].szParentInvoiceNmbr
									AND f.szUsageID = [if].szUsageID
                                WHERE  (f.szInvoiceNmbr = {GetStringOrNull(request.receipt_number)} OR ISNULL({GetStringOrNull(request.receipt_number)},'') = '')
	                                AND (f.szDate BETWEEN isnull({GetStringOrNull(request.date_start)} ,getdate())
                                        AND isnull({GetStringOrNull(request.date_end)} ,getdate())
                                    OR (ISNULL({GetStringOrNull(request.date_start)},'')='') and ISNULL({GetStringOrNull(request.date_end)},'')='') 
                                       {(request.store_id.HasValue ? @$"and (f.lRetailStoreID = {request.store_id.ToString()} )" : "")} ";
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.HasRows && reader.Read())
                            {
                                string dataStr = reader["date"].ToString()!;
                                result.Add(new RicevutaStoricoModel(reader["receipt_number"].ToString()!,
                                    reader["storno_number"].ToString(),
                                    reader["receipt_year"].ToString()!,
                                    reader["store_id"].ToString()!,
                                    new DateTime(int.Parse(dataStr.Substring(0, 4)), int.Parse(dataStr.Substring(4, 2)), int.Parse(dataStr.Substring(6, 2))),
                                    reader["receipt_type"].ToString()!
                                    ));
                            }
                        }

                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    con.Close();
                }
            }
            return result;
        }

        public GetInfoTransazioneResponse GetInfoTransazione(GetInfoTransazioneRequest request)
        {
            GetInfoTransazioneResponse result;

            string query = $@"SELECT it.lRetailStoreID store_id,it.lTxWorkstationNmbr workstation_id,it.lTxTaNmbr ta,it.szTxDate date,ici.client_id
                                FROM ITInvoiceTransaction it 
								JOIN ITInvoiceCustomer ic 
									ON it.lRetailStoreID = ic.lRetailStoreID 
									AND it.lInvoiceYear = ic.lInvoiceYear 
									AND it.szInvoiceNmbr = ic.szInvoiceNmbr 
									AND it.szUsageID = ic.szUsageID
								JOIN ITInvoiceCustomerInfo ici
									ON (ici.szTaxNmbr = ic.szTaxNmbr OR ici.szITFiscalCode = ic.szITFiscalCode OR ici.szITPassportNmbr = ic.szITPassportNmbr) 
                                WHERE it.lRetailStoreID = '{request.store_id}'
	                                AND it.lInvoiceYear = '{request.receipt_year}'
	                                AND it.szInvoiceNmbr = '{request.receipt_number}' 
                                ";
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                throw new Exception("Ricevuta non trovata");
                            }
                            if (!reader.Read())
                            {
                                throw new Exception("Ricevuta non trovata");
                            }
                            string dataStr = reader["date"].ToString()!;
                            result = new GetInfoTransazioneResponse(reader["store_id"].ToString()!,
                                    reader["workstation_id"].ToString()!,
                                    reader["ta"].ToString()!,
                                    request.date,
                                    int.Parse(reader["client_id"].ToString()!)
                                    );


                        }
                    }

                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    con.Close();
                }
            }
            return result;
        }

        public bool CheckInvoiceAlreadyDone(CheckInvoiceAlreadyDoneRequest request)
        {


            string query = $@"SELECT COUNT(1) rows
                              FROM [dbo].[ITInvoiceFooter] JOIN ITInvoiceTransaction it 
                              ON ITInvoiceFooter.lRetailStoreID = it.lRetailStoreID 
	                            AND ITInvoiceFooter.lInvoiceYear = it.lInvoiceYear 
	                            AND ITInvoiceFooter.szInvoiceNmbr = it.szInvoiceNmbr 
	                            AND ITInvoiceFooter.szUsageID = it.szUsageID
                            WHERE it.lRetailStoreID = '{request.store_id}'
	                            AND it.lTxWorkstationNmbr = '{request.workstation_id}'
	                            AND it.lTxTaNmbr = '{request.ta}' ";
            bool result;
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows && reader.Read())
                            {
                                result = int.Parse(reader["rows"].ToString()!) % 2 == 1;
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                    }
                }

                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    con.Close();
                }
            }
            return result;
        }

        public List<RicevutaSelectModel> RemoveReceiptWithInvoice(List<RicevutaSelectModel> list)
        {
            var x = list[0];
            var stt = @$"{x.nomeFile.Substring(0, x.nomeFile.LastIndexOf("_")).Split("\\").Last()}";
            return list.Where(x => !CheckInvoiceAlreadyDone(new CheckInvoiceAlreadyDoneRequest(@$"{x.nomeFile.Substring(0, x.nomeFile.LastIndexOf("_")).Split("\\").Last()}"))).ToList();
        }

        public List<RicevutaSelectModel> RemoveReceiptWithInvoiceOld(List<RicevutaSelectModel> list)
        {
            string stringList = "";
            list.ForEach(x => { stringList += @$"'{x.nomeFile.Substring(0, x.nomeFile.LastIndexOf("_")).Split("\\").Last()}',"; });
            stringList = stringList.Substring(0, stringList.Length - 1);
            string query = $@"SELECT DISTINCT convert(VARCHAR(10),it.lRetailStoreID )+ '_' + convert(VARCHAR(10),it.lTxWorkstationNmbr ) +'_'+convert(VARCHAR(10),it.lTxTaNmbr ) AS receipt_name
                                FROM ITInvoiceTransaction it 
                                WHERE convert(VARCHAR(10),it.lRetailStoreID )+ '_' + convert(VARCHAR(10),it.lTxWorkstationNmbr ) +'_'+convert(VARCHAR(10),it.lTxTaNmbr ) IN({stringList})";
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.HasRows && reader.Read())
                            {
                                list.RemoveAll(x => x.nomeFile.Substring(0, x.nomeFile.LastIndexOf("_")).Split("\\").Last() == reader["receipt_name"]!.ToString());
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    con.Close();
                }
            }
            return list;
        }

        public bool CheckValidProvince(string codice, string citta, string cod_paese, string provincia)
        {
            string query = $@"select 1
                            from ITInvoiceProvince 
                            where  codice = @codice
							and Citta = @citta
							and Cod_paese = @cod_paese
							and Provincia= @provincia";
            bool result = false;
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@codice", codice);
                        cmd.Parameters.AddWithValue("@citta", citta);
                        cmd.Parameters.AddWithValue("@cod_paese", cod_paese);
                        cmd.Parameters.AddWithValue("@provincia", provincia);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            result = reader.HasRows;


                        }
                    }

                }

                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    con.Close();
                }
            }
            return result;
        }

        public List<ProvinciaModel> GetProvince(string filter)
        {
            string query = $@"select top(50) Codice, Citta, Cod_paese,Provincia 
                            from ITInvoiceProvince 
                            where Citta_ricerca like @filter+'%'
							and isnull(codice,'')!=''
							and isnull(Citta,'')!=''
							and isnull(Cod_paese,'')!=''
							and isnull(Provincia,'')!=''";
            List<ProvinciaModel> result = new List<ProvinciaModel>();
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@filter", filter);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    result.Add(new ProvinciaModel()
                                    {
                                        codice = reader["Codice"].ToString()!,
                                        citta = reader["Citta"].ToString()!,
                                        cod_paese = reader["Cod_paese"].ToString()!,
                                        provincia = reader["Provincia"].ToString()!
                                    });
                                    //list.RemoveAll(x => x.nomeFile.Substring(0, x.nomeFile.LastIndexOf("_")).Split("\\").Last() == reader["receipt_name"]!.ToString());
                                }

                            }

                        }
                    }

                }

                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    con.Close();
                }
            }
            return result;
        }


        private string GetStringOrNull(string? str)
        {
            return string.IsNullOrWhiteSpace(str) ? "null" : $"'{str.Replace("'", "''")}'";
        }

    }
}
