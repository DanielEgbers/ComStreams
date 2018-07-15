using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ComStreams
{
    public class ComStream : IComStream
    {
        private Stream _base;
        private bool _isOwnerOfBase;
        
        public ComStream(Stream @base, bool takeOwnership = false)
        {
            _base = @base ?? throw new ArgumentNullException(nameof(@base));
            _isOwnerOfBase = takeOwnership;
        }

        public void Dispose()
        {
            if (_isOwnerOfBase)
            {
                _base.Dispose();
            }
        }

        public void Read(byte[] buffer, int count, IntPtr readCountPointer)
        {
            long readCount = _base.Read(buffer, 0, count);
            if (readCountPointer != IntPtr.Zero)
            {
                Marshal.WriteInt64(readCountPointer, readCount);
            }
        }

        public void Write(byte[] buffer, int count, IntPtr writtenCountPointer)
        {
            _base.Write(buffer, 0, count);
            if (writtenCountPointer != IntPtr.Zero)
            {
                Marshal.WriteInt64(writtenCountPointer, count);
            }
        }

        public void CopyTo(IStream destination, long count, IntPtr readCountPointer, IntPtr writtenCountPointer)
        {
            var buffer = new byte[count];

            long readCount = _base.Read(buffer, 0, buffer.Length);
            if (readCountPointer != IntPtr.Zero)
            {
                Marshal.WriteInt64(readCountPointer, readCount);
            }

            destination.Write(buffer, buffer.Length, writtenCountPointer);
        }

        public void Seek(long offset, int loc, IntPtr newPositionPointer)
        {
            var newPosition = _base.Seek(offset, (SeekOrigin)loc);
            if (newPositionPointer != IntPtr.Zero)
            {
                Marshal.WriteInt64(newPositionPointer, newPosition);
            }
        }

        public void SetSize(long length)
        {
            _base.SetLength(length);
        }

        public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG stat, int statFlag)
        {
            stat = new System.Runtime.InteropServices.ComTypes.STATSTG
            {
                cbSize = _base.Length
            };
        }

        public void Clone(out IStream ppstm) =>
            throw new NotSupportedException();

        public void Revert() =>
            throw new NotSupportedException();

        public void Commit(int grfCommitFlags) =>
            throw new NotSupportedException();

        public void LockRegion(long libOffset, long cb, int dwLockType) =>
            throw new NotSupportedException();

        public void UnlockRegion(long libOffset, long cb, int dwLockType) =>
            throw new NotSupportedException();
    }
}
