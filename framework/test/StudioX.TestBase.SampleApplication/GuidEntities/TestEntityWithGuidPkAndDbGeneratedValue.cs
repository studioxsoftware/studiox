using System;
using System.ComponentModel.DataAnnotations.Schema;
using StudioX.Domain.Entities;

namespace StudioX.TestBase.SampleApplication.GuidEntities
{
    public class TestEntityWithGuidPkAndDbGeneratedValue : Entity<Guid>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override Guid Id { get; set; }
    }
}