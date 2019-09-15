using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chess.Gui.Electron.Domain.Resources;
using Chess.Gui.Electron.Domain.Settings;
using Utf8Json;

namespace Chess.Gui.Electron.Services
{
    public class LocalizationService : ILocalizationService
    {
        private static readonly object _lock = new object();
        private static bool _isloaded;
        private static List<ResourceCollection> _allResources = new List<ResourceCollection>();

        public List<string> GetAvailableLanguages()
        {
            LoadFiles();

            return _allResources.Select(x => x.Language).ToList();
        }

        public string GetResource(string key)
        {
            return GetResource(key, UISettings.Default.Language);
        }

        public string GetResource(string key, string language)
        {
            LoadFiles();

            var languageResources = _allResources.FirstOrDefault(x => x.Language == language);
            if (languageResources == null)
                languageResources = _allResources.First(x => x.Default);
            
            var resource = languageResources?.Resources.FirstOrDefault(x => x.Key == key);

            return resource?.Value ?? key;
        }

        private void LoadFiles()
        {
            if (_isloaded)
                return;

            lock (_lock)
            {
                if (_isloaded)
                    return;

                var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "resources-*.json");

                foreach (var file in files)
                {
                    var bytes = File.ReadAllBytes(file);
                    var resources = JsonSerializer.Deserialize<ResourceCollection>(bytes);
                    _allResources.Add(resources);
                }

                _isloaded = true;
            }
        }
    }
}