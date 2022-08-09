using AuthorsAPI.DTOs;
using AuthorsAPI.Model;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AuthorsAPI.Configurations
{
    public class MapperInitializer : Profile
    {
        public MapperInitializer()
        {
            CreateMap<BookModel, BookDTO>().ReverseMap();
            CreateMap<BookModel, CreateBookDTO>().ReverseMap();
            CreateMap<AuthorModel, AuthorDTO>().ReverseMap();
            CreateMap<AuthorModel, CreateAuthorDTO>().ReverseMap();
        }
    }
}
