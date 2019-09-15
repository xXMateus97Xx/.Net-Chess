using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Chess.Uci.Connector;
using Chess.Gui.Electron.Models.Game;
using ElectronNET.API;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Chess.Gui.Electron.Services;

namespace Chess.Gui.Electron.Controllers
{
    public class GameController : Controller
    {
        private readonly UCIConnector _uciConnector;
        private readonly ILocalizationService _localizationService;

        private static bool _isInitialized;

        public GameController(UCIConnector uciConnector,
            ILocalizationService localizationService)
        {
            _uciConnector = uciConnector;
            _localizationService = localizationService;
        }

        public IActionResult Index()
        {
            var model = new GameStartModel();

            model.Colors = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "w",
                    Text = _localizationService.GetResource("colors.white")
                },
                new SelectListItem 
                {
                    Value = "b",
                    Text = _localizationService.GetResource("colors.black")
                },
                new SelectListItem 
                {
                    Value = "random",
                    Text = _localizationService.GetResource("colors.random")
                }
            };

            model.Levels = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "1",
                    Text = "1"
                },
                new SelectListItem
                {
                    Value = "2",
                    Text = "2"
                },
                new SelectListItem
                {
                    Value = "3",
                    Text = "3"
                },
                new SelectListItem
                {
                    Value = "4",
                    Text = "4"
                },
                new SelectListItem
                {
                    Value = "6",
                    Text = "5"
                },
                new SelectListItem
                {
                    Value = "8",
                    Text = "6"
                },
                new SelectListItem
                {
                    Value = "10",
                    Text = "7"
                },
                new SelectListItem
                {
                    Value = "12",
                    Text = "8"
                }
            };

            return View(model);
        }

        public IActionResult StartGame(GameStartModel model)
        {
            if (HybridSupport.IsElectronActive && !_isInitialized)
            {
                ElectronNET.API.Electron.IpcMain.OnSync("get-next-move", (args) =>
                {
                    var move = JsonConvert.DeserializeObject<NextMoveModel>(args.ToString());

                    var nextMove = _uciConnector.GetNextMoveByFen(move.Fen, move.Depth);

                    var from = nextMove.Substring(0, 2);
                    var to = nextMove.Substring(2, 2);
                    char? promotion = null;

                    if (nextMove.Length == 5)
                        promotion = nextMove.Last();
                    
                    var response = new
                    {
                        engineMove = new {
                            from,
                            to,
                            promotion
                        }
                    };

                    return JsonConvert.SerializeObject(response);
                });

                _isInitialized = true;
            }

            var gameModel = new GameConfigModel();
            
            gameModel.Depth = model.SetDepth ? model.Depth : model.SelectedLevel;
            if (model.StartColor == "random")
                gameModel.StartColor = (new Random().Next() & 1) == 0 ? "w" : "b";
            else
                gameModel.StartColor = model.StartColor;

            return View(gameModel);
        }
    }
}