using PizzaByteVo;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace PizzaByteDal.Restricoes
{
    public class FuncionarioRestricoes : EntityTypeConfiguration<FuncionarioVo>
    {
        public FuncionarioRestricoes()
        {
            // Nome da tebela no banco, nome do domínio
            ToTable("Funcionario", "PizzaByte");

            // Campo que é a chave primária
            HasKey(p => p.Id);

            // Para essa propriedade
            this.Property(p => p.Nome)
            .HasMaxLength(150) // Tamanho máximo
            .IsRequired() // Obrigartório
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("ix_Nome_Usuario") // Index para deixar a pesquisa mais rápida
            }));

            this.Property(p => p.Telefone)
            .HasMaxLength(8);
        }
    }
}
