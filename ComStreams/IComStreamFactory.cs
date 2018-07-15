using System.IO;

namespace ComStreams
{
    public interface IComStreamFactory
    {
        IComStream Create(Stream stream, bool takeOwnership = false);
    }
}
