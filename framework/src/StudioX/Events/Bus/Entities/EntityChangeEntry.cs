using System;

namespace StudioX.Events.Bus.Entities
{
    [Serializable]
    public class EntityChangeEntry
    {
        public object Entity { get; set; }

        public EntityChangeType ChangeType { get; set; }

        public EntityChangeEntry(object entity, EntityChangeType changeType)
        {
            Entity = entity;
            ChangeType = changeType;
        }
    }
}