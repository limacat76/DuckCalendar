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

    public struct Giorno
    {
        int numero;
        string nome;
        public bool Festivo
        {
            get;
        }

        public Giorno(int numero, string nome, string luna, bool festivo)
        {
            this.numero = numero;
            this.nome = $"{nome} {luna}";
            this.Festivo = festivo;
        }

        public string Stampa()
        {
            if (numero != 0)
            {
                return $"{numero} {nome}";
            }
            else
            {
                return "";
            }
        }

    }

    public struct Mese
    {

        public int anno;
        public string nome;
        public Giorno[] giorni;

        public Mese(int anno, string nome, Giorno[] giorni)
        {
            this.anno = anno;
            this.nome = nome;
            this.giorni = giorni;
        }
    }

    class CalendarGenerator
    {
        public static Giorno VUOTO = new Giorno(0, "", "", false);

        private const int new_moon = 0x1F311; // 🌑 U+1F311
        private const int first_quarter = 0x1f313; // 🌓 U+1F313
        private const int full_moon = 0x1f315; // 🌕 U+1F315
        private const int last_quarter = 0x1f317; // 🌗 U+1F317

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static Mese GenerateMonth(int year, int mesenum)
        {
            DateTimeFormatInfo dtfi = CultureInfo.CurrentCulture.DateTimeFormat;
            string monthName = UppercaseFirst(dtfi.GetMonthName(mesenum));

            // popoliamo il mese
            Giorno[] mese = new Giorno[32];
            DateTime myDT = new DateTime(year, mesenum, 1, new GregorianCalendar());
            int curMonth = myDT.Month;
            for (int i = 1; i <= 31; i++)
            {
                if (curMonth != myDT.Month)
                {
                    mese[i - 1] = VUOTO;
                }
                else
                {
                    DayOfWeek dw = myDT.DayOfWeek;

                    mese[i - 1] = new Giorno(myDT.Day, UppercaseFirst(dtfi.GetAbbreviatedDayName(dw)), IsMoon(myDT), dw == DayOfWeek.Sunday || IsFestivity(myDT.Day, myDT.Month));
                }
                myDT = myDT.AddDays(1);

            }
            mese[31] = VUOTO;

            Mese mesecontainer = new Mese(year, monthName, mese);
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

        private static String IsMoon(DateTime myDT)
        {
            int cw = Conway(myDT.Year, myDT.Month, myDT.Day);
            myDT = myDT.AddDays(-1);
            int cwYesterday = Conway(myDT.Year, myDT.Month, myDT.Day);

            if (cw == 0 && cwYesterday != 29 || cw == 29)
            {
                return char.ConvertFromUtf32(new_moon);
            }
            if (cw == 7)
            {
                return char.ConvertFromUtf32(first_quarter);
            }
            if (cw == 15)
            {
                return char.ConvertFromUtf32(full_moon);
            }
            if (cw == 22)
            {
                return char.ConvertFromUtf32(last_quarter);
            }
            return "";
        }

        public const int Month_Start = 1;
        public const int Month_End = 12;

        internal static Mese[] GenerateYear(int year)
        {
            Mese[] anno = new Mese[Month_End];
            for (var i = Month_Start; i <= Month_End; i++)
            {
                anno[i - 1] = CalendarGenerator.GenerateMonth(year, i);
            }

            return anno;
        }

        private static bool IsFestivity(int day, int month)
        {
            if (day == 1 && month == 1)
            {
                return true;
            }

            if (day == 6 && month == 1)
            {
                return true;
            }

            if (day == 4 && month == 4) // Easter
            {
                return true;
            }

            if (day == 5 && month == 4) // Italy Only: Easter Monday
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