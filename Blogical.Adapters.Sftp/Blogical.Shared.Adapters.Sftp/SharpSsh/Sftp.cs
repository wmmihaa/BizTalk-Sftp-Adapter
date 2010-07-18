using System;
using System.Collections.Generic;
using System.Text;
using Tamir.SharpSsh.jsch;
using Tamir.SharpSsh.jsch.examples;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Tamir.SharpSsh;
using Microsoft.BizTalk.Streaming;
using System.IO.IsolatedStorage;
using Blogical.Shared.Adapters.Common;

namespace Blogical.Shared.Adapters.Sftp.SharpSsh
{
    /// <summary>
    /// SharpSsh sftp wrapper class
    /// </summary>
    public class Sftp : ISftp, IDisposable
    {
        #region Private Members
        private ArrayList _applicationStorage = ArrayList.Synchronized(new ArrayList());
        const int TOTALLIFETIME = 600; // total number of seconds for reusing of connection
        const int TOTALTIMEDIFF = 4; // total number of seconds in difference between servers 
        DateTime _connectedSince;
        object createdCounterObject = null;
        SshTransfer _sftp = null;
        string _identityFile;
        string _host = String.Empty;
        string _user = String.Empty;
        string _password = String.Empty;
        int _port = 22;
        string _passphrase = String.Empty;
        #endregion
        #region ISftp Members
        public bool DebugTrace { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">eg 127.0.0.1</param>
        /// <param name="user">username</param>
        /// <param name="password">password (usualy not used together with identityFile)</param>
        /// <param name="identityFile">filename with path. eg. c:\temp\myFile.key</param>
        /// <param name="port">Port to use for connection.</param>
        /// <param name="passphrase">passphrase for identityfile.</param>
        /// <created>2007-04-01 - Mikael Hkansson</created>
        /// <history>2007-10-11 - Mikael Hkansson, Added code head to all public methods</history>
        /// <history>2008-11-23 - Johan Hedberg, Added passphrase</history>
        public Sftp(string host, string user, string password, string identityFile, int port, string passphrase, bool debugTrace)
        {
            this._applicationStorage = ApplicationStorageHelper.Load();
            this._sftp = new SshTransfer(host, user, password);
            this._identityFile = identityFile;
            this._host = host;
            this._user = user;
            this._password=password;
            this._port = port;
            this._passphrase = passphrase;
            this.DebugTrace = debugTrace;
        }
        /// <summary>
        /// Used for receiving files
        /// </summary>
        /// <param name="fromFilePath"></param>
        /// <returns></returns>
        public Stream Get(string fromFilePath)
        {
            
            
            try
            {
                connect();
                try
                {
                    return _sftp.GetStream(fromFilePath);
                }
                catch
                {
                    reConnect();
                    return _sftp.GetStream(fromFilePath);
                }
            }
            catch (Exception ex)
            {
               
                throw ExceptionHandling.HandleComponentException(
                    EventLogEventIDs.UnableToGetFile, 
                    System.Reflection.MethodBase.GetCurrentMethod(), 
                    new SftpException("Unable to get file " + fromFilePath, ex));
            }
            finally
            {
                RaiseOnDisconnect();
            }
        }
        /// <summary>
        /// Used for sending files
        /// </summary>
        /// <param name="memStream"></param>
        /// <param name="destination"></param>
        public void Put(System.IO.Stream memStream, string destination)
        {
            try
            {
  
                try
                {
                    connect();
                    _sftp.PutStream(memStream, destination);
                }
                catch
                {
                    reConnect();
                    _sftp.PutStream(memStream, destination);
                }
            }
            catch (Exception ex)
            {
                throw ExceptionHandling.HandleComponentException(
                    EventLogEventIDs.UnableToWriteFile, 
                    System.Reflection.MethodBase.GetCurrentMethod(), 
                    new SftpException("Unable write file to " + destination, ex));
            }

            finally
            {
                RaiseOnDisconnect();
            }
        }
        /// <summary>
        /// used for renaming files
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public void Rename(string oldName, string newName)
        {
            connect();
            try
            {
                try
                {
                    _sftp.Rename(oldName, newName);
                }
                catch
                {
                    reConnect();
                    _sftp.Rename(oldName, newName);
                }
            }
            catch (Exception ex)
            {
                throw ExceptionHandling.HandleComponentException(
                    EventLogEventIDs.UnableToRenameFile, 
                    System.Reflection.MethodBase.GetCurrentMethod(), 
                    new SftpException("Unable to rename " + oldName + " to " + newName, ex));
            }
            finally
            {
                RaiseOnDisconnect();
            }
        }
        /// <summary>
        /// Returns a list of files (and sizes) from a uri
        /// </summary>
        /// <param name="fileMask"></param>
        /// <param name="uri"></param>
        /// <param name="filesInProcess"></param>
        /// <returns></returns>
        public List<FileEntry> Dir(string fileMask, string uri, ArrayList filesInProcess, bool trace)
        {
            try
            {
                try
                {
                    connect();
                    return dir(fileMask, uri, 0, filesInProcess,trace);
                }
                catch
                {
                    reConnect();
                    return dir(fileMask, uri, 0, filesInProcess, trace);
                }
            }
            catch (Exception ex)
            {
                throw ExceptionHandling.HandleComponentException(
                    EventLogEventIDs.UnableToListDirectory, 
                    System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
            finally
            {
                RaiseOnDisconnect();
            }
            
        }
        /// <summary>
        /// Returns a list of files (and sizes) from a uri
        /// </summary>
        /// <param name="fileMask"></param>
        /// <param name="uri"></param>
        /// <param name="maxNumberOfFiles"></param>
        /// <param name="filesInProcess"></param>
        /// <returns></returns>
        public List<FileEntry> Dir(string fileMask, string uri, int maxNumberOfFiles, ArrayList filesInProcess, bool trace)
        {
            try
            {
                try
                {
                    connect();
                    return dir(fileMask, uri, maxNumberOfFiles, filesInProcess, trace);
                }
                catch
                {
                    reConnect();
                    return dir(fileMask, uri, maxNumberOfFiles, filesInProcess,trace);
                }
            }
            catch (Exception ex)
            {
                throw ExceptionHandling.HandleComponentException(
                    EventLogEventIDs.UnableToListDirectory,
                    System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
            finally
            {
                RaiseOnDisconnect();
            }
        }
        /// <summary>
        /// Determines wether a specified directory has files.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool Exists(string fileName)
        {
            try
            {
                return _sftp.Exists(fileName);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        /// <summary>
        /// Deletes a file
        /// </summary>
        /// <param name="filePath"></param>
        public void Delete(string filePath)
        {
            
            try
            {
                try
                {
                    connect();
                    _sftp.Delete(filePath);
                }
                catch
                {
                    reConnect();
                    _sftp.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw ExceptionHandling.HandleComponentException(System.Reflection.MethodBase.GetCurrentMethod(), new SftpException("Unable to delete file [" + filePath + "].", ex));
            }
            finally
            {
                RaiseOnDisconnect();
            }
        }
        /// <summary>
        /// Open an ssh connection
        /// </summary>
        public void Connect()
        {
            this.connect();
        }
        /// <summary>
        /// Disconnects from sever
        /// </summary>
        public void Disconnect()
        {
            if(this.DebugTrace)
                Trace.WriteLine("[SftpConnectionPool] Disconnecting from " + _host);
            try
            {
                if (this._sftp.Connected)
                {
                    this._sftp.Close();
                    this._sftp = new SshTransfer(this._host, this._user, this._password); 
                }
            }
            catch (Exception ex)
            {
                throw ExceptionHandling.HandleComponentException(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
            finally
            {
                RaiseOnDisconnect();
            }
        }
        /// <summary>
        /// A numerical representing a permission matrix. These permissions are overridden on Windows platforms, and are therefore useless on such a host. Default value on UNIX platforms are 644. If left empty, no permissioins will be applied.
        /// </summary>
        /// <param name="permissions"></param>
        /// <param name="filePath"></param>
        public void ApplySecurityPermissions(string permissions, string filePath)
        {
            // Don't apply if empty 
            if(String.IsNullOrEmpty(permissions))
                return;

            try 
            {
                byte[] buffer = Util.getBytes(permissions);
                int currentPos;
                int perm = 0;
                for (int pos = 0; pos < buffer.Length; pos++)
                {
                    currentPos = buffer[pos];
                    if (currentPos < '0' || currentPos > '7')
                    {
                        perm = -1;
                        break;
                    }

                    perm <<= 3;
                    perm |= (currentPos - '0');
                }
                
                _sftp.SftpChannel.chmod(perm, filePath);
            }
            catch { throw new Exception("Unable to parse permissions to integer"); }
        }

        /// <summary>
        /// The event is fired when Disconnect is called
        /// </summary>
        public event DisconnectEventHandler OnDisconnect;

        /// <summary>
        /// Gets information about a file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        public FileEntry GetFileEntry(string filePath, bool trace)
        {
            return getFileEntry(filePath, trace);
        }
        #endregion 
        #region Private Methods
        /// <summary>
        /// Connects to server
        /// </summary>
        private void connect()
        {
            connect(false);
        }
        private void connect(bool force)
        {
            try
            {
                if (!this._sftp.Connected || force)
                {
                    if (this.DebugTrace)
                        Trace.WriteLine("[SftpConnectionPool] Connecting to " + _host);

                    if (!String.IsNullOrEmpty(_identityFile) && !String.IsNullOrEmpty(_passphrase))
                        _sftp.AddIdentityFile(_identityFile, _passphrase);
                    else if (!String.IsNullOrEmpty(_identityFile))
                        _sftp.AddIdentityFile(_identityFile);

                    this._sftp.Connect(this._port);

                    //Make sure HostKey match previously retrieved HostKey.
                    CheckHostKey();

                    this._connectedSince = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                if (this._sftp.Connected)
                    this.Disconnect();

                throw ExceptionHandling.HandleComponentException(
                    EventLogEventIDs.UnableToConnectToHost,
                    System.Reflection.MethodBase.GetCurrentMethod(),
                        new Exception("Unable to connect to Sftp host [" + this._host + "]", ex));
            }
            finally
            {
            }
        }
        private void reConnect()
        {
            Disconnect();
            Trace.WriteLine("[SftpConnectionPool] Reconnecting to " + _host);
            connect(false);
        }
        void RaiseOnDisconnect()
        {
            TimeSpan ts = DateTime.Now.Subtract(this._connectedSince);
            if (ts.TotalSeconds > TOTALLIFETIME)
            {
                if (this._sftp.Connected)
                    this.Disconnect();

                //this.connect();

                if (OnDisconnect != null)
                    OnDisconnect(this);

                Trace.WriteLine("[SftpConnectionPool] Connection has timed out");

            }

        }
        void CheckHostKey()
        {
            object hostKey = ApplicationStorageHelper.GetHostKey(this._applicationStorage, this._host);

            if (hostKey == null)
            {
                this._applicationStorage.Add(new ApplicationStorage(this._host, this._sftp.HostKey.getKey()));
                ApplicationStorageHelper.Save(this._applicationStorage);
            }
            else if (hostKey.ToString() != this._sftp.HostKey.getKey())
                throw ExceptionHandling.HandleComponentException(System.Reflection.MethodBase.GetCurrentMethod(),
                       new Exception("HostKey does not match previously retrieved HostKey."));
        }
        private List<FileEntry> dir(string fileMask, string uri, int maxNumberOfFiles, ArrayList filesInProcess, bool trace)
        {
            try
            {
                if (trace)
                    Trace.WriteLine("[SftpReceiverEndpoint] Dir(" + fileMask + ")");

                List<FileEntry> fileEntries = new List<FileEntry>();
                ArrayList checkedOutFiles = new ArrayList();

                foreach (Tamir.SharpSsh.jsch.ChannelSftp.LsEntry entry in _sftp.Dir2(fileMask))
                {
                    string remotePath = Path.GetDirectoryName(fileMask);
                    long size = entry.getAttrs().getSize();
                    bool isDirectory = entry.getAttrs().isDir();
                    string fileName = entry.getFilename();
                    DateTime fileLastWriten;

                    if (isDirectory || size==0)
                        continue;

                    try
                    {
                        fileLastWriten = DateTime.Parse(entry.getAttrs().getMtimeString());
                    }
                    catch { fileLastWriten = DateTime.Now.AddMinutes(1); }

                    TimeSpan ts = DateTime.Now.Subtract(fileLastWriten);

                    try
                    {
                        if (ts.TotalSeconds > TOTALTIMEDIFF &&
                            !filesInProcess.Contains(CommonFunctions.CombinePath(remotePath, fileName)))
                        {
                            // "Check out" file if UseLoadBalancing == true 
                            if (uri != null && !DataBaseHelper.CheckOutFile(uri, System.Environment.MachineName, fileName, trace))
                                continue; // loadbalancing==on, checked out failed
                            else if (uri != null)// loadbalancing==on, checked out succeeded
                            {
                                // Check that file still exists
                                string filePath = fileMask.Substring(0, fileMask.LastIndexOf("/") + 1) + fileName;

                                if (_sftp.Exists(filePath))
                                    fileEntries.Add(new FileEntry(fileName, size));
                                else
                                {
                                    DataBaseHelper.CheckInFile(uri, fileName, trace);
                                    continue;
                                }
                            }
                            else //No loadbalancing
                            {
                                fileEntries.Add(new FileEntry(fileName, size));
                            }
                        }
                    }
                    catch
                    {
                        if (uri != null)
                            DataBaseHelper.CheckInFile(uri, fileName, trace);
                    }
                    if (fileEntries.Count == maxNumberOfFiles && maxNumberOfFiles > 0)
                        break;

                }
                if (trace)
                    Trace.WriteLine(string.Format("[SftpReceiverEndpoint] Found {0} files.", fileEntries.Count));
                return fileEntries;
            }
            catch (Exception ex)
            {
                throw new SftpException("Unable to perform directory list at [" + uri + "]", ex);
            }
            finally
            {
                RaiseOnDisconnect();
            }
        }
        /// <summary>
        /// This method is no longer used, but is left beacuase it might be used in the future. 
        /// Developed to create a more efficient directory listing. By creating a random order of files, 
        /// listing large amount of files from different different BizTalk nodes became more efficient.
        /// </summary>
        /// <param name="fileMask"></param>
        /// <param name="uri"></param>
        /// <param name="maxNumberOfFiles"></param>
        /// <param name="filesInProcess"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        private List<FileEntry> randomDir(string fileMask, string uri, int maxNumberOfFiles, ArrayList filesInProcess, bool trace)
        {
            List<FileEntry> fileEntries = new List<FileEntry>();
            ArrayList checkedOutFiles = new ArrayList();
            string remotePath = string.Empty;
            long size;
            bool isDirectory = false;
            string fileName = string.Empty;
            DateTime fileLastWriten;
            int index;
            TimeSpan ts;

            try
            {
                Tamir.SharpSsh.java.util.Vector entries = _sftp.Dir2(fileMask);

                System.Random autoRand = new System.Random();

                while (entries.Count > 0 && (fileEntries.Count < maxNumberOfFiles || maxNumberOfFiles == 0))
                {
                    index = autoRand.Next(entries.Count - 1);
                    Tamir.SharpSsh.jsch.ChannelSftp.LsEntry entry = (Tamir.SharpSsh.jsch.ChannelSftp.LsEntry)entries[index];

                    remotePath = Path.GetDirectoryName(fileMask);
                    size = entry.getAttrs().getSize();
                    isDirectory = entry.getAttrs().isDir();
                    fileName = entry.getFilename();

                    if (isDirectory)
                    {
                        entries.RemoveAt(index);
                        continue;
                    }

                    try
                    {
                        fileLastWriten = DateTime.Parse(entry.getAttrs().getMtimeString());
                    }
                    catch { fileLastWriten = DateTime.Now.AddMinutes(1); }
                    ts = DateTime.Now.Subtract(fileLastWriten);

                    try
                    {
                        if (ts.TotalSeconds > TOTALTIMEDIFF &&
                            !filesInProcess.Contains(CommonFunctions.CombinePath(remotePath, fileName)))
                        {
                            // "Check out" file if UseLoadBalancing == true 
                            if (uri != null && !DataBaseHelper.CheckOutFile(uri, System.Environment.MachineName, fileName, trace))
                            {
                                entries.RemoveAt(index);
                                continue;
                            }
                            else
                            {
                                // Check that file still exists
                                string filePath = fileMask.Substring(0, fileMask.LastIndexOf("/") + 1) + fileName;
                                if (_sftp.Exists(filePath))
                                    fileEntries.Add(new FileEntry(fileName, size));
                                else
                                {
                                    if (uri != null)
                                        DataBaseHelper.CheckInFile(uri, fileName, trace);
                                }
                            }
                        }
                    }
                    catch
                    {
                        if (uri != null)
                            DataBaseHelper.CheckInFile(uri, fileName, trace);
                    }
                    entries.RemoveAt(index);
                }

                Trace.WriteLine(string.Format("[SftpReceiverEndpoint] Found {0} files.", fileEntries.Count));
                return fileEntries;
            }
            catch (Exception ex)
            {
                throw new SftpException("Unable to perform directory list", ex);
            }
            finally
            {
                RaiseOnDisconnect();
            }
        }

        private FileEntry getFileEntry(string filePath, bool trace)
        {
            try
            {
                if (trace)
                    Trace.WriteLine("[SftpReceiverEndpoint] GetFileEntry(" + filePath + ")");

                foreach (Tamir.SharpSsh.jsch.ChannelSftp.LsEntry entry in _sftp.Dir2(filePath))
                {
                    long size = entry.getAttrs().getSize();
                    string fileName = entry.getFilename();
                    return new FileEntry(fileName, size);
                }

                if (trace)
                    Trace.WriteLine("[SftpReceiverEndpoint] File not Found \"" + filePath + "\".");

                throw new SftpException("File not Found \"" + filePath + "\"."); ;
            }
            catch (Exception ex)
            {
                throw new SftpException("Unable to perform directory list for \"" + filePath + "\".", ex);
            }
            finally
            {
                RaiseOnDisconnect();
            }
        }
        #endregion
        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public void Dispose()
        {
        
        }
        #endregion
        #region SshTransfer
        class SshTransfer : SshTransferProtocolBase
        {
            public SshTransfer(string sftpHost, string user, string password)
                : base(sftpHost, user, password)
            {
                
            }

            protected override string ChannelType
            {
                get { return "sftp"; }
            }

            public ChannelSftp SftpChannel
            {
                get { return (ChannelSftp)m_channel; }
            }

            ////Get
            //public Stream GetStream(string fromFilePath)
            //{
            //    MemoryOutputStream memStream = new MemoryOutputStream("");
            //    SftpChannel.get(fromFilePath, memStream);
            //    return memStream.GetMemoryStream();
            //}

            //Get
            public Stream GetStream(string fromFilePath)
            {
                MemoryOutputStream ms = new MemoryOutputStream(SftpChannel, fromFilePath);
                ms.Load();
                
                return ms.InnerStream;
            }

            //Put
            public void PutStream(System.IO.Stream memStream, string destination)
            {
                SftpChannel.put(new MemoryInputStream(memStream), destination);
            }

            //MkDir
            public override void Mkdir(string directory)
            {

                SftpChannel.mkdir(directory);
            }

            // Dir
            public Tamir.SharpSsh.java.util.Vector Dir2(string path)
            {
                try
                {
                    return SftpChannel.ls(path);
                }
                catch (Exception ex)
                {

                    if (path.IndexOf("*") != -1 || path.IndexOf("?") != -1)
                        throw ex;
                    else
                        return new Tamir.SharpSsh.java.util.Vector();

                    
                }
            }
            // Exists
            public bool Exists(string fileName)
            {
                return Dir2(fileName).Count > 0;
            }

            // Rename
            public void Rename(string oldName, string newName)
            {
                SftpChannel.rename(oldName, newName);
            }

            // Delete
            public void Delete(string path)
            {
                Trace.WriteLine("[SftpReceiverEndpoint] Delete " + path);
                SftpChannel.rm(path);
            }

            
            public override void Get(string fromFilePath, string toFilePath)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public override void Put(string fromFilePath, string toFilePath)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public override void Close()
            {
                base.Close();
            }

            public override void Cancel()
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
        #endregion
    }

}
