using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_DAL.Models;

[Table("timesheetdetail")]
public partial class Timesheetdetail
{
    [Key]
    [Column("timesheetdetailid")]
    public int Timesheetdetailid { get; set; }

    [Column("timesheetid")]
    public int Timesheetid { get; set; }

    [Column("timesheetdate")]
    public DateOnly Timesheetdate { get; set; }

    [Column("totalhours")]
    public decimal? Totalhours { get; set; }

    [Column("isweekend")]
    public bool Isweekend { get; set; }

    [Column("numberofhousecall")]
    public int? Numberofhousecall { get; set; }

    [Column("numberofphonecall")]
    public int? Numberofphonecall { get; set; }

    [Column("modifiedby")]
    [StringLength(128)]
    public string? Modifiedby { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [ForeignKey("Modifiedby")]
    [InverseProperty("Timesheetdetails")]
    public virtual Aspnetuser? ModifiedbyNavigation { get; set; }

    [ForeignKey("Timesheetid")]
    [InverseProperty("Timesheetdetails")]
    public virtual Timesheet Timesheet { get; set; } = null!;

    [InverseProperty("Timesheetdetail")]
    public virtual ICollection<Timesheetdetailreimbursement> Timesheetdetailreimbursements { get; } = new List<Timesheetdetailreimbursement>();
}
