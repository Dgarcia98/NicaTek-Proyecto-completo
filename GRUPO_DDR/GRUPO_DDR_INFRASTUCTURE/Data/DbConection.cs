using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace GRUPO_DDR_INFRASTUCTURE.Data
{
    public class DbConection
    {
        private readonly string _connectionString;

        public DbConection(string connectionString)
        {
            _connectionString = connectionString;
        }
        public SqlConnection CreateConection()
        {
            return new SqlConnection(_connectionString);
        }


    }
}
