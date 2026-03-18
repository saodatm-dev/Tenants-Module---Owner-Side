namespace Core.Infrastructure.Pdf;

/// <summary>
/// Stream wrapper that owns and disposes HttpResponseMessage and MultipartFormDataContent
/// when the stream is disposed.
/// </summary>
internal sealed class DisposableStream(Stream innerStream, HttpResponseMessage response, MultipartFormDataContent form) : Stream
{
    private readonly Stream _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
    private readonly HttpResponseMessage _response = response ?? throw new ArgumentNullException(nameof(response));
    private readonly MultipartFormDataContent _form = form ?? throw new ArgumentNullException(nameof(form));
    private bool _disposed;

    #region Stream Implementation

    public override bool CanRead => !_disposed && _innerStream.CanRead;
    public override bool CanSeek => !_disposed && _innerStream.CanSeek;
    public override bool CanWrite => !_disposed && _innerStream.CanWrite;
    public override long Length => _innerStream.Length;
    
    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }

    public override void Flush()
    {
        ThrowIfDisposed();
        _innerStream.Flush();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        return _innerStream.FlushAsync(cancellationToken);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ThrowIfDisposed();
        return _innerStream.Read(buffer, offset, count);
    }

    public override int Read(Span<byte> buffer)
    {
        ThrowIfDisposed();
        return _innerStream.Read(buffer);
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        return _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return _innerStream.ReadAsync(buffer, cancellationToken);
    }

    public override int ReadByte()
    {
        ThrowIfDisposed();
        return _innerStream.ReadByte();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        ThrowIfDisposed();
        return _innerStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        ThrowIfDisposed();
        _innerStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        ThrowIfDisposed();
        _innerStream.Write(buffer, offset, count);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        ThrowIfDisposed();
        _innerStream.Write(buffer);
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        return _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return _innerStream.WriteAsync(buffer, cancellationToken);
    }

    public override void WriteByte(byte value)
    {
        ThrowIfDisposed();
        _innerStream.WriteByte(value);
    }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        ThrowIfDisposed();
        return _innerStream.BeginRead(buffer, offset, count, callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
        ThrowIfDisposed();
        return _innerStream.EndRead(asyncResult);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        ThrowIfDisposed();
        return _innerStream.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
        ThrowIfDisposed();
        _innerStream.EndWrite(asyncResult);
    }

    public override void CopyTo(Stream destination, int bufferSize)
    {
        ThrowIfDisposed();
        _innerStream.CopyTo(destination, bufferSize);
    }

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        return _innerStream.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    #endregion

    #region Disposal

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                try
                {
                    _innerStream.Dispose();
                }
                finally
                {
                    try
                    {
                        _response.Dispose();
                    }
                    finally
                    {
                        _form.Dispose();
                    }
                }
            }
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    public override async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            try
            {
                await _innerStream.DisposeAsync().ConfigureAwait(false);
            }
            finally
            {
                try
                {
                    _response.Dispose();
                }
                finally
                {
                    _form.Dispose();
                }
            }
            _disposed = true;
        }
        await base.DisposeAsync().ConfigureAwait(false);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    #endregion
}
