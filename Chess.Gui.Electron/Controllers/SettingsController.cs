using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Chess.Gui.Electron.Models.Settings;
using Chess.Gui.Electron.Domain.Settings;
using Chess.Gui.Electron.Services;

namespace Chess.Gui.Electron.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ILocalizationService _localizationService;

        public SettingsController(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public IActionResult Index()
        {
            var uiSettings = UISettings.Default;
            var engineSettings = Chess.Uci.Connector.Settings.Default;

            var model = new SettingsModel
            {
                UISettings = new SettingsModel.UISettingsModel
                {
                    Language = uiSettings.Language,
                    AvailableLanguages = _localizationService.GetAvailableLanguages()
                        .Select(x => new SelectListItem
                        {
                            Text = x,
                            Value = x,
                            Selected = uiSettings.Language == x
                        }).ToList()
                },
                EngineSettings = new SettingsModel.EngineSettingsModel
                {
                    Threads = engineSettings.Threads,
                    EnginePath = engineSettings.EnginePath,
                    MinThreads = 1,
                    MaxThreads = Environment.ProcessorCount
                }
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(SettingsModel model)
        {
            try
            {
                var uiSettings = UISettings.Default;
                uiSettings.Language = model.UISettings.Language;
                uiSettings.Save();

                var engineSettings = Chess.Uci.Connector.Settings.Default;
                engineSettings.Threads = model.EngineSettings.Threads;
                engineSettings.EnginePath = model.EngineSettings.EnginePath;
                engineSettings.Save();
            }
            catch (Exception)
            {
                model.EngineSettings.MinThreads = 1;
                model.EngineSettings.MaxThreads = Environment.ProcessorCount;
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}