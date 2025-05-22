namespace IntegracionERP.Domain.DataAccess.Abstractions.Repositories
{
    public interface IRepository
    {
        Task<bool> AsentarComprobante(int id);
        Task<bool> ReversarComprobante(int id);
        Task<bool> AnularComprobante(int id);
        Task<bool> RecibirComprobante(int id);
        
    }
}
