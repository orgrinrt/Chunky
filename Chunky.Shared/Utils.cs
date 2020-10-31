using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Chunky.Shared
{
    public static class Utils
    {
        /// <summary>
        /// Just turns the format into an extension.
        /// NOTE that the return value includes the dot `.`
        /// </summary>
        /// <param name="format"></param>
        /// <returns>Extension including the dot (e.g ".png")</returns>
        public static string FormatExtension(ImageFormat format)
        {
            return "." + format.ToString().ToLower();
        }

        public static string SolveAssemblyRootDir(Assembly assembly)
        {
            return Path.Combine(assembly.Location
                .Remove(assembly.Location.Length - SolveDllName(assembly).Length));
        }

        public static string SolveDllName(Assembly assembly)
        {
            return assembly.GetName().ToString().Split(',')[0] + ".dll";
        }

        public static string SolveNameFromFileName(string fileName)
        {
            string[] init = fileName.Split('/', '\\');
            string[] split = init[init.Length - 1].Split('.');
            string result = "";
            
            for (int i = 0; i < split.Length - 1; i++)
            {
                result += split[i];
            }

            return result;
        }

        public static string SolveDirFromFileName(string fileName)
        {
            string[] split = fileName.Split('/', '\\');
            string result = "";
            
            for (int i = 0; i < split.Length - 1; i++)
            {
                result = Path.Combine(result, split[i]);
            }

            return result;
        }

        public static string SolveImageExtensionFromFileName(string fileName)
        {
            string[] split = fileName.Split('.');

            return split[split.Length - 1];
        }

        public static bool PathIsRelative(string path)
        {
            if (IsUnix())
            {
                if (path.StartsWith("/")) return false;
            }
            else
            {
                if (path.Length > 1)
                {
                    if (path[1] == ':') return false;
                }
            }

            return true;
        }

        public static string ParsePath(string path)
        {
            string result;
            
            if (PathIsRelative(path))
            {
                string[] split = path.Split('/', '\\');
                result = Path.GetFullPath(Environment.CurrentDirectory);
                foreach (string node in split)
                {
                    if (node == ".") continue;
                    result = Path.Combine(result, node);
                }
            }
            else
            {
                result = path;
            }

            Console.WriteLine(result);
            bool initialRun = true;
            if (result.Contains(".."))
            {
                string[] split = result.Split("..");
                result = split[0].TrimEnd('/', '\\');
                string[] firstPass = result.Split('/', '\\');
                result = "";
                for (int l = 0; l < firstPass.Length - 1; l++)
                {
                    result = Path.Combine(result, firstPass[l]);
                }
                for (int j = 1; j < split.Length; j++)
                {
                    result += split[j];
                    string[] innerSplit = result.Split('/', '\\');
                    byte length1 = (byte) innerSplit.Length;
                    byte length2 = (byte) (j == split.Length - 1 ? length1 : length1 - 2);
                    for (int i = 0; i < length2; i++)
                    {
                        result = Path.Combine(result, innerSplit[i]);
                    }
                    Console.WriteLine(result);
                    initialRun = false;
                }
            }

            return result;
        }

        public static bool IsUnix()
        {
            int platform = (int) Environment.OSVersion.Platform;
            return platform == 4 || platform == 6 || platform == 128;
        }
    }
}