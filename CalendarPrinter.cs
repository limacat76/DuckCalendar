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

        static DeviceRgb rosso = new DeviceRgb(245, 15, 15);
        static DeviceRgb bianco = new DeviceRgb(255, 255, 255);

        private static void MonthToPage(PdfFont font, Document document, Mese mesecontainer)
        {
            var table = new Table(new float[2]).UseAllAvailableWidth();
            table.SetMarginTop(0);
            table.SetMarginBottom(0);

            var cell = new Cell(2, 2).Add(new Paragraph($"{mesecontainer.nome} {mesecontainer.anno}").SetFont(font).SetFontSize(32));
            cell.SetTextAlignment(TextAlignment.CENTER);
            cell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            cell.SetFontColor(bianco);
            cell.SetBackgroundColor(rosso);
            table.AddCell(cell);

            Giorno[] mese = mesecontainer.giorni;

            for (int i = 0; i < 16; i++)
            {
                Giorno giorno;
                if (i < mese.Length)
                {
                    giorno = mese[i];
                }
                else
                {
                    giorno = CalendarGenerator.VUOTO;
                }
                cell = MakeDayCell(giorno, font);
                table.AddCell(cell);

                if (i + 16 < mese.Length)
                {
                    giorno = mese[i + 16];
                }
                else
                {
                    giorno = CalendarGenerator.VUOTO;
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

        public static void Calendar(string target, Mese[] anno)
        {
            var dest = new FileInfo(target);

            var writer = new PdfWriter(dest);
            var pdf = new PdfDocument(writer);
            Document document = new Document(pdf, PageSize.A4);

            string CAMBRIA = @"C:\Windows\Fonts\Cambriab.ttf";
            string FONT = @"D:\Temp\OpenSansEmoji.ttf";

            FontProgram fontProgram = FontProgramFactory.CreateFont(FONT);
            PdfFont font = PdfFontFactory.CreateFont(fontProgram, PdfEncodings.IDENTITY_H, true);

            bool first = true;
            foreach (Mese mese in anno)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                }
                MonthToPage(font, document, mese);
            }

            document.Close();
        }

    }
}
