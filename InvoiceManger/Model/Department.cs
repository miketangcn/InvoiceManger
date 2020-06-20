using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace InvoiceManger.Model
{
   public class Department:ObservableObject
    {
        public int DepartmentId { get; set; }
        [Required]
        public string DepartmentName { get; set; }
        public ICollection<Person> Persons { get; set; }
    }
}
