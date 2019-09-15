using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Chess.Gui.Electron.Models.Game
{
    public class GameStartModel
    {
        public string StartColor { get; set; }
        public List<SelectListItem> Colors { get; set; }
        public List<SelectListItem> Levels { get; set; }
        public bool SetDepth { get; set; }
        public int SelectedLevel { get; set; }
        public int Depth { get; set; }
    }
}