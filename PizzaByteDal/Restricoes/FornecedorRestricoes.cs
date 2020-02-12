using PizzaByteVo;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace PizzaByteDal.Restricoes
{
    public class FornecedorRestricoes : EntityTypeConfiguration<FornecedorVo>
    {
        public FornecedorRestricoes()
        {
            // Nome da tebela no banco, nome do domínio
            ToTable("Fornecedores", "PizzaByte");

            // Campo que é a chave primária
            HasKey(p => p.Id);

            // Para essa propriedade
            this.Property(p => p.NomeFantasia)
            .HasMaxLength(150) // Tamanho máximo
            .IsRequired() // Obrigartório
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("ix_NomeFantasia_Usuario") // Index para deixar a pesquisa mais rápida
            }));

            this.Property(p => p.RazaoSocial)
            .HasMaxLength(150);

            this.Property(p => p.Cnpj)
            .HasMaxLength(14)
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Cnpj", 1) { }));

            this.Property(p => p.ComplementoEndereco)
            .HasMaxLength(50);

            this.Property(p => p.NumeroEndereco)
            .HasMaxLength(10);

            this.Property(p => p.Obs)
            .HasMaxLength(2000);

            this.Property(p => p.Telefone)
            .HasMaxLength(20);

            this.HasOptional(p => p.Endereco).WithMany().HasForeignKey(p => p.IdCep);
        }
    }
}
