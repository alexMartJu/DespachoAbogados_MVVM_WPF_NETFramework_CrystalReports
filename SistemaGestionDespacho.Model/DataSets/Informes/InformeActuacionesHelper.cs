using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestionDespacho.Model.DataSets.Informes
{
    /// <summary>
    /// Helper que carga los datos necesarios para el informe de actuaciones por expediente y los coloca en un DataSet tipado.
    /// </summary>
    public class InformeActuacionesHelper
    {
        /// <summary>
        /// Carga los datos de actuaciones desde la base de datos y los inserta en un dsInformes.
        /// </summary>
        /// <returns>Un dsInformes con la tabla <c>ActuacionesPorExpediente</c> poblada.</returns>
        public dsInformes Cargar()
        {
            var ds = new dsInformes();

            using (var ctx = new SistemaGestionDespachoEntities())
            {
                var actuaciones = ctx.Actuaciones
                    .OrderBy(a => a.Expedientes.Codigo)
                    .ThenBy(a => a.Fecha)
                    .ToList();

                foreach (var a in actuaciones)
                {
                    ds.ActuacionesPorExpediente.AddActuacionesPorExpedienteRow(
                        a.Expedientes.Codigo,
                        a.Fecha,
                        a.Tipo,
                        a.Descripcion
                    );
                }
            }

            return ds;
        }
    }
}
