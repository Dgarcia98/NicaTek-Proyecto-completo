using GRUPO_DDR_DOMAIN.Entities;
using Microsoft.EntityFrameworkCore;

namespace GRUPO_DDR_INFRASTRUCTURE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Productos> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<TipoCatalogo> Tipo_Catalogos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Productos>(entity =>
            {
                entity.ToTable("Tbl_Producto");
                entity.HasKey(p => p.Id_Producto);
                entity.Property(p => p.Precio).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Precio_Compra).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Venta>(entity =>
            {
                entity.ToTable("Tbl_Ventas");
                entity.HasKey(v => v.Id_Venta);
                entity.Property(v => v.Total).HasColumnType("decimal(18,2)");
               // entity.Property(v => v.Numero_Factura).ValueGeneratedOnAddOrUpdate();
            });
        }
    }
}