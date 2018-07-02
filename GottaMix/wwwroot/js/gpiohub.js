//--- ChatHub とのコネクションを生成
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gpio")
    .configureLogging(signalR.LogLevel.Information)
    .build();

//--- 受信したときの処理
connection.on('Receive', (no, data) => {
    document.getElementById('current').textContent = no;
    document.getElementById('motion').textContent = data;
});

$(function () {
    setInterval(function () {
        console.log('aa');
        var current = document.getElementById('current').value;
        connection.invoke('GetSwitchCount').catch(e => console.log(e));
    }, 500);
});

//--- ボタンをクリックしたらデータを送信
//document.getElementById('button').addEventListener('click', event => {
//    const message = document.getElementById('message').value;
//    connection.invoke('Broadcast', message).catch(e => console.log(e));
//    event.preventDefault();
//});

//--- 接続を確立
connection.start().catch(e => console.log(e));