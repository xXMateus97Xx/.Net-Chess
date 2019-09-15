using System.Collections.Generic;

namespace Chess.Gui.Electron.Services
{
    public interface ILocalizationService
    {
        string GetResource(string key, string language);
        string GetResource(string key);
        List<string> GetAvailableLanguages();
    }
}