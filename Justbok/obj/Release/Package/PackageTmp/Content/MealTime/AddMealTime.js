$(document).ready(function () {
    $('input[type=datetime]').datepicker({
        dateFormat: "dd/m/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    //$('#txtMealTime').wickedpicker();
    $("#txtMealTime").datetimepicker({
        format: 'HH:mm',
        useCurrent: true,
        stepping: 15
    });
    $('#btnSaveMealTime').click(function () { SaveMealTime(); });

    HideLoader();
});

function SaveMealTime()
{
    ShowLoader();
    var jsonObject = {
        MealTime1: $('#txtMealTime').val(), MealDescription: $('#txtMealDescription').val(), BranchId: $('#ddlBranch option:selected').val()
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: AddMealTimeUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObject),
    success: function (data) {
        RedirectMealTime();
        HideLoader();
    },
    failure: function (errMsg) {
        HideLoader();
        alert(errMsg.responseText);
    }
});

}

function RedirectMealTime() {
    LoadPage('/MealTime/MealList', 'Justbok | MealTime List');
    return false;
}