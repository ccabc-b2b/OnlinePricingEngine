using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;

namespace GCCB_OPE_FA_API.DataManagers
{
    public class ConnectionManager : IDisposable
    {
        private readonly Connection _connection;
        private readonly SqlConnection _sqlConnection;
        private readonly ILogger _logger;
        public ConnectionManager(Connection connection, ILogger<ConnectionManager> logger)
        {
            _connection = connection;
            _sqlConnection = new SqlConnection(_connection.ConnectionString);
            _logger = logger;
        }
        public DataTable ExecuteStoredProcedure(string procedureName, SqlParameter[] parameters = null)
        {
            var dataTable = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand(procedureName, _sqlConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    _sqlConnection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing stored procedure: {ex.Message}");
                throw;
            }
            finally
            {
                if (_sqlConnection.State != ConnectionState.Closed)
                {
                    _sqlConnection.Close();
                }
            }
            return dataTable;
        }
        public int ExecuteStoredProcedureNonQuery(string procedureName, SqlParameter[] parameters = null)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlCommand cmd = new SqlCommand(procedureName, _sqlConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    _sqlConnection.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing stored procedure: {ex.Message}");
                throw;
            }
            finally
            {
                if (_sqlConnection.State != ConnectionState.Closed)
                {
                    _sqlConnection.Close();
                }
            }
            return rowsAffected;
        }
        public void Dispose()
        {
            if (_sqlConnection != null)
            {
                if (_sqlConnection.State != ConnectionState.Closed)
                {
                    _sqlConnection.Close();
                }
                _sqlConnection.Dispose();
            }
        }
    }
    public class Connection
    {
        public string ConnectionString { get; set; }
    }
}
