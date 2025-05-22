using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    [Table("MessageRecipients")]
    public class MessageRecipient
    {
        [Key]
        public int Id { get; set; }

        public int MessageId { get; set; }
        public int RecipientId { get; set; }

        [ForeignKey(nameof(MessageId))]
        public virtual Message Message { get; set; } = null!;

        [ForeignKey(nameof(RecipientId))]
        public virtual User Recipient { get; set; } = null!;

        //два останні це навігаційні властивості. якщо ви як і я забули що це, то пояснюю
        //в моєму коді MessageId — це зовнішній ключ, який зберігає ID повідомлення, на яке посилається запис.
        //public virtual Message Message — це навігаційна властивість,
        //яка дає змогу напряму отримати об’єкт повідомлення, пов’язаний із цим записом.
        //Це по сути щось типу "швидкого посилання" на повязану таблицю.
    }
}
