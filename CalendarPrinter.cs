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
using System.IO;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace DuckCalendar
{

    class CalendarPrinter
    {

        static DeviceRgb red = new DeviceRgb(245, 15, 15);
        static DeviceRgb white = new DeviceRgb(255, 255, 255);

        private static void MonthToPage(PdfFont font, Document document, Month month)
        {
            var table = new Table(new float[2]).UseAllAvailableWidth();
            table.SetMarginTop(0);
            table.SetMarginBottom(0);

            var cell = new Cell(2, 2).Add(new Paragraph($"{month.name} {month.year}").SetFont(font).SetFontSize(32));
            cell.SetTextAlignment(TextAlignment.CENTER);
            cell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            cell.SetFontColor(white);
            cell.SetBackgroundColor(red);
            table.AddCell(cell);

            Day[] days = month.days;

            for (int i = 0; i < 16; i++)
            {
                Day day;
                if (i < days.Length)
                {
                    day = days[i];
                }
                else
                {
                    day = CalendarGenerator.EMPTY;
                }
                cell = MakeDayCell(day, font);
                table.AddCell(cell);

                if (i + 16 < days.Length)
                {
                    day = days[i + 16];
                }
                else
                {
                    day = CalendarGenerator.EMPTY;
                }
                cell = MakeDayCell(day, font);
                table.AddCell(cell);
            }
            table.SetHeight(PageSize.A4.GetHeight() - 80);
            document.Add(table);
        }

        private static Cell MakeDayCell(Day giorno, PdfFont font)
        {
            Cell cell = new Cell().Add(new Paragraph(giorno.Print()).SetFont(font).SetFontSize(16));
            cell.SetPaddingLeft(10);
            cell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            if (giorno.Festive)
            {
                cell.SetFontColor(red);
            }
            return cell;
        }

        public static void Print(string filename, Month[] calendar)
        {
            var dest = new FileInfo(filename);

            var writer = new PdfWriter(dest);
            var pdf = new PdfDocument(writer);
            Document document = new Document(pdf, PageSize.A4);

            DCEnvironment environment = DCEnvironment.GetInstance();
            FontProgram fontProgram = FontProgramFactory.CreateFont(environment.FontFilename());
            PdfFont font = PdfFontFactory.CreateFont(fontProgram, PdfEncodings.IDENTITY_H, true);

            bool first = true;
            foreach (Month month in calendar)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                }
                MonthToPage(font, document, month);
            }

            document.Close();
        }

    }
}
