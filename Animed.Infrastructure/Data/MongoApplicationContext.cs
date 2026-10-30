using Animed.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Animed.Infrastructure.Data
{
    public class MongoApplicationContext : DbContext
    {
        public MongoApplicationContext(DbContextOptions<MongoApplicationContext> options) : base(options)
        {

        }

        public DbSet<HistoricoCuidadoEntity> HistoricoCuidado { get; set; }
    }
}
