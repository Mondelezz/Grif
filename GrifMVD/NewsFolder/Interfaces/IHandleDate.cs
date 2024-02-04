namespace GrifMVD.NewsFolder.Interfaces
{
    public interface IHandleDate
    {
        public bool TryParseDate(string input, out int day, out int month);
    }
}
