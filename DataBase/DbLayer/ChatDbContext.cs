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
        public DbSet<MessageRecipient> MessageRecipients { get; set; }
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

            modelBuilder.Entity<MessageRecipient>()
            .HasOne(mr => mr.Message)
            .WithMany(m => m.Recipients)
            .HasForeignKey(mr => mr.MessageId)
            .OnDelete(DeleteBehavior.Restrict);  

            modelBuilder.Entity<MessageRecipient>()
                .HasOne(mr => mr.Recipient)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(mr => mr.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            //без цих двох страшних функцій виникав конфлікт каскадних операцій видалення:
            //у нас в таблиці MessageRecipient є два зовнішні ключі: і обидва ці ключі можуть мати
            //каскадне видалення (тобто при видаленні користувача або повідомлення автоматично видаляються записи в MessageRecipients)
            //в цих фукціях по суті написанно "при видаленні User або Message не видаляти автоматично записи з MessageRecipient"
        }


    }
}
