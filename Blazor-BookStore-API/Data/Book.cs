using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor_BookStore_API.Data {
    [Table("Books")]
    public partial class Book {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public string ISBN { get; set; }
        public string Summary { get; set; }
        public string Cover { get; set; }
        public decimal? Price { get; set; }
        public int? AuthorId { get; set; }

        // Automatically retrieves record containing author of given book.
        public virtual Author Author { get; set; }
    }
}
