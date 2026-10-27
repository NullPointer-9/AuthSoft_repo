using System.Data.Entity;
using System.Data.Entity.SqlServer;
using Cartorio.Entidades;

namespace Cartorio.Dados
{
    /// <summary>
    /// Configuração do EF6 em código, para não depender da seção &lt;entityFramework&gt; no App.config.
    /// </summary>
    public class CartorioDbConfiguration : DbConfiguration
    {
        public CartorioDbConfiguration()
        {
            SetProviderServices(SqlProviderServices.ProviderInvariantName, SqlProviderServices.Instance);
        }
    }

    /// <summary>
    /// Code First mapeado para o banco que já existe (criado pelos scripts de database/).
    /// O EF não cria nem altera tabelas: o inicializador está desligado.
    /// </summary>
    [DbConfigurationType(typeof(CartorioDbConfiguration))]
    public class CartorioContext : DbContext
    {
        static CartorioContext()
        {
            Database.SetInitializer<CartorioContext>(null);
        }

        public CartorioContext(string connectionString) : base(connectionString)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Protocolo> Protocolos { get; set; }
        public DbSet<ProtocoloHistorico> Historicos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var cliente = modelBuilder.Entity<Cliente>().ToTable("Cliente", "dbo");
            cliente.HasKey(c => c.Id);
            cliente.Property(c => c.Nome).IsRequired().HasMaxLength(150);
            cliente.Property(c => c.Cpf).IsRequired().HasMaxLength(11).IsFixedLength().IsUnicode(false);
            cliente.Property(c => c.Email).HasMaxLength(150);
            cliente.Property(c => c.Telefone).HasMaxLength(20).IsUnicode(false);
            cliente.Property(c => c.DataCadastro).HasColumnType("datetime2");

            var protocolo = modelBuilder.Entity<Protocolo>().ToTable("Protocolo", "dbo");
            protocolo.HasKey(p => p.Id);
            protocolo.Property(p => p.Numero).IsRequired().HasMaxLength(20).IsUnicode(false);
            protocolo.Property(p => p.TipoServico).IsRequired().HasMaxLength(80);
            protocolo.Property(p => p.Descricao).HasMaxLength(500);
            protocolo.Property(p => p.Status).HasColumnName("StatusId");
            protocolo.Property(p => p.DataAbertura).HasColumnType("datetime2");
            protocolo.Property(p => p.DataAtualizacao).HasColumnType("datetime2");
            protocolo.Property(p => p.DataEntrega).HasColumnType("datetime2");
            protocolo.HasRequired(p => p.Cliente).WithMany().HasForeignKey(p => p.ClienteId);

            var historico = modelBuilder.Entity<ProtocoloHistorico>().ToTable("ProtocoloHistorico", "dbo");
            historico.HasKey(h => h.Id);
            historico.Property(h => h.StatusAnterior).HasColumnName("StatusAnteriorId");
            historico.Property(h => h.StatusNovo).HasColumnName("StatusNovoId");
            historico.Property(h => h.DataAlteracao).HasColumnType("datetime2");
            historico.Property(h => h.Observacao).HasMaxLength(300);
        }
    }
}
