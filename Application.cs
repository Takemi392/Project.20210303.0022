using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopIniExcludeUnnecessaryItems
{
  internal class Application
  {
    private List<Models.DesktopIni> DesktopIniCollection { get; } = new List<Models.DesktopIni>();

    public int Run(string[] args)
    {
      try
      {
        Configuration.Instance.Load();

        var backgroundTask = Task.Factory.StartNew(
          () =>
          {
            var set = new HashSet<string>();
            foreach (var directory in Configuration.Instance.Setting.TargetDirectories)
            {
              var files = this.GetFilesRecursively(directory, "desktop.ini");
              foreach (var file in files)
              {
                try
                {
                  set.Add(file);
                }
                catch (Exception)
                {
                }
              }
            }

            foreach (var file in set)
            {
              this.DesktopIniCollection.Add(
                new Models.DesktopIni(file)
              );
            }
          }
        );

        var count = 0;
        while (true)
        {
          if (backgroundTask.IsCompleted)
          {
            System.Console.WriteLine();
            break;
          }

          if (count == 0)
          {
            System.Console.Clear();

            System.Console.SetCursorPosition(0, System.Console.CursorTop);
            System.Console.Write("Wait");
          }

          System.Console.Write(".");

          count++;
          count %= 100;

          Task.Delay(100).Wait();
        }

        Parallel.ForEach(
          this.DesktopIniCollection, (obj) =>
          {
            var r = obj.Convert(Configuration.Instance.Setting.Exclude.LocalizedResourceName);

            System.Console.WriteLine(
              $"{String.Format(r ? "T" : "F")}: {obj.FilePath}"
            );
          }
        );
      }
      catch (Exception e)
      {
        System.Console.WriteLine(
          $"Error occurred during the process, Exception={e.Message}, InnerException={e.InnerException?.Message ?? "null"}."
        );

        return 1;
      }

      System.Console.WriteLine("Completed successfully.");

      return 0;
    }

    private IEnumerable<string> GetFilesRecursively(string directory, string pattern)
    {
      var r = Enumerable.Empty<string>();

      try
      {
        r = r.Concat(
          System.IO.Directory.EnumerateFiles(directory, pattern, System.IO.SearchOption.TopDirectoryOnly)
        );
      }
      catch (Exception)
      {
      }

      try
      {
        var sdirs = System.IO.Directory.EnumerateDirectories(directory, "*", System.IO.SearchOption.TopDirectoryOnly);
        foreach (var sdir in sdirs)
          r = r.Concat(this.GetFilesRecursively(sdir, pattern));
      }
      catch (Exception)
      {
      }

      return r;
    }
  }
}
