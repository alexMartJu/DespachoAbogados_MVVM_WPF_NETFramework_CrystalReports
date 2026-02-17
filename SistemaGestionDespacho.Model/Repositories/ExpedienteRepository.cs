using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestionDespacho.Model.Repositories
{
    /// <summary>
    /// Repositorio para operaciones CRUD sobre la entidad Expedientes.
    /// </summary>
    public class ExpedienteRepository
    {
        /// <summary>
        /// Obtiene un IQueryable con todos los expedientes (sin seguimiento) incluyendo las entidades relacionadas de cliente y estado.
        /// </summary>
        /// <returns>Expedientes que representa la colección de expedientes.</returns>
        public IQueryable<Expedientes> GetAll()
        {
            var context = new SistemaGestionDespachoEntities();

            return context.Expedientes
                .Include(e => e.Clientes)
                .Include(e => e.EstadosExpediente)
                .AsNoTracking();
        }

        /// <summary>
        /// Obtiene un expediente por su identificador.
        /// </summary>
        /// <param name="id">Identificador del expediente a recuperar.</param>
        /// <returns>Instancia de Expedientes si existe, o <c>null</c> si no se encuentra.</returns>
        public Expedientes GetById(int id)
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                return context.Expedientes.Find(id);
            }
        }

        /// <summary>
        /// Añade un nuevo expediente y persiste los cambios en la base de datos.
        /// </summary>
        /// <param name="expediente">Objeto Expedientes a añadir.</param>
        public void Add(Expedientes expediente)
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                context.Expedientes.Add(expediente);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza un expediente existente con los valores proporcionados.
        /// </summary>
        /// <param name="expediente">Objeto Expedientes con los datos actualizados (debe incluir <c>ExpedienteId</c>).</param>
        /// <exception cref="System.Exception">Se lanza si el expediente a actualizar no existe.</exception>
        public void Update(Expedientes expediente)
        {
            //Para evitar problemas de seguimiento de entidades, se carga la entidad existente y se actualizan sus propiedades.
            using (var context = new SistemaGestionDespachoEntities())
            {
                var entity = context.Expedientes.Find(expediente.ExpedienteId);
                if (entity == null)
                    throw new Exception("Expediente no encontrado.");

                //Actualizar solo las propiedades relevantes para evitar problemas de seguimiento de entidades.
                entity.ClienteId = expediente.ClienteId;
                entity.EstadoId = expediente.EstadoId;
                entity.Tipo = expediente.Tipo;
                entity.FechaApertura = expediente.FechaApertura;
                entity.FechaCierre = expediente.FechaCierre;
                entity.Titulo = expediente.Titulo;
                entity.Descripcion = expediente.Descripcion;
                entity.Codigo = expediente.Codigo;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Cierra un expediente (borrado lógico) estableciendo su estado a cerrado y fecha de cierre.
        /// </summary>
        /// <param name="expediente">Expediente a cerrar.</param>
        public void Delete(Expedientes expediente)
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                var entity = context.Expedientes.Find(expediente.ExpedienteId);

                if (entity != null)
                {
                    entity.EstadoId = 4; //Cerrado
                    entity.FechaCierre = System.DateTime.Now;
                    context.SaveChanges();
                }
            }
        }
    }
}
