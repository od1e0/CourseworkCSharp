using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MauiC_.Maui.Services
{
    public class StreamContentWrapper : HttpContent
    {
        private readonly Stream _stream;

        public StreamContentWrapper(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return _stream.CopyToAsync(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _stream.Length;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
