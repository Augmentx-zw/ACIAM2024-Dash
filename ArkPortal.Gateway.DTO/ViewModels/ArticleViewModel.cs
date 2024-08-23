using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArkPortal.Gateway.DTO.ViewModels
{
    public class ArticleViewModel
    {
        [Key]
        public Guid ArticleId { get; set; }
        public string? Title { get; set; }
        public string? YoutubeId { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? TempImage { get; set; }
        public string? Story { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpDatedOn { get; set; }
    }
}
