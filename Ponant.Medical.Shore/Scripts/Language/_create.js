/*Chargement de page modal Create*/
$(document).ready(function () {
    readyCreateLanguage();
});

/*Chargement de page modal Create*/
function readyCreateLanguage() {
    var modalCreate = $('#modalCreate');
    if (modalCreate != undefined) {
        // Evenement fermeture de modal
        $(modalCreate).on('hidden.bs.modal', function () {
            var url = "/Language/TemporaryFilesDelete?suppressionType=1";
            $.ajax({
                url: url,
                cache: false
            });
        })
    }

}