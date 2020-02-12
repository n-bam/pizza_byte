using PizzaByteVo;
using PizzaByteVo.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace PizzaByteDal.Restricoes
{
    public class MovimentoCaixaRestricoes : EntityTypeConfiguration<MovimentoCaixaVo>
    {
        public MovimentoCaixaRestricoes()
        {
            ToTable("MovimentosCaixa", "PizzaByte");
            HasKey(p => p.Id);

            this.Ignore(p => p.Excluido);
            this.Ignore(p => p.DataAlteracao);
            this.Ignore(p => p.Inativo);

            this.Property(p => p.Justificativa)
            .IsRequired();

            this.Property(p => p.Valor)
           .IsRequired();
            
            this.Property(p => p.DataInclusao)
            .IsRequired()
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("ix_Data_MovimentoCaixa")
            }));
        }
    }
}
