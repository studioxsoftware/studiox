using System;
using StudioX.Application.Services;
using StudioX.Collections.Extensions;
using JetBrains.Annotations;

namespace StudioX.Aspects
{
    internal static class StudioXCrossCuttingConcerns
    {
        public const string Auditing = "StudioXAuditing";
        public const string Validation = "StudioXValidation";
        public const string UnitOfWork = "StudioXUnitOfWork";
        public const string Authorization = "StudioXAuthorization";

        public static void AddApplied(object obj, params string[] concerns)
        {
            if (concerns.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(concerns), $"{nameof(concerns)} should be provided!");
            }

            (obj as IAvoidDuplicateCrossCuttingConcerns)?.AppliedCrossCuttingConcerns.AddRange(concerns);
        }

        public static void RemoveApplied(object obj, params string[] concerns)
        {
            if (concerns.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(concerns), $"{nameof(concerns)} should be provided!");
            }

            var crossCuttingEnabledObj = obj as IAvoidDuplicateCrossCuttingConcerns;
            if (crossCuttingEnabledObj == null)
            {
                return;
            }

            foreach (var concern in concerns)
            {
                crossCuttingEnabledObj.AppliedCrossCuttingConcerns.RemoveAll(c => c == concern);
            }
        }

        public static bool IsApplied([NotNull] object obj, [NotNull] string concern)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (concern == null)
            {
                throw new ArgumentNullException(nameof(concern));
            }

            return (obj as IAvoidDuplicateCrossCuttingConcerns)?.AppliedCrossCuttingConcerns.Contains(concern) ?? false;
        }

        public static IDisposable Applying(object obj, params string[] concerns)
        {
            AddApplied(obj, concerns);
            return new DisposeAction(() =>
            {
                RemoveApplied(obj, concerns);
            });
        }

        public static string[] GetApplieds(object obj)
        {
            var crossCuttingEnabledObj = obj as IAvoidDuplicateCrossCuttingConcerns;
            if (crossCuttingEnabledObj == null)
            {
                return new string[0];
            }

            return crossCuttingEnabledObj.AppliedCrossCuttingConcerns.ToArray();
        }
    }
}