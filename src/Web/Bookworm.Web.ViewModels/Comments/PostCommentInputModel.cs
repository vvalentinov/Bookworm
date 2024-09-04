namespace Bookworm.Web.ViewModels.Comments
{
    using System.ComponentModel.DataAnnotations;

    public class PostCommentInputModel
    {
        public int BookId { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
