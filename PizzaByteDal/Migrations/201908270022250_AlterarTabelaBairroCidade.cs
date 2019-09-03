namespace PizzaByteDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterarTabelaBairroCidade : DbMigration
    {
        public override void Up()
        {
            DropIndex("PizzaByte.TaxasEntrega", "IX_Bairro_TaxaEntrega");
            AddColumn("PizzaByte.TaxasEntrega", "BairroCidade", c => c.String(nullable: false, maxLength: 101, unicode: false));
            CreateIndex("PizzaByte.TaxasEntrega", "BairroCidade", unique: true, name: "IX_Bairro_TaxaEntrega");
            DropColumn("PizzaByte.TaxasEntrega", "Bairro");
        }
        
        public override void Down()
        {
            AddColumn("PizzaByte.TaxasEntrega", "Bairro", c => c.String(nullable: false, maxLength: 50, unicode: false));
            DropIndex("PizzaByte.TaxasEntrega", "IX_Bairro_TaxaEntrega");
            DropColumn("PizzaByte.TaxasEntrega", "BairroCidade");
            CreateIndex("PizzaByte.TaxasEntrega", "Bairro", unique: true, name: "IX_Bairro_TaxaEntrega");
        }
    }
}
