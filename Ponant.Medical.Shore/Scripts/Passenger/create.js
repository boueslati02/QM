/*Chargement de page modal Create*/
$(document).ready(function () {
    readyCreatePassenger();
});

/*Chargement de page modal Create*/
function readyCreatePassenger() {
    var modalCreate = $('#modalCreate');
    if (modalCreate != undefined) {
        // Evenement fermeture de modal
        $(modalCreate).on('hidden.bs.modal', function () {
            var url = "/Passenger/TemporaryFilesDelete?suppressionType=1";
            $.ajax(url, {
                cache: false
            });
        })
    }
}