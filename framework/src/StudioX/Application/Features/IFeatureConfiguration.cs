﻿using StudioX.Collections;

namespace StudioX.Application.Features
{
    /// <summary>
    ///     Used to configure feature system.
    /// </summary>
    public interface IFeatureConfiguration
    {
        /// <summary>
        ///     Used to add/remove <see cref="FeatureProvider" />s.
        /// </summary>
        ITypeList<FeatureProvider> Providers { get; }
    }
}