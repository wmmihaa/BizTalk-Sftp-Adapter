//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Tamir.SharpSsh.Streams
//{
//    public class NoneBufferedStream : System.IO.Stream
//    {
//        private Tamir.SharpSsh.java.io.JStream _jStream = null;
//        private Tamir.SharpSsh.jsch.ChannelSftp _channel = null;
//        private byte[] _handle = null;
//        private Tamir.SharpSsh.jsch.ChannelSftp.Header _header = null;
//        private Buffer _buffer;

//        public NoneBufferedStream(Tamir.SharpSsh.java.io.JStream jStream, 
//            Tamir.SharpSsh.jsch.ChannelSftp channel, 
//            byte[] handle, 
//            Tamir.SharpSsh.jsch.ChannelSftp.Header header)
//        {
//            this._jStream = jStream;
//            this._channel = channel;
//            this._handle = handle;
//            this._header = header;
//        }

//        public override bool CanRead
//        {
//            get { return this._jStream.CanRead; }
//        }

//        public override bool CanSeek
//        {
//            get { return this._jStream.CanSeek; }
//        }

//        public override bool CanWrite
//        {
//            get { return this._jStream.CanWrite; }
//        }

//        public override void Flush()
//        {
//            this._jStream.Flush();
//        }

//        public override long Length
//        {
//            get { return this._jStream.Length; }
//        }

//        public override long Position
//        {
//            get
//            {
//                return this._jStream.Position;
//            }
//            set
//            {
//                this._jStream.Position = value;
//            }
//        }

//        public override int Read(byte[] buffer, int offset, int count)
//        {
//            int ret = this._jStream.Read(buffer, offset, count);
            
//            if (ret < 0) // Close
//                this._channel.GetComplete(this._handle, this._header);

//            return ret;
//        }

//        public override long Seek(long offset, System.IO.SeekOrigin origin)
//        {
//            this._jStream.Seek(offset, origin);
//        }

//        public override void SetLength(long value)
//        {
//            this._jStream.SetLength(value);
//        }

//        public override void Write(byte[] buffer, int offset, int count)
//        {
//            this._jStream.Write(buffer, offset, count);
//        }
//    }
//}
