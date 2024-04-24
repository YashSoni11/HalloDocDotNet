using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("taskman")]
public partial class Taskman
{
    [Key]
    [Column("taskid")]
    public int Taskid { get; set; }

    [Column("taskname", TypeName = "character varying")]
    public string? Taskname { get; set; }

    [Column("assignee", TypeName = "character varying")]
    public string? Assignee { get; set; }

    [Column("categoryid")]
    public int? Categoryid { get; set; }

    [Column("description", TypeName = "character varying")]
    public string? Description { get; set; }

    [Column("duedate", TypeName = "timestamp without time zone")]
    public DateTime? Duedate { get; set; }

    [Column("city", TypeName = "character varying")]
    public string? City { get; set; }
}
