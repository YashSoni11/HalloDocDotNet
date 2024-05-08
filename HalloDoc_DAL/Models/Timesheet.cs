using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_DAL.Models;

[Table("timesheet")]
public partial class Timesheet
{
    [Key]
    [Column("timesheetid")]
    public int Timesheetid { get; set; }

    [Column("physicianid")]
    public int Physicianid { get; set; }

    [Column("startdate")]
    public DateOnly Startdate { get; set; }

    [Column("enddate")]
    public DateOnly Enddate { get; set; }

    [Column("isfinalize")]
    public bool? Isfinalize { get; set; }

    [Column("isapproved")]
    public bool? Isapproved { get; set; }

    [Column("bonusamount")]
    [StringLength(128)]
    public string? Bonusamount { get; set; }

    [Column("adminnotes")]
    public string? Adminnotes { get; set; }

    [Column("createdby")]
    [StringLength(128)]
    public string Createdby { get; set; } = null!;

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [Column("modifiedby")]
    [StringLength(128)]
    public string? Modifiedby { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [ForeignKey("Createdby")]
    [InverseProperty("TimesheetCreatedbyNavigations")]
    public virtual Aspnetuser CreatedbyNavigation { get; set; } = null!;

    [ForeignKey("Modifiedby")]
    [InverseProperty("TimesheetModifiedbyNavigations")]
    public virtual Aspnetuser? ModifiedbyNavigation { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("Timesheets")]
    public virtual Physician Physician { get; set; } = null!;

    [InverseProperty("Timesheet")]
    public virtual ICollection<Timesheetdetail> Timesheetdetails { get; } = new List<Timesheetdetail>();
}
