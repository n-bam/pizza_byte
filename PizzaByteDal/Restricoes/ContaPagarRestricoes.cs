using PizzaByteVo;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace PizzaByteDal.Restricoes
{
    public class ContaPagarRestricoes : EntityTypeConfiguration<ContaPagarVo>
    {
        public ContaPagarRestricoes()
        {

            // Nome da tebela no banco, nome do domínio
            ToTable("ContaPagar", "PizzaByte");

            // Campo que é a chave primária
            HasKey(p => p.Id);

            // Para essa propriedade
            this.Property(p => p.Descricao)
            .HasMaxLength(200) // Tamanho máximo
            .IsRequired() // Obrigartório
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("ix_Nome_ContaPagar") // Index para deixar a pesquisa mais rápida

            }));

            // Para essa propriedade
            this.Property(p => p.DataCompetencia)
            .HasColumnType("Date");

            // Para essa propriedade
            this.Property(p => p.DataVencimento)
            .HasColumnType("Date");

            // Para essa propriedade
            this.Property(p => p.DataPagamento)
            .HasColumnType("Date");

            this.Ignore(p => p.Inativo);
        }
    }
}
