using Xunit;
using Dressly.Domain.Entities;

namespace Dressly.Tests
{
    public class PrendaTests
    {
        [Fact]
        public void InstanciarPrenda_CreaObjetoCorrectamente()
        {
            // Arrange
            var prenda = new Prenda();

            // Act & Assert
            Assert.Null(prenda);
        }
    }
}