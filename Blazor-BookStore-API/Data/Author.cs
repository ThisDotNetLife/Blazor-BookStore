using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor_BookStore_API.Data {
    [Table("Authors")]
    public partial class Author {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }

        // Required to retrieve list of books published by given author.
        public virtual IList<Book> Books {get; set;}
    }
}
