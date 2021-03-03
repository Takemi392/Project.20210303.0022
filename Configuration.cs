using System;
using System.Collections.Generic;

namespace DesktopIniExcludeUnnecessaryItems
{
  internal sealed class Configuration
  {
    public static Configuration Instance { get; } = new Configuration();

    public AppSetting Setting { get; private set; }

    private Configuration()
    {
    }

    public void Load()
    {
      var path = System.IO.Path.Combine(
        System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
        @"Configuration.json"
      );

      if (!System.IO.File.Exists(path))
      {
        using (var stream = new System.IO.StreamWriter(path, false, System.Text.Encoding.UTF8))
        {
          stream.WriteLine(
            System.Text.Json.JsonSerializer.Serialize(
              new AppSetting(),
              new System.Text.Json.JsonSerializerOptions()
              {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
              }
            )
          );
        }
      }

      using (var stream = new System.IO.StreamReader(path))
      {
        this.Setting = System.Text.Json.JsonSerializer.Deserialize<AppSetting>(
          stream.ReadToEnd(),
          new System.Text.Json.JsonSerializerOptions()
          {
          }
        );
      }
    }

    internal class AppSetting
    {
      [System.Text.Json.Serialization.JsonPropertyName("TargetDirectories")]
      public string[] TargetDirectories { get; set; } = new string[]
      {
        @"C:\Users",
      };

      [System.Text.Json.Serialization.JsonPropertyName("Exclude")]
      public ExcludeSetting Exclude { get; set; } = new ExcludeSetting();

      internal class ExcludeSetting
      {
        [System.Text.Json.Serialization.JsonPropertyName("LocalizedResourceName")]
        public bool LocalizedResourceName { get; set; } = true;
      }
    }
  }
}
