using System.Reflection;
using AutoMapper;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Common.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        //Account Mapping
        CreateMap<Account, AccountResponse>()
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.Id));
        CreateMap<Customer, CustomerResponse>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id));
        CreateMap<Owner, OwnerResponse>()
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Id));

        //Court Subdivision Mapping
        CreateMap<Court, CourtResponse>()
            .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src 
            => src.CourtSubdivision.SingleOrDefault().BasePrice));

        //Sport Categories Mapping
        CreateMap<Court, CourtResponse>()
            .ForMember(dest => dest.SportCategoriesEnums, opt => opt.MapFrom(src => src.CourtCategories
                .Select(cc => cc.SportCategory.Name) // Extract the names from the SportCategory
                .Select(name => ParseEnumsExtension.ParseEnum<SportCategoriesEnums>(name))));
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
