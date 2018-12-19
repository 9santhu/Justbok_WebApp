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
    BindMealTime();

    HideLoader();
});

function SaveMealTime() {
    ShowLoader();
    var jsonObject = {
        MealTimeId: $('#txtMealTimeId').val(), MealTime1: $('#txtMealTime').val(), MealDescription: $('#txtMealDescription').val(), BranchId: $('#ddlBranch option:selected').val()
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: UpdateMealUrl,
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

function BindMealTime()
{
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: BindMealUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: "",
        success: function (data) {

            $.each(data, function (i, item) {
                $('#txtMealTimeId').val(item.MealTimeId);
                $('#txtMealTime').val(item.MealTime1);
                $('#txtMealDescription').val(item.MealDescription);
                
                HideLoader();
            });

        },
        error: function () {
            HideLoader();
            alert("Failed! Please try again.");

        }
    });

}

//function DeleteMealTime()
//{
//    ShowLoader();


//}


function RedirectMealTime() {
    LoadPage('/MealTime/MealList', 'Justbok | MealTime List');
    return false;
}