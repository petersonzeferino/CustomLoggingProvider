using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CustomLoggingProviderDAO
{
    public class ComanderWithoutDI<T>
    {
        private readonly string _connectionString;

        public ComanderWithoutDI(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string não pode ser nula ou vazia.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public async Task<int> ExecuteNonQueryAsync(string sqlCommand, IEnumerable<IDbDataParameter> parameters)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlCommand;
                cmd.CommandType = CommandType.Text;

                if (parameters != null)
                {
                    foreach (var param in parameters)
                        cmd.Parameters.Add(param);
                }

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<Dictionary<string, object>> ExecuteNonQueryWithOutputAsync(string sqlCommand, IEnumerable<IDbDataParameter> parameters)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlCommand;
                cmd.CommandType = CommandType.Text;

                if (parameters != null)
                {
                    foreach (var param in parameters)
                        cmd.Parameters.Add(param);
                }

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                var outputValues = new Dictionary<string, object>();
                foreach (IDataParameter param in cmd.Parameters)
                {
                    if (param.Direction == ParameterDirection.Output ||
                        param.Direction == ParameterDirection.InputOutput ||
                        param.Direction == ParameterDirection.ReturnValue)
                    {
                        outputValues[param.ParameterName] = param.Value;
                    }
                }

                return outputValues;
            }
        }

        public async Task<List<T>> ExecuteReaderAsync(string sqlCommand, Func<IDataReader, T> mapper, IEnumerable<IDbDataParameter> parameters = null)
        {
            var results = new List<T>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlCommand;
                cmd.CommandType = CommandType.Text;

                if (parameters != null)
                {
                    foreach (var param in parameters)
                        cmd.Parameters.Add(param);
                }

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(mapper(reader));
                    }
                }
            }

            return results;
        }

        public static List<IDbDataParameter> CreateSqlParameters<TObj>(TObj obj)
        {
            var parameters = new List<IDbDataParameter>();
            if (obj == null)
                return parameters;

            var properties = typeof(TObj).GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(obj) ?? DBNull.Value;
                var paramName = "@" + prop.Name;
                var parameter = new SqlParameter(paramName, value);
                parameters.Add(parameter);
            }

            return parameters;
        }
    }
}
