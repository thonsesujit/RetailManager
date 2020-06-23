using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDataManager.Library.Internal.DataAccess
{
    //anything outside library should not have accesst to this methods. they need to access through UserData
    public class SqlDataAccess : IDisposable, ISqlDataAccess
    {
        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
        }


        public string GetConnectionString(string name)
        {

            return _config.GetConnectionString(name);

            //return @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TRMData;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            // Old way: return ConfigurationManager.ConnectionStrings[name].ConnectionString; // passing name of our connection string and returns connection string.  its gonna get he webconfig from TRMDdataManager
        }
        //Dapper is a microORM. object relational mapper. it allows us to talk to database. get information back and maps information to object. it very fast. U is generic. i can call it any letter.
        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                //FIXME: there is an error while getting sales data.
                List<T> rows = connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName) //in generic common type is T , then we use U V W ...
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);   // infuture we can make this execute into async

            }
        }


        //We need a open connect/start transation method
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        public void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            isClosed = false;
        }

        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {
            // transation :  parameter name with value. check with ctrl+shift+space
            List<T> rows = _connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();

            return rows;
        }

        //save using the transation
        public void SaveDataInTransaction<T>(string storedProcedure, T parameters) //in generic common type is T , then we use U V W ...
        {

            _connection.Execute(storedProcedure, parameters,
                commandType: CommandType.StoredProcedure, transaction: _transaction);   // associating transaction with the call.

        }

        private bool isClosed = false;
        private readonly IConfiguration _config;


        //close connection/stop transation method
        public void CommitTransaction()
        {

            _transaction?.Commit();

            _connection?.Close();
            isClosed = true;

        }


        public void RollbackTransaction()
        {
            _transaction?.Rollback(); //deletes everchanges thats been made
            _connection?.Close(); // close and dispose methods are the same.
            isClosed = true;

        }

        //from IDispose. 
        //Dispose. Ceanup code no matter what. 
        public void Dispose()
        {
            if (isClosed == false)
            {
                try
                {
                    CommitTransaction();
                }
                catch
                {

                    //TODO:Log this
                }
            }
            _transaction = null;
            _connection = null;
        }
        //load using the transation





    }

}
