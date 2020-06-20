using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceManger.Model
{
   public class Person
    {
        public int PersonId { get; set; }
        [Required]
        public string PersonName { get; set; }
        [ForeignKey("Department")]
        public int DepId { get; set; }
        public Department Department { get; set; }
        public virtual Accountant Accountant { get; set; }
        public ICollection<Invoice> PersonInvoices { get; set; }
    }
}
