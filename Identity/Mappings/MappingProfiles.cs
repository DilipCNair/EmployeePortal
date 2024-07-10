namespace Identity.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<ProfileViewModel, Employee>().ReverseMap();
        CreateMap<BasicDetailsViewModel, Employee>().ReverseMap();
        CreateMap<AddressViewModel, Address>().ReverseMap();
        CreateMap<ManagerViewModel, Employee>().ReverseMap();
        CreateMap<BasicDetailsViewModelForManager, Employee>().ReverseMap();
        CreateMap<ExperienceViewModel, Experience>().ReverseMap();
    }
}
