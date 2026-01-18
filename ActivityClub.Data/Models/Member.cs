using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Data.Models;

[Table("Member")]
[Index("UserId", Name = "UQ__Member__1788CC4D18F998EF", IsUnique = true)]
public partial class Member
{
    [Key]
    public int MemberId { get; set; }

    public int UserId { get; set; }

    [StringLength(150)]
    public string FullName { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? MobileNumber { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? EmergencyNumber { get; set; }

    public DateOnly JoiningDate { get; set; }

    [StringLength(500)]
    public string? Photo { get; set; }

    public int? ProfessionId { get; set; }

    public int? NationalityId { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("Member")]
    public virtual ICollection<EventMember> EventMembers { get; set; } = new List<EventMember>();

    [ForeignKey("NationalityId")]
    [InverseProperty("MemberNationalities")]
    public virtual Lookup? Nationality { get; set; }

    [ForeignKey("ProfessionId")]
    [InverseProperty("MemberProfessions")]
    public virtual Lookup? Profession { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Member")]
    public virtual User User { get; set; } = null!;
}
