using System;

namespace Further.Abp.Npoi.Mapper.Attributes
{
    public class LevelStartAtAttribute: Attribute
    {
        public LevelStartAtAttribute(ushort index)
        {
            Index = index;
        }

        public int Index { get; }
    }
}