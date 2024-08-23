using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ArkPortal.Gateway.DTO.ViewModels;

namespace Ark.Gateway.Front.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ArkPortal.Gateway.DTO.ViewModels.AbstractViewModel> AbstractViewModel { get; set; } = default!;
        public DbSet<ArkPortal.Gateway.DTO.ViewModels.CommitteeViewModel> CommitteeViewModel { get; set; } = default!;
        public DbSet<ArkPortal.Gateway.DTO.ViewModels.RegistrationViewModel> RegistrationViewModel { get; set; } = default!;
        public DbSet<ArkPortal.Gateway.DTO.ViewModels.SpeakerViewModel> SpeakerViewModel { get; set; } = default!;
    }
}