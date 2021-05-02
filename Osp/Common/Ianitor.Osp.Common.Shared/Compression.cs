using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Ianitor.Osp.Common.Shared
{
    public static class Compression
    {
        public static async Task ExtractFileFromZipAsync(this Stream _this, string contentType,
            string fileExtension, string targetFile)
        {
            if (contentType.ToLower() != "application/zip")
                throw new NotSupportedException($"'{contentType}' not a supported content type.");

            using (ZipArchive zipArchive = new ZipArchive(_this))
            {
                var entry = zipArchive.Entries.FirstOrDefault(x =>
                    Path.GetExtension(x.Name).ToLower() == fileExtension.ToLower());
                if (entry == null)
                {
                    throw new NotSupportedException($"No file extension '{fileExtension}' was found in zip archive.");
                }

                var archiveFileStream = entry.Open();

                using (StreamWriter streamWriter = new StreamWriter(targetFile))
                {
                    await archiveFileStream.CopyToAsync(streamWriter.BaseStream);
                }
            }
        }

        public static async Task PackFileToZipAsync(this Stream _this, string fileNameWithExtension,
            MemoryStream zipArchiveStream)
        {
            using (ZipArchive zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Create))
            {
                var archiveEntry = zipArchive.CreateEntry(fileNameWithExtension);
                using (var stream = archiveEntry.Open())
                {
                    await _this.CopyToAsync(stream);
                }
            }
        }
    }
}