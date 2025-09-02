using System.ComponentModel.DataAnnotations;

namespace TitanTechnologyView.Models
{
    public class CustomerFormDto
    {
        public int CustomerId { get; set; }
        [Required]
        public string CompanyCode { get; set; } = string.Empty;
        [Required]
        public string CustomerName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? Gstn { get; set; }
        public string? PanNumber { get; set; }
        public string? ContactPersonName { get; set; }
        public string? ContactPersonNumber { get; set; }
        public string? PaymentTerms { get; set; }

        // For file uploads
        public IFormFile? AgreementFile1 { get; set; }
        public IFormFile? AgreementFile2 { get; set; }
        public IFormFile? AgreementFile3 { get; set; }
        public IFormFile? AgreementFile4 { get; set; }
        // For displaying existing agreements (edit case)
        public string? Agreement1 { get; set; }
        public string? Agreement2 { get; set; }
        public string? Agreement3 { get; set; }
        public string? Agreement4 { get; set; }
    }
}
