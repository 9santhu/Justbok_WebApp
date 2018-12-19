var i = 1;

$(document).ready(function () {
    $('#btnSaveWorkout').click(function () { SaveWorkOut(); });
    $('#addWorkout').click(function () {
        var wrapper = $("#AddworkoutContainer");
        var availableAttributes = [];
        availableAttributes = GetAutoCompleteOptions();
       AddNewWorkout();
      
        $(wrapper).find('.txtwrkt').autocomplete({
            source: availableAttributes
        });
    });


    HideLoader();
});

function AddNewWorkout()
{
    var workout = '<div class="row">'
    + '<div class="form-group col-md-2"><label>Workout</label><div class="input-group"><input type="text" class="form-control txtwrkt" id="txtworkout' + i + '"/></div></div>'
    + '<div class="form-group col-md-2"><label>Sets</label><div class="input-group"><input type="Text" class="form-control" id="txtsets' + i + '"/><div class="input-group-btn"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Sets <span class="caret"></span></button><ul class="dropdown-menu pull-right"><li><a onclick="return Sets(this);">Sets</a></li><li><a onclick="return Minutes(this);">Minutes</a></li></ul></div></div></div>'
  + '<div class="form-group col-md-2"><label>Repeats</label><input type="Text" class="form-control" id="txtRepeats' + i + '"/></div>'
 + '<div class="form-group col-md-2"><label>Days</label><select  class="form-control" id="txtdays' + i + '"><option>Mon-Tue-Wed-Thu-Fri-Sat</option><option>Mon-Wed-Fri</option><option>Mon-Wed</option><option>Mon-Thu</option><option>Mon</option><option>Wed</option><option>Fri</option><option>Tue-Thu-Sat</option><option>Tue-Fri</option><option>Wed-Sat</option><option>Tue</option><option>Thu</option><option>Sat</option><option>Sun</option></select></div>'
   + '<div class="form-group col-md-2"><label>Exercise Order</label><input type="Text" class="form-control" id="txtExcercise' + i + '"/></div>'
    + '<div class="form-group col-md-2"><label>Delete</label><span class="form-control" style="border-style:none;"> <a class="text-danger btndeleteWorkout" onclick="return DeleteMemberWorkout(this);"><i class="fa fa-remove" aria-hidden="true"></i></a></span></div>'
  + '</div>'
    $("#AddworkoutContainer").append(workout);
    //$("#AddworkoutContainer").append('<div class="row">');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Workout</label><div class="input-group"><input type="text" class="form-control txtwrkt" id="txtworkout' + i + '"/></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Sets</label><div class="input-group"><input type="Text" class="form-control" id="txtsets' + i + '"/><div class="input-group-btn"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Sets <span class="caret"></span></button><ul class="dropdown-menu pull-right"><li><a href="#">Sets</a></li><li><a href="#">Minutes</a></li></ul></div></div></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Repeats</label><input type="Text" class="form-control" id="txtRepeats' + i + '"/></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Days</label><select  class="form-control" id="txtdays' + i + '"><option>Mon-Tue-Wed-Thu-Fri-Sat</option><option>Mon-Wed-Fri</option><option>Mon-Wed</option><option>Mon-Thu</option><option>Mon</option><option>Wed</option><option>Fri</option><option>Tue-Thu-Sat</option><option>Tue-Fri</option><option>Wed-Sat</option><option>Tue</option><option>Thu</option><option>Sat</option><option>Sun</option></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Exercise Order</label><input type="Text" class="form-control" id="txtExcercise' + i + '"/></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Delete</label><span class="form-control" style="border-style:none;"> <a class="text-danger btndeleteWorkout" onclick="return DeleteMemberWorkout(this);"><i class="fa fa-remove" aria-hidden="true"></i></a></span></div></div>');

    i++;
}

function SaveWorkOut()
{
    var count = $("#AddworkoutContainer select").length;
    var jsonObj = [];
    jsonObj.push({ "PlanName": $('#txtWorkoutPlanName').val(), "BranchId": $('#ddlBranch option:selected').val() });
    for (i = 1; i <= count; i++)
    {
        var setMinValue = $('#txtsets' + i + '').parent().find("button").text();
        jsonObj.push(
           { "Workout": $('#txtworkout' + i + '').val(), "NumberOfSets": $('#txtsets' + i + '').val(), "Repeats": $('#txtRepeats' + i + '').val(), "NumberofDays": $('#txtdays' + i + '').val(), "ExcerciseOrder": $('#txtExcercise' + i + '').val(), "SetMin": setMinValue }
           );
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: AddWorkoutPlanUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObj),
    success: function (data) {
        RedirectWorkout();
    },
    failure: function (errMsg) {
        alert(errMsg.responseText);
    }
});
}

function RedirectWorkout() {
    LoadPage('/WorkoutPlan/GetWorkoutPlans/', 'Justbok | Workout Plan List');
    return false;
}

function GetAutoCompleteOptions()
{
    var availableAttributes = [];
    $.ajax({
        cache: false,
        type: "POST",
        contentType: "application/json",
        url: AutoCompleteUrl,
        dataType: "json",
        data:"",
        success: function (data) {
          
            for (var i in data)
            availableAttributes.push(data[i].WorkoutName);
                     
        },
        error: function (result) {
            console.log(JSON.stringify(result));
        }
    });

    
    return availableAttributes;
}

function DeleteMemberWorkout(id) {

    var $curr = $(id).closest('div');
    $curr = $curr.parent();
    $curr.remove();
}

function Sets(id)
{
    //change label text
    var $currdiv = $(id).parent().parent().parent().parent().parent();
    $currdiv.find("label").text("Sets");
    //change button value
    $(id).closest("div").find("button").text("")
    $(id).closest("div").find("button").append("Sets" + " <span class='caret'></span>");
    //hide repeat textbox
    var $curr = $(id).closest('div');
    $curr = $curr.parent().parent();
    //$curr.next().show();
    $curr = $curr.next();
    $curr.find("input").show();
    $curr.find("label").show();
}

function Minutes(id)
{
    //alert($(".setMin").text());
    //change label text
    var $currdiv = $(id).parent().parent().parent().parent().parent();
    $currdiv.find("label").text("Minutes");
    //change button value
    $(id).closest("div").find("button").text("")
    $(id).closest("div").find("button").append("Minutes" + " <span class='caret'></span>");
    //hide repeat textbox
    var $curr = $(id).closest('div');
    $curr = $curr.parent().parent();
    $curr = $curr.next();
    $curr.find("input").hide();
    $curr.find("label").hide();
}