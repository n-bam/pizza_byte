using PizzaByteVo;
using PizzaByteVo.Base;
using System.Data.Entity.ModelConfiguration;

namespace PizzaByteDal.Restricoes
{
    public class SuporteRestricoes : EntityTypeConfiguration<SuporteVo>
    {
        public SuporteRestricoes()
        {
            ToTable("Suporte", "PizzaByte");
            HasKey(p => p.Id);

            this.Ignore(p => p.Inativo);
            this.Ignore(p => p.DataAlteracao);

            this.Property(p => p.Mensagem)
            .HasMaxLength(500)
            .IsRequired();

            this.Property(p => p.Tipo)
            .IsRequired();
        }
    }
}
