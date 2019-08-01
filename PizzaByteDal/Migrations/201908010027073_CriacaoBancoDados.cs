namespace PizzaByteDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriacaoBancoDados : DbMigration
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
                "PizzaByte.Clientes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Nome = c.String(nullable: false, maxLength: 150, unicode: false),
                        Telefone = c.String(maxLength: 20, unicode: false),
                        Cpf = c.String(maxLength: 11, unicode: false),
                        DataInclusao = c.DateTime(nullable: false),
                        DataAlteracao = c.DateTime(),
                        Inativo = c.Boolean(nullable: false),
                        Excluido = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, unique: true)
                .Index(t => t.Nome, name: "ix_Nome_Cliente");
            
            CreateTable(
                "PizzaByte.ClientesEnderecos",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        NumeroEndereco = c.String(nullable: false, maxLength: 10, unicode: false),
                        ComplementeEndereco = c.String(maxLength: 50, unicode: false),
                        IdCliente = c.Guid(nullable: false),
                        IdCep = c.Guid(nullable: false),
                        DataInclusao = c.DateTime(nullable: false),
                        DataAlteracao = c.DateTime(),
                        Inativo = c.Boolean(nullable: false),
                        Excluido = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("PizzaByte.Clientes", t => t.IdCliente, cascadeDelete: true)
                .ForeignKey("PizzaByte.Ceps", t => t.IdCep, cascadeDelete: true)
                .Index(t => t.IdCliente, name: "IX_Cliente_Endereco")
                .Index(t => t.IdCep);
            
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
                "PizzaByte.Funcionario",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Nome = c.String(nullable: false, maxLength: 150, unicode: false),
                        Telefone = c.String(maxLength: 8, unicode: false),
                        Tipo = c.Int(nullable: false),
                        DataInclusao = c.DateTime(nullable: false),
                        DataAlteracao = c.DateTime(),
                        Inativo = c.Boolean(nullable: false),
                        Excluido = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Nome, name: "ix_Nome_Usuario");
            
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
                "PizzaByte.Suporte",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Mensagem = c.String(nullable: false, maxLength: 500, unicode: false),
                        Tipo = c.Int(nullable: false),
                        DataInclusao = c.DateTime(nullable: false),
                        Excluido = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "PizzaByte.TaxasEntrega",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Bairro = c.String(nullable: false, maxLength: 50, unicode: false),
                        ValorTaxa = c.Single(nullable: false),
                        DataInclusao = c.DateTime(nullable: false),
                        DataAlteracao = c.DateTime(),
                        Inativo = c.Boolean(nullable: false),
                        Excluido = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Bairro, unique: true, name: "IX_Bairro_TaxaEntrega");
            
            CreateTable(
                "PizzaByte.Usuarios",
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
            DropForeignKey("PizzaByte.ClientesEnderecos", "IdCep", "PizzaByte.Ceps");
            DropForeignKey("PizzaByte.ClientesEnderecos", "IdCliente", "PizzaByte.Clientes");
            DropIndex("PizzaByte.Usuarios", "IX_Email_Usuario");
            DropIndex("PizzaByte.TaxasEntrega", "IX_Bairro_TaxaEntrega");
            DropIndex("PizzaByte.Produtos", "IX_Produto_Descricao");
            DropIndex("PizzaByte.Produtos", "IX_Produto_Id");
            DropIndex("PizzaByte.Log", "ix_Recurso_Log");
            DropIndex("PizzaByte.Funcionario", "ix_Nome_Usuario");
            DropIndex("PizzaByte.Fornecedores", new[] { "IdCep" });
            DropIndex("PizzaByte.Fornecedores", new[] { "Cnpj" });
            DropIndex("PizzaByte.Fornecedores", "ix_NomeFantasia_Usuario");
            DropIndex("PizzaByte.ClientesEnderecos", new[] { "IdCep" });
            DropIndex("PizzaByte.ClientesEnderecos", "IX_Cliente_Endereco");
            DropIndex("PizzaByte.Clientes", "ix_Nome_Cliente");
            DropIndex("PizzaByte.Clientes", new[] { "Id" });
            DropIndex("PizzaByte.Ceps", new[] { "Cep" });
            DropIndex("PizzaByte.Ceps", new[] { "Id" });
            DropTable("PizzaByte.Usuarios");
            DropTable("PizzaByte.TaxasEntrega");
            DropTable("PizzaByte.Suporte");
            DropTable("PizzaByte.Produtos");
            DropTable("PizzaByte.Log");
            DropTable("PizzaByte.Funcionario");
            DropTable("PizzaByte.Fornecedores");
            DropTable("PizzaByte.ClientesEnderecos");
            DropTable("PizzaByte.Clientes");
            DropTable("PizzaByte.Ceps");
        }
    }
}
