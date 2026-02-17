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
    /// ViewModel para la gestión de actuaciones de un expediente.
    /// Controla CRUD y formulario, sin validaciones (van al Service).
    /// </summary>
    public class ActuacionesViewModel : BaseViewModel
    {
        private readonly ActuacionService _actuacionService;

        /// <summary>
        /// Expediente al que pertenecen las actuaciones.
        /// </summary>
        public ObservableCollection<Expedientes> Expedientes { get; set; }

        private Expedientes _expedienteSeleccionado;
        public Expedientes ExpedienteSeleccionado
        {
            get => _expedienteSeleccionado;
            set
            {
                _expedienteSeleccionado = value;
                OnPropertyChanged(nameof(ExpedienteSeleccionado));
            }
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ActuacionesViewModel"/> y carga todas las actuaciones.
        /// </summary>
        public ActuacionesViewModel()
        {
            _actuacionService = new ActuacionService();

            // Cargar expedientes para el ComboBox
            var expService = new ExpedienteService();
            Expedientes = new ObservableCollection<Expedientes>(
                expService.Filtrar(null, null, null, null, null)
            );

            // Cargar todas las actuaciones por defecto para que al abrir desde el menú se muestren
            Actuaciones = new ObservableCollection<Actuaciones>(_actuacionService.GetAll());

            InicializarListasYComandos();
        }

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ActuacionesViewModel"/> y carga las actuaciones del expediente indicado.
        /// </summary>
        /// <param name="expedienteId">Identificador del expediente cuyas actuaciones se cargarán.</param>
        public ActuacionesViewModel(int expedienteId)
        {
            _actuacionService = new ActuacionService();

            var expService = new ExpedienteService();
            Expedientes = new ObservableCollection<Expedientes>(
                expService.Filtrar(null, null, null, null, null)
            );

            ExpedienteSeleccionado = Expedientes.First(e => e.ExpedienteId == expedienteId);

            Actuaciones = new ObservableCollection<Actuaciones>(
                _actuacionService.GetByExpediente(expedienteId)
            );

            InicializarListasYComandos();
        }

        //IncializarListasYComandos() --> Método para inicializar las listas de opciones y los comandos del ViewModel
        private void InicializarListasYComandos()
        {
            TiposActuacion = new ObservableCollection<string>
            {
                "Llamada",
                "Reunión",
                "Escrito presentado",
                "Escrito recibido",
                "Notificación",
                "Gestión documental"
            };

            CrearCommand = new RelayCommand(_ => CrearActuacion());
            EditarCommand = new RelayCommand(_ => EditarActuacion(), _ => ActuacionSeleccionada != null);
            EliminarCommand = new RelayCommand(_ => EliminarActuacion(), _ => ActuacionSeleccionada != null);
            LimpiarCommand = new RelayCommand(_ => LimpiarFormulario());
            BuscarCommand = new RelayCommand(_ => BuscarActuaciones());
            LimpiarFiltrosCommand = new RelayCommand(_ => LimpiarFiltros());
        }

        // ============================
        // LISTADO Y SELECCIÓN
        // ============================

        private ObservableCollection<Actuaciones> _actuaciones;
        public ObservableCollection<Actuaciones> Actuaciones
        {
            get => _actuaciones;
            set
            {
                _actuaciones = value;
                OnPropertyChanged(nameof(Actuaciones));
            }
        }

        private Actuaciones _actuacionSeleccionada;
        public Actuaciones ActuacionSeleccionada
        {
            get => _actuacionSeleccionada;
            set
            {
                _actuacionSeleccionada = value;
                MensajeError = "";
                OnPropertyChanged(nameof(ActuacionSeleccionada));
                CargarActuacionEnFormulario();
            }
        }

        // ============================
        // LISTAS PARA COMBOS
        // ============================

        public ObservableCollection<string> TiposActuacion { get; set; }

        // ============================
        // CAMPOS DEL FORMULARIO
        // ============================

        private string _tipo;
        public string Tipo
        {
            get => _tipo;
            set
            {
                _tipo = value;
                OnPropertyChanged(nameof(Tipo));
            }
        }

        private DateTime? _fecha = DateTime.Now;
        public DateTime? Fecha
        {
            get => _fecha;
            set
            {
                _fecha = value;
                OnPropertyChanged(nameof(Fecha));
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

        // ============================
        // COMANDOS
        // ============================

        public ICommand CrearCommand { get; private set; }
        public ICommand EditarCommand { get; private set; }
        public ICommand EliminarCommand { get; private set; }
        public ICommand LimpiarCommand { get; private set; }
        public ICommand BuscarCommand { get; private set; }
        public ICommand LimpiarFiltrosCommand { get; private set; }

        // ============================
        // MÉTODOS PRINCIPALES
        // ============================

        //CrearActuacion() --> Método para crear una nueva actuación a partir de los datos del formulario
        private void CrearActuacion()
        {
            try
            {
                if (ExpedienteSeleccionado == null) 
                { 
                    MensajeError = "Debe seleccionar un expediente."; 
                    return; 
                }

                var nueva = new Actuaciones
                {
                    ExpedienteId = ExpedienteSeleccionado.ExpedienteId,
                    Tipo = Tipo,
                    Fecha = Fecha ?? DateTime.Now,
                    Descripcion = Descripcion
                };

                _actuacionService.Crear(nueva);

                BuscarActuaciones();
                LimpiarFormulario();
                MensajeError = "";
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        //EditarActuacion() --> Método para editar la actuación seleccionada con los datos del formulario
        private void EditarActuacion()
        {
            if (ActuacionSeleccionada == null)
                return;

            try
            {
                //Actualizar campos de la actuación seleccionada
                ActuacionSeleccionada.Tipo = Tipo;
                ActuacionSeleccionada.Fecha = Fecha ?? DateTime.Now;
                ActuacionSeleccionada.Descripcion = Descripcion;

                //Si se ha cambiado el expediente, actualizar el ExpedienteId
                if (ExpedienteSeleccionado != null)
                {
                    //Solo actualizar si se ha seleccionado un expediente diferente al actual
                    ActuacionSeleccionada.ExpedienteId = ExpedienteSeleccionado.ExpedienteId;
                    ActuacionSeleccionada.Expedientes = ExpedienteSeleccionado;
                }

                _actuacionService.Editar(ActuacionSeleccionada);

                BuscarActuaciones();
                LimpiarFormulario();
                MensajeError = "";
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        //EliminarActuacion() --> Método para eliminar la actuación seleccionada
        private void EliminarActuacion()
        {
            if (ActuacionSeleccionada == null)
                return;

            try
            {
                //Confirmación antes de eliminar
                var result = MessageBox.Show("¿Está seguro que desea eliminar la actuación seleccionada?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                    return;

                _actuacionService.Eliminar(ActuacionSeleccionada);
                Actuaciones.Remove(ActuacionSeleccionada);

                LimpiarFormulario();
                MensajeError = "";
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        // ============================
        // MÉTODOS AUXILIARES
        // ============================

        private void CargarActuacionesDeExpediente(int expedienteId)
        {
            Actuaciones = new ObservableCollection<Actuaciones>(
                _actuacionService.GetByExpediente(expedienteId)
            );
        }

        //BuscarActuaciones() --> Método para buscar actuaciones según los filtros aplicados
        private void BuscarActuaciones()
        {
            try
            {
                var lista = _actuacionService.Filtrar(
                    FiltroExpediente?.ExpedienteId,
                    FiltroTipo,
                    FiltroFecha
                );

                Actuaciones = new ObservableCollection<Actuaciones>(lista);
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        //LimpiarFiltros() --> Método para limpiar los filtros y mostrar todas las actuaciones
        private void LimpiarFiltros()
        {
            FiltroExpediente = null;
            FiltroTipo = null;
            FiltroFecha = null;

            BuscarActuaciones();
        }

        //LimpiarFormulario() --> Método para limpiar los campos del formulario y deseleccionar la actuación
        private void LimpiarFormulario()
        {
            Tipo = null;
            Fecha = DateTime.Now;
            Descripcion = "";
            MensajeError = "";
            ActuacionSeleccionada = null;
            ExpedienteSeleccionado = null;
        }

        //CargarActuacionEnFormulario() --> Método para cargar los datos de la actuación seleccionada en los campos del formulario
        private void CargarActuacionEnFormulario()
        {
            if (ActuacionSeleccionada == null)
                return;

            //Cargar datos de la actuación seleccionada en el formulario
            Tipo = ActuacionSeleccionada.Tipo;
            Fecha = ActuacionSeleccionada.Fecha;
            Descripcion = ActuacionSeleccionada.Descripcion;

            //Seleccionar el expediente correspondiente en el ComboBox
            if (Expedientes != null)
            {
                var match = Expedientes.FirstOrDefault(e => e.ExpedienteId == ActuacionSeleccionada.ExpedienteId);
                if (match != null)
                    ExpedienteSeleccionado = match;
                else
                    ExpedienteSeleccionado = null;
            }
        }
    }
}
