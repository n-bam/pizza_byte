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
            Database.Connection.ConnectionString = @"Data Source =.\SQLExpress;Initial Catalog = PizzariaNacoes; User ID=sa;Password=125478; Connection Timeout=30;Trusted_Connection=false;Encrypt=false;Connection Timeout=30";
            //Database.Connection.ConnectionString = @"Data Source =den1.mssql8.gear.host;Initial Catalog = PizzaByte; User ID=pizzabyte;Password=Bw3Hw50n!87_; Connection Timeout=30;Trusted_Connection=false;Encrypt=false;Connection Timeout=30";
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

        }

        public DbSet<ProdutoVo> Produto { get; set; }
        public DbSet<UsuarioVo> Usuario { get; set; }
        public DbSet<FornecedorVo> Fornecedor { get; set; }
        public DbSet<CepVo> Cep { get; set; }
        public DbSet<LogVo> Log { get; set; }


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
                    //LOGAR a exceção
                    mensagemErro = "Erro ao salvar as alterações no banco de dados: " + ex.InnerException;
                    return false;
                }
            }
        }
    }
}
