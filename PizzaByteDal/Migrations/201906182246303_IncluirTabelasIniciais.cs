namespace PizzaByteDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncluirTabelasIniciais : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "PizzaByte.Ceps",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Logradouro = c.String(nullable: false, maxLength: 150, unicode: false),
                        Cidade = c.String(nullable: false, maxLength: 50, unicode: false),
                        Bairro = c.String(nullable: false, maxLength: 50, unicode: false),
                        Cep = c.String(nullable: false, maxLength: 8, unicode: false),
                        DataInclusao = c.DateTime(nullable: false),
                        DataAlteracao = c.DateTime(),
                        Inativo = c.Boolean(nullable: false),
                        Excluido = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, unique: true)
                .Index(t => t.Cep, unique: true);
            
            CreateTable(
                "PizzaByte.Fornecedores",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        NomeFantasia = c.String(nullable: false, maxLength: 150, unicode: false),
                        RazaoSocial = c.String(maxLength: 150, unicode: false),
                        Telefone = c.String(maxLength: 20, unicode: false),
                        Cnpj = c.String(maxLength: 14, unicode: false),
                        NumeroEndereco = c.String(maxLength: 10, unicode: false),
                        ComplementoEndereco = c.String(maxLength: 50, unicode: false),
                        Obs = c.String(maxLength: 2000, unicode: false),
                        IdCep = c.Guid(),
                        DataInclusao = c.DateTime(nullable: false),
                        DataAlteracao = c.DateTime(),
                        Inativo = c.Boolean(nullable: false),
                        Excluido = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("PizzaByte.Ceps", t => t.IdCep)
                .Index(t => t.NomeFantasia, name: "ix_NomeFantasia_Usuario")
                .Index(t => t.Cnpj)
                .Index(t => t.IdCep);
            
            CreateTable(
                "PizzaByte.Log",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Recurso = c.Int(nullable: false),
                        IdUsuario = c.Guid(nullable: false),
                        IdEntidade = c.Guid(nullable: false),
                        Mensagem = c.String(nullable: false, maxLength: 8000, unicode: false),
                        DataInclusao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Recurso, name: "ix_Recurso_Log");
            
            CreateTable(
                "PizzaByte.Produtos",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Descricao = c.String(nullable: false, maxLength: 150, unicode: false),
                        Preco = c.Single(nullable: false),
                        Tipo = c.Int(nullable: false),
                        DataInclusao = c.DateTime(nullable: false),
                        DataAlteracao = c.DateTime(),
                        Inativo = c.Boolean(nullable: false),
                        Excluido = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, unique: true, name: "IX_Produto_Id")
                .Index(t => t.Descricao, unique: true, name: "IX_Produto_Descricao");
            
            CreateTable(
                "PizzaByte.Usuario",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Nome = c.String(nullable: false, maxLength: 150, unicode: false),
                        Email = c.String(nullable: false, maxLength: 100, unicode: false),
                        Senha = c.String(nullable: false, maxLength: 50, unicode: false),
                        Administrador = c.Boolean(nullable: false),
                        DataInclusao = c.DateTime(nullable: false),
                        DataAlteracao = c.DateTime(),
                        Inativo = c.Boolean(nullable: false),
                        Excluido = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true, name: "IX_Email_Usuario");
            
        }
        
        public override void Down()
        {
            DropForeignKey("PizzaByte.Fornecedores", "IdCep", "PizzaByte.Ceps");
            DropIndex("PizzaByte.Usuario", "IX_Email_Usuario");
            DropIndex("PizzaByte.Produtos", "IX_Produto_Descricao");
            DropIndex("PizzaByte.Produtos", "IX_Produto_Id");
            DropIndex("PizzaByte.Log", "ix_Recurso_Log");
            DropIndex("PizzaByte.Fornecedores", new[] { "IdCep" });
            DropIndex("PizzaByte.Fornecedores", new[] { "Cnpj" });
            DropIndex("PizzaByte.Fornecedores", "ix_NomeFantasia_Usuario");
            DropIndex("PizzaByte.Ceps", new[] { "Cep" });
            DropIndex("PizzaByte.Ceps", new[] { "Id" });
            DropTable("PizzaByte.Usuario");
            DropTable("PizzaByte.Produtos");
            DropTable("PizzaByte.Log");
            DropTable("PizzaByte.Fornecedores");
            DropTable("PizzaByte.Ceps");
        }
    }
}
