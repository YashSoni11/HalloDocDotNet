using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dotnetProc.Models;

public partial class User
{
    [Key]
    public short Id { get; set; }

    [StringLength(30)]
    public string UserName { get; set; } = null!;

    [StringLength(30)]
    public string Password { get; set; } = null!;
}
