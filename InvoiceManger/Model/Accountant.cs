using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using GalaSoft.MvvmLight;

namespace InvoiceManger.Model
{
   public class Accountant:ObservableObject
    {
        [ForeignKey("Person")]
        public int AccountantId { get; set; }
        public string Password { get; set; }
        public virtual Person Person { get; set; }
        public ICollection<Invoice> OperInvoices { get; set; }

    }
}
