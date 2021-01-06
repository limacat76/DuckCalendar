﻿/*  DuckCalendar, generates a PDF with a Calendar 
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


            public Giorno(int numero, string nome, bool festivo)
            {
                this.numero = numero;
                this.nome = nome;
                this.Festivo = festivo;
            }

            public string Stampa()
            {
                if (numero != 0)
                {
                    return "" + numero + " " + nome;
                } else
                {
                    return "";
                }
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

        private static void MonthPage(int mesenum, PdfFont font, Document document, int year)
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
            var mese = new Giorno[32];
            // TODO vedere i mesi e quando inizia l'anno
            DateTime myDT = new DateTime(year, mesenum, 1, new GregorianCalendar());
            int curMonth = myDT.Month;
            for (int i = 1; i <= 31; i++)
            {
                if (curMonth != myDT.Month)
                {
                    mese[i - 1] = new Giorno(0, "", false);
                } else
                {
                    DayOfWeek dw = myDT.DayOfWeek;
                    
                    mese[i - 1] = new Giorno(myDT.Day, UppercaseFirst(dtfi.GetAbbreviatedDayName(dw)), dw == DayOfWeek.Sunday || IsFestivity(myDT.Day, myDT.Month));
                }
                myDT = myDT.AddDays(1);

             }
            mese[31] = new Giorno(0, "", false);

            for (int i = 0; i < 16; i++)
            {
                Giorno giorno = mese[i];
                cell = MakeDayCell(giorno, font);
                table.AddCell(cell);
                giorno = mese[i + 16];
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

        // TODO: put into a better data structure
        // TODO: calculate easters
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

            string CAMBRIA = @"C:\Windows\Fonts\Cambriab.ttf";
            FontProgram fontProgram = FontProgramFactory.CreateFont(CAMBRIA);
            PdfFont font = PdfFontFactory.CreateFont(fontProgram, PdfEncodings.WINANSI, true);

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
                MonthPage(i, font, document, year);
            }

            document.Close();
        }

        static void Main(string[] args)
        {
            Calendar(@"D:\Temp\Calendario2021.pdf");
        }
    }
}
