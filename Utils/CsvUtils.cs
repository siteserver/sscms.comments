using System.Collections.Generic;
using System.IO;
using System.Text;
using SSCMS.Utils;

namespace SSCMS.Comments.Utils
{
    public static class CsvUtils
	{
        private static void WriteText(string filePath, Encoding encoding, string content)
        {
            var file = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            using (var writer = new StreamWriter(file, encoding))
            {
                writer.Write(content);
                writer.Flush();
                writer.Close();

                file.Close();
            }

            //         var sw = new StreamWriter(filePath, false, ECharsetUtils.GetEncoding(charset));
            //sw.Write(content);
            //sw.Flush();
            //sw.Close();
        }

        public static void Export(string filePath, List<string> head, List<List<string>> rows)
        {
            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);

            var builder = new StringBuilder();
            foreach (var name in head)
            {
                builder.Append(name).Append(",");
            }
            builder.Length -= 1;
            builder.Append("\n");

            foreach (var row in rows)
            {
                foreach (var r in row)
                {
                    var value = r.Replace(@"""", @"""""");
                    builder.Append(@"""" + value + @"""").Append(",");
                }
                builder.Length -= 1;
                builder.Append("\n");
            }

            WriteText(filePath, Encoding.UTF8, builder.ToString());
        }
	}
}
