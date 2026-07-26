using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace GymManager.Data
{
  public  class DataBase
    {
        private readonly string connectionString =
           @"Server=localhost\SQLEXPRESS;
              Database=GymManager;
              Trusted_Connection=True;
              TrustServerCertificate=True;";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
