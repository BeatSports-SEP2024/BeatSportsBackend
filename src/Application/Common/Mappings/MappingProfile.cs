using System.Reflection;
using AutoMapper;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.RoomMemberResponse;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Common.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        //Account Mapping
        CreateMap<Account, AccountResponse>()
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                src.Customer != null && !src.Customer.IsDelete ? src.Customer.Address :
                src.Owner != null && !src.Owner.IsDelete ? src.Owner.Address : null));

        //CreateMap<Account, AccountResponse>()
        //    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Customer.Address));

        CreateMap<Customer, CustomerResponse>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id));
        CreateMap<Customer, AccountResponse>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Owner, OwnerResponse>()
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Id));
        CreateMap<Owner, AccountResponse>()
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Id));

        //Level Mapping
        CreateMap<Level, LevelResponse>()
            .ForMember(dest => dest.LevelId, opt => opt.MapFrom(src => src.Id));

        //Wallet mapping for Wallet Response
        CreateMap<Wallet, WalletResponse>()
            .ForMember(dest => dest.WalletId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Wallet, CustomerResponse>()
            .ForMember(dest => dest.WalletId, opt => opt.MapFrom(src => src.Id));

        //CourtSport Mapping
        //CreateMap<CourtSubdivision, CourtSportCategoryResponse>()
        //    .ForMember(dest => dest.CourtSubdivisionId, opt => opt.MapFrom(src => src.Id));

        //Time Period Mapping
        /*CreateMap<TimePeriod, TimePeriodResponse>()
            .ForMember(dest => dest.TimePeriodId, opt => opt.MapFrom(src => src.Id));*/
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var mapFromType = typeof(IMapFrom<>);

        var mappingMethodName = nameof(IMapFrom<object>.Mapping);

        bool HasInterface(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == mapFromType;

        var types = assembly.GetExportedTypes().Where(t => t.GetInterfaces().Any(HasInterface)).ToList();

        var argumentTypes = new Type[] { typeof(Profile) };

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            var methodInfo = type.GetMethod(mappingMethodName);

            if (methodInfo != null)
            {
                methodInfo.Invoke(instance, new object[] { this });
            }
            else
            {
                var interfaces = type.GetInterfaces().Where(HasInterface).ToList();

                if (interfaces.Count > 0)
                {
                    foreach (var @interface in interfaces)
                    {
                        var interfaceMethodInfo = @interface.GetMethod(mappingMethodName, argumentTypes);

                        interfaceMethodInfo?.Invoke(instance, new object[] { this });
                    }
                }
            }
        }
    }
}