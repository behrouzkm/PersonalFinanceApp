using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalFinanceApp.Application.Common.Interfaces;

namespace PersonalFinanceApp.Infrastructure.Services;

public sealed class FileContentValidator : IFileContentValidator
{
    private static readonly IReadOnlyDictionary<string, byte[]> Signatures =
        new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = [0xFF, 0xD8, 0xFF],
            ["image/png"] =
                [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A],
            ["application/pdf"] = [0x25, 0x50, 0x44, 0x46]
        };

    public async Task<bool> IsValidAsync(
        Stream stream,
        string contentType,
        CancellationToken cancellationToken)
    {
        if (!stream.CanRead ||
            !Signatures.TryGetValue(contentType, out var signature))
        {
            return false;
        }

        if (!stream.CanSeek)
            throw new InvalidOperationException(
                "The stream must support seeking.");

        var originalPosition = stream.Position;

        try
        {
            stream.Position = 0;

            var buffer = new byte[signature.Length];
            var bytesRead = await stream.ReadAsync(
                buffer,
                cancellationToken);

            if (bytesRead != signature.Length)
                return false;

            return buffer.AsSpan().SequenceEqual(signature);
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }
}
