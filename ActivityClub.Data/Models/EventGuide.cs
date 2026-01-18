using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Data.Models;

[Table("EventGuide")]
[Index("EventId", "GuideId", Name = "UQ_EventGuide", IsUnique = true)]
public partial class EventGuide
{
    [Key]
    public int EventGuideId { get; set; }

    public int EventId { get; set; }

    public int GuideId { get; set; }

    public DateOnly AssignedDate { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("EventGuides")]
    public virtual Event Event { get; set; } = null!;

    [ForeignKey("GuideId")]
    [InverseProperty("EventGuides")]
    public virtual Guide Guide { get; set; } = null!;
}
