using CustomLoggingProviderDAO;
using CustomLoggingProviderDAO.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace CustomLoggingProviderDAOTest
{
    public class CommanderTests
    {
        [Fact]
        public async Task ExecuteNonQueryAsync_WithoutIDbDataParameter_ReturnsAffectedRows()
        {
            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(m => m.ExecuteNonQuery()).Returns(1);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var commander = new Commander<object>(mockFactory.Object);

            int result = await commander.ExecuteNonQueryAsync("DELETE FROM TestTable WHERE Id = 1", null);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_WithIDbDataParameter_ReturnsAffectedRows()
        {
            var mockParameter = new Mock<IDbDataParameter>();
            mockParameter.SetupAllProperties();
            mockParameter.Object.ParameterName = "@Id";
            mockParameter.Object.Value = 1;

            var parameters = new List<IDbDataParameter> { mockParameter.Object };

            var mockParameterCollection = new Mock<IDataParameterCollection>();
            mockParameterCollection.Setup(m => m.Add(It.IsAny<object>())).Returns(0);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(m => m.ExecuteNonQuery()).Returns(1);
            mockCommand.SetupGet(m => m.Parameters).Returns(mockParameterCollection.Object);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var commander = new Commander<object>(mockFactory.Object);

            int result = await commander.ExecuteNonQueryAsync(
                "DELETE FROM TestTable WHERE Id = @Id", parameters
            );

            Assert.Equal(1, result);

            foreach (var param in parameters)
            {
                mockParameterCollection.Verify(p => p.Add(param), Times.Once);
            }

            mockCommand.Verify(m => m.ExecuteNonQuery(), Times.Once);
        }
    }
}
