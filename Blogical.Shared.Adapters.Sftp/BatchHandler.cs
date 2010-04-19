using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.BizTalk.TransportProxy.Interop;
using System.Collections;
using Blogical.Shared.Adapters.Common;
using Microsoft.BizTalk.Message.Interop;
using System.Diagnostics;
using System.IO;
using Microsoft.BizTalk.Streaming;

namespace Blogical.Shared.Adapters.Sftp
{
    /// <summary>
    /// The BatchHandler is only used by the SftpReceiverEndpoint for preparing and submitting the batch to BizTalk
    /// </summary>
    internal class BatchHandler
    {
        #region Delegates
        public delegate void BatchHandlerDelegate(ISftp sftp);
        #endregion
        #region Constants
        private const string MESSAGE_BODY = "body";
        private const string REMOTEFILENAME = "FileName";
        private const string EMPTYBATCHFILENAME = "EmptyBatch.xml";
        #endregion
        #region Private Fields
        object _perfCounterObject = null;
        ISftp _sftp = null;
        string _transportType;
        string _propertyNamespace;
        bool _useLoadBalancing;
        IBTTransportProxy _transportProxy;
        ArrayList _filesInProcess = null;
        bool _traceFlag = false;
        List<BatchMessage> _files = new List<BatchMessage>();

        #endregion
        #region Constructor
        internal BatchHandler(ISftp sftp, string propertyNamespace, string transportType, IBTTransportProxy transportProxy, bool traceFlag, bool useLoadBalancing)
        {
            this._sftp = sftp;
            this._propertyNamespace = propertyNamespace;
            this._transportType = transportType;
            this._transportProxy = transportProxy;
            this._traceFlag = traceFlag;
            this._useLoadBalancing = useLoadBalancing;
        }
        #endregion
        #region Public Members
        public List<BatchMessage> Files { get { return _files; } }
        #endregion
        #region Public Methods
        internal void SubmitFiles(ControlledTermination control, ArrayList filesInProcess)
        {
            if (Files == null || Files.Count == 0)
                return;

            this._filesInProcess = filesInProcess;

            try
            {
                using (SyncReceiveSubmitBatch batch = new SyncReceiveSubmitBatch(this._transportProxy, control, Files.Count))
                {
                    ArrayList transportFailures = new ArrayList();
                    foreach (BatchMessage file in Files)
                    {
                        batch.SubmitMessage(file.Message, file.UserData);
                    }
                    batch.Done();

                    TraceMessage("[SftpReceiverEndpoint] SubmitFiles (firstAttempt) about to wait on BatchComplete");
                    if (batch.Wait())
                    {
                        TraceMessage("[SftpReceiverEndpoint] SubmitFiles (firstAttempt) overall success");
                        OnBatchComplete(true);
                    }
                }
                TraceMessage("[SftpReceiverEndpoint] Leaving SubmitFiles");
            }
            catch (Exception ex)
            {
                throw ExceptionHandling.HandleComponentException(
                    EventLogEventIDs.UnableToSubmitBizTalkMessage,
                    System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
        }
        /// <summary>
        /// Submitting a batch of files to BizTalk
        /// </summary>
        /// <param name="control"></param>
        /// <param name="filesInProcess"></param>
        internal void _SubmitFiles(ControlledTermination control, ArrayList filesInProcess)
        {
            try
            {
                if (Files == null || Files.Count == 0)
                    return;

                this._filesInProcess = filesInProcess;

                TraceMessage(string.Format("[SftpReceiverEndpoint] SubmitFiles called. Submitting a batch of {0} files to BizTalk.", Files.Count));

                // This class is used to track the files associated with this ReceiveBatch. The
                // OnBatchComplete will be raised when BizTalk has consumed the message.
                using (ReceiveBatch batch = new ReceiveBatch(this._transportProxy, control, this.OnBatchComplete, Files.Count))
                {
                    foreach (BatchMessage file in Files)
                    {
                        // Submit file to batch
                        batch.SubmitMessage(file.Message, file.UserData);

                    }
                    batch.Done(null);
                }
            }
            catch (Exception e)
            {
                throw ExceptionHandling.HandleComponentException(System.Reflection.MethodBase.GetCurrentMethod(),
                    new SftpException("Could not submit files to BTS", e));
            }
        }
        /// <summary>
        /// (1) Gets the file from the sftp host
        /// (2) Creates a IBaseMessage
        /// (3) Sets varius properties such as uri, messagepart, transporttype etc
        /// (4) Adds the message to the batch
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="uri"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        internal IBaseMessage CreateMessage(string fileName, string uri, long size,
            SftpReceiveProperties.AfterGetActions afterGetAction, string afterGetFilename)
        {
            Stream stream;
            IBaseMessage message = null;
            try
            {
                TraceMessage("[SftpReceiverEndpoint] Reading file to stream " + fileName);

                // Retrieves the message from sftp server.
                stream = this._sftp.Get(fileName);
                stream.Position = 0;


                // Creates new message
                IBaseMessageFactory messageFactory = this._transportProxy.GetMessageFactory();
                IBaseMessagePart part = messageFactory.CreateMessagePart();
                part.Data = stream;
                message = messageFactory.CreateMessage();
                message.AddPart(MESSAGE_BODY, part, true);

                // Setting metadata
                SystemMessageContext context = new SystemMessageContext(message.Context);
                context.InboundTransportLocation = uri;
                context.InboundTransportType = this._transportType;

                // Write/Promote any adapter specific properties on the message context
                message.Context.Write(REMOTEFILENAME, this._propertyNamespace, (object)fileName);

                SetReceivedFileName(message, fileName);

                message.Context.Write("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/" +
                    this._transportType.ToLower() + "-properties", fileName);

                message.Context.Write("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/file-properties", fileName);

                // Add the file to the batch
                this.Files.Add(new BatchMessage(message, fileName, BatchOperationType.Submit, afterGetAction, afterGetFilename));

                // Add the size of the file to the stream
                if (message.BodyPart.Data.CanWrite)
                    message.BodyPart.Data.SetLength(size);

                return message;
            }
            catch (Exception ex)
            {
                TraceMessage("[SftpReceiverEndpoint] Error Adding file [" + fileName + "]to batch. Error: " + ex.Message);

                if (this._useLoadBalancing)
                    DataBaseHelper.CheckInFile(uri, Path.GetFileName(fileName), this._traceFlag);

                return null;
            }
        }
        /// <summary>
        /// Creates a new message with some notification description, 
        /// and adds it to the BatchMessage
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        internal IBaseMessage CreateEmptyBatchMessage(string uri)
        {
            IBaseMessage message = null;
            try
            {

                string errorMessageFormat = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Error message=\"Empty Batch\" datetime=\"{0}\" source=\"{1}\"/>";
                string errorMessage = String.Format(errorMessageFormat, DateTime.Now.ToString(), uri);

                UTF8Encoding utf8Encoding = new UTF8Encoding();
                byte[] messageBuffer = utf8Encoding.GetBytes(errorMessage);

                MemoryStream ms = new MemoryStream(messageBuffer.Length);
                ms.Write(messageBuffer, 0, messageBuffer.Length);
                ms.Position = 0;

                ReadOnlySeekableStream ross = new ReadOnlySeekableStream(ms);

                IBaseMessageFactory messageFactory = this._transportProxy.GetMessageFactory();
                IBaseMessagePart part = messageFactory.CreateMessagePart();
                part.Data = ross;
                message = messageFactory.CreateMessage();
                message.AddPart(MESSAGE_BODY, part, true);

                SystemMessageContext context = new SystemMessageContext(message.Context);
                context.InboundTransportLocation = uri;
                context.InboundTransportType = this._transportType;

                //Write/Promote any adapter specific properties on the message context
                message.Context.Write(REMOTEFILENAME, this._propertyNamespace, EMPTYBATCHFILENAME);

                // Add the file to the batch
                this.Files.Add(new BatchMessage(message, EMPTYBATCHFILENAME, BatchOperationType.Submit));

                // Add the size of the file to the stream
                message.BodyPart.Data.SetLength(ms.Length);
                ms.Close();
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
        #region Private Methods

        private void SetReceivedFileName(IBaseMessage pInMsg, string receivedFilename)
        {
            SystemMessageContext messageContext = new SystemMessageContext(pInMsg.Context);

            pInMsg.Context.Write("ReceivedFileName",
                Constants.BIZTALK_FILE_PROPERTIES_NAMESPACE, receivedFilename);

            pInMsg.Context.Write("ReceivedFileName",
                Constants.SFTP_ADAPTER_PROPERTIES_NAMESPACE, receivedFilename);

            pInMsg.Context.Write("ReceivedFileName",
                "http://schemas.microsoft.com/BizTalk/2003/" +
                    messageContext.InboundTransportType.ToLower() + "-properties",
                receivedFilename);
        }
        private void TraceMessage(string message)
        {
            if (this._traceFlag)
                Trace.WriteLine(message);
        }
        #endregion
        #region Events
        /// <summary>
        /// Called when the BizTalk Batch has been submitted.  If all the messages were submitted (good or suspended)
        /// we delete the files from the folder
        /// </summary>
        /// <param name="overallStatus"></param>
        internal void OnBatchComplete(bool overallStatus)
        {
            string fileName = "Could not get fileName";
            try
            {
                if (overallStatus == true) //Batch completed
                {
                    lock (this._filesInProcess)
                    {

                        //Delete the files
                        foreach (BatchMessage batchMessage in Files)
                        {
                            try
                            {
                                //Close the stream so we can delete this file
                                batchMessage.Message.BodyPart.Data.Close();
                                fileName = batchMessage.UserData.ToString();

                                // Delete orginal file  
                                if (fileName != EMPTYBATCHFILENAME)
                                {
                                    if (batchMessage.AfterGetAction == SftpReceiveProperties.AfterGetActions.Delete)
                                        this._sftp.Delete(fileName);
                                    else // rename
                                    {
                                        string renameFileName = CommonFunctions.CombinePath(Path.GetDirectoryName(fileName), batchMessage.AfterGetFilename);
                                        renameFileName = renameFileName.Replace("%SourceFileName%", Path.GetFileName(fileName));
                                        /* John C. Vestal 2010/04/07 - Added DateTime and UniversalDateTime to macro list. */
                                        if (renameFileName.IndexOf("%DateTime%") > -1)
                                        {
                                            string dateTime = DateTime.Now.ToString();

                                            renameFileName = renameFileName.Replace("%DateTime%", dateTime);
                                            renameFileName = renameFileName.Replace("/", "-");
                                        }
                                        if (renameFileName.IndexOf("%UniversalDateTime%") > -1)
                                        {
                                            string dateTime = DateTime.Now.ToUniversalTime().ToString();

                                            renameFileName = renameFileName.Replace("%UniversalDateTime%", dateTime);
                                            renameFileName = renameFileName.Replace("/", "-");
                                        }

                                        this._sftp.Rename(fileName, renameFileName);
                                    }
                                }

                                // Remove filename from _filesInProcess
                                this._filesInProcess.Remove(fileName);

                                if (this._useLoadBalancing)
                                {
                                    string uri = batchMessage.Message.Context.Read(Constants.BizTalkSystemPropertyNames.INBOUNDTRANSPORTLOCATION, Constants.BIZTALK_SYSTEM_PROPERTIES_NAMESPACE).ToString();
                                    DataBaseHelper.CheckInFile(uri, Path.GetFileName(fileName), this._traceFlag);
                                }
                            }
                            catch (Exception ex)
                            {
                                TraceMessage(string.Format("[SftpReceiverEndpoint] ERROR: Could not remove {0} from its location.", ex.Message));
                            }
                        }
                    }
                    if (BatchComplete != null)
                        BatchComplete(this._sftp);


                    TraceMessage(string.Format("[SftpReceiverEndpoint] OnBatchComplete called. overallStatus == {0}.", overallStatus));
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("[SftpReceiverEndpoint] OnBatchComplete EXCEPTION!"));
                this._filesInProcess.Remove(fileName);
                throw ExceptionHandling.HandleComponentException(System.Reflection.MethodBase.GetCurrentMethod(), e);
            }
            finally
            {

            }
        }
        internal event BatchHandlerDelegate BatchComplete;
        #endregion
    }
}
