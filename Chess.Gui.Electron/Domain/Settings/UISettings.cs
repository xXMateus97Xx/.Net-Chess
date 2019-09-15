using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Chess.Gui.Electron.Domain.Settings
{
    public class UISettings
    {
        private const string CONFIG_FILE = "config-ui.conf";

        private static UISettings _settings;
        public static UISettings Default => _settings ?? (_settings = Load(CONFIG_FILE));

        private string _path;

        public string Language { get; set; }

        public void Save()
        {
            ParseAndValidateLanguage(Language);

            var lines = ReadFileLines(_path);

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.StartsWith("language"))
                    lines[i] = $"language={Language}";
            }

            File.WriteAllLines(_path, lines, Encoding.UTF8);
        }

        public static UISettings Load(string configPath)
        {
            return ParseConfig(configPath);
        }

        private static string[] ReadFileLines(string path)
        {
            return File.ReadAllLines(path);
        }

        private static UISettings ParseConfig(string path)
        {
            var settings = new UISettings();
            var lines = ReadFileLines(path);

            foreach (var line in lines.Where(x => !x.StartsWith("#") && !string.IsNullOrWhiteSpace(x)))
            {
                var lineParsed = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (lineParsed.Length < 2)
                    ThrowInvalidConfigException(lineParsed[0], "null");

                switch (lineParsed[0])
                {
                    case "language":
                        settings.Language = ParseAndValidateLanguage(lineParsed[1]);
                    break;
                    default:
                        throw new ApplicationException($"Key is not valid {lineParsed[0]}");
                }
            }

            settings._path = path;
            return settings;
        }

        private static string ParseAndValidateLanguage(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                value = "en-us";
            
            var regex = new Regex("([a-z]){2}-([a-z]){2}");
            if (!regex.IsMatch(value))
                ThrowInvalidConfigException("language", value);
            
            return value;
        }

        private static void ThrowInvalidConfigException(string key, string value)
        {
            throw new ApplicationException($"Value {value} is not valid for key {key}");
        }
    }
}