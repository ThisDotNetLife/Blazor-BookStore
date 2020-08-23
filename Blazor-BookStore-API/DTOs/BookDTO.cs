using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor_BookStore_API.DTOs {
    public class BookDTO {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public string ISBN { get; set; }
        public string Summary { get; set; }
        public string Cover { get; set; }
        public decimal? Price { get; set; }
        public int? AuthorId { get; set; }

        // Automatically retrieves record containing author of given book.
        public virtual AuthorDTO Author { get; set; }
    }

    public class BookCreateDTO {
        [Required]
        [StringLength(100)] 
        public string Title { get; set; }
        public int? Year { get; set; }
        [Required]
        [StringLength(50)] 
        public string ISBN { get; set; }
        [StringLength(500)]
        public string Summary { get; set; }
        [StringLength(150)]
        public string Cover { get; set; }
        public decimal? Price { get; set; }
        [Required]  
        public int? AuthorId { get; set; }
    }

    public class BookUpdateDTO {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        public int? Year { get; set; }
        [Required]
        [StringLength(500)]
        public string Summary { get; set; }
        [StringLength(150)]
        public string Cover { get; set; }
        public decimal? Price { get; set; }
        [Required]
        public int? AuthorId { get; set; }
    }
}
