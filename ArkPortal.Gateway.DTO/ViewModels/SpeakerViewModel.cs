using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArkPortal.Gateway.DTO.ViewModels
{
    public class SpeakerViewModel
    {
        [Key]
        public Guid SpeakerId { get; set; }
        public string? Prefix { get; set; }
        public string? FirstName { get; set; }
        public string? Lastname { get; set; }
        public string? Institution { get; set; }
        public string? Summery { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? TempImage { get; set; }
        public string? Details { get; set; }
        public string? Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpDatedOn { get; set; }
    }

}
