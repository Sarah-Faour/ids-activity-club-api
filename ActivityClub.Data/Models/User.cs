using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ActivityClub.Data.Models;

[Table("User")]
[Index("Email", Name = "UQ__Users__A9D105341E85B46A", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(150)]
    public string Name { get; set; } = null!;

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public int GenderLookupId { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("GenderLookupId")]
    [InverseProperty("Users")]
    public virtual Lookup GenderLookup { get; set; } = null!;

    [InverseProperty("User")]
    public virtual Guide? Guide { get; set; }

    [InverseProperty("User")]
    public virtual Member? Member { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
