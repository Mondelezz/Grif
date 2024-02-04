using GrifMVD.NewsFolder.Interfaces;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GrifMVD.NewsFolder.Services
{
    public class DateTreatmentService : IHandleDate
    {
        public DateTime ClearingDate(string divTime)
        {
            CultureInfo russianCulture = new CultureInfo("ru-RU");
            string dateText = divTime.Split('<')[0].Trim().Replace("\r\n", "").TrimEnd('1', '2', '3', '4', '5', '6', '7', '8', '9', '0', ':');
            dateText = Regex.Replace(dateText, @"\s+", " ").Replace("Дата", "").Replace(" 2023", "");
            string today = "Сегодня";
            int change = dateText.IndexOf(today);

            DateTime parsedTime;

            if (TryParseDate(dateText, out int day, out int month))
            {
                Console.WriteLine($"День: {day}, Месяц: {month}");
                dateText = dateText.Trim();
                parsedTime = DateTime.ParseExact(dateText, "dd MMMM", russianCulture).ToUniversalTime();

                Console.WriteLine(parsedTime);
                return parsedTime;
            }
            if (change == 0)
            {
                dateText = dateText.Replace(today, DateTime.Now.ToString("dd.MM.yyyy"));
                dateText = dateText.Trim();
                parsedTime = DateTime.ParseExact(dateText, "dd.MM.yyyy HH:mm", russianCulture).ToUniversalTime();
                Console.WriteLine(parsedTime);
                return parsedTime;
            }
            else
            {
                dateText = dateText.Trim();
                parsedTime = DateTime.ParseExact(dateText, "dd MMMM HH:mm", russianCulture).ToUniversalTime();
                Console.WriteLine(parsedTime);
                return parsedTime;
            }
        }

        // Если входные данные только день и месяц
        public virtual bool TryParseDate(string input, out int day, out int month)
        {
            day = 0;
            month = 0;
            string[] formats = { "d MMMM ", "dd MMMM " };
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
