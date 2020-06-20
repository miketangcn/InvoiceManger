namespace InvoiceManger.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20200620 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.People", "PersonName", c => c.String(nullable: false));
            AlterColumn("dbo.Departments", "DepartmentName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Departments", "DepartmentName", c => c.String());
            AlterColumn("dbo.People", "PersonName", c => c.String());
        }
    }
}
