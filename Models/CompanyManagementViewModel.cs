namespace Blanca_eManagement.Models
{
    public class CompanyManagementViewModel
    {
        public required CompanyInfoModel CompanyInfo { get; set; }
        public List<VATCategory> VATCategories { get; set; }
        public bool IsEditMode { get; set; } 
    }
}
