using UnityEngine;
using System;

namespace Arctic.Gameplay.Items
{
    public sealed class ItemPropertyData
    {
        public readonly string key;
        public readonly object value;
        public readonly Type type;

        public ItemPropertyData(string key, object value, Type type)
        {
            this.key = key; ;
            this.value = value;
            this.type = type;
        }

        public T ValueAs<T>() => (T)value;
    }
}