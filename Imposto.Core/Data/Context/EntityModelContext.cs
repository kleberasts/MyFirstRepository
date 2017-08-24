using System.Data.Entity;

namespace Imposto.Core.Data
{
    public class EntityModelContext : DbContext
    {
        public EntityModelContext()
            : base("testeContext") { }

        public DbSet<NotaFiscal> eNotaFiscal { get; set; }
    }
}
