using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaGestionDespacho.Model.Repositories;

namespace SistemaGestionDespacho.Model.Services
{
    /// <summary>
    /// Servicio para gestionar los estados de los expedientes.
    /// </summary>
    public class EstadoExpedienteService
    {
        private readonly EstadoExpedienteRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="EstadoExpedienteService"/>.
        /// </summary>
        public EstadoExpedienteService()
        {
            _repo = new EstadoExpedienteRepository();
        }

        /// <summary>
        /// Obtiene todos los estados de expediente disponibles.
        /// </summary>
        /// <returns>Lista con todos los <see cref="EstadosExpediente"/> registrados.</returns>
        public List<EstadosExpediente> GetAll()
        {
            return _repo.GetAll();
        }
    }
}
