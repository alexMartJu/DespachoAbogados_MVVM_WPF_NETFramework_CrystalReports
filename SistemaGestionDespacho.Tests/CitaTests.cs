using SistemaGestionDespacho.Model;
using SistemaGestionDespacho.Model.Services;

namespace SistemaGestionDespacho.Tests;

[TestClass]
/// <summary>
/// Prueba de integración relacionada con la creación de citas y detección de conflictos de horario.
/// </summary>
public class CitaTests
{
    /// <summary>
    /// Prueba de integración: crea un cliente + una cita en BD y verifica que intentar crear
    /// otra cita en la misma fecha/hora lanza la excepción de conflicto.
    /// </summary>
    [TestMethod]
    public void CrearCita_MismaFechaHora_DebeFallarPorConflicto()
    {
        var clienteService = new ClienteService();
        var citaService = new CitaService();

        Clientes cliente = null;
        Citas c1 = null;

        try
        {
            //Crear cliente válido
            cliente = new Clientes
            {
                Nombre = "Test",
                Apellidos = "Integracion",
                DNI_CIF = "00000000A",
                Direccion = "Calle Prueba 1",
                Email = "test_integ@test.com",
                Telefono = "612345678",
                Activo = true
            };
            clienteService.Crear(cliente);

            //Fecha futura para evitar conflicto con datos existentes
            var fecha = DateTime.Today.AddDays(1).AddHours(10);

            //Primera cita: debe crearse correctamente
            c1 = new Citas
            {
                ClienteId = cliente.ClienteId,
                FechaHora = fecha,
                Lugar = "Despacho",
                Motivo = "Prueba integración",
                Estado = "Pendiente"
            };
            citaService.Crear(c1);

            //Segunda cita en la misma fecha/hora -> debe lanzar excepción por conflicto
            var c2 = new Citas
            {
                ClienteId = cliente.ClienteId,
                FechaHora = fecha,
                Lugar = "Despacho",
                Motivo = "Prueba conflicto",
                Estado = "Pendiente"
            };

            var ex = Assert.ThrowsException<Exception>(() => citaService.Crear(c2));
            StringAssert.Contains(ex.Message, "Ya existe una cita en la misma fecha y hora");
        }
        finally
        {
            //Limpieza: cancelar la cita creada y desactivar el cliente
            try { if (c1 != null) citaService.EliminarPermanente(c1); } catch { /* intentar limpiar, ignorar errores */ }
            try { if (cliente != null) clienteService.EliminarPermanente(cliente); } catch { /* ignorar */ }
        }
    }
}
