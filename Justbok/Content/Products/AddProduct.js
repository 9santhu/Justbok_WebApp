$(document).ready(function () {
   
    $('#btnSaveProduct').click(function () { if (ValidateProduct()) { SaveProduct(); } });
    HideLoader();
});


function SaveProduct()
{
    var forsale = false;
    if ($('#chkForSale').is(':checked')) {
        forsale=true
    }
    //alert($('#chkActive').val());

    var jsonObject = {

        BrandName: $('#txtBrandName').val(), ProductName: $('#txtProductname').val(), Price: $('#txtPrice').val(),
        Quantity: $('#txtQuantity').val(), Description: $('#txtDescription').val(), LowStockQuantity: $('#txtLowStockQuantity').val(),
        ForSale: forsale, BranchId: $('#ddlBranch option:selected').val()
    }

    $.ajax({
        cache: false,
        type: "POST",
        url: AddProductUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObject),
    success: function (data) {
        RedirectProduct();
    },
    failure: function (errMsg) {
        alert(errMsg.responseText);
    }
});

}


function RedirectProduct() {
    LoadPage('/Product/GetProducts/', 'Justbok | Product List');
    return false;
}


function redirecttoedit() {
    window.location.href = '/Product/GetProducts/';
}
var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';
function ValidateProduct()
{
    var IsValid = true;
    $('#txtProductname').parent().removeClass("has-error");
    $('#txtProductname').parent().find(".help-block-error").remove();
    $('#txtPrice').parent().removeClass("has-error");
    $('#txtPrice').parent().find(".help-block-error").remove();
    $('#txtQuantity').parent().removeClass("has-error");
    $('#txtQuantity').parent().find(".help-block-error").remove();
    $('#txtLowStockQuantity').parent().removeClass("has-error");
    $('#txtLowStockQuantity').parent().find(".help-block-error").remove();

    if ($('#txtProductname').val() == "") {

        $('#txtProductname').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Product Name"));
        $('#txtProductname').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtProductname').parent().removeClass("has-error");
        $('#txtProductname').parent().find(".help-block-error").remove();
    }
    if ($('#txtPrice').val() == "") {

        $('#txtPrice').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Price"));
        $('#txtPrice').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtPrice').parent().removeClass("has-error");
        $('#txtPrice').parent().find(".help-block-error").remove();
    }

    if ($('#txtQuantity').val() == "") {

        $('#txtQuantity').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Quantity"));
        $('#txtQuantity').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtQuantity').parent().removeClass("has-error");
        $('#txtQuantity').parent().find(".help-block-error").remove();
    }

    if ($('#txtLowStockQuantity').val() == "") {

        $('#txtLowStockQuantity').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Low Stock Quantity"));
        $('#txtLowStockQuantity').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtLowStockQuantity').parent().removeClass("has-error");
        $('#txtLowStockQuantity').parent().find(".help-block-error").remove();
    }
    return IsValid;
}

function AllowOnlyText(elementid) {
    $('#' + elementid).keydown(function (e) {
        if (e.shiftKey || e.ctrlKey || e.altKey) {
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

AllowOnlyText("txtProductname");
AllowOnlyText("txtBrandName");

AllowOnlyNumbers("txtPrice");
AllowOnlyNumbers("txtQuantity");
AllowOnlyNumbers("txtLowStockQuantity");
