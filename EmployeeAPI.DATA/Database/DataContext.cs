using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.DATA.Database
{
    public sealed class DataContext
    {
        private bool withSession;

        #region Declaration & Constructor
        static DataContext dataContex = null;
        //static readonly object syncObj = new object();
        private DataContext()
        {
            withSession = false;
        }
        #endregion

        #region Properties

        private DbCommand command;
        public DbCommand Command
        {
            get { return command; }
        }
        private DbConnection connection;
        public DbConnection Connection
        {
            get { return connection; }
        }
        private IDataReader reader;
        public IDataReader Reader
        {
            get { return reader; }
        }
        private DbTransaction transaction;
        public DbTransaction Transaction
        {
            get { return transaction; }
        }
        private static DbProviderFactory factory;
        public DbProviderFactory Factory
        {
            get { return factory; }
        }
        private static ConnectionStringSettings settings;
        public ConnectionStringSettings Settings
        {
            get { return settings; }
        }

        #endregion

        #region DB Tran Method
        public static DataContext GetDataContex()
        {

            dataContex = new DataContext();
            try
            {

                settings = ConfigurationManager.ConnectionStrings["DBConn"];
                factory = DbProviderFactories.GetFactory(settings.ProviderName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }



            return dataContex;

        }

        public void Begin(bool session)
        {
            withSession = session;

            PrepareConnection();
        }

        private DbCommand GetCommand()
        {
            if (connection == null)
                PrepareConnection();

            command = factory.CreateCommand();
            command.Connection = connection;
            command.CommandTimeout = 300;
            if (withSession)
            {
                if (transaction == null)
                    transaction = connection.BeginTransaction();

                command.Transaction = transaction;

            }

            return command;
        }

        private void PrepareConnection()
        {
            connection = factory.CreateConnection();
            connection.ConnectionString = settings.ConnectionString;
            connection.Open();

        }

        public void End()
        {

            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
            }
            if (transaction != null)
            {
                transaction.Commit();
                transaction.Dispose();
            }
            if (connection != null)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                connection.Dispose();
            }

            connection = null;
            transaction = null;

            GC.Collect();

        }
        public void Rollback()
        {

            if (transaction != null)
            {
                transaction.Rollback();
                transaction.Dispose();
            }
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
            }
            if (connection != null)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                connection.Dispose();
            }

            connection = null;
            transaction = null;

            GC.Collect();

        }


        public IDataReader ExecuteReader(string spName, object[] parameterValues)
        {
            command = GetCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;
            if (parameterValues != null)
            {
                for (int i = 0; i < parameterValues.Length; i++)
                {
                    command.Parameters.Add(parameterValues[i]);
                }
            }
            reader = command.ExecuteReader();
            if (parameterValues != null)
                command.Parameters.Clear();
            return reader;
        }

        public IDataReader ExecuteReader(CommandType commandType, string commandText, object[] parameterValues)
        {
            command = GetCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;
            if (parameterValues != null)
            {
                for (int i = 0; i < parameterValues.Length; i++)
                {
                    command.Parameters.Add(parameterValues[i]);
                }
            }
            reader = command.ExecuteReader();
            if (parameterValues != null)
                command.Parameters.Clear();
            return reader;
        }

        public object ExecuteScalar(CommandType commandType, string commandText, object[] parameterValues)
        {
            command = GetCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;

            if (parameterValues != null)
            {
                for (int i = 0; i < parameterValues.Length; i++)
                {
                    command.Parameters.Add(parameterValues[i]);
                }
            }

            object obj = command.ExecuteScalar();
            if (parameterValues != null)
                command.Parameters.Clear();

            return obj;
        }


        public int ExecuteNonQuery(string spName, object[] parameterValues)
        {
            command = GetCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;
            if (parameterValues != null)
            {
                for (int i = 0; i < parameterValues.Length; i++)
                {
                    command.Parameters.Add(parameterValues[i]);
                }
            }
            int count = command.ExecuteNonQuery();
            if (parameterValues != null)
                command.Parameters.Clear();
            return count;
        }

        public int ExecuteNonQuery(string spName, DbParameter[] dbParameters)
        {
            command = GetCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;
            if (dbParameters != null)
            {
                foreach (DbParameter dbParameter in dbParameters)
                {
                    command.Parameters.Add(dbParameter);
                }
            }
            int count = command.ExecuteNonQuery();
            if (dbParameters != null)
                command.Parameters.Clear();
            return count;

        }

        public int ExecuteNonQuery(string commandText)
        {
            command = GetCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = commandText;
            return command.ExecuteNonQuery();

        }

        public DataSet GetDataSet(string spName, object[] dbParameters)
        {
            DataSet ds = new DataSet();
            DbDataAdapter dbDataAdapter = factory.CreateDataAdapter();
            command = GetCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;
            if (dbParameters != null)
            {
                for (int i = 0; i < dbParameters.Length; i++)
                {
                    command.Parameters.Add(dbParameters[i]);
                }
            }
            command.ExecuteNonQuery();
            dbDataAdapter.SelectCommand = command;
            dbDataAdapter.Fill(ds);
            command.Parameters.Clear();
            return ds;

        }
        public DataSet GetDataSet(CommandType commandType, params string[] commandTexts)
        {
            DataSet ds = new DataSet();
            DbDataAdapter dbDataAdapter = factory.CreateDataAdapter();
            command = GetCommand();
            command.CommandType = commandType;
            int index = 0;
            string tableName = "";
            foreach (string commandText in commandTexts)
            {
                tableName = "Table" + index;
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                dbDataAdapter.SelectCommand = command;
                dbDataAdapter.Fill(ds, tableName);
                index++;
            }

            return ds;

        }
        public DataSet GetDataSet(CommandType commandType, string spnameorquerystring, object[] dbParameters)
        {
            DataSet ds = new DataSet();
            DbDataAdapter dbDataAdapter = factory.CreateDataAdapter();
            command = GetCommand();
            command.CommandType = commandType;
            command.CommandText = spnameorquerystring;
            if (dbParameters != null)
            {
                for (int i = 0; i < dbParameters.Length; i++)
                {
                    command.Parameters.Add(dbParameters[i]);
                }
            }
            command.ExecuteNonQuery();
            dbDataAdapter.SelectCommand = command;
            dbDataAdapter.Fill(ds);
            command.Parameters.Clear();
            return ds;

        }

        public static DbParameter CreateParameter(string name, DbType type, object value)
        {
            DbParameter dbParameter = factory.CreateParameter();
            dbParameter.ParameterName = name;
            dbParameter.DbType = type;
            dbParameter.Value = value;
            return dbParameter;

        }

        public static DbParameter[] CreateParameters(ushort size)
        {
            DbParameter[] dbParameters = new DbParameter[size];
            for (int i = 0; i < size; i++)
            {
                dbParameters[i] = factory.CreateParameter();
            }

            return dbParameters;

        }


        #endregion

        #region common

        public DataSet GetDataSetByQuery(string query)
        {
            DataSet ds;
            string q = "";

            q = query;

            DataContext dc = DataContext.GetDataContex();
            try
            {
                dc.Begin(false);
                ds = dc.GetDataSet(CommandType.Text, q);
                dc.End();
            }
            catch (Exception ex)
            {
                dc.Rollback();
                throw new Exception(ex.Message, ex);
            }
            if (ds != null)
                return ds;
            else
                return null;
        }

        public DataTable GetDataTableByQuery(string query)
        {
            DataSet ds;
            string q = "";

            q = query;

            DataContext dc = DataContext.GetDataContex();
            try
            {
                dc.Begin(false);
                ds = dc.GetDataSet(CommandType.Text, q);
                dc.End();
            }
            catch (Exception ex)
            {
                dc.Rollback();
                throw new Exception(ex.Message, ex);
            }
            if (ds != null)
                return ds.Tables[0];
            else
                return null;
        }

        public int CRUDOperationsByQuery(string query)
        {

            string q = "";
            int retVal = 0;
            q = query;

            DataContext dc = DataContext.GetDataContex();
            try
            {
                dc.Begin(false);
                retVal = dc.ExecuteNonQuery(q);
                dc.End();

            }
            catch (Exception ex)
            {
                dc.Rollback();
                throw new Exception(ex.Message, ex);
            }
            return retVal;

        }
        #endregion

    }
}
