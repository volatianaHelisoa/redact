$(document).ready(function () {


    $(function () {
        // Declare a proxy to reference the hub.
        var notifications = $.connection.messagesHub;
       
        //debugger;
        // Create a function that the hub can call to broadcast messages.
        notifications.client.sendMessages = function () {
            console.log("connecter updateMessages");
            getAllMessages();

        };
        // Start the connection.
        $.connection.hub.start().done(function () {
            console.log("connecter");
            getAllMessages();
        }).fail(function (e) {
            alert(e);
        });
    });

    function getAllMessages()
    {
        var tbl = $('#messagesTable');
        $('#notiContent').empty();
        $('#notiContent').append($('<li>...</li>'));

        $.ajax({
            type: 'GET',
            url: '/Commandes/GetNotifications',
            success: function(response) {
                console.log(response);
                $('#notiContent').empty();
                if (response.length == 0) {
                    $('#notiContent').append($('<li>Aucune nouvelle notification.</li>'));
                }
                $.each(response, function (index, value) {
                    var tr_str = '<li> La commande  <a href="~/Commandes/DetailsCommande/"' + value.commandeId + '"> #' + value.commanderef + '</a> a été mis à jour par ' + value.fromUserName + ' le ' + value.datenotif + ' . </li>';
                    $('#notiContent').append(tr_str);
                });
            },
            error: function(error) {
                console.log(error);
            }
        });
    }


});
    