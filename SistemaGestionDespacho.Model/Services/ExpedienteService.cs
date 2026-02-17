using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaGestionDespacho.Model.Repositories;

namespace SistemaGestionDespacho.Model.Services
{
    /// <summary>
    /// Servicio que gestiona la lógica de negocio relacionada con los expedientes.
    /// </summary>
    public class ExpedienteService
    {
        private readonly ExpedienteRepository _repo;
        private readonly ClienteRepository _clienteRepo;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ExpedienteService"/>.
        /// </summary>
        public ExpedienteService()
        {
            _repo = new ExpedienteRepository();
            _clienteRepo = new ClienteRepository();
        }

        /// <summary>
        /// Filtra expedientes según los criterios especificados.
        /// </summary>
        /// <param name="clienteId">Identificador del cliente (opcional).</param>
        /// <param name="tipo">Tipo de expediente (opcional).</param>
        /// <param name="estadoId">Identificador del estado (opcional).</param>
        /// <param name="desde">Fecha de apertura mínima (opcional).</param>
        /// <param name="hasta">Fecha de apertura máxima (opcional).</param>
        /// <returns>Lista de expedientes que cumplen los criterios.</returns>
        public List<Expedientes> Filtrar(int? clienteId, string tipo, int? estadoId,
                                         DateTime? desde, DateTime? hasta)
        {
            var lista = _repo.GetAll().AsQueryable();

            if (clienteId != null)
                lista = lista.Where(e => e.ClienteId == clienteId);

            if (!string.IsNullOrWhiteSpace(tipo))
                lista = lista.Where(e => e.Tipo == tipo);

            if (estadoId != null)
                lista = lista.Where(e => e.EstadoId == estadoId);

            if (desde != null)
                lista = lista.Where(e => e.FechaApertura >= desde);

            if (hasta != null)
                lista = lista.Where(e => e.FechaApertura <= hasta);

            return lista.ToList();
        }

        /// <summary>
        /// Crea un nuevo expediente tras validar sus datos.
        /// </summary>
        /// <param name="e">Expediente a crear.</param>
        /// <exception cref="System.Exception">Se lanza si la validación falla.</exception>
        public void Crear(Expedientes e)
        {
            ValidarExpediente(e);

            // Generar un Código único si no se proporcionó
            if (string.IsNullOrWhiteSpace(e.Codigo))
            {
                // Código con marca temporal + parte de GUID para garantizar unicidad
                e.Codigo = $"EXP-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
            }

            _repo.Add(e);
        }

        /// <summary>
        /// Edita un expediente existente tras validar sus datos.
        /// </summary>
        /// <param name="e">Expediente con los datos actualizados (debe incluir ExpedienteId).</param>
        /// <exception cref="System.Exception">Se lanza si la validación falla.</exception>
        public void Editar(Expedientes e)
        {
            ValidarExpediente(e);
            _repo.Update(e);
        }

        /// <summary>
        /// Cierra un expediente. No permite cerrar si el expediente no tiene actuaciones.
        /// </summary>
        /// <param name="e">Expediente a cerrar.</param>
        /// <exception cref="System.Exception">Se lanza si el expediente no tiene actuaciones y no se puede cerrar.</exception>
        public void Cerrar(Expedientes e)
        {
            // No permitir cerrar un expediente si no tiene actuaciones
            var actuacionRepo = new ActuacionRepository();
            var actuaciones = actuacionRepo.GetByExpediente(e.ExpedienteId);

            if (actuaciones == null || actuaciones.Count == 0)
                throw new Exception("No se puede cerrar el expediente porque no tiene actuaciones.");

            _repo.Delete(e);
        }

        //ValidarExpediente --> Validar los campos obligatorios y la existencia del cliente
        private void ValidarExpediente(Expedientes e)
        {
            if (string.IsNullOrWhiteSpace(e.Titulo))
                throw new Exception("El título es obligatorio.");

            if (string.IsNullOrWhiteSpace(e.Descripcion))
                throw new Exception("La descripción es obligatoria.");

            if (string.IsNullOrWhiteSpace(e.Tipo))
                throw new Exception("El tipo de expediente es obligatorio.");

            if (e.ClienteId <= 0)
                throw new Exception("Debe seleccionar un cliente válido.");

            if (e.EstadoId <= 0)
                throw new Exception("Debe seleccionar un estado válido.");

            //El cliente debe existir y estar activo
            var cliente = _clienteRepo.GetById(e.ClienteId);
            if (cliente == null)
                throw new Exception("El cliente seleccionado no existe.");

            if (!cliente.Activo)
                throw new Exception("No se puede crear o editar un expediente para un cliente desactivado.");

            //No permitir crear con estados 'Archivado' o 'Cerrado' y no permitir establecer 'Cerrado' desde la edición
            var estadoRepo = new EstadoExpedienteRepository();
            var estado = estadoRepo.GetAll().FirstOrDefault(s => s.EstadoId == e.EstadoId);
            if (estado != null)
            {
                var nombre = (estado.Nombre ?? string.Empty).Trim().ToLowerInvariant();

                //Si es creación (ExpedienteId == 0) no se permite 'Archivado' ni 'Cerrado'
                if (e.ExpedienteId == 0)
                {
                    if (nombre == "cerrado" || nombre == "archivado")
                        throw new Exception("No se puede crear un expediente con estado 'Cerrado' o 'Archivado'.");
                }
                else //edición: no permitir establecer el estado 'Cerrado' desde la edición
                {
                    if (nombre == "cerrado")
                        throw new Exception("No se puede establecer el estado 'Cerrado' desde la edición. Use el botón 'Cerrar'.");
                }
            }
        }
    }
}
