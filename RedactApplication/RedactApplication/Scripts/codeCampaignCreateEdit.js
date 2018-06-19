$("#nbChar").css('color', "red");
$("#CharCaract").css('color', "blue");

function Decoupage(chaine, length) {
    var decoupe = chaine.substring(0, length);
    $("#message").val(decoupe);
}

$('#CampType').change(function () {
    var value = $('#CampType option:selected').val();
    switch (value) {
        case "SMS":
        case "PUSH":
            $("#message").prop("disabled", false);
            var text = $("#message").val();
            if (value == "SMS") {
                compteur = 160;
                if ((compteur - $("#message").val().length) < 0) {
                    Decoupage(text, 159);
                }
                $("#CharCaract").css('display', "");                
            }
            if (value == "PUSH") {
                compteur = 500;
                if ((compteur - $("#message").val().length) < 0) {
                    Decoupage(text, 499);
                }
                $("#CharCaract").css('display', "none");
            }
            $("#nbChar").text("" + (compteur - $("#message").val().length));
            break;
        case "":
            compteur = 0;
            $("#nbChar").text("-");
            $("#CharCaract").css('display', "none");
            $("#message").val("");
            $("#message").prop("disabled", true);
            break;
    }
});

var str = "[A-Za-z0-9 \\r\\n@£$¥èéùìòÇØøÅå\u0394_\u03A6\u0393\u039B\u03A9\u03A0\u03A8\u03A3\u0398\u039EÆæßÉ!\"#$%&amp;'()*+,\\-./:;&lt;=&gt;?¡ÄÖÑÜ§¿äöñüà^{}\\\\\\[~\\]|\u20AC]*$";
var SmstextAllowed = new RegExp(str);

function TestSmsMessage() {
    var text = $("#message").val();
    if (!SmstextAllowed.test(text)) {
        $("#SmsAllowedTextDialog").dialog({
            title: "Confirmation",
            buttons: {
                Ok: function () {
                    $(this).dialog('close');
                }
            },
            modal: true
        });
    }
}

$("#message").keyup(function () { 
    if ($('#CampType option:selected').val() == "SMS") {
        TestSmsMessage();
    }
    if ((compteur - $(this).val().length) < 0) {
        var text = $("#message").val();
        Decoupage(text, (text.length - 1));
    }
    $("#nbChar").text("" + (compteur - $(this).val().length));
});

setInterval(function () {
    var isDisabled = $('#message').prop('disabled');
    var text = $("#message").val();
    if (isDisabled != true) {
        if ((compteur - text.length) < 0) {
            var decoupe = text.substring(0, compteur);
            $("#message").val(decoupe);
            $("#nbChar").text("" + (compteur - $("#message").val().length));
        }
    }
}, 500);