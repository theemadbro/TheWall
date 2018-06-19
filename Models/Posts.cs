using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheWall.Models
{
    public class Posts
    {
        [Key]
        public int id { get; set; }
        // [ForeignKey("users")]
        public int usersid { get; set; }
        public Users User { get; set; }
        public List<Comments> Comments {get; set; }
        [MinLength(2, ErrorMessage="Post too short!")]
        public string postContent { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime created_at { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime updated_at { get; set; }

    }
}
