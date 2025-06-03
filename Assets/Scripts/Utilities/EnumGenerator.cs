using System.IO;

namespace Utilities
{
    public static class EnumGenerator
    {
        public static void GenerateEnum(string enumName, string[] entries, string path)
        {
            using var writer = new StreamWriter(path);
            writer.WriteLine("public enum " + enumName);
            writer.WriteLine("{");
            foreach (var entry in entries)
            {
                writer.WriteLine("    " + entry + ",");
            }
            writer.WriteLine("}");
        }
    }

}
