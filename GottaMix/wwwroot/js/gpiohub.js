//--- ChatHub とのコネクションを生成
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gpio")
    .configureLogging(signalR.LogLevel.Information)
    .build();

//--- 受信したときの処理
connection.on('Receive', (SwitchCount, IsDitected, IsRedOn, IsYellowOn, IsGreenOn) => {

    console.log('signal-recieve');

    document.getElementById('switch-current').textContent = SwitchCount;
    document.getElementById('motion-current').textContent = IsDitected ? 'Ditected' : 'None';

    // LEDのCSS初期化
    $('#red-led').addClass('btn-round-red-off');
    $('#red-led').removeClass('btn-round-red-on');
    document.getElementById('red-led').textContent = 'RED OFF';

    $('#yellow-led').addClass('btn-round-yellow-off');
    $('#yellow-led').removeClass('btn-round-yellow-on');
    document.getElementById('yellow-led').textContent = 'YELLOW OFF';

    $('#green-led').addClass('btn-round-green-off');
    $('#green-led').removeClass('btn-round-green-on');
    document.getElementById('green-led').textContent = 'GREEN OFF';

    // LEDの状態を反映
    if (IsRedOn) {
        $('#red-led').addClass('btn-round-red-on');
        document.getElementById('red-led').textContent = 'RED ON';
    }

    if (IsYellowOn) {
        $('#yellow-led').addClass('btn-round-yellow-on');
        document.getElementById('yellow-led').textContent = 'YELLOW ON';
    }

    if (IsGreenOn) {
        $('#green-led').addClass('btn-round-green-on');
        document.getElementById('green-led').textContent = 'GREEN ON';
    }
});

$(function () {
    setInterval(function () {
        console.log('signal-submit');
        connection.invoke('GetSwitchCount').catch(e => console.log(e));
    }, 200);
});

//--- ボタンをクリックしたらデータを送信
//document.getElementById('button').addEventListener('click', event => {
//    const message = document.getElementById('message').value;
//    connection.invoke('Broadcast', message).catch(e => console.log(e));
//    event.preventDefault();
//});

//--- 接続を確立
connection.start().catch(e => console.log(e));