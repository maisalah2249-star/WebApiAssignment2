using AutoMapper;
using LibrarySystem.Dtos.AuthorDtos;
using LibrarySystem.Entities;

namespace LibrarySystem.Helper;

public class AuthorProfile : Profile
{
    public AuthorProfile()
    {
        CreateMap<Author, GetAuthorResponse>();
        CreateMap<CreateAuthorRequest, Author>();
        CreateMap<UpdateAuthorRequest, Author>();
    }
}