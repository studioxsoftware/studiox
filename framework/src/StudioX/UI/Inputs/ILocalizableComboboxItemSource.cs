using System.Collections.Generic;

namespace StudioX.UI.Inputs
{
    public interface ILocalizableComboboxItemSource
    {
        ICollection<ILocalizableComboboxItem> Items { get; }
    }
}