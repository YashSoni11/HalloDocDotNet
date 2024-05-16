using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_DAL.Models;

[Table("chat")]
public partial class Chat
{
    [Key]
    [Column("chatid")]
    public long Chatid { get; set; }

    [Column("message")]
    [StringLength(500)]
    public string? Message { get; set; }

    [Column("adminid")]
    public int? Adminid { get; set; }

    [Column("physicianid")]
    public int? Physicianid { get; set; }

    [Column("requestid")]
    public int? Requestid { get; set; }

    [Column("sentdate", TypeName = "timestamp without time zone")]
    public DateTime? Sentdate { get; set; }

    [Column("sentby")]
    public int? Sentby { get; set; }

    [Column("chattype")]
    public int? Chattype { get; set; }
}
