namespace PizzaByteDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdicionarNovoBd : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "PizzaByte.Funcionario", newName: "Funcionarios");
            AlterColumn("PizzaByte.Funcionarios", "Telefone", c => c.String(maxLength: 20, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("PizzaByte.Funcionarios", "Telefone", c => c.String(maxLength: 8, unicode: false));
            RenameTable(name: "PizzaByte.Funcionarios", newName: "Funcionario");
        }
    }
}
