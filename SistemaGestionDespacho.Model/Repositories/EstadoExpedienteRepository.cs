using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestionDespacho.Model.Repositories
{
    /// <summary>
    /// Repositorio para obtener los estados posibles de un expediente.
    /// </summary>
    public class EstadoExpedienteRepository
    {
        /// <summary>
        /// Obtiene todos los estados de expediente disponibles en el sistema.
        /// </summary>
        /// <returns>Lista de EstadosExpediente con los estados registrados.</returns>
        public List<EstadosExpediente> GetAll()
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                return context.EstadosExpediente
                    .AsNoTracking()
                    .ToList();
            }
        }
    }
}
