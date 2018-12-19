$(document).ready(function () {
    BindCategory();
    $('#btnSavePackages').click(function () { if (ValidatePackage()) { SavePackages(); } });
    HideLoader();
});



//Binding category

function BindCategory()
{
    $.ajax({
        cache: false,
        type: "GET",
        url:GetCategoryListUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: "",
    success: function (data) {
        $.each(data, function (i, item) {
            if (item.Active)
            {
                $("#ddlCategory").append($("<option></option>").val(item.CategoryName).html(item.CategoryName));
            }
        });
    },
    error: function () {
        alert("Failed! Please try again.");
    }
});

}

    function SavePackages()
    {
        var isActive = false;
        if ($('#chkActive').is(':checked')) {
            isActive = true
        }
        var jsonObject = {

            OfferName: $('#txtName').val(), Months: $('#txtMonths').val(), Amount: $('#txtAmount').val(),
            MinimumAmount: $('#txtMinamount').val(), Category: $('#ddlCategory option:selected').val(),
            Active: isActive, BranchId: $('#ddlBranch option:selected').val()
        }
        $.ajax({
            cache: false,
            type: "POST",
            url: AddPackageUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(jsonObject),
            success: function (data) {
                RedirectPackage();
            },
            failure: function (errMsg) {
                alert(errMsg.responseText);
            }
        });

    }

function RedirectPackage() {
    LoadPage('/Packages/GetPackages/', 'Justbok | Packages List');
    return false;
}

var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';

function ValidatePackage() {
    var IsValid = true;
    $('#txtName').parent().removeClass("has-error");
    $('#txtName').parent().find(".help-block-error").remove();
    $('#txtAmount').parent().removeClass("has-error");
    $('#txtAmount').parent().find(".help-block-error").remove();
    $('#ddlCategory').parent().removeClass("has-error");
    $('#ddlCategory').parent().find(".help-block-error").remove();

    if ($('#txtName').val() == "") {

        $('#txtName').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Company Name"));
        $('#txtName').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtName').parent().removeClass("has-error");
        $('#txtName').parent().find(".help-block-error").remove();
    }
    if ($('#txtAmount').val() == "") {

        $('#txtAmount').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Amount"));
        $('#txtAmount').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtAmount').parent().removeClass("has-error");
        $('#txtAmount').parent().find(".help-block-error").remove();
    }
    if ($('#txtMonths').val() == "") {

        $('#txtMonths').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Months"));
        $('#txtMonths').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtMonths').parent().removeClass("has-error");
        $('#txtMonths').parent().find(".help-block-error").remove();
    }


    if ($('#ddlCategory  option:selected').text() == "---Select---") {
        $('#ddlCategory').parent().append(errorSpan.replace(/{{Message}}/g, "Please select Category"));
        $('#ddlCategory').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#ddlCategory').parent().removeClass("has-error");
        $('#ddlCategory').parent().find(".help-block-error").remove();
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

function AllowOnlyNumbers(elementid) {
    $("#" + elementid).keydown(function (event) {
        if (event.shiftKey == true) {
            event.preventDefault();
        }
        if ((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105) || event.keyCode == 110
            || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 46 || event.keyCode == 190) {

        } else {
            event.preventDefault();
        }
        if ($(this).val().indexOf('.') !== -1 && (event.keyCode == 190 || event.keyCode == 110))
            event.preventDefault();

    });
}

AllowOnlyText("txtName");

AllowOnlyNumbers("txtMonths");
AllowOnlyNumbers("txtAmount");
AllowOnlyNumbers("txtMinamount");
