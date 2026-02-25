using System.ComponentModel.DataAnnotations;

namespace ActivityClub.Web.ViewModels.MemberProfile
{
    public sealed class EditMemberProfileVm
    {
        [Required, StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? MobileNumber { get; set; }

        [StringLength(20)]
        public string? EmergencyNumber { get; set; }

        // not showing it in views, just to give it back to the Update DTO as it expects to get it 
        [Required]
        public DateOnly JoiningDate { get; set; }

        [StringLength(500)]
        public string? Photo { get; set; }

        public int? ProfessionId { get; set; }
        public int? NationalityId { get; set; }
    }
}