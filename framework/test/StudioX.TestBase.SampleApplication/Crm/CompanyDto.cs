using System;
using StudioX.Application.Services.Dto;
using StudioX.AutoMapper;

namespace StudioX.TestBase.SampleApplication.Crm
{
    [AutoMapFrom(typeof(Company))]
    public class CompanyDto : EntityDto
    {
        public string Name { get; set; }

        public DateTime CreationTime { get; set; }

        public Address ShippingAddress { get; set; }

        public Address BillingAddress { get; set; }
    }
}