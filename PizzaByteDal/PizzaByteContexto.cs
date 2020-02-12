using PizzaByteDal.Restricoes;
using PizzaByteVo;
using PizzaByteVo.Base;
using System;
using System.Data.Entity;

namespace PizzaByteDal
{
    public class PizzaByteContexto : DbContext
    {
        public PizzaByteContexto() : base()
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            // LOCAL
            //Database.Connection.ConnectionString = @"Data Source =.\SQLExpress;Initial Catalog = PizzariaNacoes; User ID=sa;Password=PASS; Connection Timeout=30;Trusted_Connection=false;Encrypt=false;Connection Timeout=30";

            // PRODUÇÃO.
            Database.Connection.ConnectionString = "STRING CONEXÃO";
     }

        protected override void OnModelCreating(DbModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);
            modelbuilder.Properties<string>().Configure(p => p.HasColumnType("varchar"));

            modelbuilder.Configurations.Add(new ProdutoRestricoes());
            modelbuilder.Configurations.Add(new FornecedorRestricoes());
            modelbuilder.Configurations.Add(new CepRestricoes());
            modelbuilder.Configurations.Add(new UsuarioRestricoes());
            modelbuilder.Configurations.Add(new LogRestricoes());
            modelbuilder.Configurations.Add(new TaxaEntregaRestricoes());
            modelbuilder.Configurations.Add(new ClienteRestricoes());
            modelbuilder.Configurations.Add(new ClienteEnderecoRestricoes());
            modelbuilder.Configurations.Add(new SuporteRestricoes());
            modelbuilder.Configurations.Add(new FuncionarioRestricoes());
            modelbuilder.Configurations.Add(new ContaPagarRestricoes());
            modelbuilder.Configurations.Add(new ContaReceberRestricoes());
            modelbuilder.Configurations.Add(new PedidoRestricoes());
            modelbuilder.Configurations.Add(new PedidoItemRestricoes());
            modelbuilder.Configurations.Add(new PedidoEntregaRestricoes());
            modelbuilder.Configurations.Add(new MovimentoCaixaRestricoes());

        }

        public DbSet<ContaPagarVo> ContaPagar { get; set; }
        public DbSet<ContaReceberVo> ContaReceber { get; set; }
        public DbSet<FuncionarioVo> Funcionario { get; set; }
        public DbSet<ProdutoVo> Produto { get; set; }
        public DbSet<UsuarioVo> Usuario { get; set; }
        public DbSet<FornecedorVo> Fornecedor { get; set; }
        public DbSet<CepVo> Cep { get; set; }
        public DbSet<LogVo> Log { get; set; }
        public DbSet<TaxaEntregaVo> TaxaEntrega { get; set; }
        public DbSet<ClienteVo> Cliente { get; set; }
        public DbSet<ClienteEnderecoVo> ClienteEndereco { get; set; }
        public DbSet<SuporteVo> Suporte { get; set; }
        public DbSet<PedidoVo> Pedido { get; set; }
        public DbSet<PedidoItemVo> PedidoItem { get; set; }
        public DbSet<PedidoEntregaVo> PedidoEntrega { get; set; }
        public DbSet<MovimentoCaixaVo> MovimentoCaixa { get; set; }

        /// <summary>
        /// Salva as mudanças
        /// </summary>
        public bool Salvar(ref string mensagemErro)
        {
            try
            {
                base.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.ToString().Contains("duplicada"))
                {
                    mensagemErro = "Esse cadastro já existe, não é possível incluir cadastros duplicados.";
                    return false;
                }
                if (ex.InnerException.ToString().Contains("REFERENCE"))
                {
                    mensagemErro = "Existem cadastros que estão utilizando este cadastro.";
                    return false;
                }
                else
                {
                    mensagemErro = "Erro ao salvar as alterações no banco de dados: " + ex.InnerException;
                    return false;
                }
            }
        }
    }
}
