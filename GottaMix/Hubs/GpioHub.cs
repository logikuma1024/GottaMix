using GottaMix.Extensions;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace GottaMix.Hubs
{
    /// <summary>
    /// Gpioピンの状態変更を保持・通知するハブクラス
    /// </summary>
    public class GpioHub : Hub
    {
        /// <summary>
        /// ピンの値リスト
        /// </summary>
        private List<GpioPinValue> GpioValueList;

        /// <summary>
        /// Gpioピンリスナー
        /// </summary>
        private IDisposable GpioPinListener;

        /// <summary>
        /// スイッチのON回数
        /// </summary>
        public int SwitchCount { get; set; }

        /// <summary>
        /// モーションセンサの状態
        /// </summary>
        public bool IsDitected { get; set; }

        /// <summary>
        /// 赤色LEDの状態
        /// </summary>
        public bool IsRedOn { get; set; }

        /// <summary>
        /// 黄色LEDの状態
        /// </summary>
        public bool IsYellowOn { get; set; }

        /// <summary>
        /// 緑色LEDの状態
        /// </summary>
        public bool IsGreenOn { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GpioHub()
        {
            // 初期化処理
            void init()
            {
                // ピンリストの初期状態をセット
                this.GpioValueList = Pi.Gpio.Pins.Select(x => x.ReadValue()).ToList();
                // タクトスイッチのON回数をリセット
                this.SwitchCount = 0;
                // GPIOピン3番（赤色LED）の値をリセット
                this.IsRedOn = Pi.Gpio.Pins[3].Read();
                // GPIOピン21番（黄色LED）の値をリセット
                this.IsYellowOn = Pi.Gpio.Pins[21].Read();
                // GPIOピン22番（緑色LED）の値をリセット
                this.IsGreenOn = Pi.Gpio.Pins[22].Read();
                // モーションセンサスイッチの状態をリセット
                this.IsDitected = Pi.Gpio.Pins[26].Read();
            }

            // 初期化
            init();

            // リスナーにイベントを登録
            GpioPinListener = Observable.Interval(TimeSpan.FromMilliseconds(100))
                // ピンのステータス変更を検知
                .Scan((GpioValueList.ToReadOnlyCollection(), GpioValueList.ToReadOnlyCollection()),
                        (x, y) => _ = (x.Item2, Pi.Gpio.Pins.Select(z => z.ReadValue()).ToList().ToReadOnlyCollection()))
                // ピンの値変更があった場合のみ動作させる
                .Where(x => !x.Item1.SequenceEqual(x.Item2))
                // 実行
                .Subscribe(x => {
                    // GPIOピン25番（タクトスイッチ）のONを検出する
                    if (x.Item2[25] == GpioPinValue.High)
                    {
                        SwitchCount++;
                    }
                    // GPIOピン3番（赤色LED）の値を検出する
                    IsRedOn = x.Item2[3] == GpioPinValue.High;

                    // GPIOピン21番（黄色LED）の値を検出する
                    IsYellowOn = x.Item2[21] == GpioPinValue.High;

                    // GPIOピン22番（緑色LED）の値を検出する
                    IsGreenOn = x.Item2[22] == GpioPinValue.High;

                    // GPIOピン26番（モーションセンサスイッチ）の状態をセットする
                    IsDitected = x.Item2[26] == GpioPinValue.High;
                });
        }

        /// <summary>
        /// 解放処理
        /// </summary>
        public new void Dispose()
        {
            // ハブを解放
            base.Dispose();
            // ピンリスナーを解放
            GpioPinListener.Dispose();
        }

        /// <summary>
        /// SignalR呼び出し（現在のスイッチON回数取得）
        /// </summary>
        /// <returns></returns>
        public Task GetSwitchCount() => Clients.All.SendAsync("Receive", SwitchCount, IsDitected, IsRedOn, IsYellowOn, IsGreenOn);
    }
}
