var pageNo = 1, pagerLoaded = false;
var url = "";
var TaskId;
$(document).ready(function () {
    ShowLoader();
    if ($("#tblTaskList_length").val() != null && $("#tblTaskList_length").val() != "") {
        GettingTasks(pageNo, $("#tblTaskList_length").val())
    }
    else {
        GettingTasks(pageNo, 10)
    }

    $("#ddlBranch").change(function () {
        if ($("#tblTaskList_length").val() != null && $("#tblTaskList_length").val() != "") {
            GettingTasks(pageNo, $("#tblTaskList_length").val())
        }
        else {
            GettingTasks(pageNo, 10)
        }
    });

    $(document).ready(function () { }).on('click', '#btnYes', function () { ConfirmDeleteProduct(); });

});

function GettingTasks(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetTaskListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblTaskList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    
                    var rows = '<tr role="row" class="odd">'
                        + '<td style=display:none;>' + item.TaskId + '</td>'
                        + '<td>' + item.Title + '</td>'
                        + '<td>' + item.TaskDescription + '</td>'
                        + '<td>' + item.Interval + '</td>'
                        + '<td>' + item.IntervalType + '</td>'
                         + '<td>' + item.StartDate + '</td>'
                        + '<td>'
                        + '<a class="btn btn-success btn-xs btn-flat" onclick="return EditTask(' + item.TaskId + ')" >Edit</a>&nbsp;'
                        + '<a class="btn btn-danger btn-flat btn-xs btnDelete" data-toggle="modal" data-target="#modal_Conformation" onclick="return DeleteTask(this);">Delete</a>'
                        + '</td>'
                        + '</tr >';
                    $('#tblTaskList tbody').append(rows);
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
                                if ($("#tblTaskList_length").val() != null && $("#tblTaskList_length").val() != "") {
                                    GettingTasks(pageNo, $("#tblTaskList_length").val())
                                }
                                else {
                                    GettingTasks(pageNo, 10)
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
                $('#tblTaskList tbody').append(norecords);
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

function EditTask(id) {
    LoadPage(EditTaskUrl + "/" + id, 'Justbok | Edit Task');
    return false;
}

function DeleteTask(id) {

    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    TaskId = $('#tblTaskList tr').eq(rowIndex + 1).find('td').eq(0).html();
}

function ConfirmDeleteProduct() {
    $('#modal_Conformation').modal('hide');
    $.ajax({
        cache: false,
        type: "GET",
        url: DeleteTaskUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { TaskId: TaskId },
        success: function (data) {

            if ($("#tblTaskList_length").val() != null && $("#tblTaskList_length").val() != "") {
                GettingTasks(pageNo, $("#tblTaskList_length").val())
            }
            else {
                GettingTasks(pageNo, 10)
            }
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}
function RedirectProduct() {
    LoadPage('/Task/TaskList/', 'Justbok | Task List');
    return false;
}