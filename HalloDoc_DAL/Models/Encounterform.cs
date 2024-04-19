using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc_DAL.Models;

[Table("encounterform")]
public partial class Encounterform
{
    [Key]
    [Column("encounterformid")]
    public int Encounterformid { get; set; }

    [Column("firstname")]
    [StringLength(50)]
    public string Firstname { get; set; } = null!;

    [Column("lastname")]
    [StringLength(50)]
    public string? Lastname { get; set; }

    [Column("location", TypeName = "character varying")]
    public string? Location { get; set; }

    [Column("DOB", TypeName = "timestamp without time zone")]
    public DateTime? Dob { get; set; }

    [Column("phonnumber", TypeName = "character varying")]
    public string? Phonnumber { get; set; }

    [Column("email")]
    [StringLength(256)]
    public string? Email { get; set; }

    [Column("medications", TypeName = "character varying")]
    public string? Medications { get; set; }

    [Column("allergies", TypeName = "character varying")]
    public string? Allergies { get; set; }

    [Column("temperature", TypeName = "character varying")]
    public string? Temperature { get; set; }

    [Column("hr", TypeName = "character varying")]
    public string? Hr { get; set; }

    [Column("rr", TypeName = "character varying")]
    public string? Rr { get; set; }

    [Column("blood_pressure1", TypeName = "character varying")]
    public string? BloodPressure1 { get; set; }

    [Column("blood_pressure2", TypeName = "character varying")]
    public string? BloodPressure2 { get; set; }

    [Column("o2", TypeName = "character varying")]
    public string? O2 { get; set; }

    [Column("pain", TypeName = "character varying")]
    public string? Pain { get; set; }

    [Column("heent", TypeName = "character varying")]
    public string? Heent { get; set; }

    [Column("cv", TypeName = "character varying")]
    public string? Cv { get; set; }

    [Column("chest", TypeName = "character varying")]
    public string? Chest { get; set; }

    [Column("abd", TypeName = "character varying")]
    public string? Abd { get; set; }

    [Column("extr", TypeName = "character varying")]
    public string? Extr { get; set; }

    [Column("skin", TypeName = "character varying")]
    public string? Skin { get; set; }

    [Column("neuro", TypeName = "character varying")]
    public string? Neuro { get; set; }

    [Column("other", TypeName = "character varying")]
    public string? Other { get; set; }

    [Column("diognosis", TypeName = "character varying")]
    public string? Diognosis { get; set; }

    [Column("treatmentplan", TypeName = "character varying")]
    public string? Treatmentplan { get; set; }

    [Column("medications_dispesnsed", TypeName = "character varying")]
    public string? MedicationsDispesnsed { get; set; }

    [Column("procedures", TypeName = "character varying")]
    public string? Procedures { get; set; }

    [Column("followup", TypeName = "character varying")]
    public string? Followup { get; set; }

    [Column("requestid")]
    public int Requestid { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime? Createdat { get; set; }

    [Column("history", TypeName = "character varying")]
    public string? History { get; set; }

    [Column("medical_history", TypeName = "character varying")]
    public string? MedicalHistory { get; set; }

    [Column("isfinelized")]
    public bool IsFinelized { get; set; }
}
