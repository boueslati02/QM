/*Chargement de page modal Edit*/
$(document).ready(function () {
    readyEditLanguage();
});

/*Chargement de page modal Edit*/
function readyEditLanguage() {

    var modalEdit = $('#modalEdit');
    if (modalEdit != undefined) {
        // Evenement fermeture de modal
        $(modalEdit).on('hidden.bs.modal', function () {
            var url = "/Language/TemporaryFilesDelete?suppressionType=1";
            $.ajax({
                url: url,
                cache: false
            });
        })
    }
}