using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_DAL.Models;

[Table("token")]
public partial class Token
{
    [Key]
    [Column("tokenid")]
    public int Tokenid { get; set; }

    [Column("userid", TypeName = "character varying")]
    public string Userid { get; set; } = null!;

    [Column("createdate", TypeName = "timestamp without time zone")]
    public DateTime Createdate { get; set; }

    [Column("tokenexpire", TypeName = "timestamp without time zone")]
    public DateTime? Tokenexpire { get; set; }

    [Column("isused")]
    public bool Isused { get; set; }
}
