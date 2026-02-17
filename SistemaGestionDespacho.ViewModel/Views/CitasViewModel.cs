using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Globalization;
using System.Text.RegularExpressions;
using SistemaGestionDespacho.Model;
using SistemaGestionDespacho.Model.Services;

namespace SistemaGestionDespacho.ViewModel.Views
{
    /// <summary>
    /// ViewModel para la gestión de citas del despacho.
    /// Controla CRUD, filtros y el formulario de cita.
    /// </summary>
    public class CitasViewModel : BaseViewModel
    {
        private readonly CitaService _citaService;
        private readonly ClienteService _clienteService;
        private readonly ExpedienteService _expedienteService;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="CitasViewModel"/>, carga listas y muestra todas las citas.
        /// </summary>
        public CitasViewModel()
        {
            _citaService = new CitaService();
            _clienteService = new ClienteService();
            _expedienteService = new ExpedienteService();

            Clientes = new ObservableCollection<Clientes>(_clienteService.Buscar(null, null, false));
            Expedientes = new ObservableCollection<Expedientes>(_expedienteService.Filtrar(null, null, null, null, null));

            EstadosCita = new ObservableCollection<string>
            {
                "Pendiente",
                "Realizada",
                "Cancelada"
            };

            //Cargar todas las citas inicialmente
            Citas = new ObservableCollection<Citas>(_citaService.Filtrar(null, null, null, null));

            //Inicializar valores de fecha y hora
            FechaHora = DateTime.Now;
            Hora = DateTime.Now.ToString("HH:mm");

            //Inicializar flags de habilitación: ambos habilitados por defecto
            ClienteEnabled = true;
            ExpedienteEnabled = true;

            InicializarComandos();
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="CitasViewModel"/> y carga las citas del expediente indicado.
        /// </summary>
        /// <param name="expedienteId">Identificador del expediente cuyas citas se cargarán.</param>
        public CitasViewModel(int expedienteId)
        {
            _citaService = new CitaService();
            _clienteService = new ClienteService();
            _expedienteService = new ExpedienteService();

            Clientes = new ObservableCollection<Clientes>(_clienteService.Buscar(null, null, false));
            Expedientes = new ObservableCollection<Expedientes>(_expedienteService.Filtrar(null, null, null, null, null));

            EstadosCita = new ObservableCollection<string>
            {
                "Pendiente",
                "Realizada",
                "Cancelada"
            };

            //Seleccionamos el expediente automáticamente
            ExpedienteFormulario = Expedientes.First(e => e.ExpedienteId == expedienteId);

            //Cargamos las citas de ese expediente
            Citas = new ObservableCollection<Citas>(_citaService.Filtrar(null, null, null, expedienteId));

            //Inicializar valores de fecha y hora
            FechaHora = DateTime.Now;
            Hora = DateTime.Now.ToString("HH:mm");

            InicializarComandos();
        }

        //InicializarComandos() --> Configura los comandos para las acciones del formulario (crear, editar, eliminar, limpiar, buscar, limpiar filtros)
        private void InicializarComandos()
        {
            CrearCommand = new RelayCommand(_ => CrearCita());
            EditarCommand = new RelayCommand(_ => EditarCita(), _ => CitaSeleccionada != null);
            EliminarCommand = new RelayCommand(_ => EliminarCita(), _ => CitaSeleccionada != null);
            LimpiarCommand = new RelayCommand(_ => LimpiarFormulario());
            BuscarCommand = new RelayCommand(_ => BuscarCitas());
            LimpiarFiltrosCommand = new RelayCommand(_ => LimpiarFiltros());
        }

        // ============================
        // LISTADO Y SELECCIÓN
        // ============================

        private ObservableCollection<Citas> _citas;
        public ObservableCollection<Citas> Citas
        {
            get => _citas;
            set
            {
                _citas = value;
                OnPropertyChanged(nameof(Citas));
            }
        }

        private Citas _citaSeleccionada;
        public Citas CitaSeleccionada
        {
            get => _citaSeleccionada;
            set
            {
                _citaSeleccionada = value;
                MensajeError = "";
                OnPropertyChanged(nameof(CitaSeleccionada));
                CargarCitaEnFormulario();
            }
        }

        // ============================
        // LISTAS PARA COMBOS
        // ============================

        public ObservableCollection<Clientes> Clientes { get; set; }
        public ObservableCollection<Expedientes> Expedientes { get; set; }
        public ObservableCollection<string> EstadosCita { get; set; }

        // ============================
        // CAMPOS DEL FORMULARIO
        // ============================

        private Clientes _clienteFormulario;
        public Clientes ClienteFormulario
        {
            get => _clienteFormulario;
            set
            {
                _clienteFormulario = value;
                OnPropertyChanged(nameof(ClienteFormulario));

                //Si hay un cliente seleccionado, deshabilitar el combo de expedientes
                ExpedienteEnabled = _clienteFormulario == null;
            }
        }

        private Expedientes _expedienteFormulario;
        public Expedientes ExpedienteFormulario
        {
            get => _expedienteFormulario;
            set
            {
                _expedienteFormulario = value;
                OnPropertyChanged(nameof(ExpedienteFormulario));

                //Si hay un expediente seleccionado, deshabilitar el combo de clientes
                ClienteEnabled = _expedienteFormulario == null;
            }
        }

        private DateTime? _fechaHora = DateTime.Now;
        public DateTime? FechaHora
        {
            get => _fechaHora;
            set
            {
                _fechaHora = value;
                OnPropertyChanged(nameof(FechaHora));
            }
        }

        private string _hora;
        public string Hora
        {
            get => _hora;
            set
            {
                _hora = value;
                OnPropertyChanged(nameof(Hora));
            }
        }

        private string _lugar;
        public string Lugar
        {
            get => _lugar;
            set
            {
                _lugar = value;
                OnPropertyChanged(nameof(Lugar));
            }
        }

        private string _motivo;
        public string Motivo
        {
            get => _motivo;
            set
            {
                _motivo = value;
                OnPropertyChanged(nameof(Motivo));
            }
        }

        private string _estado;
        public string Estado
        {
            get => _estado;
            set
            {
                _estado = value;
                OnPropertyChanged(nameof(Estado));
            }
        }

        private string _mensajeError;
        public string MensajeError
        {
            get => _mensajeError;
            set
            {
                _mensajeError = value;
                OnPropertyChanged(nameof(MensajeError));
            }
        }

        //Contról de habilitación de combos: si se selecciona un cliente, se deshabilita el combo de expedientes y viceversa.
        private bool _clienteEnabled = true;
        public bool ClienteEnabled
        {
            get => _clienteEnabled;
            set
            {
                _clienteEnabled = value;
                OnPropertyChanged(nameof(ClienteEnabled));
            }
        }

        //Control de habilitación de combos: si se selecciona un expediente, se deshabilita el combo de clientes y viceversa.
        private bool _expedienteEnabled = true;
        public bool ExpedienteEnabled
        {
            get => _expedienteEnabled;
            set
            {
                _expedienteEnabled = value;
                OnPropertyChanged(nameof(ExpedienteEnabled));
            }
        }

        // ============================
        // FILTROS
        // ============================

        private DateTime? _filtroFecha;
        public DateTime? FiltroFecha
        {
            get => _filtroFecha;
            set
            {
                _filtroFecha = value;
                OnPropertyChanged(nameof(FiltroFecha));
            }
        }

        private string _filtroEstado;
        public string FiltroEstado
        {
            get => _filtroEstado;
            set
            {
                _filtroEstado = value;
                OnPropertyChanged(nameof(FiltroEstado));
            }
        }

        private Clientes _filtroCliente;
        public Clientes FiltroCliente
        {
            get => _filtroCliente;
            set
            {
                _filtroCliente = value;
                OnPropertyChanged(nameof(FiltroCliente));
            }
        }

        private Expedientes _filtroExpediente;
        public Expedientes FiltroExpediente
        {
            get => _filtroExpediente;
            set
            {
                _filtroExpediente = value;
                OnPropertyChanged(nameof(FiltroExpediente));
            }
        }

        // ============================
        // COMANDOS
        // ============================

        public ICommand CrearCommand { get; private set; }
        public ICommand EditarCommand { get; private set; }
        public ICommand EliminarCommand { get; private set; }
        public ICommand LimpiarCommand { get; private set; }
        public ICommand BuscarCommand { get; private set; }

        // Añadido: comando para limpiar filtros
        public ICommand LimpiarFiltrosCommand { get; private set; }

        // ============================
        // MÉTODOS PRINCIPALES
        // ============================

        //TryCombineFechaHora() --> Combina la fecha seleccionada en el DatePicker con la hora ingresada en el TextBox, validando el formato de hora y asegurando que se mantenga la parte de fecha intacta.
        private bool TryCombineFechaHora(out DateTime result)
        {
            //Inicializar el resultado con un valor predeterminado
            result = default(DateTime);
            var date = FechaHora ?? DateTime.Now;

            //Si Hora está vacío o solo tiene espacios, asumimos la hora actual
            if (string.IsNullOrWhiteSpace(Hora))
            {
                result = date.Date + DateTime.Now.TimeOfDay;
                return true;
            }

            //Reemplazar posibles separadores de hora (p.ej. "14.30" -> "14:30")
            var input = Hora.Trim().Replace('.', ':');

            //Validar formato de hora con regex: debe ser HH:mm o H:mm, con horas entre 0-23 y minutos entre 00-59
            if (!Regex.IsMatch(input, "^\\d{1,2}:\\d{2}$"))
                return false;

            //Intentar parsear la hora usando TimeSpan para validar el formato y rango
            TimeSpan ts;
            //TimeSpan.TryParseExact permite validar formatos específicos y asegura que la hora sea válida (p.ej. no acepta "24:00" o "12:60")
            if (!TimeSpan.TryParseExact(input, new[] { "h\\:mm", "hh\\:mm" }, CultureInfo.InvariantCulture, out ts))
                return false;

            //Validar que la hora esté dentro del rango permitido (0:00 a 23:59)
            if (ts.TotalHours < 0 || ts.TotalHours >= 24)
                return false;

            //Si llegamos aquí, la hora es válida. Formateamos la hora para mostrarla siempre en formato HH:mm (p.ej. "9:5" -> "09:05")
            Hora = ts.ToString(@"hh\:mm");

            //Combinar la fecha con la hora y devolver el resultado
            result = date.Date + ts;
            return true;
        }

        //CrearCita() --> Valida los datos del formulario, crea una nueva cita y la agrega a la lista. Permite crear una cita asociada a un cliente o a un expediente, pero no requiere ambos.
        private void CrearCita()
        {
            try
            {
                //Validar que se haya seleccionado al menos un cliente o un expediente
                if (ClienteFormulario == null && ExpedienteFormulario == null) 
                { 
                    MensajeError = "Debe seleccionar cliente o expediente."; 
                    return; 
                }

                //Combinar fecha y hora, validando el formato de hora
                DateTime fechaHoraCompleta;
                if (!TryCombineFechaHora(out fechaHoraCompleta)) 
                { 
                    MensajeError = "Formato de hora inválido (HH:mm). Use 00:00 - 23:59."; 
                    return; 
                }

                var nueva = new Citas
                {
                    ClienteId = ClienteFormulario?.ClienteId,
                    ExpedienteId = ExpedienteFormulario?.ExpedienteId,
                    FechaHora = fechaHoraCompleta,
                    Lugar = Lugar,
                    Motivo = Motivo,
                    Estado = Estado
                };

                _citaService.Crear(nueva);

                //Si la cita se creó correctamente, recargar la lista de citas para reflejar el nuevo registro. Si la cita está asociada a un expediente, recargar solo las citas de ese expediente para mantener el contexto.
                if (ClienteFormulario != null)
                    nueva.Clientes = ClienteFormulario;
                if (ExpedienteFormulario != null)
                    nueva.Expedientes = ExpedienteFormulario;

                Citas.Add(nueva);
                LimpiarFormulario();
                MensajeError = "";
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        //EditarCita() --> Valida los datos del formulario, actualiza la cita seleccionada y refresca la lista. Permite editar una cita asociada a un cliente o a un expediente, pero no requiere ambos.
        private void EditarCita()
        {
            if (CitaSeleccionada == null)
                return;

            try
            {
                //Combinar fecha y hora, validando el formato de hora
                DateTime fechaHoraCompleta;
                if (!TryCombineFechaHora(out fechaHoraCompleta)) 
                { 
                    MensajeError = "Formato de hora inválido (HH:mm). Use 00:00 - 23:59."; 
                    return; 
                }

                //Actualizar los campos de la cita seleccionada con los valores del formulario
                CitaSeleccionada.ClienteId = ClienteFormulario?.ClienteId;
                CitaSeleccionada.ExpedienteId = ExpedienteFormulario?.ExpedienteId;
                CitaSeleccionada.FechaHora = fechaHoraCompleta;
                CitaSeleccionada.Lugar = Lugar;
                CitaSeleccionada.Motivo = Motivo;
                CitaSeleccionada.Estado = Estado;

                _citaService.Editar(CitaSeleccionada);
                //Después de editar, recargar la lista de citas para reflejar los cambios. Si la cita está asociada a un expediente, recargar solo las citas de ese expediente para mantener el contexto.
                if (ExpedienteFormulario != null) 
                    CargarCitasDeExpediente(ExpedienteFormulario.ExpedienteId);

                BuscarCitas();
                LimpiarFormulario();
                MensajeError = "";
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        //EliminarCita() --> Cancela la cita seleccionada tras una confirmación. En lugar de eliminar físicamente la cita, se cambia su estado a "Cancelada". Después de cancelar, se refresca la lista de citas para reflejar el cambio.
        private void EliminarCita()
        {
            if (CitaSeleccionada == null)
                return;

            try
            {
                //Confirmación antes de cancelar
                var result = MessageBox.Show("¿Está seguro que desea cancelar la cita seleccionada?", "Confirmar cancelación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                    return;

                _citaService.Cancelar(CitaSeleccionada);

                //Después de cancelar, recargar siempre todas las citas para que el DataGrid muestre la lista completa.
                Citas = new ObservableCollection<Citas>(_citaService.Filtrar(null, null, null, null));

                LimpiarFormulario();
                MensajeError = "";
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        //BuscarCitas() --> Aplica los filtros seleccionados (fecha, estado, cliente, expediente) y actualiza la lista de citas mostrada. Si no se selecciona ningún filtro, muestra todas las citas.
        private void BuscarCitas()
        {
            var lista = _citaService.Filtrar(
                FiltroFecha,
                FiltroEstado,
                FiltroCliente?.ClienteId,
                FiltroExpediente?.ExpedienteId
            );

            Citas = new ObservableCollection<Citas>(lista);
        }

        //CargarCitasDeExpediente() --> Carga solo las citas asociadas al expediente indicado. Este método se utiliza después de crear o editar una cita para recargar la lista de citas del expediente y mantener el contexto.
        private void CargarCitasDeExpediente(int expedienteId) 
        { 
            Citas = new ObservableCollection<Citas>(
                _citaService.Filtrar(null, null, null, expedienteId)); 
        }

        //LimpiarFiltros() --> Restablece los filtros a su estado inicial (sin filtros) y recarga la lista de citas para mostrar todas las citas disponibles.
        private void LimpiarFiltros()
        {
            FiltroFecha = null;
            FiltroEstado = null;
            FiltroCliente = null;
            FiltroExpediente = null;

            BuscarCitas();
        }

        //LimpiarFormulario() --> Restablece los campos del formulario a su estado inicial, limpia la selección de cita y habilita ambos combos para permitir una nueva selección. Este método se utiliza después de crear, editar o cancelar una cita para preparar el formulario para una nueva entrada.
        private void LimpiarFormulario()
        {
            ClienteFormulario = null;
            ExpedienteFormulario = null;
            FechaHora = DateTime.Now;
            Hora = DateTime.Now.ToString("HH:mm");
            Lugar = "";
            Motivo = "";
            Estado = null;
            MensajeError = "";
            CitaSeleccionada = null;

            //Al limpiar el formulario, habilitamos ambos combos para permitir seleccionar un cliente o un expediente para la nueva cita.
            ClienteEnabled = true;
            ExpedienteEnabled = true;
        }

        //CargarCitaEnFormulario() --> Carga los datos de la cita seleccionada en los campos del formulario para su visualización y edición. Si la cita está asociada a un cliente, se selecciona ese cliente en el combo y se deshabilita el combo de expedientes, y viceversa.
        //Este método se llama automáticamente cuando se selecciona una cita en el DataGrid.
        private void CargarCitaEnFormulario()
        {
            if (CitaSeleccionada == null)
                return;

            ClienteFormulario = Clientes.FirstOrDefault(c => c.ClienteId == CitaSeleccionada.ClienteId);
            ExpedienteFormulario = Expedientes.FirstOrDefault(e => e.ExpedienteId == CitaSeleccionada.ExpedienteId);

            FechaHora = CitaSeleccionada.FechaHora.Date;
            Hora = CitaSeleccionada.FechaHora.ToString("HH:mm");
            Lugar = CitaSeleccionada.Lugar;
            Motivo = CitaSeleccionada.Motivo;
            Estado = CitaSeleccionada.Estado;

            //Si la cita tiene un cliente asociado, deshabilitar el combo de expedientes para evitar seleccionar un expediente diferente, y viceversa.
            ClienteEnabled = ExpedienteFormulario == null;
            ExpedienteEnabled = ClienteFormulario == null;
        }
    }
}
