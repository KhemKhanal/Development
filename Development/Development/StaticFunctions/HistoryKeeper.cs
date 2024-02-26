using Development.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Development.StaticFunctions
{
    public class HistoryKeeper
    {
        public static Calculations AppendToHistory(string date, string time, string show, string answer)
        {
            string theFileName = "CalHistoryData.txt";
            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), theFileName);

            List<string> histories = new List<string>
            {
                $"{date},{time},{show},{answer}"
            };

            File.AppendAllLines(file, histories);

            return new Calculations(date, time, show, answer);
        }

        public static ObservableCollection<Calculations> ReadHistoryList()
        {
            LinkedList<Calculations> histories = new LinkedList<Calculations>();
            List<Calculations> remove = new List<Calculations>();
            ObservableCollection<Calculations> historyList;

            string theFileName = "CalHistoryData.txt";
            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), theFileName);

            List<string> lines = File.ReadAllLines(file).ToList();

            if (lines.Count > 50)
            {
                int removable = lines.Count - 50;

                for (int i = 0; i < removable; i++)
                {
                    lines.Remove(lines[i]);
                }
            }

            DateTime now = DateTime.Now;
            int CurMonth = now.Month;

            foreach (var line in lines)
            {

                string[] enties = line.Split(',');

                Calculations history = new Calculations
                {
                    Date = enties[0],
                    Time = enties[1],
                    Showw = enties[2],
                    Answerr = enties[3]
                };

                int month = Convert.ToInt32(history.Date.Substring(5, 2));

                //Check if the record is older than 5 months

                //If the difference is less than 5 add to the list

                int diff = MonthDifferance(CurMonth, month);
                if (diff < 5)
                {
                    histories.AddFirst(history);
                }
                else
                {
                    remove.Add(history);
                }
            }

            if (remove.Count > 0)
            {
                List<string> strings = new List<string>();

                for (int i = 0; i < histories.Count; i++)
                {
                    remove = new List<Calculations>(histories);
                    string info = $"{remove[i].Date},{remove[i].Time},{remove[i].Showw},{remove[i].Answerr}";
                    strings.Add(info);
                }

                File.WriteAllLines(file, strings);
            }


            historyList = new ObservableCollection<Calculations>(histories);

            //int count = historyList.Count;

            return historyList;
        }

        public static int MonthDifferance(int CurMonth, int RecordMonth)
        {
            if (CurMonth < RecordMonth)
            {
                return (CurMonth + 12) - RecordMonth;
            }
            else
            {
                return CurMonth - RecordMonth;
            }
        }

    }
}
