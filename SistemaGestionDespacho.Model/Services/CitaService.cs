using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaGestionDespacho.Model.Repositories;

namespace SistemaGestionDespacho.Model.Services
{
    /// <summary>
    /// Servicio que gestiona la lógica de negocio relacionada con las citas.
    /// </summary>
    public class CitaService
    { 
        private readonly CitaRepository _repo;
        private readonly ClienteRepository _clienteRepo;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="CitaService"/>.
        /// </summary>
        public CitaService()
        {
            _repo = new CitaRepository();
            _clienteRepo = new ClienteRepository();
        }

        /// <summary>
        /// Filtra las citas según los criterios especificados.
        /// </summary>
        /// <param name="fecha">Fecha para filtrar (solo la parte fecha). Si es <c>null</c> no se filtra por fecha.</param>
        /// <param name="estado">Estado de la cita (p.ej. "Pendiente", "Realizada", "Cancelada"). Si es nulo o vacío no se filtra por estado.</param>
        /// <param name="clienteId">Identificador del cliente. Si es <c>null</c> no se filtra por cliente.</param>
        /// <param name="expedienteId">Identificador del expediente. Si es <c>null</c> no se filtra por expediente.</param>
        /// <returns>Lista de citas que cumplen los criterios de búsqueda.</returns>
        public List<Citas> Filtrar(DateTime? fecha, string estado, int? clienteId, int? expedienteId)
        {
            var lista = _repo.GetAll().AsQueryable();

            if (fecha != null)
                lista = lista.Where(c => c.FechaHora.Date == fecha.Value.Date);

            if (!string.IsNullOrWhiteSpace(estado))
                lista = lista.Where(c => c.Estado == estado);

            if (clienteId != null)
                lista = lista.Where(c => c.ClienteId == clienteId);

            if (expedienteId != null)
                lista = lista.Where(c => c.ExpedienteId == expedienteId);

            return lista.ToList();
        }

        //ValidarCita() --> Validaciones comunes para creación y edición de citas
        private void ValidarCita(Citas c)
        {
            if (c.ClienteId == null && c.ExpedienteId == null)
                throw new Exception("Debe seleccionar cliente o expediente.");

            //Uso de default(DateTime) para evitar comparaciones con null si FechaHora no es nullable
            if (c.FechaHora == default(DateTime))
                throw new Exception("Debe seleccionar fecha y hora.");

            if (string.IsNullOrWhiteSpace(c.Lugar))
                throw new Exception("El lugar es obligatorio.");

            if (string.IsNullOrWhiteSpace(c.Motivo))
                throw new Exception("El motivo es obligatorio.");

            if (string.IsNullOrWhiteSpace(c.Estado))
                throw new Exception("Debe seleccionar un estado.");

            string[] estadosValidos = { "Pendiente", "Realizada", "Cancelada" };
            if (!estadosValidos.Contains(c.Estado))
                throw new Exception("El estado no es válido.");

            //No permitir que al crear o editar se establezca el estado como "Cancelada".
            //La cancelación debe realizarse exclusivamente mediante la acción de eliminar (botón de eliminar).
            if (string.Equals(c.Estado, "Cancelada", StringComparison.OrdinalIgnoreCase))
                throw new Exception("No se puede establecer el estado 'Cancelada' al crear o editar una cita. Use el botón Eliminar para cancelar citas.");

            //Si la cita está asociada a un cliente, el cliente debe existir y estar activo
            if (c.ClienteId != null)
            {
                var cliente = _clienteRepo.GetById(c.ClienteId.Value);
                if (cliente == null)
                    throw new Exception("El cliente seleccionado no existe.");

                if (!cliente.Activo)
                    throw new Exception("No se puede crear o editar una cita para un cliente desactivado.");
            }

            //No permitir dos citas en la misma fecha y hora (excepto si la cita existente está cancelada
            // o si la cita encontrada es la misma que se está editando)
            var citasExistentes = _repo.GetAll();
            bool conflicto = citasExistentes.Any(x => x.FechaHora == c.FechaHora && x.CitaId != c.CitaId && !string.Equals(x.Estado, "Cancelada", StringComparison.OrdinalIgnoreCase));
            if (conflicto)
                throw new Exception("Ya existe una cita en la misma fecha y hora.");
        }

        /// <summary>
        /// Crea una nueva cita tras validar sus datos.
        /// </summary>
        /// <param name="c">Cita a crear.</param>
        /// <exception cref="System.Exception">Se lanza si la validación falla o existe un conflicto de horario.</exception>
        public void Crear(Citas c)
        {
            ValidarCita(c);

            _repo.Add(c);
        }

        /// <summary>
        /// Edita una cita existente tras validar sus datos.
        /// </summary>
        /// <param name="c">Cita con los datos actualizados (debe incluir CitaId).</param>
        /// <exception cref="System.Exception">Se lanza si la validación falla o existe un conflicto de horario.</exception>
        public void Editar(Citas c)
        {
            ValidarCita(c);

            _repo.Update(c);
        }

        /// <summary>
        /// Cancela (elimina lógicamente) una cita.
        /// </summary>
        /// <param name="c">Cita a cancelar.</param>
        public void Cancelar(Citas c)
        {
            _repo.Delete(c);
        }

        /// <summary>
        /// Elimina permanentemente una cita.
        /// </summary>
        /// <param name="c">Cita a eliminar permanentemente.</param>
        public void EliminarPermanente(Citas c)
        {
            if (c == null) return;
            _repo.DeletePermanent(c.CitaId);
        }

        /// <summary>
        /// Elimina permanentemente una cita por su identificador.
        /// </summary>
        /// <param name="citaId">Identificador de la cita a eliminar permanentemente.</param>
        public void EliminarPermanente(int citaId)
        {
            _repo.DeletePermanent(citaId);
        }
    }
}
