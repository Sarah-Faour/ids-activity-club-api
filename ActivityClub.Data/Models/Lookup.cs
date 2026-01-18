using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Data.Models;

[Table("Lookup")]
public partial class Lookup
{
    [Key]
    [Column("LookupID")]
    public int LookupId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [StringLength(150)]
    public string Name { get; set; } = null!;

    public int? SortOrder { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Event> EventCategories { get; set; } = new List<Event>();

    [InverseProperty("Status")]
    public virtual ICollection<Event> EventStatuses { get; set; } = new List<Event>();

    [InverseProperty("Profession")]
    public virtual ICollection<Guide> Guides { get; set; } = new List<Guide>();

    [InverseProperty("Nationality")]
    public virtual ICollection<Member> MemberNationalities { get; set; } = new List<Member>();

    [InverseProperty("Profession")]
    public virtual ICollection<Member> MemberProfessions { get; set; } = new List<Member>();

    [InverseProperty("GenderLookup")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
