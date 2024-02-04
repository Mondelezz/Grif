namespace GrifMVD.NewsFolder.Interfaces
{
    public interface IHandleDate
    {
        public abstract bool TryParseDate(string input, out int day, out int month);
        public DateTime ClearingDate(string divTime);
    }
}
