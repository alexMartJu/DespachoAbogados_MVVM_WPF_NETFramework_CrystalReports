using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SistemaGestionDespacho.Model.Repositories;

namespace SistemaGestionDespacho.Model.Services
{
    /// <summary>
    /// Servicio que gestiona la lógica de negocio relacionada con clientes.
    /// </summary>
    public class ClienteService
    {
        private readonly ClienteRepository _repo;
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex NonDigitRegex = new Regex(@"\D", RegexOptions.Compiled);

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ClienteService"/>.
        /// </summary>
        public ClienteService()
        {
            _repo = new ClienteRepository();
        }

        /// <summary>
        /// Busca clientes por nombre y/o DNI/CIF y permite filtrar solo activos.
        /// </summary>
        /// <param name="nombre">Término de búsqueda para el nombre (puede ser parcial). Si es nulo o vacío no se filtra por nombre.</param>
        /// <param name="dni">Término de búsqueda para el DNI/CIF (puede ser parcial). Si es nulo o vacío no se filtra por DNI/CIF.</param>
        /// <param name="soloActivos">Si es <c>true</c> se devuelven únicamente clientes activos.</param>
        /// <returns>Lista de clientes que cumplen los criterios de búsqueda.</returns>
        public List<Clientes> Buscar(string nombre, string dni, bool soloActivos)
        {
            var lista = _repo.GetAll();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                var termino = nombre.Trim().ToLower();
                lista = lista.Where(c => c.Nombre != null && c.Nombre.ToLower().Contains(termino));
            }

            if (!string.IsNullOrWhiteSpace(dni))
            {
                var terminoDni = dni.Trim().ToLower();
                lista = lista.Where(c => c.DNI_CIF != null && c.DNI_CIF.ToLower().Contains(terminoDni));
            }

            if (soloActivos)
                lista = lista.Where(c => c.Activo);

            return lista.ToList();
        }

        //ValidarCliente() --> Validar que el nombre, dirección y DNI/CIF no estén vacíos. Validar formato DNI (8 dígitos + letra). Validar que al menos email o teléfono estén indicados. Validar formato email y teléfono si se han indicado.
        private void ValidarCliente(Clientes c)
        {
            if (string.IsNullOrWhiteSpace(c.Nombre))
                throw new Exception("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(c.Direccion))
                throw new Exception("La dirección es obligatoria.");

            if (string.IsNullOrWhiteSpace(c.DNI_CIF))
                throw new Exception("El DNI/CIF es obligatorio.");

            if (!EsDniValido(c.DNI_CIF))
                throw new Exception("El DNI no tiene un formato válido.");

            if (string.IsNullOrWhiteSpace(c.Email) &&
                string.IsNullOrWhiteSpace(c.Telefono))
                throw new Exception("Debe indicar email o teléfono.");

            if (!string.IsNullOrWhiteSpace(c.Email) && !EsEmailValido(c.Email))
                throw new Exception("El email no tiene un formato válido.");

            if (!string.IsNullOrWhiteSpace(c.Telefono) && !EsTelefonoValido(c.Telefono))
                throw new Exception("El teléfono no tiene un formato válido (9 dígitos; se permite prefijo +34 y separadores).");
        }

        //EsDniValido() --> Validar que el DNI tenga 8 dígitos seguidos de una letra mayúscula. Se pueden permitir espacios al principio o al final, pero no en medio. No se permiten caracteres especiales ni letras en el número.
        private bool EsDniValido(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return false;

            var s = dni.Trim().ToUpperInvariant();

            // Formato básico: 8 dígitos + letra
            return Regex.IsMatch(s, @"^\d{8}[A-Z]$");
        }

        /// <summary>
        /// Comprueba si un email tiene un formato válido.
        /// </summary>
        /// <param name="email">Email a validar.</param>
        /// <returns><c>true</c> si el email es válido, <c>false</c> en caso contrario.</returns>
        public bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            return EmailRegex.IsMatch(email.Trim());
        }

        /// <summary>
        /// Comprueba si un número de teléfono tiene un formato español válido.
        /// </summary>
        /// <param name="telefono">Número de teléfono a validar. Se permiten prefijos y separadores.</param>
        /// <returns><c>true</c> si el teléfono es válido, <c>false</c> en caso contrario.</returns>
        public bool EsTelefonoValido(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                return false;

            var digits = NonDigitRegex.Replace(telefono, "");

            //Si incluye prefijo internacional 34 y queda más de 9 dígitos, lo eliminamos
            if (digits.StartsWith("34") && digits.Length > 9)
                digits = digits.Substring(2);

            if (digits.Length != 9)
                return false;

            char first = digits[0];
            return first == '6' || first == '7' || first == '8' || first == '9';
        }

        //ExisteDniParaOtroCliente() --> Comprueba si el DNI/CIF ya pertenece a otro cliente distinto al que se está editando (si clienteId es 0, se considera que es un nuevo cliente).
        private bool ExisteDniParaOtroCliente(string dni, int clienteId)
        {
            var existente = _repo.GetAll()
                .FirstOrDefault(x => x.DNI_CIF == dni);

            return existente != null && existente.ClienteId != clienteId;
        }

        //ClienteTieneExpedienteAbiertoOCurso() --> Comprueba si el cliente tiene algún expediente en estado "Abierto" o "En curso". Se utiliza para impedir desactivar clientes con expedientes activos.
        private bool ClienteTieneExpedienteAbiertoOCurso(int clienteId)
        {
            var expedienteRepo = new ExpedienteRepository();

            var query = expedienteRepo.GetAll()
                .Where(e => e.ClienteId == clienteId && e.EstadosExpediente != null &&
                            (e.EstadosExpediente.Nombre == "Abierto" || e.EstadosExpediente.Nombre == "En curso"));

            return query.Any();
        }

        /// <summary>
        /// Crea un nuevo cliente tras validar sus datos.
        /// </summary>
        /// <param name="c">Cliente a crear.</param>
        /// <exception cref="System.Exception">Se lanza si la validación falla o si el DNI/CIF ya pertenece a otro cliente.</exception>
        public void Crear(Clientes c)
        {
            ValidarCliente(c);

            if (ExisteDniParaOtroCliente(c.DNI_CIF, 0))
                throw new Exception("Este DNI/CIF ya pertenece a otro cliente.");

            c.FechaRegistro = DateTime.Now;
            _repo.Add(c);
        }

        /// <summary>
        /// Edita un cliente existente tras validar sus datos.
        /// </summary>
        /// <param name="c">Cliente con los nuevos datos (debe incluir ClienteId).</param>
        /// <exception cref="System.Exception">Se lanza si la validación falla, si el DNI/CIF pertenece a otro cliente o si se intenta desactivar un cliente con expedientes abiertos/en curso.</exception>
        public void Editar(Clientes c)
        {
            ValidarCliente(c);

            if (ExisteDniParaOtroCliente(c.DNI_CIF, c.ClienteId))
                throw new Exception("Este DNI/CIF ya pertenece a otro cliente.");

            // Si se intenta desactivar el cliente, comprobar que no tenga expedientes abiertos/en curso
            if (!c.Activo && ClienteTieneExpedienteAbiertoOCurso(c.ClienteId))
                throw new Exception("No se puede desactivar al cliente porque tiene expedientes en estado 'Abierto' o 'En curso'.");

            _repo.Update(c);
        }

        /// <summary>
        /// Marca el cliente como eliminado (lógica de borrado del repositorio).
        /// Antes de desactivar comprueba si tiene expedientes abiertos o en curso.
        /// </summary>
        /// <param name="c">Cliente a eliminar (desactivar).</param>
        /// <exception cref="System.Exception">Se lanza si el cliente tiene expedientes en estado 'Abierto' o 'En curso'.</exception>
        public void Eliminar(Clientes c)
        {
            // Antes de desactivar, comprobar si tiene expedientes abiertos/en curso
            if (ClienteTieneExpedienteAbiertoOCurso(c.ClienteId))
                throw new Exception("No se puede desactivar al cliente porque tiene expedientes en estado 'Abierto' o 'En curso'.");

            _repo.Delete(c);
        }

        /// <summary>
        /// Elimina permanentemente un cliente.
        /// </summary>
        /// <param name="c">Cliente a eliminar permanentemente.</param>
        public void EliminarPermanente(Clientes c)
        {
            if (c == null) return;
            _repo.DeletePermanent(c.ClienteId);
        }

        /// <summary>
        /// Elimina permanentemente un cliente por su identificador.
        /// </summary>
        /// <param name="clienteId">Identificador del cliente a eliminar permanentemente.</param>
        public void EliminarPermanente(int clienteId)
        {
            _repo.DeletePermanent(clienteId);
        }
    }
}
