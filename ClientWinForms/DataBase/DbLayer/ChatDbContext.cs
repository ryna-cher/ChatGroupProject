using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.DbLayer
{
    public class ChatDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        //колекція таблиць у базі данних

        public ChatDbContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        public ChatDbContext(DbContextOptions<ChatDbContext> options)
            : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(config.GetConnectionString("SqlClient"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //тут можна буде налаштовувати схему бази данних (аля зв'язки, складні ключі і тд.) 
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender) // Message має одну властивість Sender
            .WithMany(u => u.SentMessages) // User має багато SentMessages
            .HasForeignKey(m => m.SenderId) // і SenderId — це FK на Users.Id
            .OnDelete(DeleteBehavior.Restrict);  // при видаленні користувача – не видаляти його повідомлення

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Recipient) // Message має одну властивість Recipient
                .WithMany(u => u.ReceivedMessages) // User має багато ReceivedMessages
                .HasForeignKey(m => m.RecipientId) // і RecipientId — це FK на Users.Id
                .OnDelete(DeleteBehavior.Restrict); // при видаленні користувача – не видаляти його повідомлення


            //У нас є кілька зовнішніх ключів на одну і ту саму таблицю, тому треба явно вказати EF Core як вони зв'язані
            //без цього з'являлась помилка.
            //по суті, в цих функціях сказано “ось SenderId — це зв'язок з User, і ця навігація називається Sender,
            //а RecipientId — це інший User, і ця навігація називається Recipient”.
        }


    }
}
