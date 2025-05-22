using RetailCitiKold.Domain.Dtos.Request;


namespace RetailCitiKold.Application.Services
{
    public interface IService
    {
        Task<ProcessResult> AsentarComprobante(int id, string tipo);
        Task<bool> ReversarComprobante(int id, string tipo);
        Task<bool> AnularComprobante(int id, string tipo);
    }
}
