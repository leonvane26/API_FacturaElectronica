using Microsoft.EntityFrameworkCore;

namespace APIFactura.Entidades
{
    public class APIDbContext : DbContext
    {
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Producto> Productos { get; set; }

        public APIDbContext(DbContextOptions<APIDbContext> options)
            : base(options)
        {
            
        }
    }
}
