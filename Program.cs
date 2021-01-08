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

namespace DuckCalendar
{
    class Program
    {
        /**
        Currently the program generates the calendar based on the month when the program is invoked.
        January - September: current year.
        October - December: next year.
         */
        static void Main(string[] args)
        {
            DateTime now = DateTime.Now;
            int year = now.Year;
            if (now.Month > 9)
            {
                year += 1;
            }

            string directory = @"D:\Temp\";
            string filenameradix = "Calendario";
            string filename = $"{directory}{filenameradix}{year}.pdf";

            Month[] calendar= CalendarGenerator.GenerateYear(year);
            CalendarPrinter.Print(filename, calendar);
        }
    }
}
