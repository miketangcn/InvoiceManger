using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceManger.Model
{
   public class DataModel: DbContext
    {
        public DataModel() : base("name=DataModel")
        {
            //Database.SetInitializer<DataModel>(new DropCreateDatabaseIfModelChanges<DataModel>());
           // Database.SetInitializer<DataModel>(new CreateDatabaseIfNotExists<DataModel>());
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataModel, InvoiceManger.Migrations.Configuration>());
            // Database.SetInitializer<DataModel>(null);
        }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Accountant> Accountants { get; set; }
        public DbSet<Person> Persons { get; set; }

    }
}
