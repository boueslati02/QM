/*Chargement de page index Cruise*/
$(function () {
    readyIndexCruise();
});

/*Chargement de page index Cruise*/
function readyIndexCruise() {

    // Filtrage de la liste
    var searchButton = $('#searchCruiseIndexButton')
    if (searchButton != undefined) {
        $(searchButton).click(function () {
            MVCGrid.setAdditionalQueryOptions('Cruise',
            {
                cruiseCodeFilter: $("#cruiseCode").val(),
                cruisePassengerFilter: $("#cruisePassenger").val(),
                cruiseShipFilter: $("#cruiseShip").val(),
                cruiseDateDepartureFilter: $("#cruiseDateDeparture").val(),
                cruiseTypeFilter: $("#cruiseType").val(),
                cruiseDestinationFilter: $("#cruiseDestination").val(),
                cruiseAgencyFilter: $("#cruiseAgency").val()
            });
        });
    }
}

