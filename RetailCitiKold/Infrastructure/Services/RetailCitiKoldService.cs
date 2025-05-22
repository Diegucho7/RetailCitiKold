using RetailCitiKold.Application.Services;
using RetailCitiKold.Domain.Dtos.Request;

namespace RetailCitiKold.Infrastructure.Services
{
    public class RetailCitiKoldService : IRetailCitiKoldService
    {
       

        public RetailCitiKoldService(
            
        
            )
        {
           
        }

        public async Task<ProcessResult> AsentarComprobante(int id, string tipo)
        {
            return tipo switch
            {
                "FACTURA" => await _facturaService.AsentarComprobante(id, tipo),
                
                _ => throw new NotImplementedException(),
            };
        }

        public async Task<bool> ReversarComprobante(int id, string tipo)
        {
            return tipo switch
            {
                "FACTURA" => await _facturaService.ReversarComprobante(id, tipo),
               
                _ => throw new NotImplementedException(),

            };
        }

        public async Task<bool> AnularComprobante(int id, string tipo)
        {
            return tipo switch
            {
                "FACTURA" => await _facturaService.AnularComprobante(id, tipo),
               
                _ => throw new NotImplementedException(),

            };
        }
    }
}
