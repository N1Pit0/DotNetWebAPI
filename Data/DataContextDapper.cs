using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotNetWebApp.Data
{
    public class DataContextDapper
    {
        private readonly IConfiguration _config;

        public DataContextDapper(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<T> LoadData<T>(string sql)
        {
            using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql);
        }
        
        public T LoadDataSingle<T>(string sql)
        {
            using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql);
        }

        public bool Execute(string sql)
        {
            using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql) > 0;
        }
        
        public int ExecuteSqlWithRowCount(string sql)
        {
            using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql);
        }

        public bool ExecuteSqlWithParameter(string sql, DynamicParameters parameters)
        {
            using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql,parameters) > 0;
            
            // using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            // using SqlCommand commandWithParams = new SqlCommand(sql, (SqlConnection)dbConnection);
            //
            // foreach (var parameter in parameters)
            // {
            //     commandWithParams.Parameters.Add(parameter);
            // }
            //
            // dbConnection.Open();
            //
            // int rowsAffected = commandWithParams.ExecuteNonQuery();
            //
            // return rowsAffected > 0;
        }
        
        public IEnumerable<T> LoadDataWithParameters<T>(string sql, DynamicParameters parameters)
        {
            using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql,parameters);
        }
        
        public T LoadDataSingleWithParameters<T>(string sql, DynamicParameters parameters)
        {
            using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql,parameters);
        }
        
    }
}