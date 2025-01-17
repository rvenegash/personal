using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kafkaTest
{
    public class DtvProfile : Profile
    {
        public DtvProfile()
        {
            CreateMap<Stg_Customer, CategoryType>()
               .ForMember(d => d.id, a => a.MapFrom(s => s.after.TYPE_ID))
                .ReverseMap();

            CreateMap<Stg_Customer, CategoryStatus>()
               .ForMember(d => d.id, a => a.MapFrom(s => s.after.STATUS_ID))
                .ReverseMap();

            CreateMap<Stg_Customer, Customer>()
                .ForMember(d => d.op_ts, a => a.MapFrom(s => s.op_ts))
                .ForMember(d => d.op_type, a => a.MapFrom(s => s.op_type))
                .ForMember(d => d._id, a => a.MapFrom(s => s.after.ENTITY_ID))
                .ForMember(d => d.ID, a => a.MapFrom(s => s.after.ID))
                .ForMember(d => d.customerRank, a => a.MapFrom(s => s.after.CLASS_ID))
                .ForMember(d => d.SegmentationKey, a => a.MapFrom(s => s.after.SEGMENTATION_ID))
                .ForMember(d => d.BusinessUnitId, a => a.MapFrom(s => s.after.BUSINESS_UNIT_ID))
                .ForMember(d => d.magazines, a => a.MapFrom(s => s.after.MAGAZINES))
                .ForMember(d => d.CategorizedBy, a => a.MapFrom(s => s))
                .ForMember(d => d.status, a => a.MapFrom(s => s))
                .ForMember(d => d.validFor, a => a.MapFrom(s => s))
                .ForMember(d => d.IndividualRole, a => a.MapFrom(s => s))
                //.ForMember(d => d.IndividualRole.IdentifiedBy.IndividualIdentifications[0].scan, a => a.MapFrom(s => s.after.REFERENCE_TYPE_ID))
                //.ForMember(d => d.IndividualRole.IdentifiedBy.IndividualIdentifications[0].cardNr, a => a.MapFrom(s => s.after.IDENTIFICATION_ID))
                //.ForMember(d => d.IndividualRole.IdentifiedBy.IndividualIdentifications[1].number, a => a.MapFrom(s => s.after.INTERNET_USER_NAME))
                //.ForMember(d => d.ID, a => a.MapFrom(s => s.after.COUNTRY_ID))
                .ReverseMap();

            CreateMap<Stg_Customer, Individual>()
                .ForMember(d => d.aliveDuring, a => a.MapFrom(s => s))
                //.ForMember(d => d.IdentifiedBy, a => a.MapFrom(s => s))
                .ReverseMap();

            CreateMap<Stg_Customer, TimePeriodAlive>()
                .ForMember(d => d.startDateTime, a => a.MapFrom(s => s.after.BIRTH_DATE))
                .ReverseMap();

            CreateMap<Stg_Customer, TimePeriodSince>()
                .ForMember(d => d.startDateTime, a => a.MapFrom(s => s.after.CUSTOMER_SINCE))
                .ReverseMap();

            /*
            CreateMap<Stg_Customer, IndividualIdentificationCollection>()
                .ForMember(d => d.IndividualIdentifications, a => a.MapFrom(s => s))
                .ReverseMap();

            CreateMap<Stg_Customer, IndividualIdentification[]>()
                .ForMember(d => d, a => a.MapFrom(s => s))
                .ReverseMap();


            CreateMap<Stg_Customer, IndividualIdentification>()
                .ForMember(d => d.scan, a => a.MapFrom(s => s.after.REFERENCE_TYPE_ID))
                .ForMember(d => d.cardNr, a => a.MapFrom(s => s.after.IDENTIFICATION_ID))
                .ReverseMap();
            */
        }
    }
}
