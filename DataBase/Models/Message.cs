using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models
{
    [Table("Messages")]
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int RecipientId { get; set; }

        [Required]
        public string Content { get; set; } = null!;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //вказує, що значення цього поля обчислюється автоматично базою даних,
        //а не встановлюється вручну в коді
        public DateTime Timestamp { get; set; }

        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; } = null!;

        [ForeignKey("RecipientId")]
        public virtual User Recipient { get; set; } = null!;
    }
}
