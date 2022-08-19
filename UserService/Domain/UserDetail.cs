﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UserService.Domain
{
    [Table("UserDetail")]
    [Index("UserId", Name = "IX_UserDetail", IsUnique = true)]
    public partial class UserDetail
    {
        [Key]
        [Column("userId")]
        public int UserId { get; set; }
        [Column("nick")]
        [StringLength(50)]
        public string Nick { get; set; }
        [Column("sex")]
        public int? Sex { get; set; }
        [Column("birthday", TypeName = "date")]
        public DateTime? Birthday { get; set; }
        [Column("registerTime", TypeName = "datetime")]
        public DateTime? RegisterTime { get; set; }
        [Column("remark")]
        [StringLength(100)]
        public string Remark { get; set; }
        [Column("score", TypeName = "decimal(18, 1)")]
        public decimal Score { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("UserDetail")]
        public virtual User User { get; set; }
    }
}