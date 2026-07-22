using Xunit;
using Dressly.Infrastructure.Repositories;

namespace Dressly.Tests
{
    public class SqlitePrendaRepositoryTests
    {
        [Fact]
        public void InstanciarRepositorioPrenda_NoEsNulo()
        {
            // Arrange & Act
            var repo = new SqlitePrendaRepository(null);

            // Assert
            Assert.NotNull(repo);
        }
    }
}