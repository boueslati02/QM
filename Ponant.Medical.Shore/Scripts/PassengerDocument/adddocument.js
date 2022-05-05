/*Chargement de page modal AddDocument*/
$(function () {
    readyAddDocumentPassengerDocument();
});

/*Chargement de page modal AddDocument*/
function readyAddDocumentPassengerDocument() {
    var modalAddDocument = $('#modalAddDocument');
    if (modalAddDocument != undefined) {
        // Evenement fermeture de modal
        $(modalAddDocument).on('hidden.bs.modal', function () {
            var url = "/PassengerDocument/TemporaryFilesDelete?suppressionType=1";
            $.ajax(url, {
                cache: false
            });
        })
    }
}