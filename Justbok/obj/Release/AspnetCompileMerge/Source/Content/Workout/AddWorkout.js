$(document).ready(function () {

    $('#btnSaveWorkout').click(function () { SaveWorkout() });
    HideLoader();
});


function SaveWorkout() {
    var jsonObject = {

        WorkoutName: $('#txtWorkoutName').val(), Unit: $('#txtUnit option:selected').val(),
        BranchId: $('#ddlBranch option:selected').val()
    }

    $.ajax({
        cache: false,
        type: "POST",
        url: AddWorkoutUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            RedirectWorkout();
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });

}


function RedirectWorkout() {
    LoadPage('/Workout/WorkoutList/', 'Justbok | Workout List');
    return false;
}
