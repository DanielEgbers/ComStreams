namespace ComStreams
{
    public interface IComMemoryStreamFactory
    {
        IComMemoryStream Create();
        IComMemoryStream Create(int capacity);
        IComMemoryStream Create(byte[] buffer);
        IComMemoryStream Create(byte[] buffer, bool writable);
        IComMemoryStream Create(byte[] buffer, int index, int count);
        IComMemoryStream Create(byte[] buffer, int index, int count, bool writable);
        IComMemoryStream Create(byte[] buffer, int index, int count, bool writable, bool publiclyVisible);
    }
}
