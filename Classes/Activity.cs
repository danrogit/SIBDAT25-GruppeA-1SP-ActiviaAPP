using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace ActiviaAPP.Classes
{
    internal class ActivityClass
    {

        public string activityTitle;
        public int groupSize;
        public DateTime date;
        public double activityPrice;

        public string ActivityTitle { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
        public DateTime Date { get; set; } 
        public string CoverImagePath { get; set; } = string.Empty;
        //public double activityPrice { get; set; }


        public override string ToString()
        {
            //Viser dato i formatet dd-MM-yyyy i listen
            return $"{ActivityTitle} ({Date:dd-MM-yyyy})";
        }
        public void CreateActivity()
        {



        }

    }
}
