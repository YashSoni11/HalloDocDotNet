using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("category")]
public partial class Category
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "character varying")]
    public string? Name { get; set; }
}
