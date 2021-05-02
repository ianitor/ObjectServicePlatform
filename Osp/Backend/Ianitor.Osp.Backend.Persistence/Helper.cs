using System;
using System.IO;

namespace Ianitor.Osp.Backend.Persistence
{
    internal static class Helper
    {
        internal static string AssemblyDirectory
        {
            get
            {
                string codeBase = typeof(Helper).Assembly.CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
