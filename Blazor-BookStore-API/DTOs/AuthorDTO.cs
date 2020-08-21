using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor_BookStore_API.DTOs {
    public class AuthorDTO {
        public int Id { get; set; }
        public string FirstNme { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }

        // Required to retrieve list of books published by given author.
        public virtual IList<BookDTO> Books { get; set; }
    }
}
