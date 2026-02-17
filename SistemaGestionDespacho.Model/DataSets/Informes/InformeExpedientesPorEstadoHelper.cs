using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestionDespacho.Model.DataSets.Informes
{
    /// <summary>
    /// Helper que carga los datos necesarios para el informe de expedientes por estado y los coloca en un DataSet tipado.
    /// </summary>
    public class InformeExpedientesPorEstadoHelper
    {
        /// <summary>
        /// Carga los datos de expedientes desde la base de datos y los inserta en un dsInformes.
        /// </summary>
        /// <returns>Un dsInformes con la tabla <c>ExpedientesPorEstado</c> poblada.</returns>
        public dsInformes Cargar()
        {
            var ds = new dsInformes();

            using (var ctx = new SistemaGestionDespachoEntities())
            {
                var expedientes = ctx.Expedientes
                    .OrderBy(e => e.EstadosExpediente.Nombre)
                    .ThenBy(e => e.Codigo)
                    .ToList();

                foreach (var e in expedientes)
                {
                    var row = ds.ExpedientesPorEstado.NewExpedientesPorEstadoRow();

                    row.Estado = e.EstadosExpediente.Nombre;
                    row.Codigo = e.Codigo;
                    row.Cliente = $"{e.Clientes.Nombre} {e.Clientes.Apellidos}";
                    row.Tipo = e.Tipo;
                    row.FechaApertura = e.FechaApertura;

                    if (e.FechaCierre.HasValue)
                        row.FechaCierre = e.FechaCierre.Value;
                    else
                        row.SetFechaCierreNull();

                    ds.ExpedientesPorEstado.AddExpedientesPorEstadoRow(row);
                }
            }

            return ds;
        }
    }
}
