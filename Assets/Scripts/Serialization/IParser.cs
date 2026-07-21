namespace Arctic.Serialization
{
    public interface IParser<TIn, TOut>
    {
        public bool TryParse(TIn input, out TOut output);
    }
}