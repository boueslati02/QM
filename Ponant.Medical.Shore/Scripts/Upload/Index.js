$(document).ready(function () {
    LoadCloseButton();
    ChangeRadioButtonsEvent();
    CheckRelatedDocuments();
    LoadInputFiles();
});

function ChangeRadioButtonsEvent() {
    var radioButtons = $('input[type=radio][name=ResendQm]');
    if ((radioButtons.length != 0)) {
            DisableMedicalsurveyInput();
        radioButtons.change(function () {
            if (this.value == 1) {
                $("#MedicalSurvey").removeAttr('disabled');
                $("#BtnMedicalSurvey").css("background-color","#26afc2");
                $("#uploadImg").css("color","white");
                $('#RelatedDocuments1_FileData-error').hide();
            }
            if (this.value == 0) {
                DisableMedicalsurveyInput();
                $('#MedicalSurvey-error').hide();
            }
        });
    }
}

function DisableMedicalsurveyInput(){
    $("#MedicalSurvey").attr('disabled', 'disabled');
    $("#BtnMedicalSurvey").css("background-color","white");
    $("#BtnMedicalSurvey").css("border","1px solid #dcdcdc");
    $("#uploadImg").css("color","#dcdcdc");
}

function LoadCloseButton() {
    $("#btnClose").click(function () {
        window.close();
    });
}

function CheckRelatedDocuments(){
    $('#frmUpload').on('submit', function (event) {
        if ($('#questionQmResent').length != 0 && $('#radioBtnCancel').prop('checked')) {
            var isRelatedDocuments = false;
            $(".inputFile").each(function () {
                if ($("#" + $(this).attr('name')).val().length != 0) {
                    isRelatedDocuments = true;
                    return false;
                }
            });
            if (!isRelatedDocuments) {
                $('#RelatedDocuments1_FileData-error').show();
                $('#btnSubmitForm').button('reset');
                return false;
            }
        }
    });
}

function LoadInputFiles(){
    $('#frmUpload').on('change','.inputFile', function(){
        var names = [];
        var length = $(this).get(0).files.length;
        for (var i = 0; i < $(this).get(0).files.length; ++i) {
                names.push($(this).get(0).files[i].name);
        }
        if(length>2) {
            var fileName = names.join(', ');
            $(this).closest('.form-group').find('.inputFileText').attr("value",length+" files selected");
        }
        else{
            $(this).closest('.form-group').find('.inputFileText').attr("value",names);
        }
    });
}