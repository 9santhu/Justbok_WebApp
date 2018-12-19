var pageNo = 1, pagerLoaded = false;
var url = "";
var MealTimeId;
$(document).ready(function () {
    ShowLoader();
    if ($("#tblMealList_length").val() != null && $("#tblMealList_length").val() != "") {
        GettingMealTime(pageNo, $("#tblMealList_length").val())
    }
    else {
        GettingMealTime(pageNo, 10)
    }

    $("#ddlBranch").change(function () {
        if ($("#tblMealList_length").val() != null && $("#tblMealList_length").val() != "") {
            GettingMealTime(pageNo, $("#tblMealList_length").val())
        }
        else {
            GettingMealTime(pageNo, 10)
        }

    });

    $(document).ready(function () { }).on('click', '#btnYes', function () { ConfirmDeleteMealTime(); });

});

function GettingMealTime(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetProductListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMealList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
                        + '<td style=display:none;>' + item.MealTimeId + '</td>'
                        + '<td>' + item.MealTime1 + '</td>'
                        + '<td>' + item.MealDescription + '</td>'
                        + '<td>'
                        + '<a class="btn btn-success btn-xs btn-flat" title="Edit" onclick="return EditMealTime(' + item.MealTimeId + ')" > <span class="glyphicon glyphicon-edit"></span>Edit</a>&nbsp;'
                        + '<a class="btn btn-danger btn-flat btn-xs btnDelete" data-toggle="modal" data-target="#modal_Conformation" onclick="return DelMealTime(this);"> <span class="glyphicon glyphicon-remove"></span> Delete</a>'
                        + '</td>'
                        + '</tr >';
                    $('#tblMealList tbody').append(rows);
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
                                if ($("#tblMealList_length").val() != null && $("#tblMealList_length").val() != "") {
                                    GettingMealTime(pageNo, $("#tblMealList_length").val())
                                }
                                else {
                                    GettingMealTime(pageNo, 10)
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
                $('#tblMealList tbody').append(norecords);
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

function EditMealTime(id) {
    LoadPage('/MealTime/EditMealtTime/' + id, 'Justbok | Edit Meal Time');
    return false;
}

function DelMealTime(id) {

    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    MealTimeId = $('#tblMealList tr').eq(rowIndex + 1).find('td').eq(0).html();
   
}

function ConfirmDeleteMealTime() {
    $('#modal_Conformation').modal('hide');
    $.ajax({
        cache: false,
        type: "GET",
        url: DeleteMealIDUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { MealTimeId: MealTimeId },
        success: function (data) {
            if ($("#tblMealList_length").val() != null && $("#tblMealList_length").val() != "") {
                GettingMealTime(pageNo, $("#tblMealList_length").val())
            }
            else {
                GettingMealTime(pageNo, 10)
            }
           
        },
        failure: function (errMsg) {
            //alert(errMsg.responseText);
        }
    });
}
function RedirectMealTime() {
    LoadPage('/MealTime/GetMealList/', 'Justbok | MealTime List');
    return false;
}