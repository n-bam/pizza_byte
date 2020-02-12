using PizzaByteVo;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace PizzaByteDal.Restricoes
{
    public class PedidoItemRestricoes : EntityTypeConfiguration<PedidoItemVo>
    {
        public PedidoItemRestricoes()
        {
            // Nome da tebela no banco, nome do domínio
            ToTable("PedidosItens", "PizzaByte");

            // Campo que é a chave primária
            HasKey(p => p.Id);

            // Para essa propriedade 
            this.Property(p => p.IdPedido)
            .IsRequired()
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("ix_IdPedido_Item") // Index para deixar a pesquisa mais rápida
            }));

            this.Property(p => p.DescricaoProduto)
            .HasMaxLength(300)
            .IsRequired();

            this.HasOptional(p => p.ProdutoComposto).WithMany().HasForeignKey(p => p.IdProdutoComposto);
            this.HasRequired(p => p.Pedido).WithMany().HasForeignKey(p => p.IdPedido);
            this.HasRequired(p => p.Produto).WithMany().HasForeignKey(p => p.IdProduto);
        }
    }
}
