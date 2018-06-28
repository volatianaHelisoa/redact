
$(document).ready(function () {
    /* VARIABLES GLOBALES */
    var $win = $(window);
    var userProfil = $('.profil');
    var popup = userProfil.next('.profil-menu');

    $(window).scroll(function (e) {
        // Scroll events
        var winScroll = $(window).scrollTop();
        var topBar = $('#top-bar');
        if (winScroll > 0) {
            $(topBar).addClass('fixedNav');
        } else {
            $(topBar).removeClass('fixedNav');
        }
        var onglets = $('#single-container .onglets');
        if (winScroll > 10) {
            $(onglets).addClass('fixedOnglets');
        } else {
            $(onglets).removeClass('fixedOnglets');
        }
    });

    $('body').on('keydown', function (e) {
        if (e.keyCode === 27) {
            popup.slideUp();
            popup.css('display', 'none');
        }
    });

    $(function () {
        $win.on("click", function (event) {
            if (userProfil.has(event.target).length == 0 && !userProfil.is(event.target)) {
                popup.slideUp();
                popup.css('display', 'none');
            } else {
                popup.toggleClass('open').slideDown();
            }
        });
    });
});


//Select all User in ListUser when the checkbox is selected.
function ClickAllUserInListUser() {
    if (document.getElementById('checkAllUser').checked) {
        CheckedClick();
    }
    else {
        DecheckedClick();
    }
}

function CheckedClick() {
    $('input[name="selectedUser"]').each(function () {        
        $(this).prop("checked", true);
    });
}

function DecheckedClick() {
    $('checkAllUser').prop("checked", false);
    $('input[name="selectedUser"]:checked').each(function () {
        $(this).prop("checked", false);
    });
}

//Select all Commande in ListCommande when the checkbox is selected.
function ClickAllCommandeInListCommande() {
    if (document.getElementById('checkAllCmde').checked) {
        CheckedCmdeClick();
    }
    else {
        DecheckedCmdeClick();
    }
}

function CheckedCmdeClick() {
 
    $('input[name="selectedCmde"]').each(function () {
        $(this).prop("checked", true);
    });
}

function DecheckedCmdeClick() {
    $('checkAllCmde').prop("checked", false);
    $('input[name="selectedCmde"]:checked').each(function () {
        $(this).prop("checked", false);
    });
}



//Select all Commande in ListCommande when the checkbox is selected.
function ClickAllFactureInListFacture() {
    if (document.getElementById('checkAllFacture').checked) {
        CheckedFactureClick();
    }
    else {
        DecheckedFactureClick();
    }
}

function CheckedFactureClick() {
    $('input[name="selectedFacture"]').each(function () {
        $(this).prop("checked", true);
    });
}

function DecheckedFactureClick() {
    $('checkAllFacture').prop("checked", false);
    $('input[name="selectedFacture"]:checked').each(function () {
        $(this).prop("checked", false);
    });
}

