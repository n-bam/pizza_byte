using PizzaByteVo;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace PizzaByteDal.Restricoes
{
    public class ClienteRestricoes : EntityTypeConfiguration<ClienteVo>
    {
        public ClienteRestricoes()
        {
            ToTable("Clientes", "PizzaByte");
            HasKey(p => p.Id);
            HasIndex(p => p.Id)
            .IsUnique();

            this.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new[]
            {
                new IndexAttribute("ix_Nome_Cliente")
            }));

            this.Property(p => p.Telefone)
            .HasMaxLength(20)
            .IsOptional();

            this.Property(p => p.Cpf)
            .IsOptional()
            .HasMaxLength(11)
            .IsFixedLength();

        }
    }
}

