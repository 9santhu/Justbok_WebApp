$(document).ready(function () {
    BindDiet();
    $('#btnSaveDiet').click(function () { SaveDiet(); });
    HideLoader();
});

function BindDiet()
{
    $.ajax({
        url: BindEditDietUrl,
        data: "",
    type: "GET",
    dataType: "json",
    success: function (data) {
        $('#tblDietPlan tbody').empty();
        $.each(data, function (i, item) {
            var rows = "<tr>"
                   + "<td  style='display:none;'> <input type='Text' class='form-control' id=txtdietid" + i + "></td>"
                     + "<td  style='display:none;'> <input type='Text' class='form-control' id=txtdietplanid" + i + "></td>"
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
            $('#txtdietid' + i + '').val(item.DietId);
            $('#txtdietplanid' + i + '').val(item.DietPlanId);
            $('#txtMealTime' + i + '').val(item.DietTime);
            $('#txtMonday' + i + '').val(item.MondayDiet);
            $('#txtTuesday' + i + '').val(item.TuesdayDiet);
            $('#txtWednesday' + i + '').val(item.WednesdayDiet);
            $('#txtThursday' + i + '').val(item.ThursdayDiet);
            $('#txtFriday' + i + '').val(item.FridayDiet);
            $('#txtSaturday' + i + '').val(item.SaturdayDiet);
            $('#txtSunday' + i + '').val(item.SundayDiet);
        })
    },
    failure: function (errMsg) {
        alert(errMsg.responseText);
    }
});
}

function SaveDiet()
{
    ShowLoader();
    var rowCount = $('#tblDietPlan tr').length;
    var jsonObj = [];
    jsonObj.push({ "DietPlanName1": $('#txtDietPlanName').val(), "BranchId": $('#ddlBranch option:selected').val() });
    for (i = 0; i < rowCount - 1; i++) {
        jsonObj.push(
           { "DietId": $('#txtdietid' + i + '').val(), "DietPlanId": $('#txtdietplanid' + i + '').val(), "MealTime1": $('#txtMealTime' + i + '').val(), "MondayDiet": $('#txtMonday' + i + '').val(), "TuesdayDiet": $('#txtTuesday' + i + '').val(), "WednesdayDiet": $('#txtWednesday' + i + '').val(), "ThursdayDiet": $('#txtThursday' + i + '').val(), "FridayDiet": $('#txtFriday' + i + '').val(), "SaturdayDiet": $('#txtSaturday' + i + '').val(), "SundayDiet": $('#txtSunday' + i + '').val() }
           );
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: EditDietPlanUrl,
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