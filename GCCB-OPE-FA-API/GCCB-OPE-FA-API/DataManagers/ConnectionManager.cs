using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Returns the result of the stored procedure as a datatable.
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable ExecuteStoredProcedure(string procedureName, SqlParameter[] parameters = null)
            {
            var dataTable = new DataTable();
            try
                {
                using (SqlConnection con = new SqlConnection(_connection.ConnectionString))
                    {
                    using (SqlCommand cmd = new SqlCommand(procedureName, con))
                        {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                            {
                            cmd.Parameters.AddRange(parameters);
                            }
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                            dataTable.Load(reader);
                            }
                        con.Close();
                        }
                    }
                }
            catch (Exception ex)
                {
                _logger.LogError($"Error executing stored procedure: {ex.Message}");
                throw;
                }
            return dataTable;
            }
        /// <summary>
        /// Execute stored procedure to insert/update/delete the records.
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>


        public List<string> FetchMaterial(string customerId)
            {
            List<string> result = new List<string>();
            using (SqlConnection con = new SqlConnection(_connection.ConnectionString))
                {
                try
                    {
                    con.Open();
                    string commandText = @"select MaterialNumber FROM tbFilterSegments Where CustomerNumber = @customerNumber and ConditionType='YPR0'";

                    SqlCommand sqlCommand = new SqlCommand(commandText,con);
                    sqlCommand.Parameters.AddWithValue("@customerNumber", customerId);
                    var dataReader = sqlCommand.ExecuteReader();                   
                    while (dataReader.Read())
                        {
                        var materialNumber = dataReader["MaterialNumber"].ToString();
                        result.Add(materialNumber);
                        }
                    con.Close();
                    return result;
                    }
                catch (Exception ex)
                    {
                    _logger.LogInformation("Catalogue.FetchMaterial()"+ex.Message);
                    return null;
                    }
                }
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
        }    /// <summary>
    /// database connection string
    /// </summary>
    public class Connection
    {
        public string ConnectionString { get; set; }
    }
}
