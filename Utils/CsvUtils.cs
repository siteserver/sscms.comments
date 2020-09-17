using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SSCMS.Comments.Utils
{
    public static class CsvUtils
	{
        public static void WriteText(string filePath, Encoding encoding, string content)
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

        public static bool IsFileExists(string filePath)
        {
            var exists = File.Exists(filePath);
            return exists;
        }

        public static bool DeleteFileIfExists(string filePath)
        {
            var retval = true;
            try
            {
                if (IsFileExists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                //try
                //{
                //    Scripting.FileSystemObject fso = new Scripting.FileSystemObjectClass();
                //    fso.DeleteFile(filePath, true);
                //}
                //catch
                //{
                //    retval = false;
                //}
                retval = false;
            }
            return retval;
        }

        public static void Export(string filePath, List<string> head, List<List<string>> rows)
        {
            DeleteFileIfExists(filePath);

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
