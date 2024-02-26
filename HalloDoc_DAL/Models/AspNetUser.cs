using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_DAL.Models;

[Table("aspnetusers")]
public partial class Aspnetuser
{
    [Key]
    [Column("id")]
    [StringLength(128)]
    public string Id { get; set; } = null!;

    [Column("username")]
    [StringLength(256)]
    public string Username { get; set; } = null!;

    [Column("passwordhash", TypeName = "character varying")]
    public string? Passwordhash { get; set; }

    [Column("email")]
    [StringLength(256)]
    public string? Email { get; set; }

    [Column("phonenumber")]
    [StringLength(20)]
    public string? Phonenumber { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("resettoken", TypeName = "character varying")]
    public string? Resettoken { get; set; }

    [Column("toekenexpire", TypeName = "timestamp without time zone")]
    public DateTime? Toekenexpire { get; set; }

    [InverseProperty("Aspnetuser")]
    public virtual ICollection<Admin> AdminAspnetusers { get; } = new List<Admin>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Admin> AdminCreatedbyNavigations { get; } = new List<Admin>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Admin> AdminModifiedbyNavigations { get; } = new List<Admin>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Business> BusinessCreatedbyNavigations { get; } = new List<Business>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Business> BusinessModifiedbyNavigations { get; } = new List<Business>();

    [InverseProperty("Aspnetuser")]
    public virtual ICollection<Physician> PhysicianAspnetusers { get; } = new List<Physician>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Physician> PhysicianCreatedbyNavigations { get; } = new List<Physician>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Physician> PhysicianModifiedbyNavigations { get; } = new List<Physician>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Requestnote> RequestnoteCreatedbyNavigations { get; } = new List<Requestnote>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Requestnote> RequestnoteModifiedbyNavigations { get; } = new List<Requestnote>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<Shiftdetail> Shiftdetails { get; } = new List<Shiftdetail>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<Shift> Shifts { get; } = new List<Shift>();

    [InverseProperty("Aspnetuser")]
    public virtual ICollection<User> UserAspnetusers { get; } = new List<User>();

    [InverseProperty("CreatedbyNavigation")]
    public virtual ICollection<User> UserCreatedbyNavigations { get; } = new List<User>();

    [InverseProperty("ModifiedbyNavigation")]
    public virtual ICollection<User> UserModifiedbyNavigations { get; } = new List<User>();

    [ForeignKey("Userid")]
    [InverseProperty("Users")]
    public virtual ICollection<Aspnetrole> Roles { get; } = new List<Aspnetrole>();
}
