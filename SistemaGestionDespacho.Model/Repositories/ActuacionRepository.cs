using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestionDespacho.Model.Repositories
{
    /// <summary>
    /// Repositorio para operaciones CRUD sobre la entidad Actuaciones.
    /// </summary>
    public class ActuacionRepository
    {
        /// <summary>
        /// Obtiene todas las actuaciones asociadas a un expediente.
        /// </summary>
        /// <param name="expedienteId">Identificador del expediente.</param>
        /// <returns>Lista de Actuaciones asociadas al expediente indicado.</returns>
        public List<Actuaciones> GetByExpediente(int expedienteId)
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                return context.Actuaciones
                    .Include(a => a.Expedientes)
                    .Where(a => a.ExpedienteId == expedienteId)
                    .AsNoTracking()
                    .ToList();
            }
        }

        /// <summary>
        /// Obtiene todas las actuaciones disponibles en el sistema.
        /// </summary>
        /// <returns>Lista de todas las Actuaciones.</returns>
        public List<Actuaciones> GetAll()
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                return context.Actuaciones
                    .Include(a => a.Expedientes)
                    .AsNoTracking()
                    .ToList();
            }
        }

        /// <summary>
        /// Añade una nueva actuación al repositorio y persiste los cambios.
        /// </summary>
        /// <param name="actuacion">Objeto Actuaciones a añadir.</param>
        public void Add(Actuaciones actuacion)
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                context.Actuaciones.Add(actuacion);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza una actuación existente con los valores proporcionados.
        /// </summary>
        /// <param name="actuacion">Objeto Actuaciones con los datos actualizados (debe incluir ActuacionId).</param>
        /// <exception cref="System.Exception">Se lanza si la actuación a actualizar no existe.</exception>
        public void Update(Actuaciones actuacion)
        {
            //Para evitar problemas de seguimiento de entidades, se carga la entidad existente y se actualizan sus propiedades.
            using (var context = new SistemaGestionDespachoEntities())
            {
                var entity = context.Actuaciones.Find(actuacion.ActuacionId);
                if (entity == null)
                    throw new Exception("Actuación no encontrada.");

                //Actualizar solo las propiedades necesarias para evitar conflictos de seguimiento
                entity.ExpedienteId = actuacion.ExpedienteId;
                entity.Fecha = actuacion.Fecha;
                entity.Tipo = actuacion.Tipo;
                entity.Descripcion = actuacion.Descripcion;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina una actuación del repositorio.
        /// </summary>
        /// <param name="actuacion">Objeto Actuaciones a eliminar.</param>
        public void Delete(Actuaciones actuacion)
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                var entity = context.Actuaciones.Find(actuacion.ActuacionId);
                if (entity != null)
                {
                    context.Actuaciones.Remove(entity);
                    context.SaveChanges();
                }
            }
        }
    }
}
