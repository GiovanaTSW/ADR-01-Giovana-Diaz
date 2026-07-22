using Xunit;
using Dressly.Domain.Entities;

namespace Dressly.Tests
{
    public class UsuarioTests
    {
        [Fact]
        public void InstanciarUsuario_CreaObjetoCorrectamente()
        {
            // Arrange
            var usuario = new Usuario();

            // Act & Assert
            Assert.NotNull(usuario);
        }
    }
}