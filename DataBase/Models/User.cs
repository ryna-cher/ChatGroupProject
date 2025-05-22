using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "user"; //вибір між 'user' або 'admin'

        public bool IsBanned { get; set; } = false;

        public DateTime? BanUntil { get; set; }

        public bool IsApproved { get; set; } = false;

        public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public virtual ICollection<MessageRecipient> ReceivedMessages { get; set; } = new List<MessageRecipient>();
    }
}
