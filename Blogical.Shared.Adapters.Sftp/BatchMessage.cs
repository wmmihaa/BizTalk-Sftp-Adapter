using System;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.BizTalk.TransportProxy.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Blogical.Shared.Adapters.Sftp
{
    /// <summary>
    /// A BatchMessage represents a file within a batch. Each BatchMessage is added to the BatchHandler.Files
    /// in the BatchHandler.CreateMessage method.
    /// </summary>
	internal class BatchMessage
    {
        #region Private Members
        private IBaseMessage _message;
		private object _userData;
		private string _correlationToken;
		private BatchOperationType _operationType;
        private SftpReceiveProperties.AfterGetActions _aftergetaction;
        private string _aftergetfilename;
        #endregion
        #region Internal Members
        internal IBaseMessage Message
        {
            get { return this._message; }
        }
        internal object UserData
        {
            get { return this._userData; }
        }
        internal string CorrelationToken
        {
            get { return this._correlationToken; }
        }
        internal BatchOperationType OperationType
        {
            get { return this._operationType; }
        }
        internal SftpReceiveProperties.AfterGetActions AfterGetAction
        {
            get { return this._aftergetaction; }
        }
        internal string AfterGetFilename
        {
            get { return this._aftergetfilename; }
        }
        #endregion
        #region Constructors
        internal BatchMessage(IBaseMessage message, object userData, BatchOperationType oppType)
		{
			this._message = message;
			this._userData = userData;
			this._operationType = oppType;
		}
        internal BatchMessage(IBaseMessage message, object userData, BatchOperationType oppType, 
            SftpReceiveProperties.AfterGetActions afterGetAction, string afterGetFilename)
        {
            this._message = message;
            this._userData = userData;
            this._operationType = oppType;
            this._aftergetaction = afterGetAction;
            this._aftergetfilename = afterGetFilename;
        }
        internal BatchMessage(string correlationToken, object userData, BatchOperationType oppType)
		{
			this._correlationToken = correlationToken;
			this._userData = userData;
			this._operationType = oppType;
        }
        #endregion      
    }
}
