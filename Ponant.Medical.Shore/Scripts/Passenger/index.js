/*Chargement de page index Passenger*/
$(function () {
    readyIndexPassenger();
});

/*Chargement de page index Passenger*/
function readyIndexPassenger() {
    readyIndivPassenger("#indivPassenger");
    readyGroupPassenger("#groupPassenger");
}

/*Chargement de la partie passager individuel*/
function readyIndivPassenger(masterContentId) {
    if ($(masterContentId).attr("id") != undefined) {

        // Filtrage de la liste des passagers individuel
        var searchButton = $(masterContentId + " #searchPassengerIndivButton")
        if (searchButton != undefined) {
            $(searchButton).off("click").click(function () { // Event clic du bouton Search
                refreshIndivPassenger(masterContentId);
            });
        }

        // Relance des passagers individuel
        var relaunchButton = $(masterContentId + " #RelaunchIndivButton")
        if (relaunchButton != undefined) {
            $(relaunchButton).off("click").click(function () { // Event clic du bouton Relaunch Individual

                var checkedPassengers = new Array();  // Récupération des ligne coché
                var needToUpdatePassengers = false;
                $(masterContentId + " .relaunchChk").each(function () {
                    var ischecked = $(this).is(':checked');
                    if (ischecked) {
                        var IdPassenger = $(this).attr("IdPassenger");
                        if (IdPassenger) {
                            checkedPassengers.push(IdPassenger);
                        }
                        // Know if need to update status
                        if (!needToUpdatePassengers && $(this).attr("passengerstatus") == '30') {
                            needToUpdatePassengers = true;
                        }
                    }
                });
                //Pop up if QmReceived have been selected
                if (needToUpdatePassengers) {
                    confirmRelaunch("/Passenger/_ConfirmRelaunch");
                }
                else {
                    var idCruiseValue = $("#IdCruise").val();
                    if (checkedPassengers.length > 0) { // Envoi des éléments au serveur
                        $.post("/Passenger/IndividualRelaunch", { idRelaunchPassengers: checkedPassengers, idCruise: idCruiseValue, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() }, function (result) {
                            displayResultRelaunchAjax(result);
                            refreshIndivPassenger(masterContentId);
                        });
                    }
                    else {
                        $("#errorAlert").hide();
                        $("#successAlert").hide();
                    }
                }
            })
        }
    }
}

/* Gere le raffraichissement de la liste des passagers individuel */
function refreshIndivPassenger(masterContentId) {
    MVCGrid.setAdditionalQueryOptions('IndividualPassenger', {
        nameFilter: $(masterContentId + " #indivNameFilter").val(),
        qmStatusFilter: $(masterContentId + " #indivQmStatusFilter").val(),
        officeFilter: $(masterContentId + " #indivOfficeFilter").val(),
    });
}

/*Chargement de la partie passager groupe*/
function readyGroupPassenger(masterContentId) {
    if ($(masterContentId).attr("id") != undefined) {
        setBookingGroupId(null, masterContentId);
        loadAgencyGroupFeatures(masterContentId);

        // Event changement d'onglet de groupe
        $(masterContentId + " a[data-toggle='tab']").on("show.bs.tab", function (e) { // Event changement de tabs group
            var idBookingPrevious = $(masterContentId + " .nav-tabs li.active a").attr("bookingGroupId");
            var idBookingCurrent = $(e.target).attr("bookingGroupId");
            saveSelectedRow(masterContentId, idBookingPrevious);
            setBookingGroupId(idBookingCurrent, masterContentId);
            loadAgencyGroupFeatures(masterContentId);
            checkGroupPassengerGridReload(masterContentId, idBookingCurrent,5);
        });

        // Filtrage de la liste des passagers de groupe
        var searchButton = $(masterContentId + " #searchPassengerGroupButton")
        if (searchButton != undefined) {
            $(searchButton).off("click").click(function () { // Event clic du bouton Search
                refreshGroupPassenger(masterContentId);
            });
        }

        // Relance des passagers individuel
        var relaunchButton = $(masterContentId + " #RelaunchIndivInGroupButton")
        if (relaunchButton != undefined) {
            $(relaunchButton).off("click").click(function () { // Event clic du bouton Relaunch Individual

                var checkedPassengers = new Array();  // Récupération des ligne coché
                var needToUpdatePassengersIndiv = false;
                $(masterContentId + " .relaunchChk").each(function () {
                    var ischecked = $(this).is(':checked');
                    if (ischecked) {
                        var IdPassenger = $(this).attr("IdPassenger");
                        if (IdPassenger) {
                            checkedPassengers.push(IdPassenger);
                        }
                        // Know if need to update status
                        if (!needToUpdatePassengersIndiv && $(this).attr("passengerstatus") == '30') {
                            needToUpdatePassengersIndiv = true;
                        }
                    }
                });
                //Pop up if QmReceived have been selected
                if (needToUpdatePassengersIndiv) {
                    confirmRelaunch("/Passenger/_ConfirmRelaunch");
                }
                else {
                    var idCruiseValue = $("#IdCruise").val();
                    if (checkedPassengers.length > 0) { // Envoi des éléments au serveur
                        $.post("/Passenger/IndividualRelaunch", { idRelaunchPassengers: checkedPassengers, idCruise: idCruiseValue, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() }, function (result) {
                            displayResultRelaunchAjax(result);
                            refreshGroupPassenger(masterContentId);
                            $(masterContentId + " #RelaunchIndivInGroupButton").attr("disabled", "disabled");
                        });
                    }
                    else {
                        $("#errorAlert").hide();
                        $("#successAlert").hide();
                    }
                }
            })
        }

        //Export des passagers
        var ExportGroupPassengerButton = $(masterContentId + " #ExportGroupPassengerButton");
        if (ExportGroupPassengerButton != undefined) {
            $(ExportGroupPassengerButton).off("click").click(function () { // Event clic du bouton ExportGroupPassenger
                var idBookingValue = $(masterContentId + " #IdBooking").val();
                if (idBookingValue) { // Envoi des éléments au serveur
                    window.open("/Passenger/ExportGroupPassenger/" + idBookingValue);
                }
            });
        }

        //Téléchargement du template
        var DownloadTemplateButton = $(masterContentId + " #DownloadTemplate");
        if (DownloadTemplateButton != undefined) {
            $(DownloadTemplateButton).off("click").click(function () { // Event clic du bouton DownloadTemplateButton
                var idBookingValue = $(masterContentId + " #IdBooking").val();
                if (idBookingValue) { // Envoi des éléments au serveur
                    window.open("/Passenger/DownloadTemplate/" + idBookingValue);
                }
            });
        }

        //Ouverture de la boîte de sélection d'un fichier
        $('#ImportPassenger').on('click', function () {
            $('#FileInputPassenger').trigger('click');
        });

        //Import des passagers
        $("#FileInputPassenger").change(function () {
            var idBookingValue = $(masterContentId + " #IdBooking").val();
            var idCruiseValue = $("#IdCruise").val();
            var fd = new FormData();
            var files = $(this)[0].files;

            if (files.length > 0) {
                fd.append("file", files[0]);
                fd.append("idCruise", idCruiseValue);
                fd.append("__RequestVerificationToken", $("input[name=__RequestVerificationToken]").val());

                $.ajax({
                    url: "/Passenger/ImportPassenger/" + idBookingValue,
                    type: "post",
                    data: fd,
                    contentType: false,
                    processData: false,
                    success: function () {
                        location.reload();
                    },
                });
            }
        });
    }
}

/* Gere le raffraichissement de la liste des passagers de group */
function refreshGroupPassenger(masterContentId) {
    MVCGrid.setAdditionalQueryOptions('GroupPassenger', {
        groupIdFilter: $(masterContentId + " #IdBooking").val(),
        nameFilter: $(masterContentId + " #groupNameFilter").val(),
        qmStatusFilter: $(masterContentId + " #groupQmStatusFilter").val(),
    });
}

/*Affichage du retour ajax pour l'affichage d'un message utilisateur*/
function displayResultRelaunchAjax(result) {
    if (result.message) {
        if (result.success) {
            $("#errorAlert").hide();
            $("#successAlert").html(result.message);
            $("#successAlert").show();
        }
        else {
            $("#successAlert").hide();
            $("#errorAlert").html(result.message);
            $("#errorAlert").show();
        }
    }
}

/*Modification du groupe afficher dans la grille*/
function setBookingGroupId(bookingGroupId, masterContentId) {
    if (bookingGroupId) {
        $(masterContentId + " #IdBooking").val(bookingGroupId);
    }


    MVCGrid.setAdditionalQueryOptions('GroupPassenger', {
        IdBookingFilter: $(masterContentId + " #IdBooking").val(),
    });
}

/* Gere l'événement de changement d'état des case à coché */
function changeCheckbox(thisElement, masterContentId) {
    // Ajout - Suppression ligne selectionné
    if ($(thisElement).is(":checked")) {
        $(thisElement).parents('tr').addClass("selectedRow");
    }
    else {
        $(thisElement).parents('tr').removeClass("selectedRow");
    }

    // Activation - Désactivation des boutons d'actions
    // Activation - Désactivation Indiv relaunch
    var nbCheckedIndiv = $(masterContentId + " .relaunchChk:checked").length;
    if (nbCheckedIndiv >= 1) {
        $(masterContentId + " #RelaunchIndivButton").removeAttr("disabled");
    }
    else {
        $(masterContentId + " #RelaunchIndivButton").attr("disabled", "disabled");
    }

    switchRelaunchGroupButton(masterContentId);
}

function switchRelaunchGroupButton(masterContentId) {
    var nbCheckedIndivInGroup = $(masterContentId + " .relaunchChk:checked").length;
    if (nbCheckedIndivInGroup >= 1) {
        $(masterContentId + " #RelaunchIndivInGroupButton").removeAttr("disabled");
    }
    else {
        $(masterContentId + " #RelaunchIndivInGroupButton").attr("disabled", "disabled");
    }
}

function ChangeStatusOrNot(statusNeedToBeUpdate) {
    var idCruiseValue = $("#IdCruise").val();
    var checkedPassengers = new Array();
    var groupOrIndiv = $(".tab-pane.active").attr("id");
    groupOrIndiv = "#" + groupOrIndiv;

    $(groupOrIndiv + " .relaunchChk").each(function () {
        var ischecked = $(this).is(':checked');
        if (ischecked) {
            var IdPassenger = $(this).attr("IdPassenger");
            if (IdPassenger) {
                checkedPassengers.push(IdPassenger);
            }
        }
    });

    $.post("/Passenger/IndividualRelaunch", { idRelaunchPassengers: checkedPassengers, idCruise: idCruiseValue, statusNeedToBeUpdate: statusNeedToBeUpdate, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() }, function (result) {
        modalNew(null, 'modalConfirmRelaunch', 'frmConfirmRelaunch');
        displayResultRelaunchAjax(result);
        refreshIndivPassenger(groupOrIndiv);
    });
}

function loadAgencyGroupFeatures(masterContentId) {
    switchDisableButtons();
    var idBooking = $(masterContentId + " #IdBooking").val();
    var idAgency = $("#group_" + idBooking + " #IdAgency").text();
    checkAgencyType(masterContentId, idAgency);
}

function switchDisableButtons() {
    $("#ExportGroupPassengerButton").prop("disabled", false);
    $("#DownloadTemplate").prop("disabled", true);
    $("#ImportPassenger").prop("disabled", true);

    if ($('#MVCGridTable_GroupPassenger tr td').attr("colspan") != undefined) {
        $("#ExportGroupPassengerButton").prop("disabled", true);
        $("#DownloadTemplate").prop("disabled", false);
        $("#ImportPassenger").prop("disabled", false);
    }
}

function checkAgencyType(masterContentId, idAgency) {
    $.ajax({
        url: "/Passenger/IsAgencyPonant",
        type: "POST",
        dataType: "json",
        data: { idAgency: idAgency, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() },
        success: function (data) {
            switchDisableButtons();
            setButtonVisibility(masterContentId, data);
            setColumnVisibility(data);
            setTimeout(function () {
                switchDisableButtons();
                setButtonVisibility(masterContentId, data);
                setColumnVisibility(data);
            }, 900);
        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}

function setButtonVisibility(masterContentId, data) {
    var relaunchIndivButton = $(masterContentId + " #RelaunchIndivInGroupButton");
    var downloadTemplateButton = $(masterContentId + " #DownloadTemplate");
    var addPassengerButton = $(masterContentId + " #AddPassenger");
    var importPassengerButton = $(masterContentId + " #ImportPassenger");

    $(relaunchIndivButton).addClass("hidden");
    $(downloadTemplateButton).addClass("hidden");
    $(addPassengerButton).addClass("hidden");
    $(importPassengerButton).addClass("hidden");

    if (!data.IsAgencyPonant && data.IsAgency) {
        $(relaunchIndivButton).removeClass("hidden");
        $(addPassengerButton).removeClass("hidden");
        $(downloadTemplateButton).removeClass("hidden");
        $(importPassengerButton).removeClass("hidden");
    }

    if (data.IsAgencyPonant && !data.IsAgency) {
        $(relaunchIndivButton).removeClass("hidden");
    }
}


function setColumnVisibility(data) {

    if (data.IsAgencyPonant && !data.IsAgency) {
        var columns = visibilityColumnsGroupPassengerAll();
        MVCGrid.setColumnVisibility('GroupPassenger', columns);
    }

    if (!data.IsAgencyPonant && data.IsAgency) {
        var columns = visibilityColumnsGroupPassengerAgency();
        MVCGrid.setColumnVisibility('GroupPassenger', columns);
    }

    if (!data.IsAgencyPonant && !data.IsAgency) {
        var columns = visibilityColumnsGroupPassengerRestricted();
        MVCGrid.setColumnVisibility('GroupPassenger', columns);
    }
}

function visibilityColumnsGroupPassengerRestricted() {
    var columnsDefinitions = {};
    columnsDefinitions["Check"] = false;
    columnsDefinitions["PassengerNumber"] = true;
    columnsDefinitions["Civility"] = true;
    columnsDefinitions["LastName"] = true;
    columnsDefinitions["UsualName"] = true;
    columnsDefinitions["FirstName"] = true;
    columnsDefinitions["Email"] = false;
    columnsDefinitions["EmailUpdated"] = false;
    columnsDefinitions["ValidEmail"] = true;
    columnsDefinitions["QMStatus"] = true;
    columnsDefinitions["SendingDate"] = true;
    columnsDefinitions["ReceiptDate"] = true;
    columnsDefinitions["NbSent"] = true;
    columnsDefinitions["AutoAttachment"] = true;
    columnsDefinitions["Actions"] = false;
    return columnsDefinitions;
}

function visibilityColumnsGroupPassengerAll() {
    var columnsDefinitions = {};
    columnsDefinitions["Check"] = true;
    columnsDefinitions["PassengerNumber"] = true;
    columnsDefinitions["Civility"] = true;
    columnsDefinitions["LastName"] = true;
    columnsDefinitions["UsualName"] = true;
    columnsDefinitions["FirstName"] = true;
    columnsDefinitions["Email"] = true;
    columnsDefinitions["EmailUpdated"] = true;
    columnsDefinitions["ValidEmail"] = true;
    columnsDefinitions["QMStatus"] = true;
    columnsDefinitions["SendingDate"] = true;
    columnsDefinitions["ReceiptDate"] = true;
    columnsDefinitions["NbSent"] = true;
    columnsDefinitions["AutoAttachment"] = true;
    columnsDefinitions["Actions"] = true;
    return columnsDefinitions;
}

function visibilityColumnsGroupPassengerAgency() {
    var columnsDefinitions = {};
    columnsDefinitions["Check"] = true;
    columnsDefinitions["PassengerNumber"] = true;
    columnsDefinitions["Civility"] = true;
    columnsDefinitions["LastName"] = true;
    columnsDefinitions["UsualName"] = true;
    columnsDefinitions["FirstName"] = true;
    columnsDefinitions["Email"] = true;
    columnsDefinitions["EmailUpdated"] = false;
    columnsDefinitions["ValidEmail"] = true;
    columnsDefinitions["QMStatus"] = true;
    columnsDefinitions["SendingDate"] = true;
    columnsDefinitions["ReceiptDate"] = true;
    columnsDefinitions["NbSent"] = true;
    columnsDefinitions["AutoAttachment"] = true;
    columnsDefinitions["Actions"] = true;
    return columnsDefinitions;
}

/*Gere l'affichage de la modal d'ajout d'un nouveau passager*/
function addNewPassenger(thisElement) {
    var additionalData = {
        cruiseId: $("#IdCruise").val(),
        bookingNumber: $("#BookingNumber").text()
    };
    getModalView(thisElement, '/Passenger/_Create', "frmCreate", "modalCreate", additionalData);
}

function editPassenger(id, idCruise, idBooking) {
    modalEditPassenger("/Passenger/_Edit/", id, idCruise, idBooking);
}

function editAgencyPassenger(id, idCruise, idBooking) {
    modalEditPassenger("/Passenger/_EditAgency/", id, idCruise, idBooking);
}


function modalEditPassenger(url, id, idCruise, idBooking) {
    var additionalData = {
        Id: id,
        idCruise: idCruise,
        idBooking: idBooking,
    };
    getModalView(this, url, "frmEdit", "modalEdit", additionalData, false);
}

function checkGroupPassengerGridReload(masterContentId, idBookingCurrent,maximumRecursionLoop) {
    if(maximumRecursionLoop > 0){
        var gridSelectedRow = JSON.parse(sessionStorage.getItem("gridSelectedRowKey" + idBookingCurrent));
        var idPassengerAssociated = JSON.parse(sessionStorage.getItem("idFirstPassenger" + idBookingCurrent));
        var idFirstPassenger = $(masterContentId + " #MVCGridTable_GroupPassenger tbody tr input.relaunchChk").eq(0).attr("idpassenger");

        if (gridSelectedRow != null && idPassengerAssociated != null) {
            if (idPassengerAssociated == idFirstPassenger && $("#MVCGrid_Loading_GroupPassenger").css("visibility") == "hidden") {
                loadSelectedRow(masterContentId, gridSelectedRow);
                switchRelaunchGroupButton(masterContentId);
            }
            else {
                maximumRecursionLoop--;
                setTimeout(function () {
                    checkGroupPassengerGridReload(masterContentId, idBookingCurrent,maximumRecursionLoop);
                }, 1000);
            }
        }
    }
}

function loadSelectedRow(masterContentId, gridSelectedRow) {
    if (gridSelectedRow != null) {
        $(masterContentId + " #MVCGridTable_GroupPassenger tbody tr").each(function (i) {
            if (gridSelectedRow.length < i) {
                return;
            }

            if (gridSelectedRow[i]) {
                $(this).find(".relaunchChk").prop("checked", "true");
                $(this).addClass("selectedRow");
            }
        });
    }
}

function saveSelectedRow(masterContentId, idBookingPrevious) {
    if ($("#MVCGridTable_GroupPassenger tbody tr input.relaunchChk").length != 0) {
        var gridSelectedRow = [];
        var idFirstPassenger = $(masterContentId + " #MVCGridTable_GroupPassenger tbody tr input.relaunchChk").eq(0).attr("idpassenger");
        sessionStorage.setItem("idFirstPassenger" + idBookingPrevious, JSON.stringify(idFirstPassenger));
        $(masterContentId + " #MVCGridTable_GroupPassenger tbody tr").each(function (i) {
            if ($(this).hasClass("selectedRow")) {
                gridSelectedRow.push(true);
            }
            else {
                gridSelectedRow.push(false);
            }
        });
        sessionStorage.setItem("gridSelectedRowKey" + idBookingPrevious, JSON.stringify(gridSelectedRow));
    }
}