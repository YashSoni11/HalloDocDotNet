using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_DAL.Models;

[Table("tokens")]
public partial class Token
{
    [Key]
    [Column("tokenid")]
    public int Tokenid { get; set; }

    [Column(TypeName = "character varying")]
    public string UserId { get; set; } = null!;

    [Column("isused")]
    public bool Isused { get; set; }

    [Column("tokenexpire", TypeName = "timestamp without time zone")]
    public DateTime Tokenexpire { get; set; }

    [Column("createdate", TypeName = "timestamp without time zone")]
    public DateTime? Createdate { get; set; }
}
