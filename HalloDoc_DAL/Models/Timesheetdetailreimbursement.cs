using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_DAL.Models;

[Table("timesheetdetailreimbursement")]
public partial class Timesheetdetailreimbursement
{
    [Key]
    [Column("timesheetdetailreimbursementid")]
    public int Timesheetdetailreimbursementid { get; set; }

    [Column("timesheetdetailid")]
    public int Timesheetdetailid { get; set; }

    [Column("itemname")]
    [StringLength(500)]
    public string Itemname { get; set; } = null!;

    [Column("amount")]
    public int Amount { get; set; }

    [Column("bill")]
    [StringLength(500)]
    public string Bill { get; set; } = null!;

    [Column("isdeleted")]
    public bool? Isdeleted { get; set; }

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
    [InverseProperty("TimesheetdetailreimbursementCreatedbyNavigations")]
    public virtual Aspnetuser CreatedbyNavigation { get; set; } = null!;

    [ForeignKey("Modifiedby")]
    [InverseProperty("TimesheetdetailreimbursementModifiedbyNavigations")]
    public virtual Aspnetuser? ModifiedbyNavigation { get; set; }

    [ForeignKey("Timesheetdetailid")]
    [InverseProperty("Timesheetdetailreimbursements")]
    public virtual Timesheetdetail Timesheetdetail { get; set; } = null!;
}
