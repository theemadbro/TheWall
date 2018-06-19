using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheWall.Models
{
    public class Comments
    {
        [Key]
        public int id { get; set; }
        public int userid { get; set; }
        public Users User { get; set; }
        public int postid {get; set;}
        public Posts Post {get; set; }
        public string comContent { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime created_at { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime updated_at { get; set; }

    }
}
