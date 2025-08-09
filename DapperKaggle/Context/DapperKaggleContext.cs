using System.Data;
using Microsoft.Data.SqlClient;

namespace DapperKaggle.Context
{
    public class DapperKaggleContext
    {
        private readonly IConfiguration _configuration;

        private readonly string _connectionString;
        public DapperKaggleContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("connection");
        }
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
