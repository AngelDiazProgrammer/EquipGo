using System;
using System.Threading.Tasks;
using Interface;
using Interface.Services.Transacciones;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_OS_APP.EQUIPGO.DTO.DTOs;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Transacciones
{
    public class TransaccionService : ITransaccionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransaccionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> RegistrarTransaccionAsync(TransaccionRequest request)
        {
            try
            {
                var transaccion = new OUT_DOMAIN_EQUIPGO.Entities.Procesos.Transacciones
                {
                    CodigoBarras = request.CodigoBarras,
                    IdTipoTransaccion = request.TipoTransaccion,
                    IdEquipoPersonal = request.IdEquipoPersonal,
                    IdUsuarioInfo = request.IdUsuarioInfo,
                    IdUsuarioSession = request.IdUsuarioSession,
                    SedeOs = request.SedeOs,
                    FechaHora = DateTime.Now,
                };

                await _unitOfWork.Transacciones.AddAsync(transaccion);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception)
            {
                // Podrías agregar logging aquí
                return false;
            }
        }
    }
}
