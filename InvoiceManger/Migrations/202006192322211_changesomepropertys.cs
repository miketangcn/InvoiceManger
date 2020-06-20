namespace InvoiceManger.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changesomepropertys : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Invoices", "InvoiceNumber", c => c.String(nullable: false));
            AlterColumn("dbo.Invoices", "Date", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.Invoices", "RecDate", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Invoices", "RecDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Invoices", "Date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Invoices", "InvoiceNumber", c => c.String());
        }
    }
}
