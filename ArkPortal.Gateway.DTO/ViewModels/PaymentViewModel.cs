using System.ComponentModel.DataAnnotations;

namespace ArkPortal.Gateway.DTO.ViewModels
{
    public class PaymentViewModel
    {
        [Key]
        public Guid PaymentId { get; set; }
        public Guid Token { get; set; }
        public Guid RegistrationId { get; set; }
        public double Amount { get; set; }
        public string? CurrencyCode { get; set; }
        public string? ReasonForPayment { get; set; }
        public string? Status { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? PollUrl { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

}
