using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Entities;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options): base(options) {

        }

      public DbSet<AppUser> Users {get;set;}

      public DbSet<UserLike> Likes { get; set; }

      public DbSet<Message> Messages { get; set; }

      protected override void OnModelCreating(ModelBuilder builder)
      {
        base.OnModelCreating(builder);

        builder.Entity<UserLike>()
          .HasKey(k => new {k.SourceUserId, k.LikedUserId});

        builder.Entity<UserLike>()
          .HasOne(s => s.SourceUser)
          .WithMany(l => l.LikedUser)
          .HasForeignKey(s => s.SourceUserId)
          .OnDelete(DeleteBehavior.NoAction);

         builder.Entity<UserLike>()
          .HasOne(s => s.LikedUser)
          .WithMany(l => l.LikedByUser)
          .HasForeignKey(s => s.LikedUserId)
          .OnDelete(DeleteBehavior.NoAction);

          builder.Entity<Message>()
          .HasOne(u => u.Recipient)
          .WithMany(l => l.MessagesReceived)
          .OnDelete(DeleteBehavior.Restrict);

           builder.Entity<Message>()
          .HasOne(u => u.Sender)
          .WithMany(l => l.MessagesSent)
          .OnDelete(DeleteBehavior.Restrict);
      }

        
    }
}