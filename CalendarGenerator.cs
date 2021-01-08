/*  DuckCalendar, generates a PDF with a Calendar 
    Copyright (C) 2021  Davide "limaCAT" Inglima

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>. */
using System;
using System.Globalization;

namespace DuckCalendar
{

    public struct Day
    {
        int number;
        string name;
        public bool Festive
        {
            get;
        }

        public Day(int number, string name, string moonphase, bool festive)
        {
            this.number = number;
            this.name = $"{name} {moonphase}";
            this.Festive = festive;
        }

        public string Print()
        {
            if (number != 0)
            {
                return $"{number} {name}";
            }
            else
            {
                return "";
            }
        }

    }

    public struct Month
    {

        public int year;
        public string name;
        public Day[] days;

        public Month(int year, string name, Day[] days)
        {
            this.year = year;
            this.name = name;
            this.days = days;
        }
    }

    class CalendarGenerator
    {
        public static Day EMPTY = new Day(0, "", "", false);

        private static string new_moon = char.ConvertFromUtf32(0x1F311); // 🌑 U+1F311
        private static string first_quarter = char.ConvertFromUtf32(0x1f313); // 🌓 U+1F313
        private static string full_moon = char.ConvertFromUtf32(0x1f315); // 🌕 U+1F315
        private static string last_quarter = char.ConvertFromUtf32(0x1f317); // 🌗 U+1F317

        private static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        private static Month GenerateMonth(int year, int month)
        {
            DateTimeFormatInfo dtfi = CultureInfo.CurrentCulture.DateTimeFormat;
            string name = UppercaseFirst(dtfi.GetMonthName(month));

            // Populating the months
            Day[] days = new Day[32];
            DateTime myDT = new DateTime(year, month, 1, new GregorianCalendar());
            int curMonth = myDT.Month;
            for (int i = 1; i <= 31; i++)
            {
                if (curMonth != myDT.Month)
                {
                    days[i - 1] = EMPTY;
                }
                else
                {
                    DayOfWeek dw = myDT.DayOfWeek;

                    days[i - 1] = new Day(myDT.Day, UppercaseFirst(dtfi.GetAbbreviatedDayName(dw)), IsMoon(myDT), dw == DayOfWeek.Sunday || IsFestivity(myDT));
                }
                myDT = myDT.AddDays(1);

            }
            days[31] = EMPTY;

            Month mesecontainer = new Month(year, name, days);
            return mesecontainer;
        }

        private static int Conway(int year, int month, int day)
        {
            double r = year % 100;
            r %= 19;
            if (r > 9)
            {
                r -= 19;
            }
            r = ((r * 11) % 30) + month + day;
            if (month < 3)
            {
                r += 2;
            }
            r -= ((year < 2000) ? 4 : 8.3);
            r = Math.Floor(r + 0.5) % 30;
            return (int)((r < 0) ? r + 30 : r);
        }

        // Easter Management, this is horrible and should be made better, 
        private static int easter_month = 0;
        private static int easter_day = 0;

        // Let's calculate easter with the gauss method
        private static void CalculateEaster(int y)
        {
           
            int a = y % 19;
            int b = y / 100;
            int c = y % 100;
            int d = b / 4;
            int e = b % 4;
            int g = (8 * b + 13) / 25;
            int h = (19 * a + b - d - g + 15) % 30;
            int j = c / 4;
            int k = c % 4;
            int m = (a + 11 * h) / 319;
            int r = (2 * e + 2 * j - k - h + m + 32) % 7;
            int n = (h - m + r + 90) / 25;
            int p = (h - m + r + n + 19) % 32;
            easter_month = n;
            easter_day = p;
        }

        private static string IsMoon(DateTime myDT)
        {
            int cw = Conway(myDT.Year, myDT.Month, myDT.Day);
            myDT = myDT.AddDays(-1);
            int cwYesterday = Conway(myDT.Year, myDT.Month, myDT.Day);

            if (cw == 0 && cwYesterday != 29 || cw == 29)
            {
                return new_moon;
            }
            if (cw == 7)
            {
                return first_quarter;
            }
            if (cw == 15)
            {
                return full_moon;
            }
            if (cw == 22)
            {
                return last_quarter;
            }
            return "";
        }

        public const int Month_Start = 1;
        public const int Month_End = 12;

        public static Month[] GenerateYear(int year)
        {
            CalculateEaster(year);
            Month[] months = new Month[Month_End];
            for (var i = Month_Start; i <= Month_End; i++)
            {
                months[i - 1] = GenerateMonth(year, i);
            }

            return months;
        }

        // I am not proud about this, but given the size / scope of the program it's enough for now...
        private static bool IsFestivity(DateTime date)
        {
            int day = date.Day;
            int month = date.Month;

            if (day == 1 && month == 1)
            {
                return true;
            }

            if (day == 6 && month == 1)
            {
                return true;
            }

            if (day == easter_day && month == easter_month) // Easter
            {
                return true;
            }

            DateTime yesterday = date.AddDays(-1);
            int yDay = yesterday.Day;
            int yMonth = yesterday.Month;
            if (yDay == easter_day && yMonth == easter_month) // Easter Monday (Italy Only)
            {
                return true;
            }

            if (day == 25 && month == 4) // Italy Only: 25 Aprile
            {
                return true;
            }

            if (day == 1 && month == 5)
            {
                return true;
            }

            if (day == 2 && month == 6)
            {
                return true;
            }

            if (day == 15 && month == 8)
            {
                return true;
            }

            if (day == 1 && month == 11)
            {
                return true;
            }

            if (day == 8 && month == 12)
            {
                return true;
            }

            if (day == 25 && month == 12)
            {
                return true;
            }

            if (day == 26 && month == 12)
            {
                return true;
            }

            return false;
        }
    }

}