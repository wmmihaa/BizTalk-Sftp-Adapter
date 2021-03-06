using System;
using System.Collections.Generic;
using System.Text;
using Blogical.Shared.Adapters.Common;
using System.Xml;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using System.Diagnostics;
using System.IO;
using Microsoft.SSO.Utility;

namespace Blogical.Shared.Adapters.Sftp
{
    /// <summary>
    /// The SftpTransmitProperties class represents the properties defined in
    /// Blogical.Shared.Adapters.Sftp.Management.TransmitLocation.xsd
    /// </summary>
    /// <history>2013-11-10 Greg Sharp, Add X.509 identity certificate support</history>
    public class SftpTransmitProperties : ConfigProperties
    {
        #region Private Fields
        private static int _handlerSendBatchSize    = 20;
        private static int _handlerbufferSize       = 4096;
        private static int _handlerthreadsPerCPU    = 1;

        string _sshHost                             = String.Empty;
        string _sshPasswordProperty                 = String.Empty;
        int _sshPort                                = 22;
        string _sshUser                             = String.Empty;
        string _sshIdentityFile                     = String.Empty;
        string _sshIdentityThumbprint               = String.Empty;
        string _ssoApplication                      = String.Empty;
        bool _sshtrace                              = false;

        string _sshRemotePath                       = String.Empty;
        string _sshRemoteTempDir                    = String.Empty;
        string _sshRemoteFile                       = String.Empty;
        int _sshErrorThreshold                      = 10;
        int _connectionLimit                        = 10;
        string _applySecurityPermissions            = String.Empty;
        bool _verifyFileSize                        = false;

        // Proxy settings
        string _proxyHost                           = String.Empty;
        int _proxyPort                              = 80;
        string _proxyUsername                       = String.Empty;
        string _proxyPassword                       = String.Empty;

        #endregion
        #region Public Properties
        //public static int BufferSize { get { return _handlerbufferSize; } }
        //public static int ThreadsPerCPU { get { return _handlerthreadsPerCPU; } }

        /// <summary>
        /// Size of entire batch
        /// </summary>
        public static int BatchSize { get { return _handlerSendBatchSize; } }
        /// <summary>
        /// The address of the SSH host
        /// </summary>
        public string SSHHost
        {
            get { return this._sshHost; }
        }
        /// <summary>
        /// The password for SSH password-based authentication
        /// </summary>
        public string SSHPasswordProperty
        {
            get { return this._sshPasswordProperty; }
        }
        /// <summary>
        /// The port in the SSH server where the SSH service is running; by default 22.
        /// </summary>
        public int SSHPort
        {
            get { return this._sshPort; }
        }
        /// <summary>
        /// The username for SSH authentication.
        /// </summary>
        public string SSHUser
        {
            get { return this._sshUser; }
        }
        /// <summary>
        /// The certificate to use for client authentication during the SSH handshake.
        /// </summary>
        public string SSHIdentityFile
        {
            get { return this._sshIdentityFile; }
        }
        /// <summary>
        /// The certificate to use for client authentication during the SSH handshake.
        /// </summary>
        public string SSHIdentityThumbprint
        {
            get { return this._sshIdentityThumbprint; }
        }
        /// <summary>
        /// The Single Sign On (SSO) Affiliate Application
        /// </summary>
        public string SSOApplication
        {
            get { return this._ssoApplication; }
        }
        /// <summary>
        /// The current path to the SFTP server
        /// </summary>
        public string RemotePath
        {
            get
            {
                if (this._sshRemotePath.EndsWith("/"))
                    return this._sshRemotePath;
                else
                    return this._sshRemotePath + "/";
            }
        }
        /// <summary>
        /// A temporary directory on the server to store files before moving them to Remote path
        /// </summary>
        public string RemoteTempDir
        {
            get
            {
                if (this._sshRemoteTempDir.Length == 0)
                    return this._sshRemoteTempDir;
                else if (this._sshRemoteTempDir.EndsWith("/"))
                    return this._sshRemoteTempDir;
                else
                    return this._sshRemoteTempDir + "/";
            }
        }
        /// <summary>
        /// SSH Remote file name
        /// </summary>
        public string RemoteFile
        {
            get { return this._sshRemoteFile; }
        }
        /// <summary>
        /// The number of errors before the adapter shuts down 
        /// </summary>
        public int ErrorThreshold
        {
            get { return this._sshErrorThreshold; }
        }
        /// <summary>
        /// Writes a message to the trace listeners
        /// </summary>
        public bool DebugTrace
        {
            get { return this._sshtrace; }
        }
        /// <summary>
        /// Uri
        /// </summary>
        public string Uri
        {
            get
            {
                return CommonFunctions.CombinePath("SFTP://" + this.SSHHost + ":" + this.SSHPort, this.RemotePath, this.RemoteFile);
            }
        }
        /// <summary>
        /// Maximum number of concurrentSftp connections that can be opened to the server. 10 is default.
        /// </summary>
        public int ConnectionLimit
        {
            get { return _connectionLimit; }
        }
        /// <summary>
        /// A numerical representing a permission matrix. These permissions are overridden on Windows platforms, and are therefore useless on such a host. Default value on UNIX platforms are 644. If left empty, no permissioins will be applied.
        /// </summary>
        public string ApplySecurityPermissions
        {
            get { return _applySecurityPermissions; }
        }

        // Proxy Settings
        /// <summary>
        /// The URI to the HTTP Proxy server
        /// </summary>
        public string ProxyHost
        {
            get { return this._proxyHost; }
        }
        /// <summary>
        /// The port on which the HTTP proxy is running on; by default 80.
        /// </summary>
        public int ProxyPort
        {
            get { return this._proxyPort; }
        }
        /// <summary>
        /// The username used for proxy authentication.
        /// </summary>
        public string ProxyUserName
        {
            get { return this._proxyUsername; }
        }
        /// <summary>
        /// The password used for proxy authentication.
        /// </summary>
        public string ProxyPassword
        {
            get { return this._proxyPassword; }
        }


        public bool VerifyFileSize
        {
            get { return _verifyFileSize; }
            set { _verifyFileSize = value; }
        }

        private string _sshRemoteTempFile;

        public string RemoteTempFile
        {
            get { return _sshRemoteTempFile; }
            set { _sshRemoteTempFile = value; }
        }

        private string _sshPassphrase;

        public string SSHPassphrase
        {
            get { return _sshPassphrase; }
            set { _sshPassphrase = value; }
        }

        #endregion
        #region Public Methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="propertyNamespace"></param>
        public SftpTransmitProperties(IBaseMessage message, string propertyNamespace)
        {
            XmlDocument locationConfigDom = null;

            //  get the adapter configuration off the message
            IBaseMessageContext context = message.Context;
            string config = (string)context.Read("AdapterConfig", propertyNamespace);

            if (null != config)
            {
                locationConfigDom = new XmlDocument();
                locationConfigDom.LoadXml(config);

                this.ReadLocationConfiguration(locationConfigDom);
            }
            else //  the config can be null all that means is that we are doing a dynamic send
            {
                this.ReadLocationConfiguration(message.Context);
            }
        }

        /// <summary>
        /// Read the Blogical.Shared.Adapters.Sftp.Management.TransmitLocation.xsd and populate 
        /// all properties 
        /// </summary>
        /// <param name="endpointConfig"></param>
        public void ReadLocationConfiguration(XmlDocument endpointConfig)
        {
            this._sshtrace = ExtractBool(endpointConfig, "/Config/trace");

            TraceMessage("[SftpTransmitProperties] ReadLocationConfiguration called");

            this._ssoApplication = IfExistsExtract(endpointConfig, "/Config/ssoapplication", String.Empty);

            if (!String.IsNullOrEmpty(this._ssoApplication))
            {
                TraceMessage("[SftpTransmitProperties] SSO Authentication");
                try
                {
                    SSOConfigHelper.Credentials credentials = SSOConfigHelper.GetCredentials(this._ssoApplication);
                    this._sshUser = credentials.Username;
                    this._sshPasswordProperty = credentials.Password;
                }
                catch (Exception e)
                {
                    throw new Exception(@"Unable to read properties from SSO database. Make sure to use ""UserName"" and ""Password"" as fields", e);
                }
            }
            else
            {
                TraceMessage("[SftpTransmitProperties] Username/Password Authentication");

                this._sshUser = Extract(endpointConfig, "/Config/user", String.Empty);
                this._sshPasswordProperty = IfExistsExtract(endpointConfig, "/Config/password", String.Empty);
            }

            //this._sshUser = Extract(endpointConfig, "/Config/user", String.Empty);
            //this._sshPasswordProperty = IfExistsExtract(endpointConfig, "/Config/password", String.Empty);

            this._sshHost = Extract(endpointConfig, "/Config/host", String.Empty);
            this._sshPort = ExtractInt(endpointConfig, "/Config/port");
            this._sshIdentityFile = IfExistsExtract(endpointConfig, "/Config/identityfile", String.Empty);
            this._sshIdentityThumbprint = IfExistsExtract(endpointConfig, "/Config/identitythumbprint", String.Empty);

            this._sshRemotePath = Extract(endpointConfig, "/Config/remotepath", String.Empty);
            this._sshRemoteTempDir = IfNotEmptyExtract(endpointConfig, "/Config/remotetempdir", false, String.Empty);
            this._sshRemoteFile = Extract(endpointConfig, "/Config/remotefile", String.Empty);
            this._sshErrorThreshold = ExtractInt(endpointConfig, "/Config/errorThreshold");
            this._connectionLimit = ExtractInt(endpointConfig, "/Config/connectionlimit");
            this._applySecurityPermissions = IfExistsExtract(endpointConfig, "/Config/applySecurityPermissions", String.Empty);
            this._verifyFileSize = IfExistsExtractBool(endpointConfig, "/Config/verifyFileSize", false);
            this._sshRemoteTempFile = IfExistsExtract(endpointConfig, "/Config/remotetempfile", String.Empty);
            this._sshPassphrase = IfExistsExtract(endpointConfig, "/Config/passphrase", String.Empty);

            // Proxy Settings
            this._proxyHost = IfExistsExtract(endpointConfig, "/Config/proxyserver", String.Empty);
            this._proxyPort = ExtractInt(endpointConfig, "/Config/proxyport");
            this._proxyUsername = IfExistsExtract(endpointConfig, "/Config/proxyusername", String.Empty);
            this._proxyPassword = IfExistsExtract(endpointConfig, "/Config/proxypassword", String.Empty);
        }

        /// <summary>
        /// Read the Blogical.Shared.Adapters.Sftp.Management.TransmitLocation.xsd from message context 
        /// and populate properties 
        /// </summary>
        /// <param name="context"></param>
        public void ReadLocationConfiguration(IBaseMessageContext context)
        {
            string propertyNS = "Blogical.Shared.Adapters.Sftp.TransmitLocation.v1";
            this._sshtrace = (bool)Extract(context, "trace", propertyNS, false, false);

            TraceMessage("[SftpTransmitProperties] ReadLocationConfiguration called");

            this._ssoApplication = (string)Extract(context, "ssoapplication", propertyNS, String.Empty, false);

            if (!String.IsNullOrEmpty(this._ssoApplication))
            {
                TraceMessage("[SftpTransmitProperties] SSO Authentication");
                try
                {
                    SSOConfigHelper.Credentials credentials = SSOConfigHelper.GetCredentials(this._ssoApplication);
                    this._sshUser = credentials.Username;
                    this._sshPasswordProperty = credentials.Password;
                }
                catch (Exception e)
                {
                    throw new Exception(@"Unable to read properties from SSO database. Make sure to use ""UserName"" and ""Password"" as fields", e);
                }
            }
            else
            {
                TraceMessage("[SftpTransmitProperties] Username/Password Authentication");
                this._sshUser = (string)Extract(context, "user", propertyNS, String.Empty, true);
                this._sshPasswordProperty = (string)Extract(context, "password", propertyNS, String.Empty, false);
            }

            // this._sshUser = (string)Extract(context, "user", propertyNS, String.Empty, true);
            // this._sshPasswordProperty = (string)Extract(context, "password", propertyNS, String.Empty, false);

            this._sshHost = (string)Extract(context, "host", propertyNS, String.Empty, true);
            this._sshPort = (int)Extract(context, "portno", propertyNS, 22, true);
            this._sshIdentityFile = (string)Extract(context, "identityfile", propertyNS, String.Empty, false);
            this._sshIdentityThumbprint = (string)Extract(context, "identitythumbprint", propertyNS, String.Empty, false);
            this._sshRemotePath = (string)Extract(context, "remotepath", propertyNS, String.Empty, false);
            this._sshRemoteTempDir = (string)Extract(context, "remotetempdir", propertyNS, String.Empty, false);
            this._sshRemoteFile = (string)Extract(context, "remotefile", propertyNS, String.Empty, true);
            this._connectionLimit = (int)Extract(context, "connectionlimit", propertyNS, 10, false);
            this._applySecurityPermissions = (string)Extract(context, "applySecurityPermissions", propertyNS, String.Empty, false);
            this._verifyFileSize = (bool)Extract(context, "verifyFileSize", propertyNS, false, false);
            this._sshRemoteTempFile = (string)Extract(context, "remotetempfile", propertyNS, String.Empty, false);
            this._sshPassphrase = (string)Extract(context, "passphrase", propertyNS, string.Empty, false);

            // Proxy Settings
            this._proxyHost = (string)Extract(context, "proxyserver", propertyNS, string.Empty, false);
            this._proxyPort = (int)Extract(context, "proxyport", propertyNS,80, false);
            this._proxyUsername = (string)Extract(context, "proxyusername", propertyNS, string.Empty, false);
            this._proxyPassword = (string)Extract(context, "proxypassword", propertyNS, string.Empty, false); 
        }

        /// <summary>
        /// Load the Transmit Handler configuration settings
        /// </summary>
        public static void ReadTransmitHandlerConfiguration(XmlDocument configDOM)
        {
            // Handler properties
            _handlerSendBatchSize = ExtractInt(configDOM, "/Config/sendBatchSize");
            _handlerbufferSize = ExtractInt(configDOM, "/Config/bufferSize");
            _handlerthreadsPerCPU = ExtractInt(configDOM, "/Config/threadsPerCPU");
        }
        /// <summary>
        /// Determines the name of the file that should be created for a transmitted message
        /// replaces %MessageID% with the message's Guid if specified.
        /// </summary>
        /// <param name="message">The Message to transmit</param>
        /// <param name="uri">The address of the message.  May contain "%MessageID%" </param>
        /// <returns>The name of the file to write to</returns>
        public static string CreateFileName(IBaseMessage message, string uri)
        {
            //string uriNew = ReplaceMessageID(message, uri);
            return ReplaceMacros(message, uri);
        }
        #endregion
        #region Private Methods
        private static string ReplaceMacros(IBaseMessage message, string uri)
        {
            if (uri.IndexOf("%MessageID%") > -1)
            {
                Guid msgId = message.MessageID;

                uri = uri.Replace("%MessageID%", msgId.ToString());
            }
            if (uri.IndexOf("%SourceFileName%") > -1)
            {
                string sourceFileName = string.Empty;
                try
                {
                    string filePath = GetReceivedFileName(message);
                    sourceFileName = Path.GetFileName(filePath);
                }
                catch
                {
                    throw new Exception("The %SourceFileName% macro can only be used with the " + Constants.SFTP_ADAPTER_PROPERTIES_NAMESPACE + " namespace.");
                }
                uri = uri.Replace("%SourceFileName%", sourceFileName);
            }

            /* John C. Vestal 2010/04/07 - Added DateTime and UniversalDateTime to macro list. */
            if (uri.IndexOf("%DateTime%") > -1)
            {
                string dateTime = DateTime.Now.ToString();
                dateTime = dateTime.Replace("/", "-");
                dateTime = dateTime.Replace(":", "");
                uri = uri.Replace("%DateTime%", dateTime);
            }
            if (uri.IndexOf("%UniversalDateTime%") > -1)
            {
                string dateTime = DateTime.Now.ToUniversalTime().ToString();
                dateTime = dateTime.Replace("/", "-");
                dateTime = dateTime.Replace(":", "");
                uri = uri.Replace("%UniversalDateTime%", dateTime);
            }
            // Peter Lindgren 2014-02-12: Added datetime macro that works exactly as the corresponding macro in the standard FTP and FILE adapters.
            if (uri.IndexOf("%datetime%") > -1)
            {
                string dateTime = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHHmmss");
                uri = uri.Replace("%datetime%", dateTime);
            }

            return uri;
        }
        private static string GetReceivedFileName(IBaseMessage pInMsg)
        {
            SystemMessageContext messageContext = new SystemMessageContext(pInMsg.Context);

            string receivedFileName =
                pInMsg.Context.Read("ReceivedFileName",
                                "http://schemas.microsoft.com/BizTalk/2003/file-properties") as string;

            if (receivedFileName == null)
            {
                if (messageContext.InboundTransportType.ToUpper().Equals("SQL"))
                {
                    // If data is retrieved via the SQL-adapter  
                    // set receivedFileName to ReceivePortName
                    if (pInMsg.Context.Read("ReceivedFileName",
                          "http://schemas.microsoft.com/BizTalk/2003/sql-properties") == null)
                    {
                        receivedFileName = pInMsg.Context.Read("ReceivePortName",
                            "http://schemas.microsoft.com/BizTalk/2003/system-properties").ToString();
                    }
                    else
                    {
                        receivedFileName = pInMsg.Context.Read("ReceivedFileName",
                          "http://schemas.microsoft.com/BizTalk/2003/sql-properties").ToString();
                    }
                }
                else
                {
                    // BizTalk OOTB pipelines use this pattern
                    // The SFTP adapter has also been coded to use this pattern - other adapters might not!!!
                    receivedFileName = pInMsg.Context.Read("ReceivedFileName",
                         "http://schemas.microsoft.com/BizTalk/2003/" +
                             messageContext.InboundTransportType.ToLower() + "-properties").ToString();
                }
            }
            return receivedFileName;
        }
        private static string ReplaceMessageID(IBaseMessage message, string uri)
        {
            Guid msgId = message.MessageID;

            string res = uri.Replace("%MessageID%", msgId.ToString());
            if (res != null)
                return res;
            else
                return uri;
        }
        private void TraceMessage(string message)
        {
            if (this.DebugTrace)
                Trace.WriteLine(message);
        }
        private object Extract(IBaseMessageContext context, string prop, string propNS, object fallback, bool isRequired)
        {
            Object o = context.Read(prop, propNS);
            if (!isRequired && null == o)
                return fallback;
            if (null == o)
                throw new NoSuchProperty(propNS + "#" + prop);
            return o;
        }

        #endregion
    }
}
