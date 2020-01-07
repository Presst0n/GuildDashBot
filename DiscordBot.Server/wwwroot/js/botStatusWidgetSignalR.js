var $statusActionBtn = $("#StatusActionButton");
var $statusMessage = $("#StatusMessage");
var $statusIcon = $("#StatusIcon");
var $statusWidget = $("#StatusWidget");

connection.on("BotConnectedChanged", setBotConnected);

function setBotConnected(isConnected) {
    $statusActionBtn.removeAttr("disabled");

    if (isConnected) {
        $statusActionBtn.text("Stop");
        $statusMessage.text("Bot is running...");
        overrideClass($statusActionBtn, "btn-outline-success", "btn-outline-secondary");
        overrideClass($statusIcon, "fa-pause", "fa-play");
        overrideClass($statusWidget, "status-negative", "status-positive");
    } else {
        $statusActionBtn.text("Start");
        $statusMessage.text("Bot is not running.");
        overrideClass($statusActionBtn, "btn-outline-secondary", "btn-outline-success");
        overrideClass($statusIcon, "fa-play", "fa-pause");
        overrideClass($statusWidget, "status-positive", "status-negative");
    }
}

$statusActionBtn.click(() => {
    $statusActionBtn.attr("disabled", "disabled");
    $statusMessage.text("Changing status...");

    connection.invoke("ToggleBotConnection").catch((err) => {
        return console.error(err.toString());
    });
});

function overrideClass($element, classA, classB) {
    $element.removeClass(classA);
    $element.addClass(classB);
}