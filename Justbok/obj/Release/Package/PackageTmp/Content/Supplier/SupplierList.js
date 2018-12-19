var pageNo = 1, pagerLoaded = false;
var url = "";
var supplierId;
$(document).ready(function () {
    ShowLoader();
    if ($("#tblSupplierList_length").val() != null && $("#tblSupplierList_length").val() != "") {
        GettingSupplier(pageNo, $("#tblSupplierList_length").val())
    }
    else {
        GettingSupplier(pageNo, 10)
    }

    $("#ddlBranch").change(function () {
        if ($("#tblSupplierList_length").val() != null && $("#tblSupplierList_length").val() != "") {
            GettingSupplier(pageNo, $("#tblSupplierList_length").val())
        }
        else {
            GettingSupplier(pageNo, 10)
        }
    });

    $(document).ready(function () { }).on('click', '#btnYes', function () { ConfirmDeleteProduct(); });

});

function GettingSupplier(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetSupplierListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblSupplierList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var forsale = "No";
                    if (item.ForSale == true) {
                        forsale = "Yes";
                    }
                    var rows = '<tr role="row" class="odd">'
                        + '<td style=display:none;>' + item.SupplierId + '</td>'
                         + '<td>' + item.CompanyName +'</td>'
                        + '<td>' + item.FirstName +" "+item.LastName +'</td>'
                        + '<td>' + item.RegistrationNumber + '</td>'
                         + '<td>'
                        + '<a class="btn btn-success btn-xs btn-flat" onclick="return EditSupplier(' + item.SupplierId + ')" >Edit</a>&nbsp;'
                        + '<a class="btn btn-danger btn-flat btn-xs btnDelete" data-toggle="modal" data-target="#modal_Conformation" onclick="return DeleteSupplier(this);">Delete</a>'
                        + '</td>'
                        + '</tr >';
                    $('#tblSupplierList tbody').append(rows);
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
                                if ($("#tblSupplierList_length").val() != null && $("#tblSupplierList_length").val() != "") {
                                    GettingSupplier(pageNo, $("#tblSupplierList_length").val())
                                }
                                else {
                                    GettingSupplier(pageNo, 10)
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
                $('#tblSupplierList tbody').append(norecords);
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

function EditSupplier(id) {
    LoadPage(EditSupplierUrl + "/" + id, 'Justbok | Edit Supplier');
    return false;
}

function DeleteSupplier(id) {

    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    supplierId = $('#tblSupplierList tr').eq(rowIndex + 1).find('td').eq(0).html();
}

function ConfirmDeleteProduct() {
    $('#modal_Conformation').modal('hide');
   
    $.ajax({
        cache: false,
        type: "GET",
        url: DeleteProductUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { SupplierId: supplierId },
        success: function (data) {
            if ($("#tblSupplierList_length").val() != null && $("#tblSupplierList_length").val() != "") {
                GettingSupplier(pageNo, $("#tblSupplierList_length").val())
                
            }
            else {
                GettingSupplier(pageNo, 10)
               
            }

           
        },
        failure: function (errMsg) {
            //HideLoader();
            alert(errMsg.responseText);
        }
    });
}

function RedirectSupplier() {
    LoadPage('/Supplier/GetSuppliers/', 'Justbok | Supplier List');
    return false;
}