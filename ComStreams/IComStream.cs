using System;
using System.Runtime.InteropServices.ComTypes;

namespace ComStreams
{
    public interface IComStream : IStream, IDisposable
    {
        new void Dispose();
        new void Read(byte[] buffer, int count, IntPtr readCountPointer);
        new void Write(byte[] buffer, int count, IntPtr writtenCountPointer);
        new void CopyTo(IStream destination, long count, IntPtr readCountPointer, IntPtr writtenCountPointer);
        new void Seek(long offset, int loc, IntPtr newPositionPointer);
        new void SetSize(long length);
        new void Stat(out STATSTG stat, int statFlag);
        new void Clone(out IStream ppstm);
        new void Revert();
        new void Commit(int grfCommitFlags);
        new void LockRegion(long libOffset, long cb, int dwLockType);
        new void UnlockRegion(long libOffset, long cb, int dwLockType);
    }
}
