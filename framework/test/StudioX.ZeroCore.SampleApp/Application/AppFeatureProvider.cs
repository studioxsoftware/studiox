using StudioX.Application.Features;
using StudioX.UI.Inputs;

using static StudioX.ZeroCore.SampleApp.Application.AppLocalizationHelper;

namespace StudioX.ZeroCore.SampleApp.Application
{
    public class AppFeatureProvider : FeatureProvider
    {
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            context.Create(
                AppFeatures.SimpleBooleanFeature,
                defaultValue: "false",
                displayName: L("SimpleBooleanFeature"),
                inputType: new CheckboxInputType()
            );
        }
    }
}
