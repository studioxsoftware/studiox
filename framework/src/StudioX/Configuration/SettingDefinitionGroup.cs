using System.Collections.Generic;
using System.Collections.Immutable;
using StudioX.Localization;

namespace StudioX.Configuration
{
    /// <summary>
    ///     A setting group is used to group some settings togehter.
    ///     A group can be child of another group and can have child groups.
    /// </summary>
    public class SettingDefinitionGroup
    {
        /// <summary>
        ///     Unique name of the setting group.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Display name of the setting.
        ///     This can be used to show setting to the user.
        /// </summary>
        public ILocalizableString DisplayName { get; }

        /// <summary>
        ///     Gets parent of this group.
        /// </summary>
        public SettingDefinitionGroup Parent { get; private set; }

        /// <summary>
        ///     Gets a list of all children of this group.
        /// </summary>
        public IReadOnlyList<SettingDefinitionGroup> Children => children.ToImmutableList();

        private readonly List<SettingDefinitionGroup> children;

        /// <summary>
        ///     Creates a new <see cref="SettingDefinitionGroup" /> object.
        /// </summary>
        /// <param name="name">Unique name of the setting group</param>
        /// <param name="displayName">Display name of the setting</param>
        public SettingDefinitionGroup(string name, ILocalizableString displayName)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            Name = name;
            DisplayName = displayName;
            children = new List<SettingDefinitionGroup>();
        }

        /// <summary>
        ///     Adds a <see cref="SettingDefinitionGroup" /> as child of this group.
        /// </summary>
        /// <param name="child">Child to be added</param>
        /// <returns>This child group to be able to add more child</returns>
        public SettingDefinitionGroup AddChild(SettingDefinitionGroup child)
        {
            if (child.Parent != null)
            {
                throw new StudioXException("Setting group " + child.Name + " has already a Parent (" +
                                           child.Parent.Name + ").");
            }

            children.Add(child);
            child.Parent = this;
            return this;
        }
    }
}