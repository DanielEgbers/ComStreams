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
    public class ComStreamTest
    {
        private ITestOutputHelper _output;

        private Random _random = new Random();        

        public ComStreamTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void WriteAndRead()
        {
            var testData = new byte[10];
            _random.NextBytes(testData);

            _output.WriteLine($"{nameof(testData)}: {string.Join(", ", testData)}");

            var baseStream = File.Create($"{nameof(WriteAndRead)}_{Guid.NewGuid()}");
            using (var stream = new ComStream(baseStream, takeOwnership: true))
            {
                var writtenCountPointer = Marshal.AllocHGlobal(sizeof(long));
                try
                {
                    var input = testData.ToArray();
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

            using (var baseInputStream = File.Create($"{nameof(CopyTo)}_{Guid.NewGuid()}"))
            using (var baseOutputStream = File.Create($"{nameof(CopyTo)}_{Guid.NewGuid()}"))
            using (var inputStream = new ComStream(baseInputStream))
            using (var outputStream = new ComStream(baseOutputStream))
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
        public void ArgumentNullExceptions()
        {
            Assert.Throws<ArgumentNullException>(() => new ComStream(null));
        }
    }
}
