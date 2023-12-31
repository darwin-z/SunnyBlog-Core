﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CommentService.Domain;

namespace CommentService
{
    public partial class CommentDBContext : DbContext
    {
        public CommentDBContext(DbContextOptions<CommentDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<View> Views { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CreateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status)
                    .HasDefaultValueSql("((1))")
                    .HasComment("1审核通过2需要审核-1禁止评论");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Comments_Comments");
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.Property(e => e.Status)
                    .HasDefaultValueSql("((1))")
                    .HasComment("1点赞2收藏 3点赞又收藏");
            });

            modelBuilder.Entity<View>(entity =>
            {
                entity.Property(e => e.UserId).HasDefaultValueSql("((0))");

                entity.Property(e => e.ViewTime).HasDefaultValueSql("(getdate())");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}