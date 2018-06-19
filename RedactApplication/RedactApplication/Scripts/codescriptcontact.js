
//Select all User in ListContact when the checkbox is selected.
function ClickAllContactInListContact() {
    if (document.getElementById('checkAllContact').checked) {
        CheckedClick();
    }
    else {
        DecheckedClick();
    }
}
function CheckedClick() {
    $('input[name="selectedContact"]').each(function () {
        $(this).prop("checked", true);
    });

    if (!document.getElementById('checkAllContact').checked) {
        $('input[name="headSelected"]').each(function () {
            $(this).prop("checked", true);
        });
    }
}

function DecheckedClick() {
    $('checkAllContact').prop("checked", false);
    $('input[name="selectedContact"]:checked').each(function () {
        $(this).prop("checked", false);
    });

    if (document.getElementById('checkAllContact').checked) {
        $('input[name="headSelected"]').each(function () {
            $(this).prop("checked", false);
        });
    }
}

function showTrigerOnly() {
    var is_register = jQuery('#onlyShowTrig').prop('checked');
    if (is_register) {
        $('#myDataTable').DataTable().column(4).search('1').draw();
    } else {
        $('#myDataTable').DataTable().column(4).search('').draw();
    }
}

var gotoEditContact = function (contactId) {
    document.location.href = "EditContact?hash=" + contactId;
}

var gotoListeContact = function () {
    document.location.href = "ListeContact";
}

function confirmDelete(contactId) {
    document.getElementById('contactIdSuppr').value = contactId;
    $("#deleteContact").show();
}

$(document).ready(function () {
    //On met les fonctions jquery ici


    var clik = $(".parentClickable > span")
    $(".parentClickable").each(function () {
        if (!$(this).has("li").length) {
            $(this).addClass('no-child');
            $(".no-child span").css("background", "none");

        }
        else {
        }
    });

    $(clik).click(function () {
        $(this).parent().toggleClass('collaps');
        $(this).toggleClass('clickcurrent');;
        $(this).parent().children().toggle();
        $(this).toggle();
    });

    $("#upload").change(function () {
        var fileName = $("#upload")[0].files[0].name;
        $('.file-detect').text((fileName));
        return false;
    });

    $(".showPhoneCache").click(function () {
        var contactReference = $("input[name='contactReference']").val();
        var contactName = $("input[name='contactName']").val();
        var contactSurname = $("input[name='contactSurname']").val();
        $("input[name='contactReference']").val(contactReference);
        $("input[name='contactName']").val(contactName);
        $("input[name='contactSurname']").val(contactSurname);
        $(".formSansPhone").hide();
        $(".formAvecPhone").show();
        ResteIDPerson();
        ResteIDChild();
    });

    $(".hidePhoneCache").click(function () {
        var contactReference = $("input[name='contactReference']").val();
        var contactName = $("input[name='contactName']").val();
        var contactSurname = $("input[name='contactSurname']").val();
        $("input[name='contactReference']").val(contactReference);
        $("input[name='contactName']").val(contactName);
        $("input[name='contactSurname']").val(contactSurname);
        $(".formSansPhone").show();
        $(".formAvecPhone").hide();
        ResteIDPerson();
        ResteIDChild();
    });

    $(function () {
        var $deleteContact = $("#deleteContact");

        /*pour fermer le menu contextuel sous division*/
        $deleteContact.on("click", "button.cancelDelete", function () {
            $deleteContact.hide();
        });

    });
    $(".parentClickable").each(function () {
        if ($(this).has("li").length === 0) {
            $(this).addClass('no-child');
            $(this).css("background", "none");
        }
        else {
        }
    });

    //$(function () {
    var $contextMenu = $("#contextMenu");
    var $CreateNewDivDialog = $("#CreateNewDivDialog");
    //var $saveDivisionSucces = $("#saveDivisionSucces");
    var $contextMenuDivision = $("#contextMenuDivision");
    var $CreateDivisionRacine = $("#CreateDivisionRacine");
    var $editDivisionDialog = $("#editDivisionDialog");
    //var $editDivisionSucces = $("#editDivisionSucces");
    var $deleteAllChildDivision = $("#deleteAllChildDivision");
    //var $deleteChildDivSucce = $("#deleteChildDivSucce");
    var $deleteDivisionAndChilds = $("#deleteDivisionAndChilds");
    var $modeDivisionContact = $("#modeDivisionContact");
    var $createNewDivError = $("#createNewDivError");
    var $invalidDivision = $("#invalidDivision");
    var $limiteError = $("#limiteError");
    var $deleteAllChildDivisionRoot = $("#deleteAllChildDivisionRoot");
    var $createSousDivisionBouton = $("#createSousDivisionBouton");

    var $contextMenuNiv2 = $("#contextMenuNiv2");

    /*menu contextuel sous division*/
    $("body").on("contextmenu", ".parentClickable span", function (e) {
        e.preventDefault()

        var parentID = $(this).parents("li").attr("id");

        if (typeof parentID === "undefined") {
            parentID = $(this).parents(".parentClickable").attr("id");
        }

        $(this).attr("id", parentID);
        //$(this).parent().removeAttr("id");

        

        var px = e.pageX;
        var py = e.pageY;

        if ($(this).parents("li").hasClass('no-child')) {
            px = px + 200;
        }

        $contextMenu.css({
            left: (e.pageX-40),
            top: (e.pageY-130)
        });        

        $("#nodeClickedValue").text($(e.target).text());
        var idDivSeleteds = this.id;
       
        if ($('#idDivSeleted').length > 0)
            $('#idDivSeleted').val(idDivSeleteds);

        if ($('#divisionParentId').length > 0)
            $('#divisionParentId').val(idDivSeleteds);

        if ($('#divisionParentIds').length > 0)
            $('#divisionParentIds').val(idDivSeleteds);

        if ($('#divisionParentIdSuppr').length > 0)
            $('#divisionParentIdSuppr').val(idDivSeleteds);

        if ($('#divParentId').length > 0)
            $('#divParentId').val(idDivSeleteds);

        if ($('#divisionIdMove').length > 0)
            $('#divisionIdMove').val(idDivSeleteds);

        if ($('#divNameEdit').length > 0)
            $('#divNameEdit').val($(e.target).text());

        $.ajax({
            type: 'POST',
            url: '/Ajax/verifContactDivision',
            data: { divid: idDivSeleteds },
            success: function (data) {
                var resultat = JSON.parse(data);
                if (resultat.deleteAllChild == 0) { $("a.deleteChildDivision").hide(); }
                else { $("a.deleteChildDivision").show(); }
                if (resultat.moveDivContact == 0) { $("a.moveContactDivision").hide(); }
                else { $("a.moveContactDivision").show(); }
                if (resultat.createNewDiv == 0) { $("a.createNewDivision").hide(); }
                else { $("a.createNewDivision").show(); }
                gestionDialogue(true, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
                $('#modeDivisionContact.move').css({
                    right: (px - 80),
                    top: (py - 200)
                });
            }
        });  
       
    });
    /*pour fermer le menu contextuel sous division*/
    $contextMenu.on("click", "a.closeDialog", function () {
        gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
    });

    /*pour fermer le dialogue de creation d'une sous division*/
    $CreateNewDivDialog.on("click", "a.closeDialog", function () {
        gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
    });

    /*pour fermer le dialogue de modification d'une division*/
    $editDivisionDialog.on("click", "a.closeDialog", function () {
        gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
    });

    /*pour fermer le dialogue de suppression des sous division d'une division*/
    $deleteAllChildDivision.on("click", "a.cancelDeleteDivision", function () {
        gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
    });

    /*pour fermer le dialogue de suppression d'une division et de ses sous divisions*/
    $deleteDivisionAndChilds.on("click", "a.cancelDeleteDivision", function () {
        gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
    });

    /*pour fermer le dialogue de suppression d'une division et de ses sous divisions*/
    $deleteAllChildDivisionRoot.on("click", "a.cancelDeleteDivision", function () {
        gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
    });

    /*pour afficher le dialogue de creation d'une sous division*/
    $contextMenu.on("click", "a.createNewDivision", function () {
        $.ajax({
            type: 'POST',
            url: '/Ajax/manageCreateDiv',
            data: { divid: document.getElementById('divisionParentId').value },
            success: function (data) {
                console.log(data);
                if (data == 1) { gestionDialogue(false, false, true, false, false, false, false, false, false, false, false, false, false, false, false); }
                else if (data == 0) { gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, true, false, false); }
                else { gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, true, false, false, false); }
            }
        });
    });

    /*pour afficher le dialogue de suppresion des sous division d'une division*/
    $contextMenu.on("click", "a.deleteChildDivision", function () {
        gestionDialogue(false, false, false, false, false, false, false, true, false, false, false, false, false, false, false);
    });

    /*pour afficher le dialogue de suppresion des sous division d'une division ROOT*/
    $contextMenuDivision.on("click", "a.deleteChildDivision", function () {
        gestionDialogue(false, false, false, false, false, false, false, true, false, false, false, false, false, true, false);
    });

    /*pour afficher le dialogue de suppresion d'une division et de ses sous divisions*/
    $contextMenu.on("click", "a.deleteDivision", function () {
        gestionDialogue(false, false, false, false, false, false, false, false, false, true, false, false, false, false, false);
    });

    /*pour afficher le dialogue de modification d'une division*/
    $contextMenu.on("click", "a.editDivision", function () {
        gestionDialogue(false, false, false, false, false, true, false, false, false, false, false, false, false, false, false);
    });

    var resultAction = document.getElementById('resultAddDivision').value;
    /*pour savoir si on vient d'enregistrer une sous division*/
    //if (resultAction == 1) {
    //    gestionDialogue(false, false, false, true, false, false, false, false, false, false, false, false, false, false, false);
    //}

    //if (resultAction == 2) {
    //    gestionDialogue(false, false, false, false, false, false, true, false, false, false, false, false, false, false, false);
    //}
    //else if (resultAction == 3) {
    //    gestionDialogue(false, false, false, false, false, false, false, false, true, false, false, false, false, false);
    //}
    if (resultAction == 4) {
        gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, true, false, false, false);
    }
    else if (resultAction == 5) {
        gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, true, false, false);
    }
    else if (resultAction == 6 || resultAction == 7) {
        console.log('passage dans le ' + resultAction);
        gestionDialogue(true, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
    }

    else if (resultAction == 8) {
        console.log('passage dans le ' + resultAction);
        gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, true);
    }

    ///*pour fermer le dialogue sur la reussite de l'enregistrement d'une sous division*/
    //$saveDivisionSucces.on("click", "button", function () {
    //    gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
    //    document.getElementById('resultAddDivision').value = 0;
    //    //document.location.href = "?savedUser=0";
    //});

    /*pour fermer le dialogue sur la reussite d'une modification d'une division*/
    //$editDivisionSucces.on("click", "button", function () {
    //    gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
    //    document.getElementById('resultAddDivision').value = 0;
    //    //document.location.href = "?savedUser=0";
    //});

    //$deleteChildDivSucce.on("click", "button", function () {
    //    gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
    //    document.getElementById('resultAddDivision').value = 0;
    //    //document.location.href = "?savedUser=0";
    //});

    /*pour fermer le dialogue sur l'echec de l'enregistrement d'une sous division*/
    $createNewDivError.on("click", "button", function () {
        gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
        document.getElementById('resultAddDivision').value = 0;
        //document.location.href = "?savedUser=0";
    });
    /*pour fermer le dialogue sur l'echec de l'enregistrement d'une division : limit atteint*/
    $limiteError.on("click", "button", function () {
        gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
        document.getElementById('resultAddDivision').value = 0;
        //document.location.href = "?savedUser=0";
    });

    /*pour fermer le dialogue sur l'echec de l'enregistrement d'une division : invalid data*/
    $invalidDivision.on("click", "button", function () {
        gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
        document.getElementById('resultAddDivision').value = 0;
        //document.location.href = "?savedUser=0";
    });

    //TRAITEMENT DES DIVISIONS
    //menu contextuel division
    $("body").on("contextmenu", ".racineDivision", function (e) {
        $contextMenuDivision.css({
            left: (e.pageX - 40),
            top: (e.pageY - 130)
        });
        $("#nodeClickedValues").text($(e.target).text());


        $.ajax({
            type: 'POST',
            url: '/Ajax/divisionRoot',
            data: { divid: "triger" },
            success: function (data) {
                console.log(data);
                if (data == 0) { $("a.deleteChildDivision").hide(); }
                else { $("a.deleteChildDivision").show(); }
                gestionDialogue(false, true, false, false, false, false, false, false, false, false, false, false, false, false, false);
                $("a.createNewDivision").show();
            }
        });
        return false;
    });

    /*pour fermer le menu contextuel division*/
    $contextMenuDivision.on("click", "a.closeDialog", function () {
        gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
    });
    /*pour afficher le dialogue de creation d'une division*/
    $contextMenuDivision.on("click", "a.createNewDivision", function () {
        $.ajax({
            type: 'POST',
            url: '/Ajax/leftDivision',
            data: { divid: "1" },
            success: function (data) {
                console.log(data);
                if (data > 0) { gestionDialogue(false, false, false, false, true, false, false, false, false, false, false, false, false, false, false); }
                else { gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, true, false, false); }
            }
        });
    });
    /*pour fermer le dialogue de creation d'une sous division*/
    $CreateDivisionRacine.on("click", "a.closeDialog", function () {
        gestionDialogue(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
    });
    /*pour afficher le dialogue de move contact division d'une division*/
    $contextMenu.on("click", "a.moveContactDivision", function () {
        var idParent = document.getElementById("divisionIdMove").value;
        $("input." + idParent).hide();
        $("label." + idParent).hide();
        gestionDialogue(true, false, false, false, false, false, false, false, false, false, true, false, false, false, false);
    });
    
    $contextMenu.on("click", "a.addContactDivision", function () {
        var idParent = document.getElementById("divisionIdMove").value;
        $.ajax({
            type: 'POST',
            url: '/Ajax/AddContactDivisionList',
            data: { data: idParent },
            success: function (data) {
                TestLimitContact('CreateContact');
            }
        });
    });

    $("body").on("click", ".parentClickable span", function (e) {

        if ($(this).hasClass("no-contact"))
            return false;

        var parentID = $(this).closest("li").attr("id");
        var url = '/Contact/ListeContact';
        window.location.href = url + "?id=" + parentID;
    })

    $contextMenu.on("click", "a.listContactDivision", function () {
        var idParent = document.getElementById("divisionIdMove").value;
        var url = '/Contact/ListeContact';
        window.location.href = url + "?id=" + idParent;
    });

    $contextMenu.on("click", "a.deleteAllDivisionContact", function () {
        var idParent = document.getElementById("divisionIdMove").value;
        var LisContactId = '';
        $.ajax({
            type: 'POST',
            url: '/Ajax/SelecteAllContactDivisionToDelete',
            data: { divid: idParent },
            success: function (data) {
                if (data != null && data.ListId != "") {                    
                    LisContactId = data.ListId;
                }
                if (LisContactId != '') {
                    $.ajax({
                        type: 'POST',
                        url: '/Contact/SelecteAllContactToDelete',
                        data: { hash: LisContactId },
                        error: function (ex) {
                        },
                        success: function () {
                            RedirectDeleteContactDivision()
                        },
                        async: true,
                        processData: true
                    });
                } else {
                    $("#DeleteDatadialog").modal();
                }
            },
            async: true,
            processData: true
        });                
    });

    function gestionDialogue(contextMenu, contextMenuDivision, CreateNewDivDialog, saveDivisionSucces,
        CreateDivisionRacine, editDivisionDialog, editDivisionSucces, deleteAllChildDivision,
        deleteChildDivSucce, deleteDivisionAndChilds, modeDivisionContact, createNewDivError,
        limiteError, deleteAllChildDivisionRoot, invalidDivision) {
        if (contextMenu) { $contextMenu.show(); }
        else { $contextMenu.hide(); }

        if (contextMenuDivision) { $contextMenuDivision.show(); }
        else { $contextMenuDivision.hide(); }

        if (CreateNewDivDialog) { $CreateNewDivDialog.modal(); }
        else { $CreateNewDivDialog.hide(); }

        //if (saveDivisionSucces) { $saveDivisionSucces.modal(); }
        //else { $saveDivisionSucces.hide();}

        if (CreateDivisionRacine) { $CreateDivisionRacine.modal(); }
        else { $CreateDivisionRacine.hide(); }

        if (editDivisionDialog) { $editDivisionDialog.modal(); }
        else { $editDivisionDialog.hide(); }

        //if (editDivisionSucces) { $editDivisionSucces.show(); }
        //else { $editDivisionSucces.hide(); }

        if (deleteAllChildDivision) { $deleteAllChildDivision.modal(); }
        else { $deleteAllChildDivision.hide(); }

        //if (deleteChildDivSucce) { $deleteChildDivSucce.modal(); }
        //else { $deleteChildDivSucce.hide(); }

        if (deleteDivisionAndChilds) { $deleteDivisionAndChilds.modal(); }
        else { $deleteDivisionAndChilds.hide(); }

        if (modeDivisionContact) { $modeDivisionContact.show(); }
        else { $modeDivisionContact.hide(); }

        if (createNewDivError) { $createNewDivError.modal(); }
        else { $createNewDivError.hide(); }

        if (limiteError) { $limiteError.modal(); }
        else { $limiteError.hide(); }

        if (deleteAllChildDivisionRoot) { $deleteAllChildDivisionRoot.show(); }
        else { $deleteAllChildDivisionRoot.hide(); }

        if (invalidDivision) {
            $invalidDivision.show();
        }
        else { $invalidDivision.hide(); }
    }
});

function showDataLine(reference) {
    $("#dlgDetailsContact").hide();
    $.ajax({
        type: 'POST',
        url: '/Ajax/getContactByReference',
        data: { reference: reference },
        success: function (data) {
            $('.autres-enfants').empty();
            $('.autres-perso-contact').empty();
            document.getElementById("conjointNom").value = "";
            document.getElementById("conjointPrenom").value = "";
            document.getElementById("conjointCountry").value = "";
            document.getElementById("conjointPhoneNumber").value = "";

            var resultat = JSON.parse(data);
            var listEnfant = resultat.listEnfant;
            for (var i = 0; i < listEnfant.length; i++) {
                var $conteneur = $(".autres-enfants")
                var contenus = "<h5>Child #" + (i + 1) + "</h5>"
                                + "<div class='field-enfant'>"
                                    + "<label>"
                                        + "<span>Name : </span>"
                                        + "<input value='" + (listEnfant[i].enfantNom) + "' type='text' style='border:none' readonly>"
                                    + "</label>"
                                    + "<label>"
                                        + "<span>First name : </span>"
                                        + "<input value='" + (listEnfant[i].enfantPrenom) + "' type='text' style='border:none' readonly>"
                                    + "</label>"
                                    + "<label>"
                                        + "<span>Age : </span>"
                                        + "<input value='" + (listEnfant[i].enfantAge) + "' type='text' style='border:none' readonly>"
                                    + "</label>"
                                    + "<label>"
                                        + "<span>School : </span>"
                                        + "<input value='" + (listEnfant[i].enfantEcole) + "' type='text' style='border:none' readonly>"
                                    + "</label>"
                                + "</div>"
                $(contenus).appendTo($conteneur);
            }

            var listPersContact = resultat.listPersContact;
            for (var i = 0; i < listPersContact.length; i++) {
                var $conteneur = $(".autres-perso-contact");
                var contenus = "<h5>Contact person #" + (i + 1) + "</h5>"
                    + "<div class='field-contact'>"
                        + "<label>"
                            + "<span> Urgent Contact person: </span>"
                            + "<input value='" + (listPersContact[i].personneContactNom) + "' type='text' style='border:none' readonly>"
                        + "</label>"
                        + "<label>"
                            + "<span>Relationship : </span>"
                            + "<input value='" + (listPersContact[i].lienParente) + "' type='text' style='border:none' readonly>"
                        + "</label>"
                        + "<div class='phone-select'>"
                            + "<p>Phone Number</p>"
                            + "<label>"
                                + "<span>Country : </span>"
                                + "<input value='" + (listPersContact[i].codePhonePays) + "' type='text' style='border:none' readonly>"
                            + "</label>"
                            + "<label>"
                                + "<span>Number : </span>"
                                + "<input value='" + (listPersContact[i].personneContactPoneNumber) + "' type='text' style='border:none' readonly>"
                            + "</label>"
                        + "</div>"

                    + "</div>";
                $(contenus).appendTo($conteneur);
            }

            document.getElementById("contactReference").value = resultat.contactReference;
            document.getElementById("contactName").value = resultat.contactName;
            document.getElementById("contactSurname").value = resultat.contactSurname;
            document.getElementById("contactEmail").value = resultat.contactEmail;
            document.getElementById("contactNationalite").value = resultat.contactNationalite;
            document.getElementById("contactNumPassport").value = resultat.contactNumPassport;
            document.getElementById("contactResidence").value = resultat.contactResidence;

            document.getElementById("dateExpirePasseport").value = resultat.contactDateExpiration;
            document.getElementById("contactGrpSanguin").value = resultat.contactGrpSanguin;
            document.getElementById("contactMaladie").value = resultat.contactMaladie;

            document.getElementById("conjointNom").value = resultat.conjointNom;
            document.getElementById("conjointPrenom").value = resultat.conjointPrenom;
            document.getElementById("conjointCountry").value = resultat.conjointCodePhone;
            document.getElementById("conjointPhoneNumber").value = resultat.conjointPhoneNumber;
            $("#dlgDetailsContact").show();
        }
    });
}

function hideDataLine() {
    $("#dlgDetailsContact").hide();
    $('.autres-enfants').empty();
    $('.autres-perso-contact').empty();
    document.getElementById("contactReference").value = "";
    document.getElementById("contactName").value = "";
    document.getElementById("contactSurname").value = "";
    document.getElementById("contactEmail").value = "";
    document.getElementById("contactNationalite").value = "";
    document.getElementById("contactNumPassport").value = "";
    document.getElementById("contactResidence").value = "";

    document.getElementById("dateExpirePasseport").value = "";
    document.getElementById("contactGrpSanguin").value = "";
    document.getElementById("contactMaladie").value = "";

    document.getElementById("conjointNom").value = "";
    document.getElementById("conjointPrenom").value = "";
    document.getElementById("conjointCountry").value = "";
    document.getElementById("conjointPhoneNumber").value = "";
}


function checkAll(name) {
    if (document.getElementById("checkAllData").checked) {
        CheckedClick(name);
    }
    else {
        DecheckedClick(name);
    }
}