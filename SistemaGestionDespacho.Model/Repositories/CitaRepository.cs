using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestionDespacho.Model.Repositories
{
    /// <summary>
    /// Repositorio para operaciones CRUD sobre la entidad Citas.
    /// </summary>
    public class CitaRepository
    {
        /// <summary>
        /// Obtiene todas las citas en el sistema, incluyendo las entidades relacionadas de cliente y expediente.
        /// </summary>
        /// <returns>Lista de Citas disponibles.</returns>
        public List<Citas> GetAll()
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                return context.Citas
                    .Include(c => c.Clientes)
                    .Include(c => c.Expedientes)
                    .AsNoTracking()
                    .ToList();
            }
        }

        /// <summary>
        /// Añade una nueva cita y persiste los cambios en la base de datos.
        /// </summary>
        /// <param name="cita">Objeto Citas a añadir.</param>
        public void Add(Citas cita)
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                context.Citas.Add(cita);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza una cita existente con los valores proporcionados.
        /// </summary>
        /// <param name="cita">Objeto Citas con los datos actualizados (debe incluir <c>CitaId</c>).</param>
        /// <exception cref="System.Exception">Se lanza si la cita a actualizar no existe.</exception>
        public void Update(Citas cita)
        {
            //Para evitar problemas de seguimiento de entidades, se carga la entidad existente y se actualizan sus propiedades.
            using (var context = new SistemaGestionDespachoEntities())
            {
                var entity = context.Citas.Find(cita.CitaId);
                if (entity == null)
                    throw new Exception("Cita no encontrada.");

                //Actualizar solo las propiedades relevantes para evitar problemas de seguimiento de entidades.
                entity.ClienteId = cita.ClienteId;
                entity.ExpedienteId = cita.ExpedienteId;
                entity.FechaHora = cita.FechaHora;
                entity.Lugar = cita.Lugar;
                entity.Motivo = cita.Motivo;
                entity.Estado = cita.Estado;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Marca una cita como cancelada (borrado lógico) estableciendo su estado a "Cancelada".
        /// </summary>
        /// <param name="cita">Objeto Citas a cancelar.</param>
        public void Delete(Citas cita)
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                var entity = context.Citas.Find(cita.CitaId);
                if (entity != null)
                {
                    entity.Estado = "Cancelada";
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina permanentemente una cita de la base de datos.
        /// </summary>
        /// <param name="citaId">Identificador de la cita a eliminar permanentemente.</param>
        public void DeletePermanent(int citaId)
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                var entity = context.Citas.Find(citaId);
                if (entity != null)
                {
                    context.Citas.Remove(entity);
                    context.SaveChanges();
                }
            }
        }
    }
}
