using AutoMapper;
using Blazor_BookStore_API.Data;
using Blazor_BookStore_API.DTOs;

namespace Blazor_BookStore_API.Mappings {
    public class Maps : Profile {
        public Maps() {
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Author, AuthorCreateDTO>().ReverseMap();
            CreateMap<Author, AuthorUpdateDTO>().ReverseMap();
            CreateMap<Book, BookDTO>().ReverseMap();
        }
    }
}
