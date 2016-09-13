using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ChartFinal
{
    class APISTATDataModel
    {

        //|Last_Time|Current_Time|Queue_Name|Last_Count|Increase_Count|Decrease_Count|Current_Count|Last_Suspend_Count|Suspend_Increase_Count|Suspend_Decrease_Count|Current_Suspend_Coun
        DateTime _Last_Time;
        DateTime _Current_Time;
        static DateTime _MAX_TIME;
        String _Queue_Name;
        int _Last_Count;
        int _Increase_Count;
        int _Decrease_Count;
        int _Current_Count;
        int _Suspend_Count;


        #region Properties

        public DateTime Last_Time
        {
            get
            {
                return _Last_Time;
            }

            set
            {
                _Last_Time = value;

            }
        }

        public DateTime Current_Time
        {
            get
            {
                return _Current_Time;
            }

            set
            {
                _Current_Time = value;

            }
        }

        public string Queue_Name
        {
            get
            {
                return _Queue_Name;
            }

            set
            {
                _Queue_Name = value;

            }
        }

        public int Last_Count
        {
            get
            {
                return _Last_Count;
            }

            set
            {
                _Last_Count = value;

            }
        }

        public int Increase_Count
        {
            get
            {
                return _Increase_Count;
            }

            set
            {
                _Increase_Count = value;

            }
        }

        public int Decrease_Count
        {
            get
            {
                return _Decrease_Count;
            }

            set
            {
                _Decrease_Count = value;

            }
        }

        public int Current_Count
        {
            get
            {
                return _Current_Count;
            }

            set
            {
                _Current_Count = value;

            }
        }

        public static DateTime MAX_TIME
        {
            get
            {
                return _MAX_TIME;
            }

            set
            {
                _MAX_TIME = value;
            }
        }

        public int Suspend_Count
        {
            get
            {
                return _Suspend_Count;
            }

            set
            {
                _Suspend_Count = value;
            }
        }
        #endregion

        public static ObservableCollection<APISTATDataModel> GetFromFile(string path, string[] Delimiters, int lastn, string qname)
        {
            ObservableCollection<APISTATDataModel> outTo = new ObservableCollection<APISTATDataModel>();
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {

                csvParser.SetDelimiters(Delimiters);
                csvParser.HasFieldsEnclosedInQuotes = false;

                // Skip the row with the column names
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvParser.ReadFields();

                    if (fields[2].Equals(qname))
                    {
                        outTo.Add(new APISTATDataModel()
                        {
                            Last_Time = DateTime.ParseExact(fields[0], "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                            Current_Time = DateTime.ParseExact(fields[1], "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                            Queue_Name = fields[2],
                            Last_Count = Int32.Parse(fields[3]),
                            Increase_Count = Int32.Parse(fields[4]),
                            Decrease_Count = Int32.Parse(fields[5]),
                            Current_Count = Int32.Parse(fields[6]),
                            Suspend_Count= int.Parse(fields[10])
                        });


                    }
                }


            }


            return new ObservableCollection<APISTATDataModel>(HelperClass.TakeLast(outTo, lastn));
        }
        public static ObservableCollection<APISTATDataModel> GetFromFile(string path, string[] Delimiters, string qname)
        {
            ObservableCollection<APISTATDataModel> outTo = new ObservableCollection<APISTATDataModel>();
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {

                csvParser.SetDelimiters(Delimiters);
                csvParser.HasFieldsEnclosedInQuotes = false;

                // Skip the row with the column names
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvParser.ReadFields();

                    if (fields[2].Equals(qname))
                    {
                       outTo.Add(new APISTATDataModel()
                        {

                            Last_Time = DateTime.ParseExact(fields[0], "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                            Current_Time = DateTime.ParseExact(fields[1], "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                            Queue_Name = fields[2],
                            Last_Count = Int32.Parse(fields[3]),
                            Increase_Count = Int32.Parse(fields[4]),
                            Decrease_Count = Int32.Parse(fields[5]),
                            Current_Count = Int32.Parse(fields[6]),
                           Suspend_Count = int.Parse(fields[10])
                       });


                    }
                }
                return outTo;
            }
        }
    }

    public static class HelperClass
    {

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }
    }
}
