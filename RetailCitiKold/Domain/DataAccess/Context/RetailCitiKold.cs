using IntegracionERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntegracionERP.Domain.DataAccess.Context
{
    public class RetailCitiKold : DbContext
    {
        public RetailCitiKold(DbContextOptions<RetailCitiKold> options) : base(options)
        {



        }
        public DbSet<Bodega> Bodega { get; set; }
       


    }
}
