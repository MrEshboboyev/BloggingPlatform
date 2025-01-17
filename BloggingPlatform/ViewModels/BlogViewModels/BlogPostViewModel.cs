﻿namespace BloggingPlatform.ViewModels.BlogViewModels
{
    public class BlogPostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string AuthorName { get; set; }
        public string AuthorId { get; set; }
    }
}
