using AutoMapper;
using LibrarySystem.Dtos.BookDtos;
using LibrarySystem.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibrarySystem.Helper;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, GetBookResponse>()
            .ForMember(dest => dest.AuthorName,
                opt => opt.MapFrom(src => src.Author.Name));

        CreateMap<CreateBookRequest, Book>();
        CreateMap<UpdateBookRequest, Book>();
    }
}