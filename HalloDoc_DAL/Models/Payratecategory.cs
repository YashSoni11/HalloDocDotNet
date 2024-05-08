using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_DAL.Models;

[Table("payratecategory")]
public partial class Payratecategory
{
    [Key]
    [Column("payratecategoryid")]
    public int Payratecategoryid { get; set; }

    [Column("categoryname")]
    [StringLength(256)]
    public string Categoryname { get; set; } = null!;

    [InverseProperty("Payratecategory")]
    public virtual ICollection<Payratebyprovider> Payratebyproviders { get; } = new List<Payratebyprovider>();
}
