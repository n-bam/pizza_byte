using PizzaByteVo;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace PizzaByteDal.Restricoes
{
    public class UsuarioRestricoes : EntityTypeConfiguration<UsuarioVo>
    {
        public UsuarioRestricoes()
        {
            ToTable("Usuarios", "PizzaByte");
            HasKey(p => p.Id);

            this.Property(p => p.Nome)
            .HasMaxLength(150)
            .IsRequired();

            this.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Email_Usuario", 1) { IsUnique = true }));

            this.Property(p => p.Senha)
            .IsRequired()
            .HasMaxLength(50);
        }
    }
}
