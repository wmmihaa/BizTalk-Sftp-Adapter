using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Configuration;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using Blogical.Shared.Adapters.Common;

namespace Blogical.Shared.Adapters.Sftp.ConnectionPool
{
    //internal class SftpHostCollectionFactory
    //{
    //    public static SftpHostCollection Load() 
    //    {
    //    }
    //    public static void Save(SftpHostCollection sftpHostCollection) 
    //    { }
    
    //}
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public static class SftpConnectionPool 
    {
        static SftpConnectionPool()
        {
            Trace.WriteLine("[SftpConnectionPool] Started...");

            try
            {
                XmlNode node = (XmlNode)ConfigurationManager.GetSection("Blogical.Shared.Adapters.Sftp");
                if (node != null)
                    Load(node);
            }
            catch 
            {
                return;
            }
            
   
        }
        /// <summary>
        /// Prepopulates the SftpConnectionPool with servers defined in config file.
        /// </summary>
        /// <param name="section"></param>
        public static void Load(System.Xml.XmlNode section)
        {
            try
            {
                DefaultConnectionLimit = int.Parse(section.SelectSingleNode("SftpConnectionPool").Attributes["defaultConnectionLimit"].Value);
                Trace.WriteLine("[SftpConnectionPool] DefaultConnectionLimit set to " +DefaultConnectionLimit.ToString());

                foreach (XmlNode node in section.SelectNodes("SftpConnectionPool/Host"))
                {
                    string name = node.Attributes["hostName"].Value;
                    int connLimit = int.Parse(node.Attributes["connectionLimit"].Value);
                    _hosts.Add(new SftpHost(name, connLimit, true));
                    Trace.WriteLine("[SftpConnectionPool] A limited connections("+connLimit.ToString()+") given to "+ name+".");
                }
                Trace.WriteLine("[SftpConnectionPool] SftpConnectionPool was loaded with " + _hosts.Count.ToString() + " hosts.");
            }
            catch (Exception e)
            {
                throw ExceptionHandling.HandleComponentException(System.Reflection.MethodBase.GetCurrentMethod(),
                        new Exception("SftpConnectionPool Load Configuration failed", e));
            }

        }
        static ArrayList _hosts = ArrayList.Synchronized(new ArrayList());
        /// <summary>
        /// Default number of connections per server
        /// </summary>
        public static int DefaultConnectionLimit = 60;
        /// <summary>
        /// Returns a SftpHost 
        /// </summary>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public static SftpHost GetHostByName(string hostName, bool trace)
        {
            lock (_hosts.SyncRoot)
            {
                foreach (SftpHost host in _hosts)
                {
                    if (host.HostName == hostName)
                        return host;
                }

                SftpHost newHost = new SftpHost(hostName, DefaultConnectionLimit, trace);
                _hosts.Add(newHost);

                return newHost;
            }
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public static void Dispose()
        {
            lock (_hosts)
            {
                foreach (SftpHost host in _hosts)
                {
                    lock (host.Connections)
                    {
                        foreach (ISftp sftp in host.Connections)
                        {
                            sftp.Disconnect();
                            sftp.Dispose();
                            Trace.WriteLine("[SftpTransmitterEndpoint] Sftp.Disconnect from " + host);
                        }
                        host.Connections.Clear();
                    }
                }
                _hosts.Clear();
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SftpHost
    {
        #region Private Members
        int _currentCount;
        bool _trace = false;
        #endregion
        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="maxNumberOfConnections"></param>
        public SftpHost(string hostName, int maxNumberOfConnections, bool trace)
        {
            this.ConnectionLimit = maxNumberOfConnections;
            this.HostName = hostName;
            this._trace = trace;
        }
        #endregion
        #region Public Members
        /// <summary>
        /// Connection pool
        /// </summary>
        public ArrayList Connections = ArrayList.Synchronized(new ArrayList());
        /// <summary>
        /// Connection limit per server
        /// </summary>
        public int ConnectionLimit;
        /// <summary>
        /// Server name
        /// </summary>
        public string HostName = string.Empty;
        #endregion
        #region Public Methods
        /// <summary>
        /// Returns a new or  free connection from the pool
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="identityFile"></param>
        /// <param name="shutdownRequested"></param>
        /// <returns></returns>
        public ISftp GetConnection(string username, string password, string identityFile, int port, bool shutdownRequested, string passphrase)
        {
            while (!shutdownRequested)
            {
                lock (this.Connections)
                {
                    if (this.Connections.Count != 0)
                    {
                        TraceMessage("[SftpConnectionPool] GetConnectionFromPool found a free connection in the pool");
                        ISftp connection = (ISftp)this.Connections[0];
                        this.Connections.RemoveAt(0);
                        return connection;
                    }
                    if (this._currentCount < this.ConnectionLimit)
                    {
                        TraceMessage("[SftpConnectionPool] GetConnectionFromPool creating a new connection for pool");
                        ISftp sftp = new SharpSsh.Sftp(this.HostName, username, password, identityFile, port, passphrase);
                        this._currentCount++;
                        return sftp;
                    }
                    Monitor.Wait(this.Connections);
                    continue;
                }
            }
            return null;

        }
        /// <summary>
        /// Release sftp connection to pool
        /// </summary>
        /// <param name="conn"></param>
        public void ReleaseConnection(ISftp conn)
        {
            if (conn != null)
            {
                lock (this.Connections)
                {
                    if (this._currentCount > this.ConnectionLimit)
                    {
                        TraceMessage("[SftpConnectionPool] ReleaseConnectionToPool disposing connection object");
                        conn.Disconnect();
                        conn.Dispose();
                        this._currentCount--;
                    }
                    else
                    {
                        TraceMessage("[SftpConnectionPool] ReleaseConnectionToPool releasing connection to pool");
                        //conn.Disconnect();
                        this.Connections.Insert(0, conn);
                        Monitor.Pulse(this.Connections);
                    }
                }
            }

        }
        private void TraceMessage(string message)
        {
            if (_trace)
                Trace.WriteLine(message);
        }
        #endregion
    }
}
