using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_DAL.Models;

[Table("businesstype")]
public partial class Businesstype
{
    [Key]
    [Column("businesstypeid")]
    public int Businesstypeid { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [InverseProperty("Businesstype")]
    public virtual ICollection<Business> Businesses { get; } = new List<Business>();
}
