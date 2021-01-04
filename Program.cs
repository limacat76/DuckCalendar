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
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.Layout;
using iText.IO.Font.Constants;
using iText.Layout.Element;

namespace DuckCalendar
{
    class Program
    {
        static readonly string target = @"D:\Temp\Astley.pdf";

        private static void Hello()
        {
            var dest = new FileInfo(target);

            var writer = new PdfWriter(dest);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            document.Add(new Paragraph("Hello World 2!"));
            document.Close();
        }

        static void Main(string[] args)
        {
            var dest = new FileInfo(target);

            var writer = new PdfWriter(dest);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Creating a PdfFont
            var font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
            document.Add(new Paragraph("itext:").SetFont(font));

            var list = new List().SetSymbolIndent(12).SetListSymbol("\u2022").SetFont(font);
            list.Add(new ListItem("Never gonna give you up"))
                .Add(new ListItem("Never gonna let you down"))
                .Add(new ListItem("Never gonna run around and desert you"))
                .Add(new ListItem("Never gonna make you cry"))
                .Add(new ListItem("Never gonna say goodbye"))
                .Add(new ListItem("Never gonna tell a lie and hurt you"));
            document.Add(list);
            document.Close();
        }
    }
}
