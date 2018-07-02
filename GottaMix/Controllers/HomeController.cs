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

        public HomeController(GpioHub chatHub)
        {
            _hub = chatHub;
        }

        public IActionResult Index()
        {
            var pinStates = getPinState();

            ViewData["Pin8Value"] = pinStates.First(x => x.pinNo == 8).pinValue;
            ViewData["Pin22Value"] = pinStates.First(x => x.pinNo == 22).pinValue;

            ViewData["ButtonCount"] = _hub.GetCurrentVal();

            return View();
        }

        /// <summary>
        /// LEDを指定状態に変更します
        /// </summary>
        /// <returns></returns>
        public IActionResult ChangeLed(int pinNo, bool isOn)
        {
            // make pin operator from 'bcm' pin no
            var pin = Pi.Gpio[pinNo];

            // set pinmode 'output'
            pin.PinMode = GpioPinDriveMode.Output;

            // change value
            pin.Write(isOn);

            // get PinMode
            var gpioValue = pin.ReadValue();
            Console.WriteLine($"pinNo:{pinNo} / value:{gpioValue}");

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
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
            var pin = Pi.Gpio[3];

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

            System.Threading.Thread.Sleep(1000);

            pin.SoftToneFrequency = 0;

            Console.WriteLine("Sound Off");

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 音階
        /// </summary>
        public enum Pitch
        {
            Do, Re, Mi, Fa, So, Ra, Si
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
