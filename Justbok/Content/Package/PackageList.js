var pageNo = 1, pagerLoaded = false;
var url = "";
var PackageId;
$(document).ready(function () {
    ShowLoader();
    if ($("#tblPackage_length").val() != null && $("#tblPackage_length").val() != "") {
        GettingPackages(pageNo, $("#tblPackage_length").val())
    }
    else {
        GettingPackages(pageNo, 10)
    }

    $("#ddlBranch").change(function () {
        if ($("#tblPackage_length").val() != null && $("#tblPackage_length").val() != "") {
            GettingPackages(pageNo, $("#tblPackage_length").val())
        }
        else {
            GettingPackages(pageNo, 10)
        }
    });

    $(document).ready(function () { }).on('click', '#btnYes', function () { ConfirmDeleteProduct(); });
    HideLoader();
});

function GettingPackages(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetPackagesListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblPackageList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var active = "No";
                    if (item.Active == true) {
                        active = "Yes";
                    }
                    var rows = '<tr role="row" class="odd">'
                        + '<td style=display:none;>' + item.MembershipOfferId + '</td>'
                        + '<td>' + item.OfferName + '</td>'
                        + '<td>' + item.Months + '</td>'
                        + '<td>' + item.Amount + '</td>'
                        + '<td>' + item.MinimumAmount + '</td>'
                         + '<td>' + item.Category + '</td>'
                          + '<td>' + active + '</td>'
                        + '<td>'
                        + '<a class="btn btn-success btn-xs btn-flat" onclick="return EditPackage(' + item.MembershipOfferId + ')" >Edit</a>&nbsp;'
                        + '<a class="btn btn-danger btn-flat btn-xs btnDelete" data-toggle="modal" data-target="#modal_Conformation" onclick="return DeletePackage(this);">Delete</a>'
                        + '</td>'
                        + '</tr >';
                    $('#tblPackageList tbody').append(rows);
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
                                if ($("#tblPackage_length").val() != null && $("#tblPackage_length").val() != "") {
                                    GettingPackages(pageNo, $("#tblPackage_length").val())
                                }
                                else {
                                    GettingPackages(pageNo, 10)
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
                $('#tblPackageList tbody').append(norecords);
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

function EditPackage(id) {
    LoadPage(EditPackageUrl + "/" + id, 'Justbok | Edit Product');
    return false;
}

function DeletePackage(id) {

    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    PackageId = $('#tblPackageList tr').eq(rowIndex + 1).find('td').eq(0).html();
}

function ConfirmDeleteProduct() {
    $('#modal_Conformation').modal('hide');
    $.ajax({
        cache: false,
        type: "GET",
        url: DeletePackageUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { PackageId: PackageId },
        success: function (data) {

            if ($("#tblPackage_length").val() != null && $("#tblPackage_length").val() != "") {
                GettingPackages(pageNo, $("#tblPackage_length").val())
            }
            else {
                GettingPackages(pageNo, 10)
            }
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}
function RedirectProduct() {
    LoadPage('/Product/GetProducts/', 'Justbok | Product List');
    return false;
}