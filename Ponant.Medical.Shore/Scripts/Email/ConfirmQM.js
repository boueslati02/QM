$(function () {
    $("#CheckBoxConfirmQM").on("click", function(e){
        if ($('input#flexCheckChecked').is(':checked')){
            $("#ConfirmQM").attr("disabled", false);
        }else{
            $("#ConfirmQM").attr("disabled", true);
        }
    })
    $("#ConfirmQM").on("click", function(e){
        e.preventDefault();
        let formdata= new FormData($('#AcceptQM')[0]);
        $.ajax({
            type: "POST", 
            url: window.location.origin+"/Email/ChangeStatusPassager/",
            data:formdata,
            dataType: 'json',
            cache: false,
            contentType: false,
            processData: false,
            success: function(data) {
                $("#ConfirmQM").attr("disabled", true);
                $("#input#flexCheckChecked").attr("disabled", true);
                $("#SuccessMessage").removeClass("hidden");
            }
      });
    })
});