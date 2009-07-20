
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Threading;
using Microsoft.BizTalk.TransportProxy.Interop;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Blogical.Shared.Adapters.Common;
using System.Collections.Generic;
using System.Diagnostics;
using Blogical.Shared.Adapters.Sftp.ConnectionPool;

namespace Blogical.Shared.Adapters.Sftp
{
    /// <summary>
	/// There is one instance of HttpTransmitterEndpoint class for each every static send port.
	/// Messages will be forwarded to this class by AsyncTransmitterBatch
	/// </summary>
	internal class SftpTransmitterEndpoint : AsyncTransmitterEndpoint 
    {
        #region Private Fields
        private ArrayList _connections;
        private bool _shutdownRequested;
        private int _currentCount;// number of connections currently used by this post
        private SftpTransmitProperties _properties = null;
        private AsyncTransmitter _asyncTransmitter = null;
        private string _propertyNamespace;
        #endregion
        #region Construktor
        public SftpTransmitterEndpoint(AsyncTransmitter asyncTransmitter)
            : base(asyncTransmitter)
		{
			this._asyncTransmitter = asyncTransmitter;
            this._connections = new ArrayList();

            Trace.WriteLine("[SftpTransmitterEndpoint] Created...");    
		}

        #endregion
        #region Public Methods
        /// <summary>
        /// This method is called when a Send Location is enabled.
        /// </summary>
        public override void Open(
            EndpointParameters endpointParameters, 
            IPropertyBag handlerPropertyBag, 
            string propertyNamespace)
        {
            this._propertyNamespace = propertyNamespace;
        }
		/// <summary>
        /// Implementation for AsyncTransmitterEndpoint::ProcessMessage
        /// Transmit the message and optionally moves the file from RemoteTempDir to RemotePath
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
        public override IBaseMessage ProcessMessage(IBaseMessage message)
        {
            
            this._properties = new SftpTransmitProperties(message, _propertyNamespace);
            ISftp sftp = SftpConnectionPool.GetHostByName(this._properties.SSHHost, this._properties.DebugTrace).GetConnection(this._properties.SSHUser, this._properties.SSHPasswordProperty, this._properties.SSHIdentityFile, this._properties.SSHPort, this._shutdownRequested, this._properties.SSHPassphrase);

            try
            {
                if (!this._shutdownRequested)
                {
                    ProcessMessageInternal(message, sftp);
                }
            }
            catch (Exception ex)
            {
                throw ExceptionHandling.HandleComponentException(System.Reflection.MethodBase.GetCurrentMethod(),ex);
            }
            finally
            {
                SftpConnectionPool.GetHostByName(this._properties.SSHHost, this._properties.DebugTrace).ReleaseConnection(sftp);
            }
            return null;
        }

        void sftp_OnDisconnect(ISftp sftp)
        {
            SftpConnectionPool.GetHostByName(this._properties.SSHHost, this._properties.DebugTrace).ReleaseConnection(sftp);
        }
        /// <summary>
        /// Executed on termination (Stop Host instance)
        /// </summary>
        public override void Dispose()
        {
            Trace.WriteLine("[SftpTransmitterEndpoint] Disposing...");
            this._shutdownRequested = true;
            int num = 0;
            while ((this._connections.Count < this._currentCount) && (num < 100))
            {
                num++;
                Thread.Sleep(50);
            }
            foreach (ISftp connection in this._connections)
            {
                Trace.WriteLine("[SftpTransmitterEndpoint] Disconnecting...");
                connection.Disconnect();
                connection.Dispose();
            }
            base.Dispose();
            Trace.WriteLine("[SftpTransmitterEndpoint] Disposed...");

        }
        #endregion
        #region Private Methods
        private IBaseMessage ProcessMessageInternal(IBaseMessage message, ISftp sftp)
		{
            string filePath = "";
            
            try
            {
                Stream source = message.BodyPart.Data;
                source.Position = 0;


                if (this._properties.RemoteTempFile.Trim().Length > 0) // Temp dir + Temp file
                    filePath = SftpTransmitProperties.CreateFileName(message, CommonFunctions.CombinePath(this._properties.RemoteTempDir, this._properties.RemoteTempFile));
                else if (this._properties.RemoteTempDir.Trim().Length > 0) // Temp dir + file
                    filePath = SftpTransmitProperties.CreateFileName(message, CommonFunctions.CombinePath(this._properties.RemoteTempDir, this._properties.RemoteFile));
                else // dir + file
                    filePath = SftpTransmitProperties.CreateFileName(message, CommonFunctions.CombinePath(this._properties.RemotePath, this._properties.RemoteFile));

                TraceMessage("[SftpTransmitterEndpoint] Sftp.Put " + filePath);
                sftp.Put(source, filePath);

                // If the RemoteTempDir is set then move the file to the RemotePath
                if (this._properties.RemoteTempDir.Trim().Length > 0)
                {
                    if (this._properties.VerifyFileSize)
                        VerifyFileSize(sftp, filePath, source.Length); // throws exception if sizes do not match

                    string toPath = SftpTransmitProperties.CreateFileName(message, 
                        CommonFunctions.CombinePath(this._properties.RemotePath, this._properties.RemoteFile));
                    sftp.Rename(filePath, toPath);
                    sftp.ApplySecurityPermissions(this._properties.ApplySecurityPermissions, toPath);
                }
                else
                    sftp.ApplySecurityPermissions(this._properties.ApplySecurityPermissions, filePath);
                
                return null;
            }
            catch (Exception ex)
            {
                string innerEx = ex.InnerException == null ? "" : ex.InnerException.Message;
                throw new SftpException("Unable to transmit file " + filePath + ".\nInner Exception:\n"+ex.Message+"\n"+innerEx, ex);
            }
            
		}
        private void VerifyFileSize(ISftp sftp, string filePath, long expectedFileSize)
        {
            FileEntry f = sftp.GetFileEntry(filePath, this._properties.DebugTrace);
            if (f.Size != expectedFileSize)
            {
                try
                {
                    sftp.Delete(filePath);
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception during Delete currupt file.", ex);
                }

                throw new SftpException("Corrupt file " + filePath +
                    ". Expected  " + expectedFileSize + " bytes, actual was " + f.Size + " bytes. " +
                    "File deleted from remote system. " +
                    "Will retry if BizTalk configured retry attempts are not exhausted.");
            }
        }
        private void connection2_OnDisconnect(ISftp sftp)
        {
            
        }
        private void TraceMessage(string message)
        {
            if (this._properties.DebugTrace)
                Trace.WriteLine(message);
        }
        #endregion
    }
}
