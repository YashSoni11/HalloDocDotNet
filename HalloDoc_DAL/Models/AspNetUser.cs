using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_DAL.Models;

public partial class AspNetUser
{
    [Key]
    [StringLength(128)]
    public string Id { get; set; } = null!;

    [StringLength(256)]
    public string UserName { get; set; } = null!;

    [Column(TypeName = "character varying")]
    public string? PasswordHash { get; set; }

    [StringLength(256)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Column("IP")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [InverseProperty("AspNetUser")]
    public virtual ICollection<Admin> AdminAspNetUsers { get; } = new List<Admin>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Admin> AdminCreatedByNavigations { get; } = new List<Admin>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Admin> AdminModifiedByNavigations { get; } = new List<Admin>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Business> BusinessCreatedByNavigations { get; } = new List<Business>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Business> BusinessModifiedByNavigations { get; } = new List<Business>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Physician> PhysicianCreatedByNavigations { get; } = new List<Physician>();

    [InverseProperty("IdNavigation")]
    public virtual ICollection<Physician> PhysicianIdNavigations { get; } = new List<Physician>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Physician> PhysicianModifiedByNavigations { get; } = new List<Physician>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<RequestNote> RequestNoteCreatedByNavigations { get; } = new List<RequestNote>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<RequestNote> RequestNoteModifiedByNavigations { get; } = new List<RequestNote>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<ShiftDetail> ShiftDetails { get; } = new List<ShiftDetail>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Shift> Shifts { get; } = new List<Shift>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<User> UserCreatedByNavigations { get; } = new List<User>();

    [InverseProperty("IdNavigation")]
    public virtual ICollection<User> UserIdNavigations { get; } = new List<User>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<User> UserModifiedByNavigations { get; } = new List<User>();

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<AspNetRole> Roles { get; } = new List<AspNetRole>();
}
