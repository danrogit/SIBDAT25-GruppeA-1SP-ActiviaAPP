using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiviaAPP.Classes
{
    internal class ActivityStore
    {
        //Delt liste-objekt for oprettede aktiviteter, som kan tilgås fra både Admin og User
        public static ObservableCollection<ActivityClass> activities { get; } = new ObservableCollection<ActivityClass>();
    }
}