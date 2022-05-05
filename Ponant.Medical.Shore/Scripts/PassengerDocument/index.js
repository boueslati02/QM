/*Chargement de page index PassengerDocument*/
$(function () {
    readyIndexPassengerDocument();
});

/*Chargement de page index PassengerDocument*/
function readyIndexPassengerDocument() {

    // Filtrage de la liste
    var searchButton = $("#searchPassengerDocumentIndexButton")
    if (searchButton != undefined) {
        $(searchButton).off("click").click(function () {
            MVCGrid.setAdditionalQueryOptions('PassengerDocument',
            {
                passengerFilter: $("#passenger").val(),
                cruiseFilter: $("#cruise").val(),
                bookingFilter: $("#booking").val()
            });
            $("#labInfo").hide();
        });
    }
}

/*Gere l'affichage de la modal d'ajout d'un nouveau passager*/
function addNewPassenger()
{
    var additionalData = {
        cruiseCode: $("#cruise").val(),
        bookingCode: $("#booking").val()
    };
    getModalView(undefined, '/PassengerDocument/_Create', "frmCreate", "modalCreate", additionalData);
}

/*Gere l'evenement clic de détachement d'un document du passager*/
function detachDocument(thisElement, idDocument) {
    if ($(thisElement) != undefined) {
        if (confirm("Are you sure you want to unlink this document ?")) {
            $.ajax("/PassengerDocument/Detach", {
                data: { idDocument: idDocument, withLink: true },
                method: "GET",
                cache: false
            }).done(function (data) {
                if (data.result) {
                    if (confirm("Would you like link this document to another passenger ?")) {
                        var idsDocumentsValue = new Array();
                        idsDocumentsValue.push(idDocument);
                        $.post("/AvailableDocument/_LinkGet", { idsDocuments: idsDocumentsValue, controllerName: "PassengerDocument", "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() }, function (data) {
                            displayModalView("frmLink", "modalLink", data, true);
                        });
                    }
                    else {
                        if (data.result && data.url) {
                            window.location.href = data.url;
                        }
                    }
                }
            });
        }
    }
}
