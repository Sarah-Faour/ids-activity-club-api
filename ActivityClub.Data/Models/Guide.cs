using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Data.Models;

[Table("Guide")]
[Index("UserId", Name = "UQ__Guide__1788CCAD8FD61B3B", IsUnique = true)]
public partial class Guide
{
    [Key]
    public int GuideId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(150)]
    public string FullName { get; set; } = null!;

    public DateOnly JoiningDate { get; set; }

    [StringLength(500)]
    public string? Photo { get; set; }

    [Column("ProfessionID")]
    public int? ProfessionId { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("Guide")]
    public virtual ICollection<EventGuide> EventGuides { get; set; } = new List<EventGuide>();

    [ForeignKey("ProfessionId")]
    [InverseProperty("Guides")]
    public virtual Lookup? Profession { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Guide")]
    public virtual User User { get; set; } = null!;
}
