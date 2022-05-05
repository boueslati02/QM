/*Chargement de page index AvailableDocument*/
$(function () {
    readyIndexAvailableDocument();
});

/*Chargement de page index AvailableDocument*/
function readyIndexAvailableDocument() {
    // Chargement initial de la liste
    refreshGrid();

    // Filtrage de la liste
    var searchButton = $("#searchAvailableDocumentIndexButton")
    if (searchButton != undefined) {
        $(searchButton).off("click").click(function () {
            refreshGrid();
        });
    }

    getLinkModal("btnDocumentLink");
    sendActionCommand("btnDocumentDetach", "Detach", "Are you sure you want to detach these documents from their passengers ?");
    sendActionCommand("btnDocumentDelete", "Delete", "Are you sure you want to delete these documents ?");
}

/* Gere le raffraichissement de la grille des documents */
function refreshGrid() {
    var availableDocumentId = $("#MVCGridContainer_AvailableDocument").attr("id");
    if (availableDocumentId != undefined) {
        MVCGrid.setAdditionalQueryOptions('AvailableDocument',
           {
               documentFilter: $("#document").val(),
               passengerFilter: $("#passenger").val(),
               emailFilter: $("#email").val(),
               receiptDateFilter: $("#receiptDate").val()
           });
        $("#labInfo").hide();
    }
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

    // Activation - Désactivation des bouton d'action 
    var nbChecked = $(masterContentId + " .actionChk:checked").length;
    if (nbChecked >= 1) {
        $(masterContentId + " #btnDocumentLink").removeAttr("disabled");
        $(masterContentId + " #btnDocumentDetach").removeAttr("disabled");
        $(masterContentId + " #btnDocumentDelete").removeAttr("disabled");
    }
    else {
        $(masterContentId + " #btnDocumentLink").attr("disabled", "disabled");
        $(masterContentId + " #btnDocumentDetach").attr("disabled", "disabled");
        $(masterContentId + " #btnDocumentDelete").attr("disabled", "disabled");
    }
}

/* Gére la récupération de la modal de liaison des documents*/
function getLinkModal(idButton) {
    var actionButton = $("#" + idButton);
    if (actionButton != undefined) {
        $(actionButton).off("click").click(function () { // Event clic du bouton d'action
            var checkedDocuments = getCheckedDocuments();
            if (checkedDocuments.length > 0) { // Envoi des éléments au serveur
                $.post("/AvailableDocument/_LinkGet", { idsDocuments: checkedDocuments, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() }, function (data) {
                    displayModalView("frmLink", "modalLink", data, true);
                });
            }
        });
    }
}

/* Gére l'envoi des action vers le serveur */
function sendActionCommand(idButton, nomAction, messageConfirm) {
    var actionButton = $("#" + idButton);
    if (actionButton != undefined) {
        $(actionButton).off("click").click(function () { // Event clic du bouton d'action
            var confirmOk = false;
            if (messageConfirm) {
                confirmOk = confirm(messageConfirm);
            }
            else {
                confirmOk = true;
            }

            if (confirmOk) {
                var checkedDocuments = getCheckedDocuments();
                if (checkedDocuments.length > 0) { // Envoi des éléments au serveur
                    $.post("/AvailableDocument/" + nomAction, { idsDocuments: checkedDocuments, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() }, function (data) {
                        if (data.result && data.url) {
                            window.location.href = data.url;
                        }
                    });
                }
            }
        });
    }
}

/*Récupération des lignes coché*/
function getCheckedDocuments() {
    var checkedDocuments = new Array();
    $(".actionChk").each(function () {
        var ischecked = $(this).is(':checked');
        if (ischecked) {
            var idDocument = $(this).attr("idDocument");
            if (idDocument) {
                checkedDocuments.push(idDocument);
            }
        }
    });
    return checkedDocuments;
}