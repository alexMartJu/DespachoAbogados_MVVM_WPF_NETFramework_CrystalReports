using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestionDespacho.Model.Repositories
{
    /// <summary>
    /// Repositorio para operaciones CRUD sobre la entidad Clientes.
    /// </summary>
    public class ClienteRepository
    {
        /// <summary>
        /// Obtiene un IQueryable con todos los clientes (sin seguimiento) para permitir
        /// consultas diferidas y filtrado por el consumidor.
        /// </summary>
        /// <returns>Clientes que representa la colección de clientes.</returns>
        public IQueryable<Clientes> GetAll()
        {
            var context = new SistemaGestionDespachoEntities();
            return context.Clientes.AsNoTracking();
        }

        /// <summary>
        /// Obtiene un cliente por su identificador.
        /// </summary>
        /// <param name="id">Identificador del cliente a recuperar.</param>
        /// <returns>Instancia de Clientes si existe, o <c>null</c> si no se encuentra.</returns>
        public Clientes GetById(int id)
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                return context.Clientes.Find(id);
            }
        }

        /// <summary>
        /// Añade un nuevo cliente y persiste los cambios en la base de datos.
        /// </summary>
        /// <param name="cliente">Objeto Clientes a añadir.</param>
        public void Add(Clientes cliente)
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                context.Clientes.Add(cliente);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza un cliente existente en la base de datos.
        /// </summary>
        /// <param name="cliente">Objeto Clientes con los datos actualizados (debe incluir <c>ClienteId</c>).</param>
        public void Update(Clientes cliente)
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                context.Entry(cliente).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Marca un cliente como inactivo (borrado lógico) estableciendo su propiedad <c>Activo</c> a <c>false</c>.
        /// </summary>
        /// <param name="cliente">Objeto Clientes a desactivar.</param>
        public void Delete(Clientes cliente)
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                var entity = context.Clientes.Find(cliente.ClienteId);
                if (entity != null)
                {
                    entity.Activo = false;
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina permanentemente un cliente de la base de datos.
        /// </summary>
        /// <param name="clienteId">Identificador del cliente a eliminar permanentemente.</param>
        public void DeletePermanent(int clienteId)
        {
            using (var context = new SistemaGestionDespachoEntities())
            {
                var entity = context.Clientes.Find(clienteId);
                if (entity != null)
                {
                    context.Clientes.Remove(entity);
                    context.SaveChanges();
                }
            }
        }
    }
}
