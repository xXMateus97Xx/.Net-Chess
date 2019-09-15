using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Chess.Uci.Connector
{
    public class Settings
    {
        private const string CONFIG_FILE = "config.conf";

        private static Settings _settings;
        public static Settings Default => _settings ?? (_settings = Load(CONFIG_FILE));

        private string _path;

        public int Threads { get; set; }
        public string EnginePath { get; set; }

        public void Save()
        {
            ParseAndValidateEnginePath(EnginePath);
            ParseAndValidateThreads(Threads.ToString());

            var lines = ReadFileLines(_path);

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.StartsWith("threads"))
                    lines[i] = $"threads={Threads}";

                if (line.StartsWith("engine"))
                    lines[i] = $"engine={EnginePath}";
            }

            File.WriteAllLines(_path, lines, Encoding.UTF8);
        }

        public static Settings Load(string configPath)
        {
            return ParseConfig(configPath);
        }

        private static string[] ReadFileLines(string path)
        {
            return File.ReadAllLines(path);
        }

        private static Settings ParseConfig(string path)
        {
            var settings = new Settings();
            var lines = ReadFileLines(path);

            foreach (var line in lines.Where(x => !x.StartsWith("#") && !string.IsNullOrWhiteSpace(x)))
            {
                var lineParsed = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (lineParsed.Length < 2)
                    ThrowInvalidConfigException(lineParsed[0], "null");

                switch (lineParsed[0])
                {
                    case "threads":
                        settings.Threads = ParseAndValidateThreads(lineParsed[1]);
                    break;
                    case "engine":
                        settings.EnginePath = ParseAndValidateEnginePath(lineParsed[1]);
                    break;
                    default:
                        throw new ApplicationException($"Key is not valid {lineParsed[0]}");
                }
            }

            settings._path = path;
            return settings;
        }

        private static string ParseAndValidateEnginePath(string value)
        {
            if (!File.Exists(value))
                ThrowInvalidConfigException("engine", value);
            
            return value;
        }

        private static int ParseAndValidateThreads(string value)
        {
            if (!int.TryParse(value, out var threads) || threads <= 0)
                ThrowInvalidConfigException("threads", value);

            return threads;
        }

        private static void ThrowInvalidConfigException(string key, string value)
        {
            throw new ApplicationException($"Value {value} is not valid for key {key}");
        }
    }
}