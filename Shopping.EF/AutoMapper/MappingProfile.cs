using AutoMapper;
using Shopping.Core.Dtos.BookDtos.RequestDtos;
using Shopping.Core.Dtos.BookDtos.ResponseDtos;
using Shopping.Core.Dtos.CartsDtos.ResponseDtos;
using Shopping.Core.Dtos.RequestDtos;
using Shopping.Core.Dtos.ResponseDtos;
using Shopping.Core.Dtos.StatisticsDtos.ResponseDtos;
using Shopping.Core.Models.AuthModule;
using Shopping.Core.Models.BookModule;
using Shopping.Core.Models.CartModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.EF.AutoMapper
{
    public class MappingProfile : Profile
    {
        public  MappingProfile()
        {
            CreateMap<SignUpDto, User>()
                .ForMember(src => src.Password, opt => opt.Ignore());
            CreateMap<SignUpDto, UserDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<AddCategoryDto, CategoryDto>();
            CreateMap<AddCategoryDto, Category>();
            CreateMap<Book, BookDto>();
            CreateMap<BookRequestDto, Book>()
                .ForMember(src => src.bookCategories, opt => opt.Ignore())
                .ForMember(src => src.Poster, opt => opt.Ignore());
            CreateMap<Book, CartDto>()
               .ForMember(src => src.WantedCopies, opt => opt.Ignore())
               .ForMember(src => src.Price, opt => opt.Ignore());
            CreateMap<CartBooks, CartBooksDto>();
            CreateMap<Book, TopRatedDto>()
                .ForMember(src => src.Categories, opt => opt.Ignore());
            CreateMap<Book, MostSoldBooksDto>()
               .ForMember(src => src.Categories, opt => opt.Ignore());
            CreateMap<Category, TopCategoryDto>();
        }
    }
}
