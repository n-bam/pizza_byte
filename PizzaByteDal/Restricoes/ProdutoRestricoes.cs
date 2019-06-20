using PizzaByteVo;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace PizzaByteDal.Restricoes
{
    public class ProdutoRestricoes : EntityTypeConfiguration<ProdutoVo>
    {
        public ProdutoRestricoes()
        {
            ToTable("Produtos", "PizzaByte");
            HasKey(p => p.Id);

            this.Property(p => p.Descricao)
            .HasMaxLength(150)
            .IsRequired()
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Produto_Descricao", 1) { IsUnique = true }));

            this.Property(p => p.Id)
            .IsRequired()
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Produto_Id", 1) { IsUnique = true }));
        }
    }
}
