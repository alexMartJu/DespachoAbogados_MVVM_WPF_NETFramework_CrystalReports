using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SistemaGestionDespacho.ViewModel.Views;

namespace SistemaGestionDespacho.View.Views
{
    /// <summary>
    /// Lógica de interacción para InformesView.xaml
    /// </summary>
    public partial class InformesView : UserControl
    {
        //InformesView() --> Constructor que inicializa el componente y se suscribe al evento DataContextChanged para detectar cambios en el DataContext y actualizar el visor de informes en consecuencia
        public InformesView()
        {
            InitializeComponent();
            this.DataContextChanged += InformesView_DataContextChanged;
        }

        //InformesView_DataContextChanged() --> Método que se ejecuta cuando cambia el DataContext del control. Si el nuevo DataContext es un InformesViewModel, se suscribe al evento PropertyChanged para detectar cambios en la propiedad InformeActual y actualizar el visor de informes
        private void InformesView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is InformesViewModel vm)
            {
                vm.PropertyChanged += Vm_PropertyChanged;
            }
        }

        //Vm_PropertyChanged() --> Método que se ejecuta cuando cambia una propiedad en el InformesViewModel. Si la propiedad que cambió es InformeActual, actualiza el ReportSource del visor de informes con el nuevo informe
        private void Vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(InformesViewModel.InformeActual))
            {
                var vm = (InformesViewModel)sender;
                reportViewer.ViewerCore.ReportSource = vm.InformeActual;
            }
        }
    }
}
