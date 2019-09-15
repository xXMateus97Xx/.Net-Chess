using System.Collections.Generic;

namespace Chess.Gui.Electron.Domain.Resources
{
    public class ResourceCollection
    {
        public string Language { get; set; }
        public bool Default { get; set; }
        public List<Resource> Resources { get; set; }
    }
}