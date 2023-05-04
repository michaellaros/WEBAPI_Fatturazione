namespace FatturazioneAPI.Models.Requests
{
    public class CheckInvoiceAlreadyDoneRequest
    {
        public string store_id { get; set; }
        public string workstation_id { get; set; }
        public string ta { get; set; }

        public CheckInvoiceAlreadyDoneRequest(string fileName)
        {
            string[] fileNameSplit = fileName.Substring(fileName.LastIndexOf("\\") + 1).Split('_');
            store_id = fileNameSplit[0];
            workstation_id = fileNameSplit[1];
            ta = fileNameSplit[2];
        }
    }
}
