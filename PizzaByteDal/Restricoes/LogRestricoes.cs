using PizzaByteVo.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace PizzaByteDal.Restricoes
{
    public class LogRestricoes : EntityTypeConfiguration<LogVo>
    {
        public LogRestricoes()
        {
            ToTable("Log", "PizzaByte");
            HasKey(p => p.Id);

            this.Ignore(p => p.Excluido);
            this.Ignore(p => p.DataAlteracao);
            this.Ignore(p => p.Inativo);

            this.Property(p => p.IdEntidade)
            .IsRequired();

            this.Property(p => p.IdUsuario)
           .IsRequired();

            this.Property(p => p.Mensagem)
          .IsRequired();

            this.Property(p => p.Recurso)
            .IsRequired()
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("ix_Recurso_Log")
            }));
        }
    }
}
