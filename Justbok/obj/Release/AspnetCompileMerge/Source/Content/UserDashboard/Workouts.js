$(document).ready(function () {
    BindPlan();
    EditMemberWorkoutPlan();
    $("#txtWorkoutPlanName").prop("disabled", true);
   

});


function BindPlan() {
    $.ajax({
        url: GetPlanList,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $.each(data, function (i, item) {
                $("#txtWorkoutPlanName").append($("<option></option>").val(item.PlaneNameId).html(item.PlanName));
            })
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}



function EditMemberWorkoutPlan() {
    ShowLoader();
    var x = $('#myHiddenVar').val();
    var userid = x.toString();
    $.ajax({
        url: EditMemberWorkoutPlanUrl,
        data: { Memberid: userid },
        type: "GET",
        dataType: "json",
        success: function (data) {
            $('#AddworkoutContainer').empty();

            $.each(data, function (i, item) {
                $('#txtWorkoutPlanName').val(item.PlaneNameId);
                $("#AddworkoutContainer").append('<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text"  class="form-control"  id="txtplanenameid' + i + '"></div>');
                $("#AddworkoutContainer").append('<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control"  id="txtMemberworkoutid' + i + '"></div>');
                $("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Workout</label><div class="input-group"><input type="Text" class="form-control" readonly id="txtworkout' + i + '"></div>');
                $("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Sets</label><div class="input-group"><input type="Text" class="form-control" readonly id="txtsets' + i + '"/><div class="input-group-btn"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Sets <span class="caret"></span></button><ul class="dropdown-menu pull-right"><li><a href="#">Sets</a></li><li><a href="#">Minutes</a></li></ul></div></div></div>');
                $("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Repeats</label><input type="Text" class="form-control" readonly id="txtRepeats' + i + '"/></div>');
                $("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Days</label><select  class="form-control days"  id="txtdays' + i + '"><option>Mon-Tue-Wed-Thu-Fri-Sat</option><option>Mon-Wed-Fri</option><option>Mon-Wed</option><option>Mon-Thu</option><option>Mon</option><option>Wed</option><option>Fri</option><option>Tue-Thu-Sat</option><option>Tue-Fri</option><option>Wed-Sat</option><option>Tue</option><option>Thu</option><option>Sat</option><option>Sun</option></div>');
                $("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Exercise Order</label><input type="Text" readonly class="form-control" id="txtExcercise' + i + '"/></div>');
                $("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Delete</label><span class="form-control" style="border-style:none;"> <a ><i class="fa fa-remove" aria-hidden="true"></i></a></span></div>');

                $('#txtplanenameid' + i + '').val(item.PlaneNameId);
                $('#txtMemberworkoutid' + i + '').val(item.MemberWorkoutPlanid);
                $('#txtworkout' + i + '').val(item.Workout);
                $('#txtsets' + i + '').val(item.NumberOfSets);
                $('#txtRepeats' + i + '').val(item.Repeats);
                $('#txtdays' + i + '').val(item.NumberofDays);
                $('#txtExcercise' + i + '').val(item.ExcerciseOrder)
                
            })
            $(".days").prop("disabled", true);
            HideLoader();
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });

}