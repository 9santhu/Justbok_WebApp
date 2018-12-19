$(document).ready(function () {
    BindDietPlan();
    EditMemberDietPlan();
    $("#txtDietPlanName").prop("disabled", true);

    HideLoader();
});

function BindDietPlan() {
    $.ajax({
        url: GetDietPlanList,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $.each(data, function (i, item) {
                $("#txtDietPlanName").append($("<option></option>").val(item.PlaneNameId).html(item.PlanName));
            })
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function EditMemberDietPlan() {
    ShowLoader();
    var x = $('#myHiddenVar').val();
    var userid = x.toString();
    $.ajax({
        url: EditMemberDietPlanUrl,
        data: { Memberid: userid },
        type: "GET",
        dataType: "json",
        success: function (data) {
            $('#tblDietPlan tbody').empty();
            $.each(data, function (i, item) {

                if (i == 0) {
                    $('#txtDietPlanName').val(item.DietPlanId);
                }
                var rows = "<tr>"
                       + "<td  style='display:none;'> <input type='Text' class='form-control' id=txtMemberdietplan" + i + "></td>"
                         + "<td  style='display:none;'> <input type='Text' class='form-control' id=txtdietplanid" + i + "></td>"
                    + "<td> <input type='Text' class='form-control' readonly id=txtMealTime" + i + "></td>"
                     + "<td> <input type='Text' class='form-control' readonly id=txtMonday" + i + "></td>"
                      + "<td> <input type='Text' class='form-control' readonly  id=txtTuesday" + i + "></td>"
                       + "<td> <input type='Text' class='form-control' readonly id=txtWednesday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control' readonly id=txtThursday" + i + "></td>"
                         + "<td> <input type='Text' class='form-control' readonly id=txtFriday" + i + "></td>"
                          + "<td> <input type='Text' class='form-control' readonly id=txtSaturday" + i + "></td>"
                           + "<td> <input type='Text' class='form-control' readonly id=txtSunday" + i + "></td>"
                + "</tr>";

                $('#tblDietPlan tbody').append(rows);
                $('#txtMemberdietplan' + i + '').val(item.MemberDietPlanid);
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
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
        }
    });
}