using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using PizzaByteVo;

namespace PizzaByteDal.Restricoes
{
    public class TaxaEntregaRestricoes : EntityTypeConfiguration<TaxaEntregaVo>
    {
        public TaxaEntregaRestricoes()
        {
            ToTable("TaxasEntrega", "PizzaByte");
            HasKey(p => p.Id);

            this.Property(p => p.BairroCidade)
            .HasMaxLength(101)
            .IsRequired()
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Bairro_TaxaEntrega", 1) { IsUnique = true }));
        }
    }
}
