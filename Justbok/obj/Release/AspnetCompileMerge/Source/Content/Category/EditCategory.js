$(document).ready(function () {

    $('#btnSaveCategory').click(function () { if (ValidateCategory()) { SaveCategory(); } });
    HideLoader();
});


function SaveCategory() {
    var active = false;
    if ($('#chkActive').is(':checked')) {
        active = true
    }
    //alert($('#chkActive').val());

    var jsonObject = {

        CategoryId: $('#txtCategoryid').val(), CategoryName: $('#txtCategoryName').val(), CategoryDescription: $('#txtDescription').val(),
        Active: active, BranchId: $('#ddlBranch option:selected').val()
    }

    $.ajax({
        cache: false,
        type: "POST",
        url: AddCategoryUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            RedirectCategory();
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });

}

function RedirectCategory() {
    LoadPage('/Category/GetCategory/', 'Justbok | Category List');
    return false;
}

var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';

function ValidateCategory() {
   
    var IsValid = true;
    $('#txtCategoryName').parent().removeClass("has-error");
    $('#txtCategoryName').parent().find(".help-block-error").remove();

    if ($('#txtCategoryName').val() == "") {
        $('#txtCategoryName').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Category"));
        $('#txtCategoryName').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtCategoryName').parent().removeClass("has-error");
        $('#txtCategoryName').parent().find(".help-block-error").remove();
    }

    return IsValid;
}

function AllowOnlyText(elementid) {
    $('#' + elementid).keydown(function (e) {
        if (e.ctrlKey || e.altKey) {
            e.preventDefault();
        } else {
            var key = e.keyCode;
            if (!((key == 8) || (key == 32) || (key == 46) || (key >= 35 && key <= 40) || (key >= 65 && key <= 90))) {
                e.preventDefault();
            }
        }
    });
}

AllowOnlyText("txtCategoryName");
