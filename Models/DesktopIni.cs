using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopIniExcludeUnnecessaryItems.Models
{
  internal class DesktopIni
  {
    public string FilePath { get; private set; }

    public DesktopIni(string filePath)
    {
      this.FilePath = filePath;
    }

    public bool Convert(bool localizedResourceName = true)
    {
      try
      {
        var beforeText = String.Empty;
        using (var stream = new System.IO.StreamReader(this.FilePath))
        {
          beforeText = stream.ReadToEnd();
        }

        var lines = new List<string>();
        using (var stream = new System.IO.StreamReader(this.FilePath))
        {
          while (true)
          {
            var line = stream.ReadLine();
            if (line == null)
              break;

            // [削除] LocalizedResourceName => ;LocalizedResourceName
            if (localizedResourceName)
            {
              if (line.StartsWith("LocalizedResourceName="))
                line = line.Insert(0, ";");
            }

            lines.Add(line);
          }
        }

        // 同じ行は排除
        var text = new StringBuilder();
        var set = new HashSet<string>();
        foreach (var line in lines)
        {
          if (!set.Contains(line))
          {
            text.Append(line);
            text.Append(Environment.NewLine);
          }

          if (line.Length != 0)
            set.Add(line);
        }

        var afterText = text.ToString();

        // 削除して再作成
        if (!String.Equals(beforeText, afterText))
        {
          using (var fileStream = new System.IO.FileStream(this.FilePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
          {
            fileStream.Position = 0;
            fileStream.SetLength(0);

            using (var streamWriter = new System.IO.StreamWriter(fileStream, Encoding.Unicode))
            {
              streamWriter.Write(afterText);
            }
          }
        }
      }
      catch (Exception)
      {
        return false;
      }

      return true;
    }
  }
}
