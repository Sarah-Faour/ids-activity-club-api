using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Data.Models;

[Table("EventMember")]
[Index("EventId", "MemberId", Name = "UQ_EventMember", IsUnique = true)]
public partial class EventMember
{
    [Column("EventID")]
    public int EventId { get; set; }

    [Column("MemberID")]
    public int MemberId { get; set; }

    public DateOnly JoinDate { get; set; }

    [Key]
    public int EventMemberId { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("EventMembers")]
    public virtual Event Event { get; set; } = null!;

    [ForeignKey("MemberId")]
    [InverseProperty("EventMembers")]
    public virtual Member Member { get; set; } = null!;
}
