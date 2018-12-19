var pageNo = 1, pagerLoaded = false;
var url = "";
//var workoutPlanId;
$(document).ready(function () {
    ShowLoader();
    if ($("#tblWorkoutplan_length").val() != null && $("#tblWorkoutplan_length").val() != "") {
        GettingWorkoutPlan(pageNo, $("#tblWorkoutplan_length").val())
    }
    else {
        GettingWorkoutPlan(pageNo, 10)
    }

    $("#ddlBranch").change(function () {
        if ($("#tblWorkoutplan_length").val() != null && $("#tblWorkoutplan_length").val() != "") {
            GettingWorkoutPlan(pageNo, $("#tblWorkoutplan_length").val())
        }
        else {
            GettingWorkoutPlan(pageNo, 10)
        }
    });

    //$(document).ready(function () { }).on('click', '#btnYes', function () { ConfirmDeleteProduct(); });

});

function GettingWorkoutPlan(pageno, pagesize) {
   
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetWorkoutPlanListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblWorkoutPlanList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
                        + '<td style=display:none;>' + item.PlaneNameId + '</td>'
                        + '<td>' + item.PlanName + '</td>'
                        //+ '<td>' + item.Unit + '</td>'
                        + '<td>'
                        + '<a class="btn btn-success btn-xs btn-flat" onclick="return EditPlanName(' + item.PlaneNameId + ')" >Edit</a>&nbsp;'
                        //+ '<a class="btn btn-danger btn-flat btn-xs btnDelete" data-toggle="modal" data-target="#modal_Conformation" onclick="return DeletePlanName(this);">Delete</a>'
                        + '</td>'
                        + '</tr >';
                    $('#tblWorkoutPlanList tbody').append(rows);
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
                                if ($("#tblWorkoutplan_length").val() != null && $("#tblWorkoutplan_length").val() != "") {
                                    GettingWorkoutPlan(pageNo, $("#tblWorkoutplan_length").val())
                                }
                                else {
                                    GettingWorkoutPlan(pageNo, 10)
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
                $('#tblWorkoutPlanList tbody').append(norecords);
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

function EditPlanName(id) {
    LoadPage(EditWorkoutPlanUrl + "/" + id, 'Justbok | Edit Workout Plan');
    return false;
}

//function DeletePlanName(id) {

//    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
//    workoutId = $('#tblWorkoutList tr').eq(rowIndex + 1).find('td').eq(0).html();
//}

//function ConfirmDeleteProduct() {
//    $('#modal_Conformation').modal('hide');
//    $.ajax({
//        cache: false,
//        type: "GET",
//        url: DeleteWorkoutUrl,
//        dataType: "json",
//        contentType: "application/json; charset=utf-8",
//        data: { WorkoutId: workoutId },
//        success: function (data) {

//            if ($("#tblWorkout_length").val() != null && $("#tblWorkout_length").val() != "") {
//                GettingWorkouts(pageNo, $("#tblWorkout_length").val())
//            }
//            else {
//                GettingWorkouts(pageNo, 10)
//            }
//        },
//        failure: function (errMsg) {
//            alert(errMsg.responseText);
//        }
//    });
//}
function RedirectProduct() {
    LoadPage('/Workout/WorkoutList/', 'Justbok | Workout List');
    return false;
}