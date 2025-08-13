using CustomLoggingProviderDAO.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CustomLoggingProviderDAO
{
    public class Commander<T>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public Commander(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> ExecuteNonQueryAsync(string sqlCommand, IEnumerable<IDbDataParameter> parameters)
        {
            using (IDbConnection conn = _connectionFactory.CreateConnection())
            {
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;
                    cmd.CommandType = CommandType.Text;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                            cmd.Parameters.Add(param);
                    }

                    conn.Open();
                    return await Task.Run(() => cmd.ExecuteNonQuery());
                }
            }
        }

        public async Task<Dictionary<string, object>> ExecuteNonQueryWithOutputAsync(string sqlCommand, IEnumerable<IDbDataParameter> parameters)
        {
            using (IDbConnection conn = _connectionFactory.CreateConnection())
            {
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;
                    cmd.CommandType = CommandType.Text;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }

                    conn.Open();

                    await Task.Run(() => cmd.ExecuteNonQuery());

                    var outputValues = new Dictionary<string, object>();
                    foreach (IDataParameter param in cmd.Parameters)
                    {
                        var directionProperty = param.GetType().GetProperty("Direction");
                        var direction = directionProperty != null ? (ParameterDirection)directionProperty.GetValue(param) : ParameterDirection.Input;

                        if (direction == ParameterDirection.Output ||
                            direction == ParameterDirection.InputOutput ||
                            direction == ParameterDirection.ReturnValue)
                        {
                            outputValues[param.ParameterName] = param.Value;
                        }
                    }

                    return outputValues;
                }
            }
        }


        public async Task<List<T>> ExecuteReaderAsync(string sqlCommand, Func<IDataReader, T> mapper, IEnumerable<IDbDataParameter> parameters = null)
        {
            var results = new List<T>();

            using (IDbConnection conn = _connectionFactory.CreateConnection())
            {
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;
                    cmd.CommandType = CommandType.Text;

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }

                    conn.Open();

                    using (IDataReader reader = await Task.Run(() => cmd.ExecuteReader()))
                    {
                        while (await Task.Run(() => reader.Read()))
                        {
                            results.Add(mapper(reader));
                        }
                    }

                    return results;
                }
            }
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
