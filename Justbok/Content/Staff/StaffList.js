var pageNo = 1, pagerLoaded = false;
var url = "";
var staffid;

$(document).ready(function () {
    ShowLoader();
    if ($("#tblStaffList_length").val() != null && $("#tblStaffList_length").val() != "") {
       
        GettingStaff(pageNo, $("#tblStaffList_length").val())
    }
    else {
        GettingStaff(pageNo, 10)
    }

    $("#ddlBranch").change(function () {
        if ($("#tblStaffList_length").val() != null && $("#tblStaffList_length").val() != "") {
            GettingStaff(pageNo, $("#tblStaffList_length").val())
        }
        else {
            GettingStaff(pageNo, 10)
        }
    });

    $(document).ready(function () { }).on('click', '#btnYes', function () { ConfirmDeleteStaff(); });

    //$('#btnSearchMember').click(function () { SearchMember(); });

    //$('#btnSearchReset').click(function () { SearchReset(); });

});





function GettingStaff(pageno, pagesize) {
    $.ajax({
        cache: false,
        type: "GET",
        url: GetStaffUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblStaffList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.StaffId + '</td>'
                        + '<td>' + item.FirstName + " " + item.LastName + '</td>'
                        + '<td>' + item.Email + '</td>'
                        + '<td>' + item.PhoneNumber + '</td>'
                        + '<td>' + item.StaffRole + '</td>'
                        + '<td>'
                        + '<a class="btn btn-info EditMember" onclick="return EditStaff(' + item.StaffId + ')">Edit</a>&nbsp;'
                        + '<a class="btn btn-info" data-toggle="modal" data-target="#modal_Conformation" onclick="return DeleteProduct(this);">Delete</a>'
                        + '</td>'
                        + '</tr >';
                    $('#tblStaffList tbody').append(rows);
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
                                if ($("#tblStaffList_length").val() != null && $("#tblStaffList_length").val() != "") {
                                    GettingStaff(pageNo, $("#tblStaffList_length").val())
                                }
                                else {
                                    GettingStaff(pageNo, 10)
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
                $('#tblStaffList tbody').append(norecords);
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


function EditStaff(id) {
    LoadPage(EditStaffUrl + "/" + id, 'Justbok | Edit Staff');
    return false;
}


function DeleteProduct(id) {

    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    staffid = $('#tblStaffList tr').eq(rowIndex + 1).find('td').eq(0).html();
}

function ConfirmDeleteStaff() {
    $('#modal_Conformation').modal('hide');
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: DeleteStaffUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { staffid: staffid },
        success: function (data) {

            if ($("#tblStaffList_length").val() != null && $("#tblStaffList_length").val() != "") {
                GettingStaff(pageNo, $("#tblStaffList_length").val())
                HideLoader();
            }
            else {
                GettingStaff(pageNo, 10)
                HideLoader();
            }
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });
}