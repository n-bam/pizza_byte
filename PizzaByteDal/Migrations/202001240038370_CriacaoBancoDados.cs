namespace PizzaByteDal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CriacaoBancoDados : DbMigration
    {
        public override void Up()
        {
            PizzaByteContexto contexto = new PizzaByteContexto();
            if (!contexto.Database.Exists())
            {
                Sql("CREATE DATABASE PizzariaNacoes COLLATE COLLATE SQL_LATIN1_GENERAL_CP1_CI_AI");
            }

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
                    ComplementoEndereco = c.String(maxLength: 50, unicode: false),
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
                "PizzaByte.ContaPagar",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    DataPagamento = c.DateTime(storeType: "date"),
                    IdFornecedor = c.Guid(),
                    DataVencimento = c.DateTime(nullable: false, storeType: "date"),
                    Descricao = c.String(nullable: false, maxLength: 200, unicode: false),
                    Valor = c.Single(nullable: false),
                    DataCompetencia = c.DateTime(nullable: false, storeType: "date"),
                    Status = c.Int(nullable: false),
                    DataInclusao = c.DateTime(nullable: false),
                    DataAlteracao = c.DateTime(),
                    Excluido = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Descricao, name: "ix_Nome_ContaPagar");

            CreateTable(
                "PizzaByte.ContaReceber",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    IdPedido = c.Guid(),
                    DataVencimento = c.DateTime(nullable: false, storeType: "date"),
                    Descricao = c.String(nullable: false, maxLength: 200, unicode: false),
                    Valor = c.Single(nullable: false),
                    DataCompetencia = c.DateTime(nullable: false, storeType: "date"),
                    Status = c.Int(nullable: false),
                    DataInclusao = c.DateTime(nullable: false),
                    DataAlteracao = c.DateTime(),
                    Excluido = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Descricao, name: "ix_Nome_ContaReceber");

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
                "PizzaByte.Funcionarios",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Nome = c.String(nullable: false, maxLength: 150, unicode: false),
                    Telefone = c.String(maxLength: 20, unicode: false),
                    Tipo = c.Int(nullable: false),
                    DataInclusao = c.DateTime(nullable: false),
                    DataAlteracao = c.DateTime(),
                    Inativo = c.Boolean(nullable: false),
                    Excluido = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Nome, name: "ix_Nome_Funcionario");

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
                "PizzaByte.MovimentosCaixa",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Justificativa = c.String(nullable: false, maxLength: 8000, unicode: false),
                    Valor = c.Single(nullable: false),
                    DataInclusao = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.DataInclusao, name: "ix_Data_MovimentoCaixa");

            CreateTable(
                "PizzaByte.Pedidos",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Tipo = c.Int(nullable: false),
                    Total = c.Single(nullable: false),
                    Troco = c.Single(nullable: false),
                    TaxaEntrega = c.Single(nullable: false),
                    RecebidoDinheiro = c.Single(nullable: false),
                    RecebidoCredito = c.Single(nullable: false),
                    RecebidoDebito = c.Single(nullable: false),
                    Obs = c.String(maxLength: 2000, unicode: false),
                    PedidoIfood = c.Boolean(nullable: false),
                    JustificativaCancelamento = c.String(maxLength: 100, unicode: false),
                    IdCliente = c.Guid(),
                    DataInclusao = c.DateTime(nullable: false),
                    DataAlteracao = c.DateTime(),
                    Inativo = c.Boolean(nullable: false),
                    Excluido = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("PizzaByte.Clientes", t => t.IdCliente)
                .Index(t => t.IdCliente, name: "ix_IdCliente_Pedido");

            CreateTable(
                "PizzaByte.PedidosEntregas",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Conferido = c.Boolean(nullable: false),
                    ValorRetorno = c.Single(nullable: false),
                    Obs = c.String(maxLength: 2000, unicode: false),
                    IdEndereco = c.Guid(nullable: false),
                    IdFuncionario = c.Guid(),
                    IdPedido = c.Guid(nullable: false),
                    DataInclusao = c.DateTime(nullable: false),
                    DataAlteracao = c.DateTime(),
                    Inativo = c.Boolean(nullable: false),
                    Excluido = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("PizzaByte.ClientesEnderecos", t => t.IdEndereco, cascadeDelete: true)
                .ForeignKey("PizzaByte.Funcionarios", t => t.IdFuncionario)
                .ForeignKey("PizzaByte.Pedidos", t => t.IdPedido, cascadeDelete: true)
                .Index(t => t.IdEndereco, name: "ix_IdEndereco_Entrega")
                .Index(t => t.IdFuncionario)
                .Index(t => t.IdPedido, name: "ix_IdPedido_Entrega");

            CreateTable(
                "PizzaByte.PedidosItens",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    DescricaoProduto = c.String(nullable: false, maxLength: 300, unicode: false),
                    PrecoProduto = c.Single(nullable: false),
                    TipoProduto = c.Int(nullable: false),
                    Quantidade = c.Single(nullable: false),
                    IdProdutoComposto = c.Guid(),
                    IdProduto = c.Guid(nullable: false),
                    IdPedido = c.Guid(nullable: false),
                    DataInclusao = c.DateTime(nullable: false),
                    DataAlteracao = c.DateTime(),
                    Inativo = c.Boolean(nullable: false),
                    Excluido = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("PizzaByte.Pedidos", t => t.IdPedido, cascadeDelete: true)
                .ForeignKey("PizzaByte.Produtos", t => t.IdProduto, cascadeDelete: true)
                .ForeignKey("PizzaByte.Produtos", t => t.IdProdutoComposto)
                .Index(t => t.IdProdutoComposto)
                .Index(t => t.IdProduto)
                .Index(t => t.IdPedido, name: "ix_IdPedido_Item");

            CreateTable(
                "PizzaByte.Produtos",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Descricao = c.String(nullable: false, maxLength: 150, unicode: false),
                    Preco = c.Single(nullable: false),
                    Detalhes = c.String(maxLength: 200, unicode: false),
                    Tipo = c.Int(nullable: false),
                    DataInclusao = c.DateTime(nullable: false),
                    DataAlteracao = c.DateTime(),
                    Inativo = c.Boolean(nullable: false),
                    Excluido = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, unique: true, name: "IX_Produto_Id")
                .Index(t => t.Descricao, unique: true, name: "IX_Produto_Descricao");

            Sql("INSERT INTO PizzaByte.Produtos(Id, Descricao, Preco, Tipo, DataInclusao, DataAlteracao, Inativo, Excluido) " +
               "Values('E6219299-E232-43C8-B07E-C7B1CAD8C19D', 'Brinde da promoção (seg-qui)', 0, 2, GETDATE(), NULL, 0, 0)");

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
                    BairroCidade = c.String(nullable: false, maxLength: 101, unicode: false),
                    ValorTaxa = c.Single(nullable: false),
                    DataInclusao = c.DateTime(nullable: false),
                    DataAlteracao = c.DateTime(),
                    Excluido = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.BairroCidade, unique: true, name: "IX_Bairro_TaxaEntrega");

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
            DropForeignKey("PizzaByte.PedidosItens", "IdProdutoComposto", "PizzaByte.Produtos");
            DropForeignKey("PizzaByte.PedidosItens", "IdProduto", "PizzaByte.Produtos");
            DropForeignKey("PizzaByte.PedidosItens", "IdPedido", "PizzaByte.Pedidos");
            DropForeignKey("PizzaByte.PedidosEntregas", "IdPedido", "PizzaByte.Pedidos");
            DropForeignKey("PizzaByte.PedidosEntregas", "IdFuncionario", "PizzaByte.Funcionarios");
            DropForeignKey("PizzaByte.PedidosEntregas", "IdEndereco", "PizzaByte.ClientesEnderecos");
            DropForeignKey("PizzaByte.Pedidos", "IdCliente", "PizzaByte.Clientes");
            DropForeignKey("PizzaByte.Fornecedores", "IdCep", "PizzaByte.Ceps");
            DropForeignKey("PizzaByte.ClientesEnderecos", "IdCep", "PizzaByte.Ceps");
            DropForeignKey("PizzaByte.ClientesEnderecos", "IdCliente", "PizzaByte.Clientes");
            DropIndex("PizzaByte.Usuarios", "IX_Email_Usuario");
            DropIndex("PizzaByte.TaxasEntrega", "IX_Bairro_TaxaEntrega");
            DropIndex("PizzaByte.Produtos", "IX_Produto_Descricao");
            DropIndex("PizzaByte.Produtos", "IX_Produto_Id");
            DropIndex("PizzaByte.PedidosItens", "ix_IdPedido_Item");
            DropIndex("PizzaByte.PedidosItens", new[] { "IdProduto" });
            DropIndex("PizzaByte.PedidosItens", new[] { "IdProdutoComposto" });
            DropIndex("PizzaByte.PedidosEntregas", "ix_IdPedido_Entrega");
            DropIndex("PizzaByte.PedidosEntregas", new[] { "IdFuncionario" });
            DropIndex("PizzaByte.PedidosEntregas", "ix_IdEndereco_Entrega");
            DropIndex("PizzaByte.Pedidos", "ix_IdCliente_Pedido");
            DropIndex("PizzaByte.MovimentosCaixa", "ix_Data_MovimentoCaixa");
            DropIndex("PizzaByte.Log", "ix_Recurso_Log");
            DropIndex("PizzaByte.Funcionarios", "ix_Nome_Funcionario");
            DropIndex("PizzaByte.Fornecedores", new[] { "IdCep" });
            DropIndex("PizzaByte.Fornecedores", new[] { "Cnpj" });
            DropIndex("PizzaByte.Fornecedores", "ix_NomeFantasia_Usuario");
            DropIndex("PizzaByte.ContaReceber", "ix_Nome_ContaReceber");
            DropIndex("PizzaByte.ContaPagar", "ix_Nome_ContaPagar");
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
            DropTable("PizzaByte.PedidosItens");
            DropTable("PizzaByte.PedidosEntregas");
            DropTable("PizzaByte.Pedidos");
            DropTable("PizzaByte.MovimentosCaixa");
            DropTable("PizzaByte.Log");
            DropTable("PizzaByte.Funcionarios");
            DropTable("PizzaByte.Fornecedores");
            DropTable("PizzaByte.ContaReceber");
            DropTable("PizzaByte.ContaPagar");
            DropTable("PizzaByte.ClientesEnderecos");
            DropTable("PizzaByte.Clientes");
            DropTable("PizzaByte.Ceps");
        }
    }
}
