using System;
using UnityEngine;

namespace _Framework.Scripts.Util
{
    public class BitMaskAttribute : PropertyAttribute
    {
        public Type propType;
        public BitMaskAttribute(Type aType)
        {
            propType = aType;
        }
    }
}