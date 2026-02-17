using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaGestionDespacho.Model.Repositories;

namespace SistemaGestionDespacho.Model.Services
{
    /// <summary>
    /// Servicio que gestiona la lógica de negocio relacionada con las actuaciones de un expediente.
    /// </summary>
    public class ActuacionService
    {
        private readonly ActuacionRepository _repo;
        private readonly ExpedienteRepository _expRepo;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ActuacionService"/>.
        /// </summary>
        public ActuacionService()
        {
            _repo = new ActuacionRepository();
            _expRepo = new ExpedienteRepository();
        }

        /// <summary>
        /// Obtiene las actuaciones asociadas a un expediente.
        /// </summary>
        /// <param name="expedienteId">Identificador del expediente.</param>
        /// <returns>Lista de actuaciones del expediente especificado.</returns>
        public List<Actuaciones> GetByExpediente(int expedienteId)
        {
            return _repo.GetByExpediente(expedienteId);
        }

        /// <summary>
        /// Obtiene todas las actuaciones disponibles.
        /// </summary>
        /// <returns>Lista con todas las actuaciones.</returns>
        public List<Actuaciones> GetAll()
        {
            return _repo.GetAll();
        }

        /// <summary>
        /// Filtra actuaciones según criterios opcionales.
        /// </summary>
        /// <param name="expedienteId">Identificador del expediente (opcional).</param>
        /// <param name="tipo">Tipo de actuación (opcional).</param>
        /// <param name="fecha">Fecha para filtrar (solo la parte fecha) (opcional).</param>
        /// <returns>Lista de actuaciones que coinciden con los filtros aplicados.</returns>
        public List<Actuaciones> Filtrar(int? expedienteId, string tipo, DateTime? fecha)
        {
            var lista = _repo.GetAll().AsQueryable();

            if (expedienteId != null)
                lista = lista.Where(a => a.ExpedienteId == expedienteId.Value);

            if (!string.IsNullOrWhiteSpace(tipo))
                lista = lista.Where(a => a.Tipo != null && a.Tipo == tipo);

            if (fecha != null)
            {
                var d = fecha.Value.Date;
                lista = lista.Where(a => a.Fecha.Date == d);
            }

            return lista.ToList();
        }

        //ValidarActuacion() --> Validaciones comunes para crear o editar una actuación
        private void ValidarActuacion(Actuaciones a)
        {
            //Verificar que se ha indicado un expediente
            if (a.ExpedienteId <= 0)
                throw new Exception("El expediente es obligatorio.");

            //Verificar que el expediente existe
            var expediente = _expRepo.GetById(a.ExpedienteId);
            if (expediente == null)
                throw new Exception("El expediente indicado no existe.");

            //EstadoId 4 = cerrado, no se pueden crear o editar actuaciones en un expediente cerrado
            if (expediente.EstadoId == 4)
                throw new Exception("No se pueden crear o editar actuaciones en un expediente cerrado.");

            if (string.IsNullOrWhiteSpace(a.Tipo))
                throw new Exception("Debe seleccionar un tipo de actuación.");

            if (a.Fecha == null)
                throw new Exception("Debe seleccionar una fecha.");

            if (string.IsNullOrWhiteSpace(a.Descripcion))
                throw new Exception("La descripción es obligatoria.");

            string[] tiposValidos =
            {
            "Llamada",
            "Reunión",
            "Escrito presentado",
            "Escrito recibido",
            "Notificación",
            "Gestión documental"
            };

            if (!tiposValidos.Contains(a.Tipo))
                throw new Exception("El tipo de actuación no es válido.");
        }

        /// <summary>
        /// Crea una nueva actuación tras validar sus datos.
        /// </summary>
        /// <param name="a">Actuación a crear.</param>
        /// <exception cref="System.Exception">Se lanza si la validación falla.</exception>
        public void Crear(Actuaciones a)
        {
            ValidarActuacion(a);
            _repo.Add(a);
        }

        /// <summary>
        /// Edita una actuación existente tras validar sus datos.
        /// </summary>
        /// <param name="a">Actuación con los datos actualizados (debe incluir ActuacionId).</param>
        /// <exception cref="System.Exception">Se lanza si la validación falla.</exception>
        public void Editar(Actuaciones a)
        {
            ValidarActuacion(a);
            _repo.Update(a);
        }

        /// <summary>
        /// Elimina (borrado lógico) una actuación.
        /// </summary>
        /// <param name="a">Actuación a eliminar.</param>
        public void Eliminar(Actuaciones a)
        {
            _repo.Delete(a);
        }
    }
}
