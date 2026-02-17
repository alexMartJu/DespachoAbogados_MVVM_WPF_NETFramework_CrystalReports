using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CrystalDecisions.CrystalReports.Engine;
using SistemaGestionDespacho.Informes;
using SistemaGestionDespacho.Model.DataSets.Informes;

namespace SistemaGestionDespacho.ViewModel.Views
{
    public class InformesViewModel : BaseViewModel
    {
        private ReportDocument _informeActual;

        /// <summary>
        /// Informe actualmente cargado en el visor.
        /// </summary>
        public ReportDocument InformeActual
        {
            get => _informeActual;
            set
            {
                _informeActual = value;
                OnPropertyChanged(nameof(InformeActual));
            }
        }

        // ============================
        // COMANDOS
        // ============================

        public ICommand MostrarInformeClientesCommand { get; }
        public ICommand MostrarInformeExpedientesCommand { get; }
        public ICommand MostrarInformeActuacionesCommand { get; }

        // ============================
        // CONSTRUCTOR
        // ============================

        public InformesViewModel()
        {
            MostrarInformeClientesCommand = new RelayCommand(_ => CargarInformeClientes());
            MostrarInformeExpedientesCommand = new RelayCommand(_ => CargarInformeExpedientes());
            MostrarInformeActuacionesCommand = new RelayCommand(_ => CargarInformeActuaciones());
        }

        // ============================
        // MÉTODOS PRIVADOS
        // ============================

        //CargarInformeClientes() --> Carga el informe de clientes utilizando el helper correspondiente y lo asigna a InformeActual
        private void CargarInformeClientes()
        {
            var helper = new InformeClientesHelper();
            var ds = helper.Cargar();

            var rpt = new crClientesListado();
            rpt.SetDataSource(ds);

            InformeActual = rpt;
        }

        //CargarInformeExpedientes() --> Carga el informe de expedientes por estado utilizando el helper correspondiente y lo asigna a InformeActual
        private void CargarInformeExpedientes()
        {
            var helper = new InformeExpedientesPorEstadoHelper();
            var ds = helper.Cargar();

            var rpt = new crExpedientesPorEstado();
            rpt.SetDataSource(ds);

            InformeActual = rpt;
        }

        //CargarInformeActuaciones() --> Carga el informe de actuaciones por expediente utilizando el helper correspondiente y lo asigna a InformeActual
        private void CargarInformeActuaciones()
        {
            var helper = new InformeActuacionesHelper();
            var ds = helper.Cargar();

            var rpt = new crActuacionesPorExpediente();
            rpt.SetDataSource(ds);

            InformeActual = rpt;
        }
    }
}
