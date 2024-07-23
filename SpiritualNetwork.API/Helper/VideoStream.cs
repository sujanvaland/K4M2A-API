namespace SpiritualNetwork.API.Helper
{
    public class VideoStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly int _chunkSize;

        public VideoStream(Stream baseStream, int chunkSize)
        {
            _baseStream = baseStream;
            _chunkSize = chunkSize;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => _baseStream.Length;

        public override long Position
        {
            get => _baseStream.Position;
            set => throw new NotSupportedException("Setting Position is not supported in this stream.");
        }

        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;
            while (bytesRead < _chunkSize)
            {
                int result = _baseStream.Read(buffer, offset + bytesRead, count - bytesRead);
                if (result == 0)
                {
                    return bytesRead == 0 ? 0 : bytesRead;
                }
                bytesRead += result;
            }
            return bytesRead;
            
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
