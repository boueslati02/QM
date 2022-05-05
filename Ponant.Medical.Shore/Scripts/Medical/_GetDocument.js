/*Chargement de page _GetDocument*/
$(function () {
    readyGetDocument("#modalDocuments");
});

/*Chargement de page _GetDocument*/
function readyGetDocument(masterContentId) {
    if ($(masterContentId).attr("id") != undefined) {

        // Event clic des liens de document
        $(".linkDocument").off("click").click(function () { 
            var seenText = $(masterContentId + " #SeenText").val();
            $(this).parent().parent().children(".tdStatus").text(seenText);
        });
    }
}