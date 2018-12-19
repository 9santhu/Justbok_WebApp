﻿var pageNo = 1, pagerLoaded = false;
var url = "";
var CategoryId;
$(document).ready(function () {
    ShowLoader();
    if ($("#tblCategoryList_length").val() != null && $("#tblCategoryList_length").val() != "") {
        GettingCategory(pageNo, $("#tblCategoryList_length").val())
    }
    else {
        GettingCategory(pageNo, 10)
    }

    $("#ddlBranch").change(function () {
        if ($("#tblCategoryList_length").val() != null && $("#tblCategoryList_length").val() != "") {
            GettingCategory(pageNo, $("#tblCategoryList_length").val())
        }
        else {
            GettingCategory(pageNo, 10)
        }
    });

    $(document).ready(function () { }).on('click', '#btnYes', function () { ConfirmDeleteProduct(); });

});

function GettingCategory(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetCategoryListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblCategoryList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var Active = "No";
                    if (item.Active == true) {
                        Active = "Yes";
                    }
                    var rows = '<tr role="row" class="odd">'
                        + '<td style=display:none;>' + item.CategoryId + '</td>'
                        + '<td>' + item.CategoryName + '</td>'
                        + '<td>' + item.CategoryDescription + '</td>'
                          + '<td>' + Active + '</td>'
                        + '<td>'
                        + '<a class="btn btn-success btn-xs btn-flat" onclick="return EditCategoery(' + item.CategoryId + ')" >Edit</a>&nbsp;'
                        + '<a class="btn btn-danger btn-flat btn-xs btnDelete" data-toggle="modal" data-target="#modal_Conformation" onclick="return DeleteCategory(this);">Delete</a>'
                        + '</td>'
                        + '</tr >';
                    $('#tblCategoryList tbody').append(rows);
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
                                if ($("#tblCategoryList_length").val() != null && $("#tblCategoryList_length").val() != "") {
                                    GettingCategory(pageNo, $("#tblCategoryList_length").val())
                                }
                                else {
                                    GettingCategory(pageNo, 10)
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
                $('#tblCategoryList tbody').append(norecords);
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

function EditCategoery(id) {
    LoadPage(EditCategoryUrl + "/" + id, 'Justbok | Edit Category');
    return false;
}

function DeleteCategory(id) {

    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    CategoryId = $('#tblCategoryList tr').eq(rowIndex + 1).find('td').eq(0).html();
}

function ConfirmDeleteProduct() {
    $('#modal_Conformation').modal('hide');
    $.ajax({
        cache: false,
        type: "GET",
        url: DeleteCategoryUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { CategoryId: CategoryId },
        success: function (data) {

            if ($("#tblCategoryList_length").val() != null && $("#tblCategoryList_length").val() != "") {
                GettingCategory(pageNo, $("#tblCategoryList_length").val())
            }
            else {
                GettingCategory(pageNo, 10)
            }
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}
function RedirectCategory() {
    LoadPage('/Category/GetCategory/', 'Justbok | Category List');
    return false;
}