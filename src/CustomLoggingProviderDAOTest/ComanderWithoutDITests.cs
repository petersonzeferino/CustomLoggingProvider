using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Xunit;

namespace CustomLoggingProviderDAO.Tests
{
    public class ComanderWithoutDITests
    {
        private readonly string _testConnectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=TestDB;Trusted_Connection=True;";

        [Fact]
        public async Task ExecuteNonQueryAsync_ShouldInsertRecord()
        {
            // Arrange
            var commander = new ComanderWithoutDI<object>(_testConnectionString);
            var parametros = ComanderWithoutDI<object>.CreateSqlParameters(new { Nome = "Maria", Idade = 25 });

            // Act
            var linhasAfetadas = await commander.ExecuteNonQueryAsync(
                "INSERT INTO Pessoas (Nome, Idade) VALUES (@Nome, @Idade)",
                parametros
            );

            // Assert
            Assert.Equal(1, linhasAfetadas);
        }

        [Fact]
        public async Task ExecuteReaderAsync_ShouldReturnInsertedRecord()
        {
            // Arrange
            var commander = new ComanderWithoutDI<Pessoa>(_testConnectionString);

            // Act
            var pessoas = await commander.ExecuteReaderAsync(
                "SELECT Nome, Idade FROM Pessoas",
                reader => new Pessoa
                {
                    Nome = reader["Nome"].ToString(),
                    Idade = Convert.ToInt32(reader["Idade"])
                }
            );

            // Assert
            Assert.NotEmpty(pessoas);
            Assert.Contains(pessoas, p => p.Nome == "Maria" && p.Idade == 25);
        }

        [Fact]
        public async Task ExecuteNonQueryWithOutputAsync_ShouldReturnOutputParameter()
        {
            // Arrange
            var commander = new ComanderWithoutDI<object>(_testConnectionString);

            var parametros = new List<IDbDataParameter>
            {
                new SqlParameter("@Total", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                }
            };

            string sql = "SELECT @Total = COUNT(*) FROM Pessoas";

            // Act
            var output = await commander.ExecuteNonQueryWithOutputAsync(sql, parametros);

            // Assert
            Assert.True(output.ContainsKey("@Total"));
            Assert.True(Convert.ToInt32(output["@Total"]) >= 1);
        }
    }

    public class Pessoa
    {
        public string Nome { get; set; }
        public int Idade { get; set; }
    }
}
