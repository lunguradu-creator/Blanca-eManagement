namespace Blanca_eManagement.Models
{
    public class CompanyInfoModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string FiscalCode { get; set; }
        public string TradeRegisterNumber { get; set; }
        public string Address { get; set; }
        public string Bank { get; set; }
        public string BankAccount { get; set; }
        public string LegalRepresentative { get; set; }
        public bool IsVATPayer { get; set; }
        public double VATPercentage { get; set; }
        public bool IsEditMode { get; set; } = true;
    }
}
