using SistemaGestionDespacho.Model.Services;

namespace SistemaGestionDespacho.Tests;

[TestClass]
/// <summary>
/// Pruebas unitarias para la validación de números de teléfono de clientes.
/// </summary>
public class ClienteTelefonoTests
{
    /// <summary>
    /// Comprueba que un teléfono con prefijo internacional y separadores se considera válido.
    /// </summary>
    [TestMethod]
    public void EsTelefonoValido_TelefonoConPrefijoYSeparadores_DebeRetornarTrue()
    {
        var service = new ClienteService();
        var telefono = "+34 612-345-678";

        var resultado = service.EsTelefonoValido(telefono);

        Assert.IsTrue(resultado);
    }

    /// <summary>
    /// Comprueba que un teléfono demasiado corto se considera inválido.
    /// </summary>
    [TestMethod]
    public void EsTelefonoValido_TelefonoDemasiadoCorto_DebeRetornarFalse()
    {
        var service = new ClienteService();
        var telefono = "12345";

        var resultado = service.EsTelefonoValido(telefono);

        Assert.IsFalse(resultado);
    }
}
