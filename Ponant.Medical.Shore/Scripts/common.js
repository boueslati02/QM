/*Chargement de page générique*/
$(document).ready(function () {
    documentReady();
});

/*Evenement au scroll de la page*/
$(window).scroll(function () {
    fixFilterDivOnScroll();
});

/*Chargement de page générique*/
function documentReady() {
    MVCGrid.init();
    // Application d'un champ input de type numeric
    $(".numericInput").TouchSpin({
        initval: 0,
        min: 0,
        max: 1000000,
        verticalbuttons: true
    });

    // Application d'un champ listbox de type multiselect
    $('.listboxMultiSelect').multiselect({
        includeSelectAllOption: true
    });

    // Application des champs de type date picker
    $('.datepickerInput').datepicker({
        format: "m/d/yyyy",
        clearBtn: true,
        calendarWeeks: true,
        autoclose: true
    });

    changeValuesUserCreate();
    changeValuesUserEdit();
    saveAndReloadScroll();// Enregistre la position de scroll vertical
    starForRequiredInput();// Ajoute une étoile à la fin de chaque label de champ requis (sauf pour les input checkbox)
    loadFileInput(); // Application d'un champ input de type file 
    setLoadingFormButton(1000); // Changement du bouton submit après clic
    hideAlertAfterTime(10000)// Masque les alertes aprés un temps défini
    CorrectGridPagination(); //Supprime la zone de pagination des grille et modifie l'affichage du nombre d'élement de la grille
    displaySelectedRow(); // Rechargement de la ligne selectionné  
    loadStorageFilter(); // Rechargement des filtre enregistré dans session storage
    $("[title]").tooltip(); // Application des tooltip javascript 
}

/* Enregistre et recharge la position du scroll vertical*/
function saveAndReloadScroll() {
    if (sessionStorage.scrollTop != "undefined") {
        $(window).scrollTop(sessionStorage.scrollTop);
    }
    $(window).scroll(function () {
        sessionStorage.scrollTop = $(this).scrollTop();
    });
}

/*Masque les alertes aprés un temps défini*/
function hideAlertAfterTime(time) {
    setTimeout(function () {
        var succesAlert = $("#successAlert");
        if (succesAlert != undefined && $(succesAlert).is(":visible")) {
            succesAlert.slideUp("slow", function () {
                $(errorAlert).hide();
            });
        }

        var errorAlert = $("#errorAlert");
        if (errorAlert != undefined && $(errorAlert).is(":visible")) {
            errorAlert.slideUp("slow", function () {
                $(errorAlert).hide();
            });
        }
    }, time);
}

/* Changement du bouton submit en loading après clic */
function setLoadingFormButton(time) {
    $("form").submit(function () {
        var form = $(this);
        var formId = $(form).attr("id");
        if ($(form).valid()) {
            $("#" + formId + " input[type=submit]").button('loading');
        }

        // Correction bug bouton loading => TODO a revoir
        setTimeout(function () {
            if (!$(form).valid()) {
                $("#" + formId + " input[type=submit]").button('reset');
            }
        }, time);
    });
}

/*Application des champ input de type file*/
function loadFileInput() {
    $(".fileInput").each(function () {
        var input = $(this);
        var acceptExtensions = $(input).attr("accept").replace(/\.| /g, '').split(',');
        $(input).fileinput({
            showCaption: true,
            showPreview: false,
            showUpload: false,
            showCancel: false,
            showRemove: true,
            removeLabel: '',
            showBrowse: true,
            browseLabel: '',
            showUploadedThumbs: false,
            autoReplace: true,
            allowedFileExtensions: acceptExtensions,
            maxFileSize: $(input).attr("maxFileSize"),
            maxFileCount: 1,
            uploadAsync: true,
            uploadUrl: $(input).attr("baseUploadUrl") + "?name=" + $(input).attr("name"),
            uploadExtraData: { "__RequestVerificationToken": $("input[name = __RequestVerificationToken]").val() },
            language: $(input).attr("language")
        }).on("filebatchselected", function (event, files) { // Evenement de selection d'un fichier
            if (files.length > 0) {
                var errormessage = $("#fileuploaderror");
                if (errormessage != undefined) {
                    $(errormessage).html("");
                    $(errormessage.hide());
                }
                var uploadFileError = $("#UploadFileError")
                if (uploadFileError != undefined) {
                    $(uploadFileError).val(false);
                }
                var filename = files[0].name;
                $(input).fileinput("upload");
                $("#" + $(input).attr("name")).val(filename);
            }
        }).on("fileuploaderror", function (event, folders, msg) { // Evenement erreur lors de la selection d'un fichier
            var errormessage = $("#fileuploaderror");
            if (errormessage != undefined) {
                $(errormessage).html(msg);
                $(errormessage.show());
            }
            var uploadFileError = $("#UploadFileError")
            if (uploadFileError != undefined) {
                $(uploadFileError).val(true);
            }
        })
    });
}

/*Ajoute une étoile à la fin de chaque label de champ requis (sauf pour les input checkbox)*/
function starForRequiredInput() {
    $('form').each(function () {
        var formId = $(this).attr('id');
        $("#" + formId + " input[data-val-required]:not(:checkbox), #" + formId + " select[data-val-required] , #" + formId + " textarea[data-val-required]").each(function () {
            if (($("#" + formId + " label[for=" + $(this).attr('id') + "]").children("span[class='text-danger']").length) <= 0) {
                $("#" + formId + " label[for=" + $(this).attr('id') + "]").append(" <span class='text-danger'>*</span>");
            }
        });
    });
}

/*Supprime la zone de pagination des grille et modifie l'affichage du nombre d'élement de la grille*/
function CorrectGridPagination() {
    $(".pagination").remove();
    $("table[id*='MVCGridTable_']").each(function () {
        var gridTable = $(this);
        if (gridTable != undefined) {
            var nodeText = $(gridTable).next().children().first();
            var textSplit = nodeText.text().split(" ");
            $(nodeText).text(textSplit[0] + " " + textSplit[textSplit.length - 2] + " " + textSplit[textSplit.length - 1]);
        }
    });
}

/*Rechargement de la ligne selectionné */
function displaySelectedRow() {
    var idElement = sessionStorage.getItem("selectedRow");
    if (idElement) {
        var thisElement = $("[idelement='" + idElement + "']");
        $(thisElement).parents('tr').addClass("selectedRow");
    }
}

/* Fonction Executer aprés fin de chargement de la grille*/
function execCompleteGridLoad() {
    saveAndReloadScroll();
    CorrectGridPagination();
    displaySelectedRow();
    saveStorageFilter();
    $("[title]").tooltip();
}

/*Enregistre les filtre saisie par l'utilisateur*/
function saveStorageFilter() {
    $(".filterdiv input[type='text'], .filterdiv select").each(function () {
        var idInput = $(this).attr("id");
        var value = $(this).val();
        if (value) {
            sessionStorage.setItem(idInput, value);
        } else {
            sessionStorage.removeItem(idInput);
        }
    });
    sessionStorage.setItem("currentPath", window.location.pathname);
}

/*Recharge les filtre saisie par l'utilisateur*/
function loadStorageFilter() {
    var storageCurrentPath = sessionStorage.getItem("currentPath");
    if (storageCurrentPath && window.location.pathname == storageCurrentPath) {
        var reloadGrid = false;
        $(".filterdiv input[type='text'], .filterdiv select").each(function () {
            var idInput = $(this).attr("id");
            var value = sessionStorage.getItem(idInput);
            if (value) {
                $(this).val(value);
                reloadGrid = true;
            } else {
                sessionStorage.removeItem(idInput);
            }
        });

        // Recharge la grille si necessaire
        setTimeout(function () {
            $(".filterdiv input[type='button']").each(function () {
                var idFilterButton = $(this).attr("id");            
                var idGrid = $("table[id*='MVCGridTable_']").attr("id");
               
                //if (idFilterButton && reloadGrid && !idGrid) {  // Controle si la grille est déjà chargée => DESACTIVER
                if (idFilterButton && (reloadGrid || !idGrid)) {
                    $("#" + idFilterButton).click();
                }
            });
        }, 300);
    } else {
        sessionStorage.removeItem("currentPath");
        $(".filterdiv input[type='text'], .filterdiv select").each(function () {
            var idInput = $(this).attr("id");
            sessionStorage.removeItem(idInput);
        });
    }
}

/*Gere l'affichage d'une fenetre modale*/
function modalNew(data, modal, div) {
    if (data != null && data.result) {
        $("#" + modal).modal("hide");
        $(".modal-backdrop").remove();
        $("body").removeClass("modal-open");

        if (data.url) {
            window.location.href = data.url;
        }
    } else {
        $("#" + div).empty().html(data);
        $("#" + modal).removeClass("fade");
        $(".modal-backdrop").remove();
        $("#" + modal).modal({
            backdrop: "static",
            keyboard: true
        }, "show");
        $("#" + modal).addClass("fade in");
        $(".modal-backdrop").addClass("fade");
        $.validator.unobtrusive.parse($("#" + modal + " form"));
        documentReady();
    }
}

/*Récupere les modal de type Create*/
function create(url) {
    getModalView(undefined, url, "frmCreate", "modalCreate");
}

/*Récupere la modale de confirmation de relance des passagers*/
function confirmRelaunch(url) {
    getModalView(undefined, url, "frmConfirmRelaunch", "modalConfirmRelaunch", undefined, false);
}

/*Récupere les vue partielle modal*/
function getModalView(thisElement, urlString, divId, modalId, additionalData, reloadPage) {
    if (reloadPage == undefined) {
        reloadPage = true;
    }

    // Enregistrement ligne selectionné
    if ($(thisElement) != undefined)
    {
        $(".selectedRow").removeClass("selectedRow");
        $(thisElement).parents('tr').addClass("selectedRow");
        var idElement =  $(thisElement).attr("idElement");
        sessionStorage.setItem("selectedRow", idElement);
    }

    // Affichage de la vue modale
    if (urlString) {
        if (additionalData) {
            $.ajax(urlString, {
                data: additionalData,
                method: "GET",
                cache: false
            }).done(function (data) {
                displayModalView(divId, modalId, data, reloadPage);
            });
        } else {
            $.ajax(urlString, {
                method: "GET",
                cache: false
            }).done(function (data) {
                displayModalView(divId, modalId, data, reloadPage);
            });
        }
    }
}

/*Charge le contenu d'une fenetre modale*/
function displayModalView(divId, modalId, data, reloadPage) {
    $("#" + divId).empty().html(data);
    $("#" + modalId).modal({
        backdrop: "static",
        keyboard: true
    }, "show");
    $.validator.unobtrusive.parse($("#" + divId + " form"));
    if (reloadPage) {
        documentReady();
    }
}

//Vérifie la valeur selectionnée de la DropDownList et masque le champs Ship dans la vue User create
function changeValuesUserCreate() {
    $("#divHideShipUserCreate").hide();

    $("#changeValuesUserCreate").change(function () {
        var selectedBoard = $("#changeValuesUserCreate").val();
        if (selectedBoard.toString() == "8d65a01e-8cb4-4f01-a65a-290e65fc2baf") {
            $("#divHideShipUserCreate").show();
        } else {
            $("#divHideShipUserCreate").hide();
        }
    })
}

//Vérifie la valeur selectionnée de la DropDownList et masque le champs Ship dans la vue User Edit
function changeValuesUserEdit() {
    $("#divHideShipUserEdit").hide();
    //Vérification de la présence de la valeur Board si présent on active le champs
    if ($("#changeValuesUserEdit").val() == "8d65a01e-8cb4-4f01-a65a-290e65fc2baf") {
        $("#divHideShipUserEdit").show();
    }
    else {
        $("#divHideShipUserEdit").hide();
    }

    $("#changeValuesUserEdit").change(function () {
        var selectedBoard = $("#changeValuesUserEdit").val();
        if (selectedBoard.toString() == "8d65a01e-8cb4-4f01-a65a-290e65fc2baf") {
            $("#divHideShipUserEdit").show();
        } else {
            $("#divHideShipUserEdit").hide();
        }
    })
}

//Fixe le volet de recherche au scroll
function fixFilterDivOnScroll() {
    var fixedfilterdiv = $('.fixedfilterdiv:visible');
    if (fixedfilterdiv.length) {
        var searchBarTop = $(fixedfilterdiv).position().top;
        if ($(window).scrollTop() > searchBarTop - 20) {
            // Ajout de la class fixed-search-div
            $(fixedfilterdiv).addClass("fixed-search-div");
        } else {
            // Enlève la classe si en haut
            $(fixedfilterdiv).removeClass("fixed-search-div");
            $(".fixed-search-div").removeClass("fixed-search-div");
        }
    }
}