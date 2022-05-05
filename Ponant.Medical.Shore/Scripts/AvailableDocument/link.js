/*Chargement de page Link AvailableDocument*/
$(function () {
    readyLinkPassenger("#modalLink");
});

/*Chargement de la partie passager individuel*/
function readyLinkPassenger(masterContentId) {
    if ($(masterContentId).attr("id") != undefined) {
        // Filtrage de la liste des passagers
        var searchButton = $(masterContentId + " #searchAvailableDocumentLinkButton")
        if (searchButton != undefined) {
            $(searchButton).off("click").click(function () { // Event clic du bouton Search
                MVCGrid.setAdditionalQueryOptions('DocumentLinkPassenger',
                {
                    linkCruiseFilter: $(masterContentId + " #cruiseFilter").val(),
                    linkBookingFilter: $(masterContentId + " #bookingFilter").val(),
                    linkGroupFilter: $(masterContentId + " #groupFilter").val(),
                    linkPassengerFilter: $(masterContentId + " #passengerFilter").val(),
                });
                $(masterContentId + " #labInfo").hide();
            });
        }
    }
}

/*Event de selection d'un radio bouton d'un passager*/
function radioPassengerChange(thisElement) {
    var idPassenger = $(thisElement).val();
    $("#modalLink #IdPassenger").val(idPassenger);
}