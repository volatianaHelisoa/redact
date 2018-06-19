function CheckedClick(name) {
    var val = "input[name = '" + name + "']";
    $(val).each(function () {
        $(this).prop("checked", true);
    });

    if (!document.getElementById("checkAllData").checked) {
        var vals = "input[name = headCkeck]";
        $(vals).each(function () {
            $(this).prop("checked", true);
        });
    }
}

function DecheckedClick(name) {
    var val = "input[name = '" + name + "']:checked";
    $(val).each(function () {
        $(this).prop("checked", false);
    });

    if (document.getElementById("checkAllData").checked) {
        var vals = "input[name = headCkeck]";
        $(vals).each(function () {
            $(this).prop("checked", false);
        });
    }
}

function showMessageLine(message) {
    document.getElementById("messageCampaign").value = message;
    $("#showMessageCampaign").show();
}

function hideMessageLine() {
    $("#showMessageCampaign").hide();
}


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