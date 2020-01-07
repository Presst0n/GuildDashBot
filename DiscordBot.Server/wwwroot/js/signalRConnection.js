var connection = new signalR.HubConnectionBuilder().withUrl("/botHub").build();

connection.start().then(function () {
    // on connection started
}).catch(function (err) {
    return console.error(err.toString());
    });
