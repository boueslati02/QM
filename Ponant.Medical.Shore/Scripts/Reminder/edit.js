/*Chargement de page Reminder*/
$(function () {
    ReminderPageReady();
});

/*Chargement de page Reminder*/
function ReminderPageReady() {
    ChangeCheckBoxValue($("#FirstReminderEnabled"));
    ChangeCheckBoxValue($("#SecondReminderEnabled"));
    ChangeCheckBoxValue($("#ThirdReminderEnabled"));

    $("#FirstReminderEnabled, #SecondReminderEnabled, #ThirdReminderEnabled").change(function () {
        ChangeCheckBoxValue($(this));
    });
}

/*Event change checkbox activation rappels*/
function ChangeCheckBoxValue(thisElement, recursiveCall) {  
    var inputjoin = $(thisElement).attr("inputjoin");

    // Désactivation des zones de saisies
    if ($(thisElement).is(":checked")) { 
        $("#" + inputjoin).removeAttr("disabled");
        $("#" + inputjoin).parent().find("button").removeAttr("disabled");
    }
    else{
        $("#" + inputjoin).attr("disabled", "disabled");
        $("#" + inputjoin).parent().find("button").attr("disabled", "disabled");
        $("#" + inputjoin).val(0);
    }
 
    // Désactivation des case à cochés inférieures
    if (!recursiveCall) { 
        if ($("#FirstReminderEnabled").is(":checked")) {
            $("#SecondReminderEnabled").removeAttr("disabled");
        }
        else {
            $("#SecondReminderEnabled").attr("disabled", "disabled");
            $("#SecondReminderEnabled").prop('checked', false);
            ChangeCheckBoxValue($("#SecondReminderEnabled"), true);
        }

        if ($("#SecondReminderEnabled").is(":checked")) {
            $("#ThirdReminderEnabled").removeAttr("disabled");
        }
        else {
            $("#ThirdReminderEnabled").attr("disabled", "disabled");
            $("#ThirdReminderEnabled").prop('checked', false);
            ChangeCheckBoxValue($("#ThirdReminderEnabled"), true);
        }
    }
}