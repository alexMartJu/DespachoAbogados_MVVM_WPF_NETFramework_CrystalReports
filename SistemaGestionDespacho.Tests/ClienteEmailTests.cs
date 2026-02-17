using SistemaGestionDespacho.Model.Services;

namespace SistemaGestionDespacho.Tests;

[TestClass]
/// <summary>
/// Pruebas unitarias para la validación de direcciones de email de clientes.
/// </summary>
public class ClienteEmailTests
{
    /// <summary>
    /// Comprueba que un email con formato correcto devuelve true.
    /// </summary>
    [TestMethod]
    public void EsEmailValido_EmailCorrecto_DebeRetornarTrue()
    {
        var service = new ClienteService();
        var email = "usuario@dominio.com";

        // Act
        var resultado = service.EsEmailValido(email);

        // Assert
        Assert.IsTrue(resultado);
    }

    /// <summary>
    /// Comprueba que un email incorrecto devuelve false.
    /// </summary>
    [TestMethod]
    public void EsEmailValido_EmailIncorrecto_DebeRetornarFalse()
    {
        var service = new ClienteService();
        var email = "usuario.com";

        var resultado = service.EsEmailValido(email);

        Assert.IsFalse(resultado);
    }
}
