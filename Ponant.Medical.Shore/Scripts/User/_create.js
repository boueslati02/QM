/*Chargement de page modal Create*/
$(document).ready(function () {
    autocompleteAgencyCreateUser();
    readyCreateUser();
    changeValuesUserLogoCreate();
});

function autocompleteAgencyCreateUser() {
    $("#agencyFormUserCreate").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/AgencyAccessRight/GetAgencyList",
                type: "POST",
                dataType: "json",
                data: { name: request.term, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.Text, value: item.Value };
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            })
        },
        select: function (e, i) {
            $("#agencyFormUserCreate").val(i.item.label);
            $("#IdAgency").val(i.item.value);
            return false;
        },
        appendTo: $("#formUserCreate"),
        minLength: 3
    });
}

/*Chargement de page modal Create*/
function readyCreateUser() {
    var modalCreate = $('#modalCreate');
    if (modalCreate != undefined) {
        // Evenement fermeture de modal
        $(modalCreate).on('hidden.bs.modal', function () {
            var url = "/User/TemporaryLogosDelete?suppressionType=1";
            $.ajax({
                url: url,
                cache: false
            });
        })
    }

}

function changeValuesUserLogoCreate() {
    $("#divHideLogoUserCreate").hide();
    $("#changeValuesUserCreate").change(function () {
        var selectedBoard = $("#changeValuesUserCreate").val();
        if (selectedBoard.toString() == "5F2BBA55-37C6-4179-BC08-8E10F579E9DF") {
            $("#divHideLogoUserCreate").show();
        } else {
            $("#divHideLogoUserCreate").hide();
        }
    })
}