using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Blogical.Shared.Adapters.Sftp
{
    internal static class CommonFunctions
    {
        public static string CombinePath(params string[] parts)
        {
            string path = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                path = Path.Combine(path, parts[i].Trim('/', '\\'));
            }
    
            return path.Replace("\\", "/");
        }
    }
}
