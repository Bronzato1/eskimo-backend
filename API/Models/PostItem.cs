using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class PostItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Creation { get; set; }
        public int ReadingTime { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public List<Tag> Tags { get; set; }
    }
}
