using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace API.Models
{
    public class PostItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Image { get; set; }
        public DateTime Creation { get; set; }
        public int ReadingTime { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public List<Tag> Tags { get; set; }

        // FRENCH
        public string FrenchTitle { get; set; }
        public string FrenchContent { get; set; }

        // ENGLISH
        public string EnglishTitle { get; set; }
        public string EnglishContent { get; set; }
    }
}
 