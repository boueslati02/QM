$(document).ready(function () {
    autocompleteAgency();
});

function autocompleteAgency() {
    $("#agencyCreate").autocomplete({
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
            $("#agencyCreate").val(i.item.label);
            $("#IdAgency").val(i.item.value);
            return false;
        },
        appendTo: $("#formAgencyAccessRightCreate"),
        minLength: 3
    });
}