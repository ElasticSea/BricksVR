using UnityEngine;

namespace _Framework.Scripts.Util
{
    public class EnumFlagAttribute : PropertyAttribute
    {
        public string name;

        public EnumFlagAttribute()
        {
        }

        public EnumFlagAttribute(string name)
        {
            this.name = name;
        }
    }
}