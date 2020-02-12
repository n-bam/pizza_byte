using PizzaByteVo;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace PizzaByteDal.Restricoes
{
    public class PedidoRestricoes : EntityTypeConfiguration<PedidoVo>
    {
        public PedidoRestricoes()
        {
            // Nome da tebela no banco, nome do domínio
            ToTable("Pedidos", "PizzaByte");

            // Campo que é a chave primária
            HasKey(p => p.Id);

            // Para essa propriedade
            this.Property(p => p.IdCliente)
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("ix_IdCliente_Pedido") // Index para deixar a pesquisa mais rápida
            }));

            this.Property(p => p.JustificativaCancelamento)
            .HasMaxLength(100);

            this.Property(p => p.Obs)
            .HasMaxLength(2000);

            this.HasOptional(p => p.Cliente).WithMany().HasForeignKey(p => p.IdCliente);
        }
    }
}
