var $activeAccountAvatar = $("#ActiveAccountAvatar");
var $activeAccountLabel = $("#ActiveAccountLabel");

connection.on("BotAccountSelected", setBotConnected);

function setBotConnected(botAccount) {
    if (!$activeAccountAvatar.length) {
        location.reload(); 
    }

    $activeAccountAvatar.attr("src", botAccount.avatarUrl);
    $activeAccountLabel.text(botAccount.name);
}
