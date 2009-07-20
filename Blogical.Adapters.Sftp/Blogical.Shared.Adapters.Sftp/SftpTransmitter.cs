using System;
using System.Xml;
using Microsoft.BizTalk.Component.Interop;
using Blogical.Shared.Adapters.Common;
using Microsoft.BizTalk.TransportProxy.Interop;
using Blogical.Shared.Adapters.Sftp.ConnectionPool;
using System.Diagnostics;

namespace Blogical.Shared.Adapters.Sftp
{
	/// <summary>
	/// This is a singleton class for Sftp send adapter. All the messages, going to various
	/// send ports of this adapter type, will go through this class.
	/// </summary>
	public class SftpTransmitter : AsyncTransmitter
	{
		/// <summary>
		/// Constructor
		/// </summary>
        public SftpTransmitter()
            : base(
			"Sftp Transmit Adapter",
			"1.0",
			"Send files from BizTalk to ssh server",
            "Blogical.Shared.Adapters.Sftp",
            new Guid("225d6dd3-ea0f-49db-942a-61c078e7c8c0"),
            "Blogical.Shared.Adapters.Sftp",
			typeof(SftpTransmitterEndpoint),
			SftpTransmitProperties.BatchSize)
		{
		}
	
        
        /// <summary>
        /// 
        /// </summary>
		protected override void HandlerPropertyBagLoaded ()
		{
			IPropertyBag config = this.HandlerPropertyBag;
			if (null != config)
			{
				XmlDocument handlerConfigDom = ConfigProperties.IfExistsExtractConfigDom(config);
				if (null != handlerConfigDom)
				{
					SftpTransmitProperties.ReadTransmitHandlerConfiguration(handlerConfigDom);
				}
			}
		}
        public override void Terminate()
        {
            Trace.WriteLine("[SftpTransmitterAdapter] Adapter Terminates...");
            try
            {
                SftpConnectionPool.Dispose();
                base.Terminate();
            }
            catch (Exception ex)
            {
                ExceptionHandling.HandleComponentException(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
            Trace.WriteLine("[SftpTransmitterAdapter] Adapter Terminated");
        }
        
	}
}
