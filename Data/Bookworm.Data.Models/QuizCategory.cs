namespace Bookworm.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Bookworm.Data.Common.Models;

    public class QuizCategory : BaseModel<string>
    {
        public QuizCategory()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Quizzes = new HashSet<Quiz>();
        }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; }
    }
}
