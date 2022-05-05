/*Chargement de page modal Create*/
$(document).ready(function () {
    readyCreatePassengerDocument();
});

/*Chargement de page modal Create*/
function readyCreatePassengerDocument() {
    var modalCreate = $('#modalCreate');
    if (modalCreate != undefined) {
        // Evenement fermeture de modal
        $(modalCreate).on('hidden.bs.modal', function () {
            var url = "/PassengerDocument/TemporaryFilesDelete?suppressionType=1";
            $.ajax(url, {
                cache: false
            });
        })
    }
}