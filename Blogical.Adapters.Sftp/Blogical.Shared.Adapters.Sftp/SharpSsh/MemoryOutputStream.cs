using System;
using System.Collections.Generic;
using System.Text;
using Tamir.SharpSsh.jsch;
using Microsoft.BizTalk.Streaming;
using Tamir.SharpSsh.java.io;
using System.IO;

namespace Blogical.Shared.Adapters.Sftp.SharpSsh
{
    class MemoryOutputStream : OutputStream, IDisposable
    {
        #region Private Members
        ChannelSftp _channel;
        string _fileName;
        bool _isPopulated = false;
        VirtualStream _stream = new VirtualStream(VirtualStream.MemoryFlag.AutoOverFlowToDisk);
        #endregion
        #region Constructors
        public MemoryOutputStream(ChannelSftp channel, string fileName)
        {
            this._channel = channel;
            this._fileName = fileName;
        }
        
        #endregion
        #region Public Methods
        public void Load()
        {
            _channel.get(_fileName, this);
            _isPopulated = true;
            _stream.Position = 0;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            this._stream.Write(buffer, offset, count);
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!_isPopulated)
            {
                _channel.get(_fileName, this);
                _isPopulated = true;
                _stream.Position = 0;
            }

            return this._stream.Read(buffer, offset, count);
        }
        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            return this._stream.Seek(offset, origin);
        }
        public override void Flush()
        {
            this._stream.Flush();
        }
        public override void Close()
        {
            this._stream.Close();
        }
        public override int ReadByte()
        {
            return this._stream.ReadByte();
        }
        public override void SetLength(long value)
        {
            this._stream.SetLength(value);
        }
        #endregion
        #region Public Members
        public VirtualStream InnerStream
        {
            get { return _stream; }
        }
        public override long Position
        {
            get
            {
                return this._stream.Position;
            }
            set
            {
                this._stream.Position = value;
            }
        }
        public override long Length
        {
            get
            {
                return this._stream.Length;
            }
        }
        public override bool CanSeek
        {
            get
            {
                return this._stream.CanSeek;
            }
        }
        public override bool CanRead
        {
            get
            {
                return this._stream.CanRead;
            }
        }
        public override bool CanWrite
        {
            get
            {
                return this._stream.CanWrite;
            }
        }
        #endregion
        #region IDisposable Members

        void IDisposable.Dispose()
        {
            _stream.Dispose();
        }

        #endregion
    }
}
