using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.dtvla.newk2view.customerproducts
{
    public class DtvProfile : Profile
    {
        public DtvProfile()
        {
            CreateMap<Stg_Agreement_Detail, CustomerProduct>()
                .ForMember(d => d.op_ts, a => a.MapFrom(s => s.op_ts))
                .ForMember(d => d.op_type, a => a.MapFrom(s => s.op_type))
                .ForMember(d => d._id, a => a.MapFrom(s => s.after.ENTITY_ID))
                //.ForMember(d => d.Product, a => a.MapFrom(s => s))
                .ReverseMap();

            CreateMap<Stg_Agreement_Detail, CustomerProductItem>()
                .ForMember(d => d.productKey, a => a.MapFrom(s => s.after.AGREEMENT_ITEM_ID))
                .ForMember(d => d.ID, a => a.MapFrom(s => s.after.AGREEMENT_ITEM_ID))
                .ForMember(d => d.CustomerKey, a => a.MapFrom(s => s.after.CUSTOMER_ID))
                .ForMember(d => d.contractStartDatetime, a => a.MapFrom(s => s.after.FROM_DATE))
                .ForMember(d => d.contractEndDatetime, a => a.MapFrom(s => s.after.TO_DATE))
                .ForMember(d => d.contractPeriod, a => a.MapFrom(s => s.after.CONTRACT_PERIOD))
                .ForMember(d => d.financialAccountID, a => a.MapFrom(s => s.after.ACCOUNT_ID))
                .ForMember(d => d.agreementID, a => a.MapFrom(s => s.after.AGREEMENT_ID))
                .ForMember(d => d.productCategory, a => a.MapFrom(s => s))
                .ForMember(d => d.productStatus, a => a.MapFrom(s => s))
                .ForMember(d => d.financeOption, a => a.MapFrom(s => s))
                .ReverseMap();


            CreateMap<Stg_Agreement_Detail, CustomerProductSingle>()
                .ForMember(d => d._id, a => a.MapFrom(s => s.after.AGREEMENT_ITEM_ID))
                .ForMember(d => d.op_ts, a => a.MapFrom(s => s.op_ts))
                .ForMember(d => d.op_type, a => a.MapFrom(s => s.op_type))
                .ForMember(d => d.productKey, a => a.MapFrom(s => s.after.AGREEMENT_ITEM_ID))
                .ForMember(d => d.ID, a => a.MapFrom(s => s.after.AGREEMENT_ITEM_ID))
                .ForMember(d => d.CustomerKey, a => a.MapFrom(s => s.after.CUSTOMER_ID))
                .ForMember(d => d.contractStartDatetime, a => a.MapFrom(s => s.after.FROM_DATE))
                .ForMember(d => d.contractEndDatetime, a => a.MapFrom(s => s.after.TO_DATE))
                .ForMember(d => d.contractPeriod, a => a.MapFrom(s => s.after.CONTRACT_PERIOD))
                .ForMember(d => d.financialAccountID, a => a.MapFrom(s => s.after.ACCOUNT_ID))
                .ForMember(d => d.agreementID, a => a.MapFrom(s => s.after.AGREEMENT_ID))
                .ForMember(d => d.productCategory, a => a.MapFrom(s => s))
                .ForMember(d => d.productStatus, a => a.MapFrom(s => s))
                .ForMember(d => d.financeOption, a => a.MapFrom(s => s))
                .ReverseMap();

            CreateMap<Stg_Agreement_Detail, CategoryFinOp>()
                .ForMember(d => d.id, a => a.MapFrom(s => s.after.FINANCE_OPTION_ID))
                .ReverseMap();

            CreateMap<Stg_Agreement_Detail, CategoryStatus>()
                .ForMember(d => d.id, a => a.MapFrom(s => s.after.PRODUCT_STATUS_ID))
                .ReverseMap();

            CreateMap<Stg_Agreement_Detail, ProductSpecification>()
                .ForMember(d => d.id, a => a.MapFrom(s => s.after.PRODUCT_SPECIFICATION_ID))
                .ReverseMap();

        }
    }
}
