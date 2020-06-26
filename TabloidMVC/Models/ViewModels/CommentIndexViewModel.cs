using System.Collections.Generic;
namespace TabloidMVC.Models.ViewModels
{
    public class CommentIndexViewModel
    {
        public Post Post { get; set; }
        public Comment Comment { get; set; }
        public List<Comment> PostComments { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}