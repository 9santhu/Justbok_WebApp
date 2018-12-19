var pageNo = 1, pagerLoaded = false;
var url = "";
$(document).ready(function () {
    ShowLoader();
    if ($("#tblDietPlan_length").val() != null && $("#tblDietPlan_length").val() != "") {
        GettingDietList(pageNo, $("#tblDietPlan_length").val())
    }
    else {
        GettingDietList(pageNo, 10)
    }

    $("#ddlBranch").change(function () {
        if ($("#tblDietPlan_length").val() != null && $("#tblDietPlan_length").val() != "") {
            GettingDietList(pageNo, $("#tblDietPlan_length").val())
        }
        else {
            GettingDietList(pageNo, 10)
        }
    });

    $(document).ready(function () { }).on('click', '#btnYes', function () { ConfirmDeleteProduct(); });

});

function GettingDietList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetDietPlanListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblDietList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                  
                    var rows = '<tr role="row" class="odd">'
                        + '<td style=display:none;>' + item.DietPlanId + '</td>'
                        + '<td>' + item.DietPlanName1 + '</td>'
                        + '<td>'
                        + '<a class="btn btn-success btn-xs btn-flat" onclick="return EditProduct(' + item.DietPlanId + ')" >Edit</a>&nbsp;'
                        //+ '<a class="btn btn-danger btn-flat btn-xs btnDelete" data-toggle="modal" data-target="#modal_Conformation" onclick="return DeleteProduct(this);">Delete</a>'
                        + '</td>'
                        + '</tr >';
                    $('#tblDietList tbody').append(rows);
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
                                if ($("#tblDietPlan_length").val() != null && $("#tblDietPlan_length").val() != "") {
                                    GettingDietList(pageNo, $("#tblDietPlan_length").val())
                                }
                                else {
                                    GettingDietList(pageNo, 10)
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
                $('#tblDietList tbody').append(norecords);
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

function EditProduct(id) {
    LoadPage(EditDietUrl + "/" + id, 'Justbok | Edit Diet Plan');
    return false;
}