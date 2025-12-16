using System;

namespace Arctic.Utilities.Serialization
{
    /// <summary>
    /// An immutable implementation of <see cref="IProperty"/> that explicitly stores
    /// both the value and its runtime type. This property is intended for safe,
    /// read-only access where the value type must be preserved and mutation is
    /// disallowed to prevent invalid casting or type corruption.
    /// The key may be modified.
    /// </summary>
    [System.Serializable]
    public sealed class ExplicitProperty : IProperty
    {
        public string key;
        public readonly object value;
        public readonly System.Type type;

        public ExplicitProperty(string key, object value, Type type)
        {
            this.key = key;
            this.value = value;
            this.type = type;
        }
        public string GetKey() => key;
        public object GetValue() => value;
        public System.Type GetValueType() => type;

        //public void SetKey(string key) => this.key = key;
        //public T ValueAs<T>() => (T)value;
    }
}