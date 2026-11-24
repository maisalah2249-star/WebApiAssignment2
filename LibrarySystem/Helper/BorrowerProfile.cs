using AutoMapper;
using LibrarySystem.Dtos.BorrowerDtos;
using LibrarySystem.Entities;

namespace LibrarySystem.Helper;

public class BorrowerProfile : Profile
{
    public BorrowerProfile()
    {
        CreateMap<Borrower, GetBorrowerResponse>();
        CreateMap<CreateBorrowerRequest, Borrower>();
        CreateMap<UpdateBorrowerRequest, Borrower>();
    }
}