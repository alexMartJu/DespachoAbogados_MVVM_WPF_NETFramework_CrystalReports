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
    /// ViewModel para la gestión de clientes del despacho.
    /// Controla el CRUD, filtros y validaciones.
    /// </summary>
    public class ClientesViewModel : BaseViewModel
    {
        private readonly ClienteService _clienteService;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ClientesViewModel"/> y carga la lista de clientes.
        /// También inicializa los comandos del ViewModel.
        /// </summary>
        public ClientesViewModel()
        {
            _clienteService = new ClienteService();

            Clientes = new ObservableCollection<Clientes>(_clienteService.Buscar(null, null, false));

            //Solo permitir crear si el checkbox Activo está marcado
            CrearCommand = new RelayCommand(_ => CrearCliente(), _ => Activo);
            //Solo permitir editar o eliminar si hay un cliente seleccionado
            EditarCommand = new RelayCommand(_ => EditarCliente(), _ => ClienteSeleccionado != null);
            EliminarCommand = new RelayCommand(_ => EliminarCliente(), _ => ClienteSeleccionado != null);
            LimpiarCommand = new RelayCommand(_ => LimpiarFormulario());
            BuscarCommand = new RelayCommand(_ => BuscarClientes());
        }

        // ============================
        // LISTADO Y SELECCIÓN
        // ============================

        //Clientes --> Lista de clientes mostrada en la vista. Se actualiza al buscar o modificar clientes.
        private ObservableCollection<Clientes> _clientes;
        public ObservableCollection<Clientes> Clientes
        {
            get => _clientes;
            set
            {
                _clientes = value;
                OnPropertyChanged(nameof(Clientes));
            }
        }

        //ClienteSeleccionado --> Cliente actualmente seleccionado en la vista. Al cambiar, carga sus datos en el formulario para edición.
        private Clientes _clienteSeleccionado;
        public Clientes ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set
            {
                _clienteSeleccionado = value;
                MensajeError = string.Empty;
                OnPropertyChanged(nameof(ClienteSeleccionado));
                CargarClienteEnFormulario();
            }
        }

        // ============================
        // CAMPOS DEL FORMULARIO (CORREGIDOS)
        // ============================

        private string _nombre;
        public string Nombre
        {
            get => _nombre;
            set
            {
                _nombre = value;
                OnPropertyChanged(nameof(Nombre));
            }
        }

        private string _apellidos;
        public string Apellidos
        {
            get => _apellidos;
            set
            {
                _apellidos = value;
                OnPropertyChanged(nameof(Apellidos));
            }
        }

        private string _dniCif;
        public string DNI_CIF
        {
            get => _dniCif;
            set
            {
                _dniCif = value;
                OnPropertyChanged(nameof(DNI_CIF));
            }
        }

        private string _telefono;
        public string Telefono
        {
            get => _telefono;
            set
            {
                _telefono = value;
                OnPropertyChanged(nameof(Telefono));
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _direccion;
        public string Direccion
        {
            get => _direccion;
            set
            {
                _direccion = value;
                OnPropertyChanged(nameof(Direccion));
            }
        }

        private bool _activo = true;
        public bool Activo
        {
            get => _activo;
            set
            {
                _activo = value;
                OnPropertyChanged(nameof(Activo));
                //Forzar reevaluación de CanExecute en los comandos (especialmente CrearCommand)
                CommandManager.InvalidateRequerySuggested();
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

        private string _filtroNombre;
        public string FiltroNombre
        {
            get => _filtroNombre;
            set
            {
                _filtroNombre = value;
                OnPropertyChanged(nameof(FiltroNombre));
            }
        }

        private string _filtroDNI;
        public string FiltroDNI
        {
            get => _filtroDNI;
            set
            {
                _filtroDNI = value;
                OnPropertyChanged(nameof(FiltroDNI));
            }
        }

        private bool _filtroActivos;
        public bool FiltroActivos
        {
            get => _filtroActivos;
            set
            {
                _filtroActivos = value;
                OnPropertyChanged(nameof(FiltroActivos));
            }
        }

        // ============================
        // COMANDOS
        // ============================

        public ICommand CrearCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand LimpiarCommand { get; }
        public ICommand BuscarCommand { get; }

        // ============================
        // MÉTODOS PRINCIPALES
        // ============================

        //CrearCliente() --> Crea un nuevo cliente con los datos del formulario. Valida los datos antes de crear. Si la creación es exitosa, limpia el formulario y actualiza la lista de clientes. Si hay un error, muestra el mensaje de error.
        private void CrearCliente()
        {
            try
            {
                var nuevo = new Clientes
                {
                    Nombre = Nombre,
                    Apellidos = Apellidos,
                    DNI_CIF = DNI_CIF,
                    Telefono = Telefono,
                    Email = Email,
                    Direccion = Direccion,
                    Activo = Activo
                };

                _clienteService.Crear(nuevo);

                Clientes.Add(nuevo);
                LimpiarFormulario();
                MensajeError = "";
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        //EditarCliente() --> Edita el cliente seleccionado con los datos del formulario. Valida los datos antes de editar. Si la edición es exitosa, actualiza la lista de clientes. Si hay un error, muestra el mensaje de error.
        private void EditarCliente()
        {
            if (ClienteSeleccionado == null)
                return;

            try
            {
                ClienteSeleccionado.Nombre = Nombre;
                ClienteSeleccionado.Apellidos = Apellidos;
                ClienteSeleccionado.DNI_CIF = DNI_CIF;
                ClienteSeleccionado.Telefono = Telefono;
                ClienteSeleccionado.Email = Email;
                ClienteSeleccionado.Direccion = Direccion;
                ClienteSeleccionado.Activo = Activo;

                _clienteService.Editar(ClienteSeleccionado);

                BuscarClientes();
                LimpiarFormulario();
                MensajeError = "";
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        //EliminarCliente() --> Desactiva el cliente seleccionado (no lo elimina físicamente). Muestra una confirmación antes de desactivar. Si la desactivación es exitosa, actualiza la lista de clientes. Si hay un error, muestra el mensaje de error.
        private void EliminarCliente()
        {
            if (ClienteSeleccionado == null)
                return;

            try
            {
                //Confirmación antes de desactivar
                var result = MessageBox.Show("¿Está seguro que desea desactivar al cliente seleccionado?", "Confirmar desactivación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                    return;

                _clienteService.Eliminar(ClienteSeleccionado);
                BuscarClientes();
                LimpiarFormulario();
                MensajeError = "";
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        //BuscarClientes() --> Busca clientes según los filtros de nombre, DNI/CIF y solo activos. Actualiza la lista de clientes mostrada en la vista.
        private void BuscarClientes()
        {
            var lista = _clienteService.Buscar(FiltroNombre, FiltroDNI, FiltroActivos);
            Clientes = new ObservableCollection<Clientes>(lista);
        }

        //LimpiarFormulario() --> Limpia los campos del formulario y deselecciona el cliente seleccionado. También limpia el mensaje de error.
        private void LimpiarFormulario()
        {
            Nombre = "";
            Apellidos = "";
            DNI_CIF = "";
            Telefono = "";
            Email = "";
            Direccion = "";
            Activo = true;
            MensajeError = "";
            ClienteSeleccionado = null;
        }

        //CargarClienteEnFormulario() --> Carga los datos del cliente seleccionado en los campos del formulario para su edición. Si no hay cliente seleccionado, no hace nada.
        private void CargarClienteEnFormulario()
        {
            if (ClienteSeleccionado == null)
                return;

            Nombre = ClienteSeleccionado.Nombre;
            Apellidos = ClienteSeleccionado.Apellidos;
            DNI_CIF = ClienteSeleccionado.DNI_CIF;
            Telefono = ClienteSeleccionado.Telefono;
            Email = ClienteSeleccionado.Email;
            Direccion = ClienteSeleccionado.Direccion;
            Activo = ClienteSeleccionado.Activo;
        }
    }
}
