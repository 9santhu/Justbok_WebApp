$(document).ready(function () {
    BindMealTime();
    $('#btnSaveDiet').click(function () { SaveDiet(); });

    $("#ddlBranch").change(function () { BindMealTime(); });
    HideLoader();
});

function BindMealTime()
{
    $.ajax({
        url: BindMealTimeUrl,
        data: { BranchId: $('#ddlBranch option:selected').val() },
    type: "GET",
    dataType: "json",
    success: function (data) {

        $('#tblDietPlan tbody').empty();
        $.each(data, function (i, item) {
            var rows = "<tr>"
                + "<td> <input type='Text' class='form-control' readonly id=txtMealTime" + i + "></td>"
                 + "<td> <input type='Text' class='form-control'  id=txtMonday" + i + "></td>"
                  + "<td> <input type='Text' class='form-control'  id=txtTuesday" + i + "></td>"
                   + "<td> <input type='Text' class='form-control'  id=txtWednesday" + i + "></td>"
                    + "<td> <input type='Text' class='form-control'  id=txtThursday" + i + "></td>"
                     + "<td> <input type='Text' class='form-control'  id=txtFriday" + i + "></td>"
                      + "<td> <input type='Text' class='form-control'  id=txtSaturday" + i + "></td>"
                       + "<td> <input type='Text' class='form-control'  id=txtSunday" + i + "></td>"
            + "</tr>";
            $('#tblDietPlan tbody').append(rows);
            $('#txtMealTime' + i + '').val(item.MealTime1);
        })
    },
    error: function () {
        alert("Failed! Please try again.");
    }
});
}

function SaveDiet()
{
    ShowLoader();
    var rowCount = $('#tblDietPlan tr').length;
    var jsonObj = [];
    jsonObj.push({ "DietPlanName1": $('#txtDietPlanName').val(), "BranchId": $('#ddlBranch option:selected').val() });
    for (i = 0; i < rowCount-1; i++) {
        jsonObj.push(
           { "MealTime1": $('#txtMealTime' + i + '').val(), "MondayDiet": $('#txtMonday' + i + '').val(), "TuesdayDiet": $('#txtTuesday' + i + '').val(), "WednesdayDiet": $('#txtWednesday' + i + '').val(), "ThursdayDiet": $('#txtThursday' + i + '').val(), "FridayDiet": $('#txtFriday' + i + '').val(), "SaturdayDiet": $('#txtSaturday' + i + '').val(), "SundayDiet": $('#txtSunday' + i + '').val() }
           );
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: AddDietPlanUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObj),
    success: function (data) {
        HideLoader();
        RedirectDiet();
    },
    failure: function (errMsg) {
        HideLoader();
        alert(errMsg.responseText);
    }
});
}

function RedirectDiet() {
    LoadPage('/Diet/DietPlanList/', 'Justbok | Diet Plan List');
    return false;
}