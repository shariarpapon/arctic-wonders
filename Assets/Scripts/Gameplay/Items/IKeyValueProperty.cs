namespace Arctic.Gameplay.Items
{
    public interface IKeyValueProperty<TKey, TValue>
    {
        public TKey Key { get; }
        public TValue Value { get; }
    }
}