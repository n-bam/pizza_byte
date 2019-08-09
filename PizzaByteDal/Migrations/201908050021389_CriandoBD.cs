namespace PizzaByteDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriandoBD : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "PizzaByte.Funcionario", name: "ix_Nome_Usuario", newName: "ix_Nome_Funcionario");
        }
        
        public override void Down()
        {
            RenameIndex(table: "PizzaByte.Funcionario", name: "ix_Nome_Funcionario", newName: "ix_Nome_Usuario");
        }
    }
}
