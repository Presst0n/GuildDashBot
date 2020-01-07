var $chatList = $("#GlobalChatList");

connection.on("ChatGlobal", addNewMessage);

function addNewMessage(message) {
    $chatList.append(`<li><div class="container-fluid"><div class="row d-flex align-content-center align-items-center"><div class="col-1 p-0"><img class="msg-avatar" src="${
        message.senderAvatarUrl}" /></div><div class="col-11 p-0"><span class="msg-username text-lime">${message.senderUsername}</span><span class="msg-addon">${message.senderReputation
        } rep</span><span class="msg-addon">${message.channelName
        }</span></div></div><div class="row"><div class="col-1 p-0"></div><div class="col-12"><span class="msg-message">${message.message
        }</span></div></div></div></li>`);
    for (let fileUrl of message.fileLinks) {
        $chatList.append(`<li><a href="${fileUrl}">ITEM</a></li>`);
    }
}
