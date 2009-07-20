using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections;
using Blogical.Shared.Adapters.Common;
using System.Configuration;

namespace Blogical.Shared.Adapters.Sftp
{
    /// <summary>
    /// The DataBaseHelper class is used for controling files received from different 
    /// BizTalk nodes, and is only used when the UseLoadBalancing property is set to true.
    /// </summary>
    internal class DataBaseHelper
    {
        #region Internal Methods
        /// <summary>
        /// Used for making sure other processes (nodes) are not processing the same file
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="node"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        internal static bool CheckOutFile(string uri, string node, string filename,bool trace)
        {
            
            #region query
            
                string queryFormat = @"if(
(select count(*) 
from [dbo].[SftpWorkingProcess] 
where [URI] = '{0}' 
and [FileName] ='{2}'
and datediff(minute, [Timestamp], getdate()) <10) =0
)
begin
INSERT INTO [dbo].[SftpWorkingProcess]
           ([URI]
           ,[Node]
           ,[FileName])
     VALUES
           ('{0}','{1}','{2}') 

select 1 as WorkInProcess
end
else
select 0 as WorkInProcess";
            #endregion
                
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();

                SqlTransaction transaction;
                SqlCommand command = connection.CreateCommand();

                transaction = connection.BeginTransaction("CheckoutFile");

                command.Connection = connection;
                command.Transaction = transaction;
                command.CommandText = String.Format(queryFormat, uri, node, filename);
                try
                {
                   
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        int result = (int)reader["WorkInProcess"];
                        reader.Close();
                        transaction.Commit();

                        if (result == 1)
                        {
                            if(trace)
                                Trace.WriteLine("[SftpReceiverEndpoint] Checked Out [" + filename+"]");
    
                            connection.Close();
                            return true;
                        }
                        else
                        {
                            if(trace)
                                Trace.WriteLine("[SftpReceiverEndpoint] Unable to check Out [" + filename + "]");
    
                            connection.Close();
                            return false;
                        }
                    }
                    else
                    {
                        if(trace)
                            Trace.WriteLine("[SftpReceiverEndpoint] Unable to check Out [" + filename + "]");
    
                        connection.Close();
                        return false;
                    }
                }
                catch
                {
                    transaction.Rollback();
                    connection.Close();
                    return false;
                }
            }
        }
        /// <summary>
        /// Used for making sure other processes (nodes) are not processing the same file
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="filename"></param>
        internal static void CheckInFile(string uri, string filename, bool trace)
        {
            if(trace)
                Trace.WriteLine("[SftpReceiverEndpoint] CheckInFile [" + filename + "]");
            
            #region query

            string queryFormat = @"delete from [dbo].[SftpWorkingProcess] 
where [URI] = '{0}' 
and [FileName] ='{1}' ";
            #endregion

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                try
                {
                    SqlCommand command = new SqlCommand(String.Format(queryFormat, uri, filename), connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch
                {
                    if(trace)
                        Trace.WriteLine("[SftpReceiverEndpoint] Error when Checking in file [" + filename + "]");
    
                    return ;
                }
            }
        }

        internal static ArrayList GetCheckedOutFiles(string uri)
        {
            #region query
            string queryFormat = @"
select [FileName] 
from [dbo].[SftpWorkingProcess] 
where [URI] = '{0}' ";
            #endregion
            ArrayList files = new ArrayList();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                
                try
                {
                    SqlCommand command = new SqlCommand(String.Format(queryFormat, uri), connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        files.Add(reader[0].ToString().ToLower());
                    }
                }
                catch
                {
                    return files;
                }
            }
            return files;
        }
        #endregion
        #region Private Methods
        private static string GetConnectionString()
        {
            // This connectionstring doesn't need to be Integrated Security=SSPI, but the user that runs the
            // thread of execution for this service needs to have those rights due to an unfourtunate design
            // in BtsCatalogExplorer. Thus it makes sense that the connectionstring alse be integrated authentication.

            string connectionString = ConfigurationManager.ConnectionStrings["Blogical.Shared.Adapters.Sftp"].ConnectionString;
            return connectionString;
        }
        #endregion
    }
}
