using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace InvoiceManger.Model
{
   public class ProgramData:ObservableObject
    {
        private int myVar;

        public int MyVar
        {
            get { return myVar; }
            set { myVar = value;RaisePropertyChanged(() => MyVar); }
        }

    }
}
