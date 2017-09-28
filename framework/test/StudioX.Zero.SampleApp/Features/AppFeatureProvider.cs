using StudioX.Application.Features;
using StudioX.Dependency;
using StudioX.UI.Inputs;

namespace StudioX.Zero.SampleApp.Features
{
    public class AppFeatureProvider : FeatureProvider
    {
        public const string MyBoolFeature = "MyBoolFeature";
        public const string MyNumericFeature = "MyNumericFeature";

        private readonly IIocResolver iocResolver; //Just for injection testing

        public AppFeatureProvider(IIocResolver iocResolver)
        {
            this.iocResolver = iocResolver;
        }

        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            var boolFeature = context.Create(MyBoolFeature, "false", inputType: new CheckboxInputType());
            var numericFrature = boolFeature.CreateChildFeature(MyNumericFeature, "42");
        }
    }
}
