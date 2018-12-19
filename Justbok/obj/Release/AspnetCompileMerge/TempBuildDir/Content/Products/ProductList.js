var pageNo = 1, pagerLoaded = false;
var url = "";
var ProductID;
$(document).ready(function () {
    ShowLoader();
    if ($("#tblProductList_length").val() != null && $("#tblProductList_length").val() != "") {
        GettingProducts(pageNo, $("#tblProductList_length").val())
    }
    else {
        GettingProducts(pageNo, 10)
    }

    $("#ddlBranch").change(function () {
        if ($("#tblProductList_length").val() != null && $("#tblProductList_length").val() != "") {
            GettingProducts(pageNo, $("#tblProductList_length").val())
        }
        else {
            GettingProducts(pageNo, 10)
        }
    });

    $(document).ready(function () { }).on('click', '#btnYes', function () { ConfirmDeleteProduct(); });

});

function GettingProducts(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetProductListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblProductList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var forsale="No";
                    if (item.ForSale == true)
                    {
                        forsale = "Yes";
                    }
                    var rows = '<tr role="row" class="odd">'
                        + '<td style=display:none;>' + item.ProductId + '</td>'
                        + '<td>' + item.BrandName + '</td>'
                        + '<td>' + item.ProductName + '</td>'
                        + '<td>' + item.Price + '</td>'
                        + '<td>' + item.LowStockQuantity + '</td>'
                         + '<td>' + item.Description + '</td>'
                          + '<td>' + forsale + '</td>'
                        + '<td>'
                        + '<a class="btn btn-success btn-xs btn-flat" onclick="return EditProduct(' + item.ProductId + ')" >Edit</a>&nbsp;'
                        + '<a class="btn btn-danger btn-flat btn-xs btnDelete" data-toggle="modal" data-target="#modal_Conformation" onclick="return DeleteProduct(this);">Delete</a>'
                        + '</td>'
                        + '</tr >';
                    $('#tblProductList tbody').append(rows);
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
                                if ($("#tblProductList_length").val() != null && $("#tblProductList_length").val() != "") {
                                    GettingProducts(pageNo, $("#tblProductList_length").val())
                                }
                                else {
                                    GettingProducts(pageNo, 10)
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
                $('#tblProductList tbody').append(norecords);
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
    LoadPage(EditProductUrl + "/" + id, 'Justbok | Edit Product');
    return false;
}

function DeleteProduct(id) {

    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
     ProductID = $('#tblProductList tr').eq(rowIndex + 1).find('td').eq(0).html();
}

function ConfirmDeleteProduct() {
    $('#modal_Conformation').modal('hide');
    $.ajax({
        cache: false,
        type: "GET",
        url: DeleteProductUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { Productid: ProductID },
        success: function (data) {

            if ($("#tblProductList_length").val() != null && $("#tblProductList_length").val() != "") {
                GettingProducts(pageNo, $("#tblProductList_length").val())
            }
            else {
                GettingProducts(pageNo, 10)
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