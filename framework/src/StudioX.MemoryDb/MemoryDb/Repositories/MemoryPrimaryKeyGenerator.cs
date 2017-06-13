using System;

namespace StudioX.MemoryDb.Repositories
{
    public class MemoryPrimaryKeyGenerator<TPrimaryKey>
    {
        private object lastPk;

        public MemoryPrimaryKeyGenerator()
        {
            InitializeLastPk();
        }

        public TPrimaryKey GetNext()
        {
            lock (this)
            {
                return GetNextPrimaryKey();
            }
        }

        private void InitializeLastPk()
        {
            if (typeof(TPrimaryKey) == typeof(int))
            {
                lastPk = 0;
            }
            else if (typeof(TPrimaryKey) == typeof(long))
            {
                lastPk = 0L;
            }
            else if (typeof(TPrimaryKey) == typeof(short))
            {
                lastPk = (short)0;
            }
            else if (typeof(TPrimaryKey) == typeof(byte))
            {
                lastPk = (byte)0;
            }
            else if (typeof(TPrimaryKey) == typeof(Guid))
            {
                lastPk = null;
            }
            else
            {
                throw new StudioXException("Unsupported primary key type: " + typeof(TPrimaryKey));
            }
        }

        private TPrimaryKey GetNextPrimaryKey()
        {
            if (typeof(TPrimaryKey) == typeof(int))
            {
                lastPk = ((int)lastPk) + 1;
            }
            else if (typeof(TPrimaryKey) == typeof(long))
            {
                lastPk = ((long)lastPk) + 1L;
            }
            else if (typeof(TPrimaryKey) == typeof(short))
            {
                lastPk = (short)(((short)lastPk) + 1);
            }
            else if (typeof(TPrimaryKey) == typeof(byte))
            {
                lastPk = (byte)(((byte)lastPk) + 1);
            }
            else if (typeof(TPrimaryKey) == typeof(Guid))
            {
                lastPk = Guid.NewGuid();
            }
            else
            {
                throw new StudioXException("Unsupported primary key type: " + typeof(TPrimaryKey));
            }

            return (TPrimaryKey)lastPk;
        }
    }
}
