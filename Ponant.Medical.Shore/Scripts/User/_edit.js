/*Chargement de page modal Edit*/
$(document).ready(function () {
    autocompleteAgencyEditUser();
    readyEditUser();
    changeValuesUserLogoEdit();
});

function autocompleteAgencyEditUser() {
    $("#agencyFormUserEdit").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/AgencyAccessRight/GetAgencyList",
                type: "POST",
                dataType: "json",
                data: { name: request.term , "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() },
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
            $("#agencyFormUserEdit").val(i.item.label);
            $("#IdAgency").val(i.item.value);
            return false;
        },
        appendTo: $("#formUserEdit"),
        minLength: 3
    });
}

/*Chargement de page modal Edit*/
function readyEditUser() {

    var modalEdit = $('#modalEdit');
    if (modalEdit != undefined) {
        // Evenement fermeture de modal
        $(modalEdit).on('hidden.bs.modal', function () {
            var url = "/User/TemporaryLogosDelete?suppressionType=1";
            $.ajax({
                url: url,
                cache: false
            });
        })
    }
}

function changeValuesUserLogoEdit() {
    $("#divHideLogoUserEdit").hide();
    if ($("#changeValuesUserEdit").val() == "5F2BBA55-37C6-4179-BC08-8E10F579E9DF") {
        $("#divHideLogoUserEdit").show();
    }
    else {
        $("#divHideLogoUserEdit").hide();
    }

    $("#changeValuesUserEdit").change(function () {
        var selectedBoard = $("#changeValuesUserEdit").val();
        if (selectedBoard.toString() == "5F2BBA55-37C6-4179-BC08-8E10F579E9DF") {
            $("#divHideLogoUserEdit").show();
        } else {
            $("#divHideLogoUserEdit").hide();
        }
    })
}