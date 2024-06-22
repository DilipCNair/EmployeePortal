namespace Identity.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<ProfileViewModel, Employee>().ReverseMap();
        CreateMap<BasicDetailsViewModel, Employee>().ReverseMap();
        CreateMap<AddressViewModel, Address>().ReverseMap();
    }
}
