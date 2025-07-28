using AutoMapper;
using OnionWebApi.Application.Features.Brands.Commands.Create;
using OnionWebApi.Domain.Entities;

namespace OnionWebApi.Mapper.AutoMapper;

public class BrandProfile : Profile
{
    public BrandProfile()
    {
        CreateMap<CreateBrandCommandRequest, Brand>();
    }
}
