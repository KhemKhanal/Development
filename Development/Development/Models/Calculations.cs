using System;
using System.Collections.Generic;
using System.Text;

namespace Development.Models
{
    public class Calculations
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string Showw { get; set; }
        public string Answerr { get; set; }
        

        public Calculations(string date, string time, string show, string answer) 
        {
            Date = date;
            Time = time;
            Showw = show; 
            Answerr = answer;
        }

        public Calculations() { }


    }
}
