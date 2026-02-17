using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestionDespacho.Model.DataSets.Informes
{
    /// <summary>
    /// Helper que carga los datos necesarios para el informe de clientes y los coloca en un DataSet tipado.
    /// </summary>
    public class InformeClientesHelper
    {
        /// <summary>
        /// Carga los datos de clientes desde la base de datos y los inserta en un dsInformes.
        /// </summary>
        /// <returns>Un dsInformes con la tabla <c>ClientesListado</c> poblada.</returns>
        public dsInformes Cargar()
        {
            var ds = new dsInformes();

            using (var ctx = new SistemaGestionDespachoEntities())
            {
                var clientes = ctx.Clientes
                    .OrderBy(c => c.Nombre)
                    .ToList();

                foreach (var c in clientes)
                {
                    ds.ClientesListado.AddClientesListadoRow(
                        c.Nombre,
                        c.DNI_CIF,
                        c.Telefono ?? "",
                        c.Email ?? "",
                        c.FechaRegistro,
                        c.Activo
                    );
                }
            }

            return ds;
        }
    }
}
