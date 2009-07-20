using System;
using System.Runtime.Serialization;
using Blogical.Shared.Adapters.Common;

namespace Blogical.Shared.Adapters.Sftp
{
	internal class SftpException : ApplicationException
	{
		public static string UnhandledTransmit_Error = "The Sftp Adapter encounted an error transmitting a batch of messages.";

        public SftpException () { }

		public SftpException (string msg) : base(msg) { }

		public SftpException (Exception inner) : base(String.Empty, inner) { }

		public SftpException (string msg, Exception e) : base(msg, e) { }

        protected SftpException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}

