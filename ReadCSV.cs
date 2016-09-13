using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace ChartFinal
{
   static  class ReadCSV
    {
     ///<summary>
     /// Reads the String data from a file and returns each value as String
     /// Reads Only 2nd Line
     /// </summary>   
        public static string[] getDataFromFile(string path, string seperetor)

        {
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.SetDelimiters(seperetor);
                csvParser.HasFieldsEnclosedInQuotes = false;
                csvParser.ReadLine();
                string[] fields = csvParser.ReadFields();
                return fields;

            }

        }
    }
}
