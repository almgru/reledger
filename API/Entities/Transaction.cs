using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Description { get; set; }

        [Required]
        public Account DebitAccount { get; set; }

        [Required]
        public Account CreditAccount { get; set; }

        public IEnumerable<Tag> Tags { get; set; }

        public IEnumerable<Attachment> Attachments { get; set; }
    }
}