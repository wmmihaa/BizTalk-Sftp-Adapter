using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace Blogical.Shared.Adapters.Sftp
{
    /// <summary>
    /// Captures all methods needed sending or receiving files from and to the ssh server.  
    /// </summary>
    public interface ISftp: IDisposable
    {
        /// <summary>
        /// Get a readable stream from a file on the ssh server
        /// </summary>
        /// <param name="fromFilePath">ssh path eg. /home/username/afile.txt</param>
        /// <returns></returns>
        Stream Get(string fromFilePath);
        /// <summary>
        /// Write a file to a ssh sever
        /// </summary>
        /// <param name="memStream"></param>
        /// <param name="destination">ssh destination path eg. /home/username/afile.txt</param>
        void Put(System.IO.Stream memStream, string destination);
        /// <summary>
        /// Renameing or moving a file or files.
        /// </summary>
        /// <param name="oldName">ssh destination path eg. /home/username/TEMP/afile.txt</param>
        /// <param name="newName">ssh destination path eg. /home/username/IN/afile.txt</param>
        void Rename(string oldName, string newName);
        /// <summary>
        /// Returns a FileEntry (name and size)list of files and subdirectories in a directory.
        /// </summary>
        /// <param name="fileMask"></param>
        /// <param name="uri"></param>
        /// <param name="filesInProcess"></param>
        /// <returns></returns>
        List<FileEntry> Dir(string fileMask, string uri, ArrayList filesInProcess,bool trace);
        /// <summary>
        /// Returns a FileEntry (name and size)list of files and subdirectories in a directory.
        /// </summary>
        /// <param name="fileMask"></param>
        /// <param name="uri"></param>
        /// <param name="maxNumberOfFiles"></param>
        /// <param name="filesInProcess"></param>
        /// <returns></returns>
        List<FileEntry> Dir(string fileMask, string uri, int maxNumberOfFiles, ArrayList filesInProcess, bool trace);
        /// <summary>
        /// Determines wether a specified directory has files.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool Exists(string fileName);
        /// <summary>
        /// Deletes one or more files.
        /// </summary>
        /// <param name="filePath"></param>
        void Delete(string filePath);
        /// <summary>
        /// Open an ssh connection
        /// </summary>
        void Connect();
        /// <summary>
        /// Closing the ssh connection
        /// </summary>
        void Disconnect();
        /// <summary>
        /// A numerical representing a permission matrix. These permissions are overridden on Windows platforms, and are therefore useless on such a host. Default value on UNIX platforms are 644. If left empty, no permissioins will be applied.
        /// </summary>
        /// <param name="permissions"></param>
        /// <param name="filePath"></param>
        void ApplySecurityPermissions(string permissions, string filePath);
        /// <summary>
        /// The event is fired when Disconnect is called
        /// </summary>
        event DisconnectEventHandler OnDisconnect;
        /// <summary>
        /// Gets information about a file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        FileEntry GetFileEntry(string filePath, bool trace);
    }
    /// <summary>
    /// Used for ISftp.Dir
    /// </summary>
    public class FileEntry
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="size"></param>
        public FileEntry(string fileName, long size)
        {
            this.FileName = fileName;
            this.Size = size;
        }
        /// <summary>
        /// Name of file. eg Afile.txt
        /// </summary>
        public string FileName;
        /// <summary>
        /// Size in bytes. Later used for setting the size of the stream.
        /// </summary>
        public long Size;
    }

    /// <summary>
    /// Declare a delegate type for diconnection an sftp connection
    /// </summary>
    /// <param name="sftp"></param>
    public delegate void DisconnectEventHandler(ISftp sftp);
}
