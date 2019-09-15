using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Chess.Gui.Electron.Models;
using ElectronNET.API;

namespace Chess.Gui.Electron.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HybridSupport.IsElectronActive)
            {
                ElectronNET.API.Electron.IpcMain.On("exit-app", (args) =>
                {
                    Environment.Exit(0);
                });
            }

            return View();
        }
    }
}
