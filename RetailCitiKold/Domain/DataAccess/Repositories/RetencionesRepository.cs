using IntegracionERP.Domain.DataAccess.Abstractions.Repositories;

namespace IntegracionERP.Domain.DataAccess.Repositories;

public class RetencionesRepository : IRetencionesRepository
{
    public async Task<bool> AsentarComprobante(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ReversarComprobante(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AnularComprobante(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RecibirComprobante(int id)
    {
        throw new NotImplementedException();
    }
}