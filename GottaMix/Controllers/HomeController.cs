using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GottaMix.Models;
using GottaMix.Hubs;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace GottaMix.Controllers
{
    public class HomeController : Controller
    {
        // Injection
        private GpioHub _hub;

        public HomeController(GpioHub gpioHub)
        {
            _hub = gpioHub;
        }

        public IActionResult Index()
        {
            var pinStates = getPinState();

            // FIXME: そのうち入力はすべてハブクラスへすげ替え
            ViewData["RedClass"] = _hub.IsRedOn ? "btn-round-red-on" : "btn-round-red-off";
            ViewData["RedName"] = _hub.IsRedOn ? "RED ON" : "RED OFF";
            ViewData["YellowClass"] = _hub.IsYellowOn ? "btn-round-yellow-on" : "btn-round-yellow-off";
            ViewData["YellowName"] = _hub.IsRedOn ? "YELLOW ON" : "YELLOW OFF";
            ViewData["GreenClass"] = _hub.IsGreenOn ? "btn-round-green-on" : "btn-round-green-off";
            ViewData["GreenName"] = _hub.IsRedOn ? "GREEN ON" : "GREEN OFF";
            ViewData["ButtonCount"] = _hub.SwitchCount;
            ViewData["Ditected"] = _hub.IsDitected ? "Ditected" : "None";

            return View();
        }

        /// <summary>
        /// LEDの状態を変更します
        /// </summary>
        /// <returns></returns>
        public IActionResult ToggleLed(int pinNo)
        {
            // ピン状態を[wiringPi]の番号で取得する
            var pin = Pi.Gpio[pinNo];

            // LEDを点灯させるため、ピンを出力方向にする
            pin.PinMode = GpioPinDriveMode.Output;

            // ピン状態を現在と逆転
            pin.Write(!pin.Read());

            // デバッグ出力
            var gpioValue = pin.ReadValue();
            Console.WriteLine($"pinNo:{pinNo} / value:{gpioValue}");

            return RedirectToAction("Index");
        }

        /// <summary>
        /// アクティブなピンの値を全て取得します
        /// </summary>
        /// <returns></returns>
        private IEnumerable<(int pinNo, GpioPinValue pinValue)> getPinState()
        {
            return Pi.Gpio.Select(x => (x.PinNumber, x.ReadValue()));
        }

        /// <summary>
        /// 指定した音階のビープ音を再生します
        /// </summary>
        /// <param name="pitch"></param>
        /// <returns></returns>
        public IActionResult Beep(Pitch pitch)
        {
            var pin = Pi.Gpio[29];

            Console.WriteLine("Sound On");

            switch (pitch)
            {
                case Pitch.Do:
                    pin.SoftToneFrequency = 523;
                    break;

                case Pitch.Re:
                    pin.SoftToneFrequency = 587;
                    break;

                case Pitch.Mi:
                    pin.SoftToneFrequency = 659;
                    break;

                case Pitch.Fa:
                    pin.SoftToneFrequency = 698;
                    break;

                case Pitch.So:
                    pin.SoftToneFrequency = 783;
                    break;

                case Pitch.Ra:
                    pin.SoftToneFrequency = 880;
                    break;

                case Pitch.Si:
                    pin.SoftToneFrequency = 987;
                    break;
            }

            System.Threading.Thread.Sleep(800);

            pin.SoftToneFrequency = 0;

            Console.WriteLine("Sound Off");

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    /// <summary>
    /// 音階
    /// </summary>
    public enum Pitch
    {
        Do, Re, Mi, Fa, So, Ra, Si
    }
}
