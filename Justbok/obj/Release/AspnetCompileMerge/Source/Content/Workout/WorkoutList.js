var pageNo = 1, pagerLoaded = false;
var url = "";
var workoutId;
$(document).ready(function () {
    ShowLoader();
    if ($("#tblWorkout_length").val() != null && $("#tblWorkout_length").val() != "") {
        GettingWorkouts(pageNo, $("#tblWorkout_length").val())
    }
    else {
        GettingWorkouts(pageNo, 10)
    }

    $("#ddlBranch").change(function () {
        if ($("#tblWorkout_length").val() != null && $("#tblWorkout_length").val() != "") {
            GettingWorkouts(pageNo, $("#tblWorkout_length").val())
        }
        else {
            GettingWorkouts(pageNo, 10)
        }
    });

    $(document).ready(function () { }).on('click', '#btnYes', function () { ConfirmDeleteWorkout(); });

});

function GettingWorkouts(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetWorkoutListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblWorkoutList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var forsale = "No";
                    if (item.ForSale == true) {
                        forsale = "Yes";
                    }
                    var rows = '<tr role="row" class="odd">'
                        + '<td style=display:none;>' + item.WorkoutId + '</td>'
                        + '<td>' + item.WorkoutName + '</td>'
                        + '<td>' + item.Unit + '</td>'
                        + '<td>'
                        + '<a class="btn btn-success btn-xs btn-flat" onclick="return EditWorkout(' + item.WorkoutId + ')" ><span class="glyphicon glyphicon-edit"></span>  Edit</a>&nbsp;'
                        + '<a class="btn btn-danger btn-flat btn-xs btnDelete" data-toggle="modal" data-target="#modal_Conformation" onclick="return DeleteWorkOutList(this);"><span class="glyphicon glyphicon-remove"></span> Delete</a>'
                        + '</td>'
                        + '</tr >';
                    $('#tblWorkoutList tbody').append(rows);
                    //" onclick = "return EditMember(' + item.ProductId + ')"
                });

                if (data.Pages < pageNo) {
                    pageNo = data.Pages;
                }
                if (!pagerLoaded) {
                    pagerLoaded = true;
                    $('#pagination').twbsPagination({
                        totalPages: data.Pages,
                        visiblePages: 7,
                        startPage: pageNo,
                        onPageClick: function (event, page) {
                            ShowLoader();
                            if (pageNo != page) {
                                pageNo = page;
                                if ($("#tblWorkout_length").val() != null && $("#tblWorkout_length").val() != "") {
                                    GettingWorkouts(pageNo, $("#tblWorkout_length").val())
                                }
                                else {
                                    GettingWorkouts(pageNo, 10)
                                }
                            }
                        }
                    });
                }
            }
            else {
                var norecords = "<tr>" +
                    '<td colspan="4">No data available</td>'
                    + "</tr>";
                $('#tblWorkoutList tbody').append(norecords);
            }

            HideLoader();
            setClass();
        },
        error: function (errMsg) {
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
        }
    });


}

function setClass() {
    $("th").removeClass();
    $("th").addClass("sorting");

    if (HeaderId != "") {
        $('#' + HeaderId).removeClass();
        var classname = (sortDirection == "ASC") ? "sorting_asc" : "sorting_desc";
        $('#' + HeaderId).addClass(classname);
    }
}

function EditWorkout(id) {
    LoadPage(EditWorkoutUrl + "/" + id, 'Justbok | Edit Workout');
    return false;
}

function DeleteWorkOutList(id) {

    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    workoutId = $('#tblWorkoutList tr').eq(rowIndex + 1).find('td').eq(0).html();
}

function ConfirmDeleteWorkout() {
    $('#modal_Conformation').modal('hide');
    $.ajax({
        cache: false,
        type: "GET",
        url: DeleteWorkoutUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { WorkoutId: workoutId },
        success: function (data) {

            if ($("#tblWorkout_length").val() != null && $("#tblWorkout_length").val() != "") {
                GettingWorkouts(pageNo, $("#tblWorkout_length").val())
            }
            else {
                GettingWorkouts(pageNo, 10)
            }
        },
        failure: function (errMsg) {
            //alert(errMsg.responseText);
        }
    });
}
function RedirectProduct() {
    LoadPage('/Workout/WorkoutList/', 'Justbok | Workout List');
    return false;
}