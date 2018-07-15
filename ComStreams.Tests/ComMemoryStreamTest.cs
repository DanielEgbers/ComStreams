using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;

namespace ComStreams.Tests
{
    [ExcludeFromCodeCoverage]
    public class ComMemoryStreamTest
    {
        private ITestOutputHelper _output;

        private Random _random = new Random();        

        public ComMemoryStreamTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Constructors()
        {
            var testData = new byte[10];
            _random.NextBytes(testData);

            new ComMemoryStream();
            new ComMemoryStream(capacity: 10);
            new ComMemoryStream(testData);
            new ComMemoryStream(testData, writable: true);
            new ComMemoryStream(testData, index: 4, count: 4);
            new ComMemoryStream(testData, index: 4, count: 4, writable: true);
            new ComMemoryStream(testData, index: 4, count: 4, writable: true, publiclyVisible: true);
        }

        [Fact]
        public void WriteAndRead()
        {
            var testData = new byte[10];
            _random.NextBytes(testData);

            _output.WriteLine($"{nameof(testData)}: {string.Join(", ", testData)}");
            
            using (var stream = new ComMemoryStream())
            {
                var writtenCountPointer = Marshal.AllocHGlobal(sizeof(long));
                try
                {
                    var input = testData.ToArray();
                    stream.SetSize(input.Length);
                    stream.Write(input, input.Length, writtenCountPointer);
                    var writtenCount = Marshal.ReadInt32(writtenCountPointer);
                    Assert.True(writtenCount == input.Length);
                }
                finally
                {
                    Marshal.FreeHGlobal(writtenCountPointer);
                }

                var newPositionPointer = Marshal.AllocHGlobal(sizeof(long));
                var readCountPointer = Marshal.AllocHGlobal(sizeof(long));
                try
                {
                    var output = new byte[4];

                    stream.Stat(out var stat, 0);
                    var offset = _random.Next(0, (int)stat.cbSize - output.Length);
                    stream.Seek(offset, (int)SeekOrigin.Begin, newPositionPointer);
                    var newPosition = Marshal.ReadInt32(newPositionPointer);
                    Assert.True(newPosition == offset);

                    stream.Read(output, output.Length, readCountPointer);
                    var readCount = Marshal.ReadInt32(readCountPointer);
                    Assert.True(readCount == output.Length);
                    Assert.Equal(output, testData.Skip(offset).Take(output.Length));
                }
                finally
                {
                    Marshal.FreeHGlobal(readCountPointer);
                    Marshal.FreeHGlobal(newPositionPointer);
                }
            }
        }

        [Fact]
        public void CopyTo()
        {
            var testData = new byte[10];
            _random.NextBytes(testData);

            _output.WriteLine($"{nameof(testData)}: {string.Join(", ", testData)}");
            
            using (var inputStream = new ComMemoryStream(testData.Length))
            using (var outputStream = new ComMemoryStream(testData.Length))
            {
                var writtenCountPointer = Marshal.AllocHGlobal(sizeof(long));
                var readCountPointer = Marshal.AllocHGlobal(sizeof(long));
                try
                {
                    var input = testData.ToArray();
                    inputStream.Write(input, input.Length, IntPtr.Zero);

                    inputStream.Seek(0, (int)SeekOrigin.Begin, IntPtr.Zero);
                    inputStream.CopyTo(outputStream, input.Length, readCountPointer, writtenCountPointer);
                    var readCount = Marshal.ReadInt32(readCountPointer);
                    var writtenCount = Marshal.ReadInt32(writtenCountPointer);
                    Assert.True(readCount == input.Length);
                    Assert.True(writtenCount == input.Length);

                    var output = new byte[input.Length];
                    outputStream.Seek(0, (int)SeekOrigin.Begin, IntPtr.Zero);
                    outputStream.Read(output, output.Length, IntPtr.Zero);
                    Assert.Equal(output, input);
                }
                finally
                {
                    Marshal.FreeHGlobal(readCountPointer);
                    Marshal.FreeHGlobal(writtenCountPointer);
                }
            }
        }

        [Fact]
        public void NotSupportedExceptions()
        {
            Assert.Throws<NotSupportedException>(() => new ComMemoryStream().Clone(out _));
            Assert.Throws<NotSupportedException>(() => new ComMemoryStream().Revert());
            Assert.Throws<NotSupportedException>(() => new ComMemoryStream().Commit(0));
            Assert.Throws<NotSupportedException>(() => new ComMemoryStream().LockRegion(0, 0, 0));
            Assert.Throws<NotSupportedException>(() => new ComMemoryStream().UnlockRegion(0, 0, 0));
        }
    }
}
