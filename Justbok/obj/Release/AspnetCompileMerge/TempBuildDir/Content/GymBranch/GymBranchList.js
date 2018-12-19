$(document).ready(function () {
    ShowLoader();
    BindBranchList();
    $('#btSaveBranch').click(function () { if (ValidateBranch()) { SaveBranch(); } });
});


function BindBranchList() {
    $.ajax({
        url: BranchesListUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            paymentData = data;
            $('#tblBranches tbody').empty();
            $.each(data, function (i, item) {
                var rows = "<tr>"
                      + "<td style='display:none'>" + '<input type="hidden" name="branchId" value=' + item.BranchId + '>' + "</td>"
                        + "<td style='display:none'>" + '<input type="hidden" name="gymId" value=' + item.GymId + '>' + "</td>"
    + "<td>" + item.Branchcode + "</td>"
    + "<td>" + item.BranchName + "</td>"
    + "<td>" + item.PhoneNo + "</td>"
    + "<td>" + item.City + "</td>"
    + "<td>" + item.BranchState + "</td>"
    + "<td> <i class='fa fa-fw fa-edit btnEditBranch' onclick='return EditBranch(this)' data-toggle='modal' data-target='#modal-branch'></i></td>"
    + "</tr>";
                $('#tblBranches tbody').append(rows);
            });
            HideLoader();
        },
        error: function () {
            HideLoader();
            alert("Failed! Please try again.");
        }
    });

}

function EditBranch(id) {
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var branchid = $('#tblBranches tr').eq(rowIndex + 1).find('td').eq(0).find('input[type="hidden"]').val();
    $.ajax({
        cache: false,
        type: "GET",
        url: EditBranchUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { BranchId: branchid },
        success: function (data) {
            $.each(data, function (i, obj) {
                $('#txtBranchID').val(obj.BranchId);
                $('#txtGymId').val(obj.GymId);
                $('#txtBranchCode').val(obj.Branchcode);
                $('#txtBranchName ').val(obj.BranchName);
                $('#txtBranchAddress').val(obj.BranchAdress);
                $('#txtBranchPhoneNumber').val(obj.PhoneNo);
                $('#txtCity').val(obj.City);
                $('#txtState').val(obj.BranchState);
            });
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });
}

function SaveBranch() {
    var jsonObject = {
        Branchcode: $('#txtBranchCode').val(), BranchName: $('#txtBranchName').val(), BranchAdress: $('#txtBranchAddress').val(),
        PhoneNo: $('#txtBranchPhoneNumber').val(),
        City: $('#txtCity').val(), BranchState: $('#txtState').val(), GymId: $('#txtGymId').val(), BranchId: $('#txtBranchID').val()
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: AddGymBranchUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            $('#modal-branch').modal('hide');
            BindBranchList();
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';
function ValidateBranch() {
    var IsValid = true;
    $('#txtBranchCode').parent().removeClass("has-error");
    $('#txtBranchCode').parent().find(".help-block-error").remove();
    $('#txtBranchName').parent().removeClass("has-error");
    $('#txtBranchName').parent().find(".help-block-error").remove();
    $('#txtBranchPhoneNumber').parent().removeClass("has-error");
    $('#txtBranchPhoneNumber').parent().find(".help-block-error").remove();

    if ($('#txtBranchCode').val() == "") {

        $('#txtBranchCode').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Branch Code"));
        $('#txtBranchCode').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtBranchCode').parent().removeClass("has-error");
        $('#txtBranchCode').parent().find(".help-block-error").remove();
    }
    if ($('#txtBranchName').val() == "") {

        $('#txtBranchName').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Branch Name"));
        $('#txtBranchName').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtBranchName').parent().removeClass("has-error");
        $('#txtBranchName').parent().find(".help-block-error").remove();
    }

    if ($('#txtBranchPhoneNumber').val() == "") {

        $('#txtBranchPhoneNumber').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Phone Number"));
        $('#txtBranchPhoneNumber').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtBranchPhoneNumber').parent().removeClass("has-error");
        $('#txtBranchPhoneNumber').parent().find(".help-block-error").remove();
    }
    return IsValid;
}


function AllowOnlyText(elementid) {
    $('#' + elementid).keydown(function (e) {
        if (e.ctrlKey || e.altKey) {
            e.preventDefault();
        } else {
            var key = e.keyCode;
            if (!((key == 8) || (key == 9) || (key == 32) || (key == 46) || (key >= 35 && key <= 40) || (key >= 65 && key <= 90))) {
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
            || event.keyCode == 8  || event.keyCode == 9 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 46 || event.keyCode == 190) {

        } else {
            event.preventDefault();
        }
        if ($(this).val().indexOf('.') !== -1 && (event.keyCode == 190 || event.keyCode == 110))
            event.preventDefault();

    });
}

AllowOnlyText("txtBranchName"); AllowOnlyText("txtCity"); AllowOnlyText("txtState");
AllowOnlyNumbers("txtBranchPhoneNumber");