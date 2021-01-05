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
        // static readonly string target = @"D:\Temp\Astley.pdf";

        private static void Hello(string target)
        {
            var dest = new FileInfo(target);

            var writer = new PdfWriter(dest);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            document.Add(new Paragraph("Hello World 2!"));
            document.Close();
        }

        private static void Astley(string target)
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

        private static void FoxAndDog(string target, string dogf, string foxf)
        {
            var dest = new FileInfo(target);

            var writer = new PdfWriter(dest);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            iText.Layout.Element.Image fox = new Image(ImageDataFactory.Create(foxf));
            iText.Layout.Element.Image dog = new Image(ImageDataFactory.Create(dogf));
            Paragraph p = new Paragraph("The quick brown ").Add(fox).Add(" jumps over the lazy ").Add(dog);
            document.Add(p);
            document.Close();
        }

        private static void DataTable(string target, string data)
        {
            var dest = new FileInfo(target);

            var writer = new PdfWriter(dest);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4.Rotate());

            document.SetMargins(20, 20, 20, 20);

            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            var table = new Table(UnitValue.CreatePercentArray(new float[]
            {
                4, 1, 3, 4, 3, 3, 3, 3, 1 
            })).UseAllAvailableWidth();

            using (StreamReader sr = File.OpenText(data))
            {
                var line = sr.ReadLine();
                Process(table, line, bold, true);
                while ((line = sr.ReadLine()) != null)
                {
                    Process(table, line, font, false);
                }
            }

            document.Add(table);
            document.Close();
        }

        public static void Process(Table table, String line, PdfFont font, bool isHeader)
        {
            var tokenizer = new StringTokenizer(line, ";");
            while (tokenizer.HasMoreTokens())
            {
                var cell = new Cell().Add(new Paragraph(tokenizer.NextToken()).SetFont(font));
                if (isHeader)
                {
                    table.AddHeaderCell(cell);
                } else
                {
                    table.AddCell(cell);
                }
            }
        }

        public struct Giorno
        {
            int numero;
            string nome;

            public Giorno(int numero, string nome)
            {
                this.numero = numero;
                this.nome = nome;
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

        private static void Calendar(string target)
        {
            var dest = new FileInfo(target);

            var writer = new PdfWriter(dest);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4);

            var cellHeight = (PageSize.A4.GetHeight() - 40) / 18;

            document.SetMargins(20, 20, 20, 20);

            var table = new Table(new float[2]).UseAllAvailableWidth();
            table.SetMarginTop(0);
            table.SetMarginBottom(0);

            var rosso = new DeviceRgb(245, 15, 15);
            var bianco = new DeviceRgb(255, 255, 255);

            var cell = new Cell(2, 2).Add(new Paragraph("Gennaio"));
            cell.SetTextAlignment(TextAlignment.CENTER);
            cell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            cell.SetFontColor(bianco);
            cell.SetBackgroundColor(rosso);
            // cell.SetHeight(cellHeight * 2);
            table.AddCell(cell);

            // popoliamo il mese
            var mese = new Giorno[32];
            // TODO vedere i mesi e quando inizia l'anno
            for (int i = 1; i <= 31; i++)
            {
                mese[i - 1] = new Giorno(i, "Dormidì");
            }
            mese[31] = new Giorno(0, "");

            for (int i = 0; i < 16; i++)
            {
                cell = new Cell().Add(new Paragraph(mese[i].Stampa())).SetHeight(cellHeight);
                table.AddCell(cell);
                cell = new Cell().Add(new Paragraph(mese[i + 16].Stampa())).SetHeight(cellHeight);
                table.AddCell(cell);
            }

            document.Add(table);
            document.Close();

        }

        static void Main(string[] args)
        {
            //Hello(@"D:\Temp\Hello.pdf");
            // Astley(@"D:\Temp\Astley.pdf");
            // FoxAndDog(@"D:\Temp\FoxAndDog.pdf", @"D:\Temp\dog.bmp", @"D:\Temp\fox.bmp");
            // DataTable(@"D:\Temp\DataTable.pdf", @"D:\Temp\united_states.csv");
            Calendar(@"D:\Temp\Gennaio.pdf");
        }
    }
}
