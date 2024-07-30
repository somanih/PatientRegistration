namespace PatientRegistration.Services.Profile
{
    using AutoMapper;
    using PatientRegistration.DTOs;
    using PatientRegistration.Infrastructure.DbModels;

    public class MappingProfiler : Profile
    {
        public MappingProfiler()
        {
            CreateMap<PatientDetail, PatientDetailDTO>().ReverseMap();
        }
    }
}
