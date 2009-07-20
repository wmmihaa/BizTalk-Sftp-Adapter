using System;
using IO = System.IO;
using Tamir.SharpSsh.java.io;
using Microsoft.BizTalk.Streaming;

namespace Blogical.Shared.Adapters.Sftp.SharpSsh
{

    
    /// <summary>
    /// Summary description for MemoryOutputStream.
    /// </summary>
    class MemoryInputStream : InputStream
    {
        IO.Stream fs;
        public MemoryInputStream(IO.Stream memStream)
        {
            fs = memStream;
        }

        public override void Close()
        {
            fs.Close();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return fs.Read(buffer, offset, count);
        }

        public override bool CanSeek
        {
            get
            {
                return fs.CanSeek;
            }
        }

        public override long Seek(long offset, IO.SeekOrigin origin)
        {
            return fs.Seek(offset, origin);
        }
    }
}

