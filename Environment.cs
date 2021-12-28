using System;
using System.IO;
namespace DuckCalendar
{
    public class DCEnvironment
    {
        private static readonly object _door = new object();

        private string targetDirectory;

        private string targetFont;

        private const string C_TEMP = @"C:\Temp\";

        private const string D_TEMP = @"D:\Temp\";

        private const string C_WINDOWS_FONTS = @"C:\Windows\Fonts\";

        private const string FONT_NAME = @"OpenSansEmoji.ttf";

        private const string filenameradix = "Calendario";

        private DCEnvironment()
        {
            string userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar;
            if (Directory.Exists(C_TEMP))
            {
                targetDirectory = C_TEMP;
            }
            else
            {
                targetDirectory = userHome;
            }

            if (File.Exists($"{C_TEMP}{FONT_NAME}"))
            {
                targetFont = $"{C_TEMP}{FONT_NAME}";
            }
            else if (File.Exists($"{D_TEMP}{FONT_NAME}"))
            {
                targetFont = $"{D_TEMP}{FONT_NAME}";
            }
            else if (File.Exists($"{C_WINDOWS_FONTS}{FONT_NAME}"))
            {
                targetFont = $"{C_WINDOWS_FONTS}{FONT_NAME}";
            }
            else
            {
                targetFont = $"{userHome}{FONT_NAME}";
            }
        }

        private static DCEnvironment instance;

        public static DCEnvironment GetInstance()
        {
            lock (_door)
            {
                if (instance == null)
                {
                    instance = new DCEnvironment();
                }
                return instance;
            }
        }

        public string CalendarFilename(int year)
        {
            string filename = $"{targetDirectory}{filenameradix}{year}.pdf";
            return filename;
        }

        public string FontFilename()
        {
            return targetFont;
        }
    }
}
