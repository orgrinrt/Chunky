using System;
using System.IO;
using Newtonsoft.Json;

namespace Chunky.Shared
{
    public class FileHelper
    {
        public static void SerializeAsJson(string targetPath, object toSerialize)
        {
            FileStream stream;

            Directory.CreateDirectory(Utils.SolveDirFromFileName(targetPath));

            File.Delete(targetPath);
            
            stream = !File.Exists(targetPath) ? new FileStream(targetPath, FileMode.CreateNew, FileAccess.ReadWrite)
                : new FileStream(targetPath, FileMode.Open, FileAccess.ReadWrite);
            
            using (stream)
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    JsonSerializer serializer = new JsonSerializer {Formatting = Formatting.Indented};
                    JsonWriter jsonWriter = new JsonTextWriter(writer);
                    serializer.Serialize(jsonWriter, toSerialize);
                }
            }
            
            Print.Line(ConsoleColor.Green, "Successfully serialized GlobalConfig to file: " + targetPath);
        }

        public static T DeserializeFromJson<T>(string pathToFile)
        {
            FileStream stream;
            T loaded;

            stream = !File.Exists(pathToFile) ? new FileStream(pathToFile, FileMode.CreateNew, FileAccess.ReadWrite) 
                : new FileStream(pathToFile, FileMode.Open, FileAccess.ReadWrite);
            
            using (stream)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    JsonSerializer serializer = new JsonSerializer {Formatting = Formatting.Indented};
                    JsonReader jsonReader = new JsonTextReader(reader);
                    loaded = serializer.Deserialize<T>(jsonReader);
                }
            }

            if (loaded == null)
            {
                Print.Line(ConsoleColor.Red, "Couldn't load " + pathToFile);
                throw new IOException("Couldn't load file from " + pathToFile);
            }
            
            Print.Line(ConsoleColor.Green, "Successfully loaded " + pathToFile);

            return loaded;
        }
    }
}