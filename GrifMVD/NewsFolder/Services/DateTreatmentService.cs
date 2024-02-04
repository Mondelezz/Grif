using GrifMVD.NewsFolder.Interfaces;
using System.Globalization;

namespace GrifMVD.NewsFolder.Services
{
    public class DateTreatmentService : IHandleDate
    {
        // Если входные данные день и месяц
        public bool TryParseDate(string input, out int day, out int month)
        {
            day = 0;
            month = 0;
            string[] formats = { "d MMMM", "dd MMMM" };
            if (DateTime.TryParseExact(input, formats, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime result))
            {
                day = result.Day;
                month = result.Month;
                return true;
            }
            return false;
        }
    }
}
