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

        public static string SolveAssemblyRootDir()
        {
            return Path.Combine(Assembly.GetCallingAssembly().Location
                .Remove(Assembly.GetCallingAssembly().Location.Length - SolveDllName().Length));
        }

        public static string SolveDllName()
        {
            return Assembly.GetCallingAssembly().GetName().ToString().Split(',')[0] + ".dll";
        }
    }
}