using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SALTPages.Pages
{
    public class DataBaseConnection
    {
        string connectionString = null;
        private static readonly log4net.ILog log =
  log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        string actualDate;
        SqlConnection connection;

        SqlDataReader rdr = null;

        /// <summary>
        /// To connect database
        /// Search order number in database
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="databaseName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="orderID"></param>
        public void ConnectionToDatabase()
        {
            string env = ConfigurationManager.AppSettings["Environment"];
            string DBServerName = ConfigurationManager.AppSettings[env + "_" + "DBServerName"];
            string DatabaseName = ConfigurationManager.AppSettings[env + "_" + "DatabaseName"];
            string UserName = ConfigurationManager.AppSettings[env + "_" + "DBUserName"];
            string Password = ConfigurationManager.AppSettings[env + "_" + "DBPassword"];
            connectionString = "Data Source=" + DBServerName + "; Initial Catalog=" + DatabaseName + "; User ID=" + UserName + "; Password=" + Password;
            connection = new SqlConnection(connectionString);
        }



        /// <summary>
        /// To verify actual date with expected date
        /// </summary>
        /// <param name="expectedDate"></param>
        public Boolean VerifyActualDateAndExpectedDate(string expectedDate)
        {
            bool result = false;

            while (rdr.Read())
            {
                actualDate = rdr[4].ToString();

                if (actualDate.Contains(expectedDate))
                {
                    result = true;
                }
            }

            // close the reader
            if (rdr != null)
            {
                rdr.Close();
            }

            // 5. Close the connection
            if (connection != null)
            {
                connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Function to reduce order placed date by one day
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>bool</returns>
        public bool ChangeOrderDateToOneDayBack(string orderId, string NewDate)
        {
            ConnectionToDatabase();
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("update [SALTAPP].[dbo].[Order] set Created =DATEADD(dd, " + NewDate + ", Created) where ReferenceNumber='" + orderId + "'", connection);
                rdr = cmd.ExecuteReader();
                connection.Close();
                return true;

            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }



        /// <summary>
        /// This method will execute a single query with parameters
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool ExecuteSingleQuery(string query, SqlParameter param = null)
        {
            ConnectionToDatabase();
            SqlCommand cmd = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                if (param != null)
                {
                    cmd.Parameters.Add(param);
                }
                cmd.CommandTimeout = 6000;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                cmd.Parameters.Clear();
                connection.Close();
            }
        }

        /// <summary>
        /// To change status of the order
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="orderId"></param>
        public void ChangeTheStatus(string ID, string orderId, string status)
        {

            int statusCode = status == "Pending Packing Vault" ? 1 : status == "Completed" ? 2 : 0;

            List<string> queries = new List<string>()
            {
               "update [order] set status = '" + ID + "' where referencenumber = '" + orderId + "'",
             "update BasketItem set Status = "+statusCode+" where orderid = (select id from [order] where ReferenceNumber='" + orderId + "')",
           "insert into StatusHistory(OrderId, Status, Timestamp, Username) values((select id from [order] where ReferenceNumber='" + orderId + "'), " + ID + ", GETDATE(), NULL)"
        };

            foreach (string item in queries)
            {
                ConnectionToDatabase();
                connection.Open();
                SqlCommand cmd = new SqlCommand(item, connection);
                rdr = cmd.ExecuteReader();
                connection.Close();
            }
        }

        /// <summary>
        /// This method will execute a scalar query with parameters
        /// </summary>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public string ExecuteScalarQuery(string query, SqlParameter param = null)
        {
            string value = null;
            ConnectionToDatabase();
            SqlCommand cmd = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                if (param != null)
                {
                    cmd.Parameters.Add(param);
                }
                value = cmd.ExecuteScalar().ToString();//changed from casting datatype to ToString() command. 

                return value.ToString();
            }
            catch (Exception ex)
            {
                return value;
            }
            finally
            {
                cmd.Parameters.Clear();
                connection.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateCache()
        {
            ConnectionToDatabase();
            try
            {
                List<string> queries = new List<string>()
                {
                    "update CacheRoot set LastModified = getdate() where TYPE =  'PartnerConfig'",
                    "update CacheRoot set LastModified = getdate() where TYPE like '%store%'",
                    "update CacheRoot set LastModified = getdate() where TYPE like '%site%'",
                    "update CacheRoot set LastModified = getdate() where TYPE like '%Rate%'"
                };
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;

                foreach (string query in queries)
                {
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                connection.Close();
            }
        }


        /// <summary>
        /// To get ID of the status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public string getIDOfStatus(string status)
        {
            ConnectionToDatabase();
            string query = "Select * from OrderStatus where InternalName='" + status + "'";
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            DataRow row = dt.Rows[0];
            string InternalName = row["InternalName"].ToString();
            string id = row["Id"].ToString();
            connection.Close();
            return id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public DataSet ExecuteReaderQuery(string query, SqlParameter[] param)
        {
            ConnectionToDatabase();

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.Clear();
            try
            {
                connection.Open();
                if (param != null)
                {
                    foreach (var para in param)
                    {
                        cmd.Parameters.Add(para);
                    }
                }
                //SqlDataReader reader = cmd.ExecuteReader();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds);

                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public dynamic ExecuteScalarQuery(string query, params IDataParameter[] param)
        {
            ConnectionToDatabase();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = query;
            cmd.Connection = connection;
            try
            {
                connection.Open();
                if (param != null)
                {
                    foreach (IDataParameter para in param)
                    {
                        cmd.Parameters.Add(para);
                    }
                }
                dynamic reader = cmd.ExecuteScalar();

                return reader;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void AddNewPaymentType(string paymentType, string country)
        {
            //Dictionary<string, string> storeData = new Dictionary<string, string>();
            string getStoreData = null;
            ConnectionToDatabase();
            SqlParameter param = new SqlParameter();
            param.ParameterName = "@paymentType";
            param.Value = paymentType;

            ConnectionToDatabase();
            string query = "select top 1 id,CountryId from HolidayProfile where countryid=(select id from Country where internalname='" + country + "')";
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);
            DataRow row = dt.Rows[0];
            string id = row["id"].ToString();
            string CountryId = row["CountryId"].ToString();

            SqlParameter param1 = new SqlParameter();
            param1.ParameterName = "@hpid";
            param1.Value = id;

            SqlParameter param2 = new SqlParameter();
            param2.ParameterName = "@CountryID";
            param2.Value = CountryId;

            switch (paymentType.ToUpper())
            {
                case "BANKDEBIT":
                    getStoreData = @"/* INSERT FOR NEW Payment Type */
                             IF NOT EXISTS(SELECT 1 FROM PaymentType WHERE InternalName = @paymentType)
                             BEGIN
                                  INSERT INTO PaymentType(Id,InternalName) VALUES(7,@paymentType)
                                  Insert into PaymentTypeDetails (Id,PaymentTypeId,CardTypeId,IsActive,CountryId,InstanceId,PaymentDays,PaymentLeadTime,HolidayProfileId,CardFundingId,FunnelText)
                                  values (NEWID(),7,0,1,@CountryID,null,127,0,@hpid,0,@paymentType) 
                             END";
                    break;
            }
        }

        /// <summary>
        /// To get ID of the status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public string getIDOfNewsletter(string status)
        {
            ConnectionToDatabase();
            string query = "select InstanceId from Newsletter where internalname='" + status + "'";
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            DataRow row = dt.Rows[0];
            //string InternalName = row["InternalName"].ToString();
            string id = row["InstanceId"].ToString();
            id = id.ToLower();
            connection.Close();
            return id;
        }

        /// <summary>
        /// This method will execute a single query with parameters
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public bool ExecuteSingleQueryWithMultipleParameters(string query, SqlParameter[] param = null)
        {
            ConnectionToDatabase();
            SqlCommand cmd = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                if (param != null)
                {
                    foreach (var para in param)
                    {
                        cmd.Parameters.Add(para);
                    }
                }
                cmd.CommandTimeout = 600;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
            finally
            {
                cmd.Parameters.Clear();
                connection.Close();
            }
        }

        /// <summary>
        /// This method will execute a scalar query with parameters
        /// </summary>
        /// <param name="query"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public string ExecuteScalarQueries(string query, SqlParameter[] param = null)
        {
            string value = null;
            ConnectionToDatabase();
            SqlCommand cmd = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                if (param != null)
                {
                    foreach (var para in param)
                    {
                        cmd.Parameters.Add(para);
                    }
                }
                value = cmd.ExecuteScalar().ToString();//changed from casting datatype to ToString() command. 

                return value.ToString();
            }
            catch (Exception ex)
            {
                return value;
            }
            finally
            {
                cmd.Parameters.Clear();
                connection.Close();
            }
        }
    }
}
