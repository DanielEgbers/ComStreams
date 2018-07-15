using System.IO;

namespace ComStreams
{
    public class ComMemoryStream : ComStream, IComMemoryStream
    {
        public ComMemoryStream() :
            base(new MemoryStream())
        {

        }

        public ComMemoryStream(int capacity) : 
            base(new MemoryStream(capacity))
        {

        }

        public ComMemoryStream(byte[] buffer) :
            base(new MemoryStream(buffer))
        {

        }

        public ComMemoryStream(byte[] buffer, bool writable) :
            base(new MemoryStream(buffer, writable))
        {

        }

        public ComMemoryStream(byte[] buffer, int index, int count) :
            base(new MemoryStream(buffer, index, count))
        {

        }

        public ComMemoryStream(byte[] buffer, int index, int count, bool writable) :
            base(new MemoryStream(buffer, index, count, writable))
        {

        }

        public ComMemoryStream(byte[] buffer, int index, int count, bool writable, bool publiclyVisible) :
            base(new MemoryStream(buffer, index, count, writable, publiclyVisible))
        {

        }
    }
}