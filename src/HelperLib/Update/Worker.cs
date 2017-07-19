using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Verloka.HelperLib.Update
{
    public static class Worker
    {
        public async static Task Archive(string path, string save)
        {
            await Task.Run(() => ZipFile.CreateFromDirectory(path, save, CompressionLevel.Optimal, false, Encoding.UTF8));
        }
        public async static Task Unarchive(string path, string save)
        {
            if (!Directory.Exists(save))
                Directory.CreateDirectory(save);

            await Task.Run(() => ZipFile.ExtractToDirectory(path, save, Encoding.UTF8));
        }
    }
}
