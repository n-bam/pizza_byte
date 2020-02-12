using PizzaByteVo;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace PizzaByteDal.Restricoes
{
    public class PedidoEntregaRestricoes : EntityTypeConfiguration<PedidoEntregaVo>
    {
        public PedidoEntregaRestricoes()
        {
            // Nome da tebela no banco, nome do domínio
            ToTable("PedidosEntregas", "PizzaByte");

            // Campo que é a chave primária
            HasKey(p => p.Id);

            this.Property(p => p.IdPedido)
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("ix_IdPedido_Entrega") // Index para deixar a pesquisa mais rápida
            }));

            this.Property(p => p.IdEndereco)
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("ix_IdEndereco_Entrega") // Index para deixar a pesquisa mais rápida
            }));

            this.Property(p => p.Obs)
            .HasMaxLength(2000);

            this.HasOptional(p => p.Funcionario).WithMany().HasForeignKey(p => p.IdFuncionario);
            this.HasRequired(p => p.ClienteEndereco).WithMany().HasForeignKey(p => p.IdEndereco);
            this.HasRequired(p => p.Pedido).WithMany().HasForeignKey(p => p.IdPedido);
        }
    }
}
