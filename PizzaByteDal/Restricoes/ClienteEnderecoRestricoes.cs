using PizzaByteVo;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace PizzaByteDal.Restricoes
{
    public class ClienteEnderecoRestricoes : EntityTypeConfiguration<ClienteEnderecoVo>
    {
        public ClienteEnderecoRestricoes()
        {
            // Nome da tebela no banco, nome do domínio
            ToTable("ClientesEnderecos", "PizzaByte");

            // Campo que é a chave primária
            HasKey(p => p.Id);

            // Para essa propriedade
            this.Property(p => p.NumeroEndereco)
            .HasMaxLength(10) // Tamanho máximo
            .IsRequired(); ;

            this.Property(p => p.ComplementoEndereco)
            .HasMaxLength(50);

            this.Property(p => p.IdCliente)
            .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_Cliente_Endereco", 1) { }));
            
            this.HasRequired(p => p.Endereco).WithMany().HasForeignKey(p => p.IdCep);

            this.HasRequired(p => p.Cliente).WithMany().HasForeignKey(p => p.IdCliente);
        }
    }
}
