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
using System.IO;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace DuckCalendar
{
    class Program
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
                } else
                {
                    return "";
                }
            }

        }
        public static Giorno VUOTO = new Giorno(0, "", "", false);

        private const int new_moon = 0x1F311; // 🌑 U+1F311
        private const int first_quarter = 0x1f313; // 🌓 U+1F313
        private const int full_moon = 0x1f315; // 🌕 U+1F315
        private const int last_quarter = 0x1f317; // 🌗 U+1F317

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

            public string Stampa()
            {
                    return $"{nome} {anno}";
            }
        }

        public const int Month_Start = 1;
        public const int Month_End = 12;
        
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

        static DeviceRgb rosso = new DeviceRgb(245, 15, 15);
        static DeviceRgb bianco = new DeviceRgb(255, 255, 255);

        private static Mese GenerateMonth(int year, int mesenum)
        {
            Mese mese = new Mese(0, "", new Giorno[] { });
            return mese;
        }        

        private static void MonthToPage(PdfFont font, Document document, int year, int mesenum)
        {
            var table = new Table(new float[2]).UseAllAvailableWidth();
            table.SetMarginTop(0);
            table.SetMarginBottom(0);

            string monthName = UppercaseFirst(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mesenum)) + " " + year;
            DateTimeFormatInfo dtfi = CultureInfo.CurrentCulture.DateTimeFormat;
            // CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames

            var cell = new Cell(2, 2).Add(new Paragraph(monthName).SetFont(font).SetFontSize(32));
            cell.SetTextAlignment(TextAlignment.CENTER);
            cell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            cell.SetFontColor(bianco);
            cell.SetBackgroundColor(rosso);
            table.AddCell(cell);

            // popoliamo il mese
            Giorno[] mese = new Giorno[32];
            DateTime myDT = new DateTime(year, mesenum, 1, new GregorianCalendar());
            int curMonth = myDT.Month;
            for (int i = 1; i <= 31; i++)
            {
                if (curMonth != myDT.Month)
                {
                    mese[i - 1] = VUOTO;
                } else
                {
                    DayOfWeek dw = myDT.DayOfWeek;
                    
                    mese[i - 1] = new Giorno(myDT.Day, UppercaseFirst(dtfi.GetAbbreviatedDayName(dw)), IsMoon(myDT), dw == DayOfWeek.Sunday || IsFestivity(myDT.Day, myDT.Month));
                }
                myDT = myDT.AddDays(1);

             }
            mese[31] = VUOTO;

            for (int i = 0; i < 16; i++)
            {
                Giorno giorno;
                if (i < mese.Length) {
                    giorno = mese[i];
                } else
                {
                    giorno = VUOTO;
                }
                cell = MakeDayCell(giorno, font);
                table.AddCell(cell);

                if (i + 16 < mese.Length)
                {
                    giorno = mese[i + 16];
                }
                else
                {
                    giorno = VUOTO;
                }
                cell = MakeDayCell(giorno, font);
                table.AddCell(cell);
            }
            table.SetHeight(PageSize.A4.GetHeight() - 80);
            document.Add(table);
        }


        private static Cell MakeDayCell(Giorno giorno, PdfFont font)
        {
            Cell cell = new Cell().Add(new Paragraph(giorno.Stampa()).SetFont(font).SetFontSize(16)); // .SetHeight(cellHeight);
            cell.SetPaddingLeft(10);
            cell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            if (giorno.Festivo)
            {
                cell.SetFontColor(rosso);
            }
            return cell;
        }

        private static int Conway(int year, int month, int day)
        {
            double r = year % 100;
            r %= 19;
            if (r > 9) { r -= 19; }
            r = ((r * 11) % 30) + month + day;
            if (month < 3) { r += 2; }
            r -= ((year < 2000) ? 4 : 8.3);
            r = Math.Floor(r + 0.5) % 30;
            return (int)( (r < 0) ? r + 30 : r );
        }

        private static String IsMoon(DateTime myDT)
        {
            int cw = Conway(myDT.Year, myDT.Month, myDT.Day);
            myDT = myDT.AddDays(-1);
            int cwYesterday  = Conway(myDT.Year, myDT.Month, myDT.Day);

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



        private static void Calendar(string target)
        {
            var year = 2021;
            var dest = new FileInfo(target);

            var writer = new PdfWriter(dest);
            var pdf = new PdfDocument(writer);
            Document document = new Document(pdf, PageSize.A4);

            string FONT = @"D:\Temp\OpenSansEmoji.ttf";

            FontProgram fontProgram = FontProgramFactory.CreateFont(FONT);
            PdfFont font = PdfFontFactory.CreateFont(fontProgram, PdfEncodings.IDENTITY_H, true);

            bool first = true;
            for (var i = Month_Start; i <= Month_End; i++)
            {
                if (first)
                {
                    first = false;
                } else
                {
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                }
                MonthToPage(font, document, year, i);
            }

            document.Close();
        }

        static void Main(string[] args)
        {
            Calendar(@"D:\Temp\Calendario2021.pdf");
        }
    }
}
