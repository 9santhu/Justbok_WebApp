$(document).ready(function () {
    $('input[type=datetime]').datepicker({
        dateFormat: "dd/m/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });

    $('#txtStartDate').datepicker('setDate', new Date());

    $('#btnSaveTask').click(function () { if (ValidateTask()) { SaveTask(); } });
    HideLoader();
});


function SaveTask() {
    var jsonObject = {
        Title: $('#txtTitle').val(), TaskDescription: $('#txtdescription').val(), Interval: $('#txtInterval').val(),
        IntervalType: $('#ddlIntervalType option:selected').val(), StartDate: $('#txtStartDate').val(), BranchId: $('#ddlBranch option:selected').val()
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: AddTaskUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            RedirectTask();
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });

}


function RedirectTask() {
    LoadPage('/Task/TaskList/', 'Justbok | Task List');
    return false;
}


var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';

function ValidateTask() {
    var IsValid = true;
    $('#txtTitle').parent().removeClass("has-error");
    $('#txtTitle').parent().find(".help-block-error").remove();
    $('#txtInterval').parent().removeClass("has-error");
    $('#txtInterval').parent().find(".help-block-error").remove();
    $('#ddlIntervalType').parent().removeClass("has-error");
    $('#ddlIntervalType').parent().find(".help-block-error").remove();

    if ($('#txtTitle').val() == "") {

        $('#txtTitle').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Title"));
        $('#txtTitle').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtTitle').parent().removeClass("has-error");
        $('#txtTitle').parent().find(".help-block-error").remove();
    }
    if ($('#txtInterval').val() == "") {

        $('#txtInterval').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Interval"));
        $('#txtInterval').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtInterval').parent().removeClass("has-error");
        $('#txtInterval').parent().find(".help-block-error").remove();
    }

    if ($('#ddlIntervalType  option:selected').text() == "--Select--") {
        $('#ddlIntervalType').parent().append(errorSpan.replace(/{{Message}}/g, "Please select IntervalType"));
        $('#ddlIntervalType').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#ddlIntervalType').parent().removeClass("has-error");
        $('#ddlIntervalType').parent().find(".help-block-error").remove();
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

AllowOnlyText("txtTitle");

AllowOnlyNumbers("txtInterval");
AllowOnlyNumbers("txtAmount");
AllowOnlyNumbers("txtMinamount");