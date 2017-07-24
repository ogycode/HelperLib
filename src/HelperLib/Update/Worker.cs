/*
 * Worker.cs
 * Verloka Vadim, 2017
 * https://verloka.github.io
 */

using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Verloka.HelperLib.Update
{
    /// <summary>
    /// Static class for zip and unzip files
    /// </summary>
    public static class Worker
    {
        /// <summary>
        /// Create archive (zip files)
        /// </summary>
        /// <param name="path">Path to folder with files</param>
        /// <param name="save">Destination location of save</param>
        /// <returns>Async <see cref="Task"/></returns>
        public async static Task Archive(string path, string save)
        {
            await Task.Run(() => ZipFile.CreateFromDirectory(path, save, CompressionLevel.Optimal, false, Encoding.UTF8));
        }
        /// <summary>
        /// Unzip archive (unzio files)
        /// </summary>
        /// <param name="path">Path to archive</param>
        /// <param name="save">Destination location of save</param>
        /// <returns>Async <see cref="Task"/></returns>
        public async static Task Unarchive(string path, string save)
        {
            if (!Directory.Exists(save))
                Directory.CreateDirectory(save);

            await Task.Run(() => ZipFile.ExtractToDirectory(path, save, Encoding.UTF8));
        }
    }
}
