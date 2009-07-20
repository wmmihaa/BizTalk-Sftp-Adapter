using System;
using System.Xml;
using Microsoft.BizTalk.Component.Interop;
using Blogical.Shared.Adapters.Common;
using System.Diagnostics;
using Blogical.Shared.Adapters.Sftp.ConnectionPool;

namespace Blogical.Shared.Adapters.Sftp
{
    /// <summary>
    /// Main class for Sftp receive adapters. It provides the implementations of
    /// core interfaces needed to comply with receiver adapter contract.
    /// (1) This class is actually a Singleton. That is there will only ever be one
    /// instance of it created however many locations of this type are actually defined.
    /// (2) Individual locations are identified by a URI and are associated with SftpReceiverEndpoint
    /// (3) It is legal to have messages from different locations submitted in a single
    /// batch and this may be an important optimization. And this is fundamentally why
    /// the Receiver is a singleton.
    /// </summary>
    public class SftpReceiver : Receiver 
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SftpReceiver()
            : base(
            "Sftp Receive Adapter",
            "1.0",
            "Submits files from ssh server into BizTalk",
            "Blogical.Shared.Adapters.Sftp",
            new Guid("a55cbe83-f168-435a-b63e-a506df52e7ef"),
            "http://schemas.microsoft.com/BizTalk/2006/sftp-properties",
           typeof(SftpReceiverEndpoint))
        {
        }
        /// <summary>
        /// This function is called when BizTalk runtime gives the handler properties to adapter.
        /// </summary>
        protected override void HandlerPropertyBagLoaded ()
        {
            IPropertyBag config = this.HandlerPropertyBag;
            if (null != config)
            {
                
                XmlDocument handlerConfigDom = ConfigProperties.IfExistsExtractConfigDom(config);
                if (null != handlerConfigDom)
                {
                    SftpReceiveProperties.ReceiveHandlerConfiguration(handlerConfigDom);
                }
            }
        }
        /// <summary>
        /// Overrides Initialize to show trace information
        /// </summary>
        /// <param name="transportProxy"></param>
        public override void Initialize(Microsoft.BizTalk.TransportProxy.Interop.IBTTransportProxy transportProxy)
        {
            
            Trace.WriteLine("[SftpReceiveAdapter] Adapter Initializing...");
            base.Initialize(transportProxy);
            Trace.WriteLine("[SftpReceiveAdapter] Adapter Initialized");
        }
        /// <summary>
        /// Overrides Terminate to show trace information
        /// </summary>
        public override void Terminate()
        {
            Trace.WriteLine("[SftpReceiveAdapter] Adapter Terminates...");
            try
            {
                SftpConnectionPool.Dispose();
                base.Terminate();
            }
            catch(Exception ex)
            {
                ExceptionHandling.HandleComponentException(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
            Trace.WriteLine("[SftpReceiveAdapter] Adapter Terminated");
        }

    }
}
