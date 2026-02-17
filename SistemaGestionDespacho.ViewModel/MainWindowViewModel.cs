using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SistemaGestionDespacho.ViewModel.Views;

namespace SistemaGestionDespacho.ViewModel
{
    /// <summary>
    /// ViewModel de la ventana principal, se encarga de la navegación entre vistas
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        public static MainWindowViewModel Instance { get; private set; }


        private BaseViewModel _vistaActual;

        /// <summary>
        /// Vista actual que se muestra en el contenedor principal
        /// </summary>
        public BaseViewModel VistaActual
        {
            get => _vistaActual;
            set
            {
                _vistaActual = value;
                OnPropertyChanged(nameof(VistaActual));
                ActualizarTitulo();
            }
        }

        private string _titulo;

        /// <summary>
        /// Título dinámico de la ventana que cambia según la vista actual
        /// </summary>
        public string Titulo
        {
            get => _titulo;
            set
            {
                _titulo = value;
                OnPropertyChanged(nameof(Titulo));
            }
        }

        // ============================
        // PROPIEDADES PARA MARCAR EL MENÚ
        // ============================

        private bool _esClientes;
        public bool EsClientes
        {
            get => _esClientes;
            set { 
                _esClientes = value; 
                OnPropertyChanged(nameof(EsClientes)); 
            }
        }

        private bool _esExpedientes;
        public bool EsExpedientes
        {
            get => _esExpedientes;
            set { 
                _esExpedientes = value; 
                OnPropertyChanged(nameof(EsExpedientes)); 
            }
        }

        private bool _esActuaciones;
        public bool EsActuaciones
        {
            get => _esActuaciones;
            set { 
                _esActuaciones = value; 
                OnPropertyChanged(nameof(EsActuaciones)); 
            }
        }

        private bool _esCitas;
        public bool EsCitas
        {
            get => _esCitas;
            set { 
                _esCitas = value; 
                OnPropertyChanged(nameof(EsCitas)); 
            }
        }

        private bool _esInformes;
        public bool EsInformes
        {
            get => _esInformes;
            set
            {
                _esInformes = value;
                OnPropertyChanged(nameof(EsInformes));
            }
        }

        private void SeleccionarMenu(string vista)
        {
            EsClientes = vista == "Clientes";
            EsExpedientes = vista == "Expedientes";
            EsActuaciones = vista == "Actuaciones";
            EsCitas = vista == "Citas";
            EsInformes = vista == "Informes";
        }

        // Comandos de navegación
        public ICommand MostrarClientesCommand { get; }
        public ICommand MostrarExpedientesCommand { get; }
        public ICommand MostrarActuacionesCommand { get; }
        public ICommand MostrarCitasCommand { get; }
        public ICommand MostrarInformesCommand { get; }

        /// <summary>
        /// Constructor que inicializa la vista por defecto y los comandos de navegación
        /// </summary>
        public MainWindowViewModel()
        {
            Instance = this;

            //Vista inicial
            VistaActual = new ClientesViewModel();
            SeleccionarMenu("Clientes");
            Titulo = "Gestión de Clientes";

            //Comandos
            MostrarClientesCommand = new RelayCommand(_ =>
            {
                VistaActual = new ClientesViewModel();
                SeleccionarMenu("Clientes");
            });

            MostrarExpedientesCommand = new RelayCommand(_ =>
            {
                VistaActual = new ExpedientesViewModel();
                SeleccionarMenu("Expedientes");
            });

            MostrarActuacionesCommand = new RelayCommand(_ =>
            {
                VistaActual = new ActuacionesViewModel();
                SeleccionarMenu("Actuaciones");
            });

            MostrarCitasCommand = new RelayCommand(_ =>
            {
                VistaActual = new CitasViewModel();
                SeleccionarMenu("Citas");
            });

            MostrarInformesCommand = new RelayCommand(_ =>
            {
                VistaActual = new InformesViewModel();
                SeleccionarMenu("Informes");
            });
        }

        /// <summary>
        /// Abre la vista de Actuaciones mostrando las actuaciones del expediente indicado.
        /// </summary>
        /// <param name="expedienteId">Identificador del expediente cuyas actuaciones se mostrarán.</param>
        public void AbrirActuaciones(int expedienteId)
        {
            VistaActual = new ActuacionesViewModel(expedienteId);
            SeleccionarMenu("Actuaciones");
        }

        /// <summary>
        /// Abre la vista de Citas mostrando las citas del expediente indicado.
        /// </summary>
        /// <param name="expedienteId">Identificador del expediente cuyas citas se mostrarán.</param>
        public void AbrirCitas(int expedienteId)
        {
            VistaActual = new CitasViewModel(expedienteId);
            SeleccionarMenu("Citas");
        }

        //ActualizarTitulo --> Actualiza el título según la vista actual
        private void ActualizarTitulo()
        {
            if (VistaActual is ClientesViewModel)
                Titulo = "Gestión de Clientes";

            else if (VistaActual is ExpedientesViewModel)
                Titulo = "Gestión de Expedientes";

            else if (VistaActual is ActuacionesViewModel)
                Titulo = "Actuaciones del Expediente";

            else if (VistaActual is CitasViewModel)
                Titulo = "Agenda de Citas";

            else if (VistaActual is InformesViewModel)
                Titulo = "Informes del Despacho";
        }
    }
}
