using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Chess.Gui.Electron.Models.Settings
{
    public class SettingsModel
    {
        public UISettingsModel UISettings { get; set; }
        public EngineSettingsModel EngineSettings { get; set; }

        public class UISettingsModel
        {
            public string Language { get; set; }
            public List<SelectListItem> AvailableLanguages { get; set; }
        }

        public class EngineSettingsModel
        {
            public string EnginePath { get; set; }
            public int Threads { get; set; }
            public int MaxThreads { get; set; }
            public int MinThreads { get; set; }
        }
    }
}