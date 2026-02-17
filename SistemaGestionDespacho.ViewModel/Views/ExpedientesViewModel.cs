using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SistemaGestionDespacho.Model;
using SistemaGestionDespacho.Model.Services;

namespace SistemaGestionDespacho.ViewModel.Views
{
    /// <summary>
    /// ViewModel para la gestión de expedientes jurídicos.
    /// Controla CRUD, filtros y validaciones.
    /// </summary>
    public class ExpedientesViewModel : BaseViewModel
    {
        private readonly ExpedienteService _expedienteService;
        private readonly ClienteService _clienteService;
        private readonly EstadoExpedienteService _estadoService;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ExpedientesViewModel"/>, carga listas y configura comandos.
        /// </summary>
        public ExpedientesViewModel()
        {
            _expedienteService = new ExpedienteService();
            _clienteService = new ClienteService();
            _estadoService = new EstadoExpedienteService();

            Expedientes = new ObservableCollection<Expedientes>(_expedienteService.Filtrar(null, null, null, null, null));
            Clientes = new ObservableCollection<Clientes>(_clienteService.Buscar(null, null, false));
            Estados = new ObservableCollection<EstadosExpediente>(_estadoService.GetAll());

            TiposExpediente = new ObservableCollection<string>
            {
                "Civil",
                "Penal",
                "Laboral",
                "Administrativo",
                "Familia"
            };

            CrearCommand = new RelayCommand(_ => CrearExpediente());
            EditarCommand = new RelayCommand(_ => EditarExpediente(), _ => ExpedienteSeleccionado != null);
            CerrarCommand = new RelayCommand(_ => CerrarExpediente(), _ => ExpedienteSeleccionado != null);
            LimpiarCommand = new RelayCommand(_ => LimpiarFormulario());
            BuscarCommand = new RelayCommand(_ => BuscarExpedientes());

            VerActuacionesCommand = new RelayCommand(_ => VerActuaciones(), _ => ExpedienteSeleccionado != null);
            VerCitasCommand = new RelayCommand(_ => VerCitas(), _ => ExpedienteSeleccionado != null);
            LimpiarFiltrosCommand = new RelayCommand(_ => LimpiarFiltros());
        }

        // ============================
        // LISTADO Y SELECCIÓN
        // ============================

        private ObservableCollection<Expedientes> _expedientes;
        public ObservableCollection<Expedientes> Expedientes
        {
            get => _expedientes;
            set
            {
                _expedientes = value;
                OnPropertyChanged(nameof(Expedientes));
            }
        }

        private Expedientes _expedienteSeleccionado;
        public Expedientes ExpedienteSeleccionado
        {
            get => _expedienteSeleccionado;
            set
            {
                _expedienteSeleccionado = value;
                MensajeError = string.Empty;
                OnPropertyChanged(nameof(ExpedienteSeleccionado));
                CargarExpedienteEnFormulario();
            }
        }

        // ============================
        // LISTAS PARA COMBOS
        // ============================

        public ObservableCollection<Clientes> Clientes { get; set; }
        public ObservableCollection<EstadosExpediente> Estados { get; set; }
        public ObservableCollection<string> TiposExpediente { get; set; }

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
            }
        }

        private string _tipoFormulario;
        public string TipoFormulario
        {
            get => _tipoFormulario;
            set
            {
                _tipoFormulario = value;
                OnPropertyChanged(nameof(TipoFormulario));
            }
        }

        private EstadosExpediente _estadoFormulario;
        public EstadosExpediente EstadoFormulario
        {
            get => _estadoFormulario;
            set
            {
                _estadoFormulario = value;
                OnPropertyChanged(nameof(EstadoFormulario));
            }
        }

        private string _titulo;
        public string Titulo
        {
            get => _titulo;
            set
            {
                _titulo = value;
                OnPropertyChanged(nameof(Titulo));
            }
        }

        private string _descripcion;
        public string Descripcion
        {
            get => _descripcion;
            set
            {
                _descripcion = value;
                OnPropertyChanged(nameof(Descripcion));
            }
        }

        private DateTime? _fechaApertura;
        public DateTime? FechaApertura
        {
            get => _fechaApertura;
            set
            {
                _fechaApertura = value;
                OnPropertyChanged(nameof(FechaApertura));
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

        // ============================
        // FILTROS
        // ============================

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

        private string _filtroTipo;
        public string FiltroTipo
        {
            get => _filtroTipo;
            set
            {
                _filtroTipo = value;
                OnPropertyChanged(nameof(FiltroTipo));
            }
        }

        private EstadosExpediente _filtroEstado;
        public EstadosExpediente FiltroEstado
        {
            get => _filtroEstado;
            set
            {
                _filtroEstado = value;
                OnPropertyChanged(nameof(FiltroEstado));
            }
        }

        private DateTime? _filtroDesde;
        public DateTime? FiltroDesde
        {
            get => _filtroDesde;
            set
            {
                _filtroDesde = value;
                OnPropertyChanged(nameof(FiltroDesde));
            }
        }

        private DateTime? _filtroHasta;
        public DateTime? FiltroHasta
        {
            get => _filtroHasta;
            set
            {
                _filtroHasta = value;
                OnPropertyChanged(nameof(FiltroHasta));
            }
        }

        // ============================
        // COMANDOS
        // ============================

        public ICommand CrearCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand CerrarCommand { get; }
        public ICommand LimpiarCommand { get; }
        public ICommand BuscarCommand { get; }
        public ICommand VerActuacionesCommand { get; }
        public ICommand VerCitasCommand { get; }
        public ICommand LimpiarFiltrosCommand { get; }

        //VerActuaciones() --> Abre la ventana de actuaciones filtrada por el expediente seleccionado.
        private void VerActuaciones()
        {
            MainWindowViewModel.Instance.AbrirActuaciones(ExpedienteSeleccionado.ExpedienteId);
        }

        //VerCitas() --> Abre la ventana de citas filtrada por el expediente seleccionado.
        private void VerCitas()
        {
            MainWindowViewModel.Instance.AbrirCitas(ExpedienteSeleccionado.ExpedienteId);
        }

        // ============================
        // MÉTODOS PRINCIPALES
        // ============================

        //CrearExpediente() --> Valida los datos del formulario y llama al servicio para crear un nuevo expediente.
        //Luego actualiza la lista y limpia el formulario.
        private void CrearExpediente()
        {
            try
            {
                var exp = new Expedientes
                {
                    ClienteId = ClienteFormulario?.ClienteId ?? 0,
                    Tipo = TipoFormulario,
                    EstadoId = EstadoFormulario?.EstadoId ?? 0,
                    Titulo = Titulo,
                    Descripcion = Descripcion,
                    FechaApertura = FechaApertura ?? DateTime.Now
                };

                _expedienteService.Crear(exp);
                BuscarExpedientes();
                LimpiarFormulario();
                MensajeError = "";
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        //EditarExpediente() --> Valida los datos del formulario y llama al servicio para editar el expediente seleccionado.
        private void EditarExpediente()
        {
            if (ExpedienteSeleccionado == null)
                return;

            try
            {
                //Actualizar los campos del expediente seleccionado con los datos del formulario
                ExpedienteSeleccionado.ClienteId = ClienteFormulario?.ClienteId ?? 0;
                ExpedienteSeleccionado.Tipo = TipoFormulario;
                ExpedienteSeleccionado.EstadoId = EstadoFormulario?.EstadoId ?? 0;
                ExpedienteSeleccionado.Titulo = Titulo;
                ExpedienteSeleccionado.Descripcion = Descripcion;
                ExpedienteSeleccionado.FechaApertura = FechaApertura ?? DateTime.Now;

                _expedienteService.Editar(ExpedienteSeleccionado);

                BuscarExpedientes();
                LimpiarFormulario();
                MensajeError = "";
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        //CerrarExpediente() --> Cambia el estado del expediente seleccionado a "Cerrado" y establece la fecha de cierre.
        private void CerrarExpediente()
        {
            if (ExpedienteSeleccionado == null)
                return;

            try
            {
                //Confirmación antes de cerrar
                var result = MessageBox.Show("¿Está seguro que desea cerrar el expediente seleccionado?", "Confirmar cierre", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                    return;

                _expedienteService.Cerrar(ExpedienteSeleccionado);
                BuscarExpedientes();
                LimpiarFormulario();
                MensajeError = "";
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        //BuscarExpedientes() --> Llama al servicio para obtener la lista de expedientes que cumplen los criterios de filtro y actualiza la colección.
        private void BuscarExpedientes()
        {
            int? clienteId = FiltroCliente?.ClienteId;
            int? estadoId = FiltroEstado?.EstadoId;
            string tipo = string.IsNullOrWhiteSpace(FiltroTipo) ? null : FiltroTipo;

            var lista = _expedienteService.Filtrar(
                clienteId,
                tipo,
                estadoId,
                FiltroDesde,
                FiltroHasta
            );

            Expedientes = new ObservableCollection<Expedientes>(lista);
        }

        //LimpiarFormulario() --> Restablece los campos del formulario a sus valores predeterminados para preparar la creación de un nuevo expediente.
        private void LimpiarFormulario()
        {
            ClienteFormulario = null;
            TipoFormulario = null;
            EstadoFormulario = null;
            Titulo = "";
            Descripcion = "";
            FechaApertura = DateTime.Now;
            MensajeError = "";
            ExpedienteSeleccionado = null;
        }

        //LimpiarFiltros() --> Restablece los campos de filtro a sus valores predeterminados y realiza una nueva búsqueda para mostrar todos los expedientes.
        private void LimpiarFiltros()
        {
            FiltroCliente = null;
            FiltroTipo = null;
            FiltroEstado = null;
            FiltroDesde = null;
            FiltroHasta = null;

            BuscarExpedientes();
        }

        //CargarExpedienteEnFormulario() --> Carga los datos del expediente seleccionado en los campos del formulario para su visualización o edición.
        private void CargarExpedienteEnFormulario()
        {
            if (ExpedienteSeleccionado == null)
                return;

            //Buscar el cliente y estado correspondientes al expediente seleccionado para mostrar en los combos
            ClienteFormulario = null;
            foreach (var c in Clientes)
            {
                if (c.ClienteId == ExpedienteSeleccionado.ClienteId)
                {
                    ClienteFormulario = c;
                    break;
                }
            }

            EstadoFormulario = null;
            foreach (var e in Estados)
            {
                if (e.EstadoId == ExpedienteSeleccionado.EstadoId)
                {
                    EstadoFormulario = e;
                    break;
                }
            }

            //Asignar el tipo, título, descripción y fecha de apertura al formulario
            TipoFormulario = ExpedienteSeleccionado.Tipo;
            Titulo = ExpedienteSeleccionado.Titulo;
            Descripcion = ExpedienteSeleccionado.Descripcion;
            FechaApertura = ExpedienteSeleccionado.FechaApertura;
        }
    }
}
