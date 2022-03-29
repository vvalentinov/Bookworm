namespace Bookworm.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Bookworm.Data.Common.Models;

    public class Answer : BaseDeletableModel<string>
    {
        public Answer()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string Content { get; set; }

        public bool IsCorrect { get; set; }

        [ForeignKey(nameof(Question))]
        public string QuestionId { get; set; }

        public virtual Question Question { get; set; }
    }
}
