using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TitanTechnologyView.Models
{
    public class VendorDto
    {
        public int? VendorId { get; set; }
        public string CompanyCode { get; set; }
        public string VendorName { get; set; }
        public string Address { get; set; }
        public string Gstn { get; set; }
        public string PanNumber { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonNumber { get; set; }
        public string PaymentTerms { get; set; }

        // Bank Details
        public string AccountHolderName { get; set; }
        public string AccountNumber1 { get; set; }
        public string IfscCode { get; set; }
        public string BankAccountName { get; set; }

        // File Uploads (ONLY for form post, never for JSON)
        [JsonIgnore] public IFormFile? GstnUpload { get; set; }
        [JsonIgnore] public IFormFile? PanUpload { get; set; }
        [JsonIgnore] public IFormFile? CancelledCheque { get; set; }
        [JsonIgnore] public IFormFile? Agreement1 { get; set; }
        [JsonIgnore] public IFormFile? Agreement2 { get; set; }
        [JsonIgnore] public IFormFile? Agreement3 { get; set; }
        [JsonIgnore] public IFormFile? Agreement4 { get; set; }

        // Existing file paths (map from API JSON string)
        [JsonProperty("gstnUpload")]
        public string? ExistingGstnUpload { get; set; }

        [JsonProperty("panUpload")]
        public string? ExistingPanUpload { get; set; }

        [JsonProperty("cancelledCheque")]
        public string? ExistingCancelledCheque { get; set; }

        [JsonProperty("agreement1")]
        public string? ExistingAgreement1 { get; set; }

        [JsonProperty("agreement2")]
        public string? ExistingAgreement2 { get; set; }

        [JsonProperty("agreement3")]
        public string? ExistingAgreement3 { get; set; }

        [JsonProperty("agreement4")]
        public string? ExistingAgreement4 { get; set; }
    }
}
