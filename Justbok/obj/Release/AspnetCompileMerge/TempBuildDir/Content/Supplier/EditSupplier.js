$(document).ready(function () {

    $('#btnSaveSupplier').click(function () { if (ValidateSupplier()) { SaveSupplier(); } });
    HideLoader();
});


function SaveSupplier() {
    ShowLoader();
    var jsonObject = {
        SupplierId: $('#txtSupplierid').val(), CompanyName: $('#txtCompanyName').val(), RegistrationNumber: $('#txtRegno').val(), FirstName: $('#txtFirstName').val(),
        LastName: $('#txtLastname').val(), Email: $('#txtEmail').val(), PhoneNumber: $('#txtPhnumber').val(),
        FaxNumber: $('#txtFaxnumber').val(), SupplierAddress: $('#txtAddress').val(), BranchId: $('#ddlBranch option:selected').val()
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: AddSupplierUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            HideLoader();
            RedirectSupplier();
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });

}


function RedirectSupplier() {
    LoadPage('/Supplier/GetSuppliers/', 'Justbok | Supplier List');
    return false;
}

var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';
function ValidateSupplier() {
    var IsValid = true;
    var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    $('#txtCompanyName').parent().removeClass("has-error");
    $('#txtCompanyName').parent().find(".help-block-error").remove();
    $('#txtRegno').parent().removeClass("has-error");
    $('#txtRegno').parent().find(".help-block-error").remove();
    $('#txtFirstName').parent().removeClass("has-error");
    $('#txtFirstName').parent().find(".help-block-error").remove();
    $('#txtLastname').parent().removeClass("has-error");
    $('#txtLastname').parent().find(".help-block-error").remove();

    $('#txtEmail').parent().removeClass("has-error");
    $('#txtEmail').parent().find(".help-block-error").remove();
    $('#txtPhnumber').parent().removeClass("has-error");
    $('#txtPhnumber').parent().find(".help-block-error").remove();

    if ($('#txtCompanyName').val() == "") {

        $('#txtCompanyName').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Company Name"));
        $('#txtCompanyName').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtCompanyName').parent().removeClass("has-error");
        $('#txtCompanyName').parent().find(".help-block-error").remove();
    }
    if ($('#txtRegno').val() == "") {

        $('#txtRegno').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Registration No"));
        $('#txtRegno').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtRegno').parent().removeClass("has-error");
        $('#txtRegno').parent().find(".help-block-error").remove();
    }

    if ($('#txtFirstName').val() == "") {

        $('#txtFirstName').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter FirstName"));
        $('#txtFirstName').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtFirstName').parent().removeClass("has-error");
        $('#txtFirstName').parent().find(".help-block-error").remove();
    }

    if ($('#txtLastname').val() == "") {

        $('#txtLastname').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter LastName"));
        $('#txtLastname').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtLastname').parent().removeClass("has-error");
        $('#txtLastname').parent().find(".help-block-error").remove();
    }
    if ($('#txtEmail').val() == "") {

        $('#txtEmail').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Email"));
        $('#txtEmail').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtEmail').parent().removeClass("has-error");
        $('#txtEmail').parent().find(".help-block-error").remove();
    }
    if ($('#txtPhnumber').val() == "") {

        $('#txtPhnumber').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter PhoneNumber"));
        $('#txtPhnumber').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtPhnumber').parent().removeClass("has-error");
        $('#txtPhnumber').parent().find(".help-block-error").remove();
    }
    if ($('#txtEmail').val()!="")
    {
        if (filter.test($('#txtEmail').val())) {

        }
        else {
            $('#txtEmail').parent().append(errorSpan.replace(/{{Message}}/g, "Not a valid format"));
            $('#txtEmail').parent().addClass("has-error");
            IsValid = false;
        }
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

AllowOnlyText("txtCompanyName");
AllowOnlyText("txtFirstName");
AllowOnlyText("txtLastname");

AllowOnlyNumbers("txtPhnumber");
AllowOnlyNumbers("txtFaxnumber");