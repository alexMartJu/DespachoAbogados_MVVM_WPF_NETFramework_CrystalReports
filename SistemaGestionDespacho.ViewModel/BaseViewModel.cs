using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestionDespacho.ViewModel
{
    /// <summary>
    /// Clase base para todos los ViewModels 
    /// Implementa INotifyPropertyChanged para avisar a la vista cuando cambia una propiedad
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Evento que se dispara cuando una propiedad cambia su valor
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Método que lanza el evento de cambio de propiedad
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad que ha cambiado</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
