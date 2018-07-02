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
        private int SwitchCount;

        /// <summary>
        /// モーションセンサの状態
        /// </summary>
        private bool Motion;

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
                // モーションセンサスイッチの状態をリセット
                this.Motion = Pi.Gpio.Pins[6].Read();
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
                    // GPIOピン4番（タクトスイッチ）のONを検出する
                    if (x.Item2[4] == GpioPinValue.High)
                    {
                        SwitchCount++;
                    }
                    // GPIOピン6番（モーションセンサスイッチ）の状態をセットする
                    Motion = x.Item2[6] == GpioPinValue.High;
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
        /// スイッチの押下回数を取得
        /// </summary>
        /// <returns></returns>
        public int GetCurrentVal() => SwitchCount;

        /// <summary>
        /// SignalR呼び出し（現在のスイッチON回数取得）
        /// </summary>
        /// <returns></returns>
        public Task GetSwitchCount() => Clients.All.SendAsync("Receive", SwitchCount, Motion);
    }
}
