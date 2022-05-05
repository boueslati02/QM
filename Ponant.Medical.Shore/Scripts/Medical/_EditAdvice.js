/*Chargement de page _EditAdvice*/
$(function () {
    readyEditAdvice("#modalEditAdvice");
});

/*Chargement de page _EditAdvice*/
function readyEditAdvice(masterContentId) {
    if ($(masterContentId).attr("id") != undefined) {

        // Event change du choix d'avis médical
        $(masterContentId + " #IdAdvice").off("change").change(function () { 
            $(masterContentId + " #divDocumentsTypes").hide();
            $(masterContentId + " #divUnfavorableOpinion").hide();
            $(masterContentId + " #divRestrictionsTypes").hide();
            $(masterContentId + " #divRestrictionsPeople").hide();
            $(masterContentId + " .listboxMultiSelect").multiselect("deselectAll", false);
            $(masterContentId + " .listboxMultiSelect").multiselect('updateButtonText');

            var adviceValue = $(this).val();
            $("div[adviceValue='" + adviceValue + "']").show();
        });
    }
}