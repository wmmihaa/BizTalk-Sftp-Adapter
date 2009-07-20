using System;
using System.Collections.Generic;
using System.Text;

namespace Blogical.Shared.Adapters.Common
{
    /// <summary>
    /// This class contains shared constants that can/will be used in different projects.
    /// </summary>
    public static class Constants
    {

        #region Public Constants
        
        /// <summary>
        /// The namespace to BizTalk file context properties.
        /// </summary>
        public const string BIZTALK_FILE_PROPERTIES_NAMESPACE = "http://schemas.microsoft.com/BizTalk/2003/file-properties";

        /// <summary>
        /// The namespace to BizTalk system context properties.
        /// </summary>
        public const string BIZTALK_SYSTEM_PROPERTIES_NAMESPACE = "http://schemas.microsoft.com/BizTalk/2003/system-properties";

        /// <summary>
        /// The namespace to SFTP adapter properties.
        /// </summary>
        public const string SFTP_ADAPTER_PROPERTIES_NAMESPACE = "http://schemas.microsoft.com/BizTalk/2006/sftp-properties";

        #endregion

        #region BizTalkSystemPropertyNames

        /// <summary>
        /// This class contains BizTalk message context system property-names.
        /// </summary>
        public static class BizTalkSystemPropertyNames
        {
            /// <summary>
            /// Send port name.
            /// </summary>
            public const string SEND_PORT_NAME = "SPName";
            /// <summary>
            /// ReceivePortID.
            /// </summary>
            public const string RECEIVE_PORT_ID = "ReceivePortID";

            /// <summary>
            /// Inbound transport location
            /// </summary>
            public const string INBOUNDTRANSPORTLOCATION = "InboundTransportLocation";

            /// <summary>
            /// Interchange ID
            /// </summary>
            public const string INTERCHANGE_ID = "InterchangeID";

            /// <summary>
            /// Actual retry count
            /// </summary>
            public const string ACTUAL_RETRY_COUNT = "ActualRetryCount";

            /// <summary>
            /// ACK / NACK
            /// </summary>
            public const string ACK_TYPE = "AckType";

            /// <summary>
            /// Failure description
            /// </summary>
            public const string ACK_DESCRIPTION = "AckDescription";

            /// <summary>
            /// Originating send port 
            /// </summary>
            public const string ACK_SEND_PORT_NAME = "AckSendPortName";

            /// <summary>
            /// Receive port name
            /// </summary>
            public const string ACK_RECEIVE_PORT_NAME = "AckReceivePortName";

            /// <summary>
            /// SendPortID.
            /// </summary>
            public const string SEND_PORT_ID = "SPID";
        }

        #endregion
    }

    /// <summary>
    /// Class containing constants holding connection-string keys.
    /// </summary>
    public static class ConnectionStringKeys
    {
        #region ConnectionStringConstants
        /// <summary>
        /// Key to connection-string for tracking database
        /// </summary>
        public const string BLOGICALDB = "BlogicalDB";

        #endregion
    }

    /// <summary>
    /// IMPORTANT!
    /// The Sources that are added to this class, must also be added in the installer above.
    /// </summary>
    public static class EventLogSources
    {
        public const string SFTPAdapter = "SFTPAdapter";
    }

    public static class EventLogEventIDs
    {
        // GeneralError
        public const int GeneralUnknownError = 0;

        // Adapters are 1xxx
        public const int UnableToConnectToHost = 1001;
        public const int UnableToListDirectory = 1002;
        public const int UnableToGetFile = 1003;
        public const int UnableToRenameFile = 1004;
        public const int UnableToWriteFile = 1005;

        public const int UnableToCreateBizTalkMessage = 1010;
        public const int UnableToSubmitBizTalkMessage = 1011;

    }

}
