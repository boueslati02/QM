/*Chargement de page index Medical*/
$(function () {
    readyIndexMedical();
});

/*Chargement de page index Medical*/
function readyIndexMedical() {
    readyListPassenger("#todoPassenger", "TodoPassenger", "todo");
    readyListPassenger("#donePassenger", "DonePassenger", "done");
}

/*Chargement des bouton de filtrage d'une vue*/
function readyListPassenger(masterContentId, mvcgridName, filterPrefix) {
    if ($(masterContentId).attr("id") != undefined) {

        // Affichage du nombre de ligne affiché dans le titre de l'onglet
        var link = $("[href='" + masterContentId + "']");
        if (!$(link).attr("addCounter"))
        {
            var rows = $("#MVCGridTable_" + mvcgridName + " tbody tr");
            var nbRows = 0;
            if ($(rows).find('td').length > 1) {
                nbRows = rows.length;
            }
            link.text($(link).text() + " (" + nbRows + ")");
            link.attr("addCounter", true);
        }  

        // Filtrage de la liste
        var searchButton = $(masterContentId + " #" + filterPrefix + "SearchMedicalButton")
        if (searchButton != undefined) {
            $(searchButton).off("click").click(function () { // Event clic du bouton Search
                MVCGrid.setAdditionalQueryOptions(mvcgridName,
                {
                    nameFilter: $(masterContentId + " #" + filterPrefix + "NameFilter").val(),
                    adviceFilter: $(masterContentId + " #" + filterPrefix + "AdviceFilter").val(),
                    doctorFilter: $(masterContentId + " #" + filterPrefix + "DoctorFilter").val(),
                });
            });
        }
    }
}