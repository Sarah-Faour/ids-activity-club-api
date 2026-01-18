using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Data.Models;

[Table("Event")]
public partial class Event
{
    [Key]
    public int EventId { get; set; }

    [StringLength(150)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    [StringLength(200)]
    public string Destination { get; set; } = null!;

    public DateOnly DateFrom { get; set; }

    public DateOnly DateTo { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Cost { get; set; }

    public int StatusId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("EventCategories")]
    public virtual Lookup Category { get; set; } = null!;

    [InverseProperty("Event")]
    public virtual ICollection<EventGuide> EventGuides { get; set; } = new List<EventGuide>();

    [InverseProperty("Event")]
    public virtual ICollection<EventMember> EventMembers { get; set; } = new List<EventMember>();

    [ForeignKey("StatusId")]
    [InverseProperty("EventStatuses")]
    public virtual Lookup Status { get; set; } = null!;
}
