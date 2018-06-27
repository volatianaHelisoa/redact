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