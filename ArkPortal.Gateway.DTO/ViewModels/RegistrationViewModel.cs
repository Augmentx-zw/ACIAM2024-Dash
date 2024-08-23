using System.ComponentModel.DataAnnotations;

namespace ArkPortal.Gateway.DTO.ViewModels
{
    public class RegistrationViewModel
    {
        [Key]
        public Guid RegistrationId { get; set; }
        public Guid PaymentId { get; set; }
        public string? Prefix { get; set; }
        public string? Designation { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? StateOrProvince { get; set; }
        public string? ZipOrPostalCode { get; set; }
        public string? Country { get; set; }
        public string? RegistrationStatus { get; set; }

        //Institution
        public string? Institution { get; set; }
        public string? InstitutionAddress { get; set; }
        public string? InstitutionCity { get; set; }
        public string? InstitutionStateOrProvince { get; set; }
        public string? InstitutionZipOrPostalCode { get; set; }
        public string? InstitutionCountry { get; set; }
        public string? InstitutionContactNumber { get; set; }
        public int YearsEmployed { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpDatedOn { get; set; }
    }
}
