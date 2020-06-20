namespace InvoiceManger.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Invoices", "InvoiceNumber", c => c.String(nullable: false));
            AlterColumn("dbo.Invoices", "Date", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.Invoices", "RecDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.People", "PersonName", c => c.String(nullable: false));
            AlterColumn("dbo.Departments", "DepartmentName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Departments", "DepartmentName", c => c.String());
            AlterColumn("dbo.People", "PersonName", c => c.String());
            AlterColumn("dbo.Invoices", "RecDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Invoices", "Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Invoices", "InvoiceNumber", c => c.String());
        }
    }
}
