using PizzaByteVo;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace PizzaByteDal.Restricoes
{
    public class CepRestricoes : EntityTypeConfiguration<CepVo>
    {
        public CepRestricoes()
        {
            ToTable("Ceps", "PizzaByte");
            HasKey(p => p.Id);

            this.Ignore(p => p.Inativo);

            this.Property(p => p.Bairro)
            .HasMaxLength(50)
            .IsRequired();

            this.Property(p => p.Cep)
            .HasMaxLength(8)
            .IsRequired()
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Cep", 1) { IsUnique = true }));

            this.Property(p => p.Id)
            .IsRequired()
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Id", 1) { IsUnique = true }));

            this.Property(p => p.Cidade)
            .HasMaxLength(50)
            .IsRequired();

            this.Property(p => p.Logradouro)
            .HasMaxLength(150)
            .IsRequired();

        }
    }
}
