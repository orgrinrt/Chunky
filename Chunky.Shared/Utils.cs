using System;
using System.Drawing.Imaging;
using System.IO;
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
    }
}