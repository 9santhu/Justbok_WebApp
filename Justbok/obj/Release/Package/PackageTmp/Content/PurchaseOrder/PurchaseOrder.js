var pagerLoaded = false, pageNo = 1;

var option = '<option value="{{value}}">{{text}}</option>';

var sortBy = "", sortDirection = "", HeaderId = "";

var _purchaseId = 0;

$(document).ready(function () {
    ShowLoader();
    $('input[type=datetime]').datepicker({
        dateFormat: "M dd, yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });

    $('input[type=datetime]').datepicker('setDate', new Date());

    LoadingOrders();

    $('#btnSearch').click(function () { SearchClick() });
    $('#btnClear').click(function () { ClearClick() });
});

function LoadingOrders() {
    ShowLoader();
    pageNo = 1;
    sortBy = "OrderDate";
    sortDirection = "DESC";
    HeaderId = "OrderDate";

    pagerLoaded = false

    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }

    if ($("#tblorders_length").val() != null && $("#tblorders_length").val() != "") {
        GettingPurchaseOrders(pageNo, $("#tblorders_length").val())
    }
    else {
        GettingPurchaseOrders(pageNo, 10)
    }
}

function ShowChange() {
    ShowLoader();
    pageNo = 1;

    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    pagerLoaded = false;

    if ($("#tblorders_length").val() != null && $("#tblorders_length").val() != "") {
        GettingPurchaseOrders(pageNo, $("#tblorders_length").val())
    }
    else {
        GettingPurchaseOrders(pageNo, 10)
    }
    return false;
}

function SortData(obj) {
    if (obj) {
        ShowLoader();
        var orderBy = obj.getAttribute('key');
        var header = obj.getAttribute('headerid')
        IsHeaderBindingRequired = true;
        if (sortBy.toUpperCase() == orderBy.toUpperCase()) {
            sortDirection = sortDirection.toUpperCase() == "ASC" ? "DESC" : "ASC";
        }
        else {
            sortBy = orderBy;
            sortDirection = "ASC";
        }

        if ($("#tblorders_length").val() != null && $("#tblorders_length").val() != "") {
            GettingPurchaseOrders(pageNo, $("#tblorders_length").val())
        }
        else {
            GettingPurchaseOrders(pageNo, 10)
        }
        HeaderId = header;
    }
}

function setClass() {
    $("th.Sortable").removeClass("sorting");
    $("th.Sortable").removeClass("sorting_asc");
    $("th.Sortable").removeClass("sorting_desc");
    $("th.Sortable").addClass("sorting");

    if (HeaderId != "") {
        $('#' + HeaderId).removeClass("sorting");
        var classname = (sortDirection == "ASC") ? "sorting_asc" : "sorting_desc";
        $('#' + HeaderId).addClass(classname);
    }
}

function pad(num) {
    num = "0" + num;
    return num.slice(-2);
}

function GettingPurchaseOrders(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetPurchaseOrdersUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, strSearch: $('#txtSearch').val(), FromDate: $('#txtDateFirst').val(), ToDate: $('#txtDateTo').val(), sortBy: sortBy, sortDirection: sortDirection, branchId: $("#ddlBranch").val() },
        success: function (data) {
            $('#tblorders tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var str = item.OrderDate.replace(/\D/g, "")

                    var FirstName = item.FirstName == null ? "" : item.FirstName;
                    var LastName = item.LastName == null ? "" : item.LastName;
                    var PhoneNo = item.PhoneNo == null ? "" : item.PhoneNo;
                    var GSTAmount = item.GSTAmount == null ? "" : parseFloat(item.GSTAmount).toFixed(2);
                    var TotalAmount = item.TotalAmount == null ? "" : parseFloat(item.TotalAmount).toFixed(2);
                    var PaymentVia = item.PaymentVia == null ? "" : item.PaymentVia;
                    var Representative = item.Representative == null ? "" : item.Representative;

                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.OrderNo + '</td>'
                        + '<td>' + pad(new Date(parseInt(str)).getDate()) + '/' + pad(new Date(parseInt(str)).getMonth() + 1) + '/' + new Date(parseInt(str)).getFullYear() + '</td>'
                        + '<td>' + FirstName + '</td>'
                        + '<td>' + LastName + '</td>'
                        + '<td>' + PhoneNo + '</td>'
                        + '<td>' + GSTAmount + '</td>'
                        + '<td>' + TotalAmount + '</td>'
                        + '<td>' + PaymentVia + '</td>'
                        + '<td>' + Representative + '</td>'
                        + '<td>'
                        + '<button class="btn btn-primary btn-xs btn-flat btnPrint" id="278" style="margin-right:10px;" title="Print" onclick="return PrintOrder(' + item.OrderNo + ')">'
                        + '<span class="glyphicon glyphicon-print"></span> Print'
                        + '</button>'
                        + '<button class="btn btn-success btn-xs btn-flat btnEdit" id="278" title="Edit" onclick="return ShowModel(' + item.OrderNo + ')">'
                        + '<span class="glyphicon glyphicon-edit"></span> Edit'
                        + '</button>'
                        + '</td>'
                        + '</tr >';
                    $('#tblorders tbody').append(rows);
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
                                if ($("#tblorders_length").val() != null && $("#tblorders_length").val() != "") {
                                    GettingPurchaseOrders(pageNo, $("#tblorders_length").val())
                                }
                                else {
                                    GettingPurchaseOrders(pageNo, 10)
                                }
                            }
                        }
                    });
                }
            }
            else {
                var norecords = "<tr>" +
                    '<td colspan="10" class="Nodata">No data available</td>'
                    + "</tr>";
                $('#tblorders tbody').append(norecords);
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

function SearchClick() {
    ShowLoader();
    pagerLoaded = false;
    pageNo = 1;
    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    if ($("#tblorders_length").val() != null && $("#tblorders_length").val() != "") {
        GettingPurchaseOrders(pageNo, $("#tblorders_length").val())
    }
    else {
        GettingPurchaseOrders(pageNo, 10)
    }
}

function ClearClick() {
    ShowLoader();
    $('input[type=datetime]').datepicker('setDate', new Date());
    $('#txtSearch').val("");

    pagerLoaded = false;
    pageNo = 1;
    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    if ($("#tblorders_length").val() != null && $("#tblorders_length").val() != "") {
        GettingPurchaseOrders(pageNo, $("#tblorders_length").val())
    }
    else {
        GettingPurchaseOrders(pageNo, 10)
    }
}

function ShowModel(purchaseId) {
    ShowLoader();
    _purchaseId = purchaseId;
    $.ajax({
        type: "GET",
        url: NewPurchaseOrderUrl,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#ModalBody').html(data);
            $('#purchaseorder').modal('show');
        },
        error: function () {
            HideLoader();
            alert("Content load failed.");
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });
    return false;
}

function HideModel(id) {
    $("#modal-membersearch").hide();
    $(id).modal('hide');
    $('.modal-backdrop').hide();
    return false;
}

//New Purchase Order Related
var subpagerLoaded = false, subpageNo = 1;
var productdata = null;
var options = '<option value={{productId}}>{{ProductName}}</option>';

var isStaffLoaded = false;

function PurchaseOrderLoading() {
    ShowLoader();
    $('input[type=datetime]').datepicker({
        dateFormat: "M dd, yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });

    $('input[type=datetime]').datepicker('setDate', new Date());
    AddProduct();
    GetRepresentative();

    $('#HFPurchaseId').val(_purchaseId);

    if (_purchaseId != 0) {
        LoadingPurchaseOrderInfo();
    }
}

function LoadingPurchaseOrderInfo() {
    if (isStaffLoaded && productdata != null) {
        gettingPurchaseOrderDetailsById();
    }
    else {
        setTimeout(LoadingPurchaseOrderInfo, 1000);
    }
}

function CustomerSearch(subpageNo) {
    $.ajax({
        cache: false,
        type: "GET",
        url: GetMemberDetailsUrl,
        dataType: "json",
        data: { searchCharacter: $('#txtSelectCustomers').val(), page: subpageNo, branchId: $("#ddlBranch").val() },
        success: function (data) {
            $('#tblSelectMembers tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = "<tr>" +
                        '<td><button type="button" class="btn btn-sm btn-primary btn-flat" id="btnSelect" onclick="SelectMember(this)">Select</button></td>'
                        + '<td class="MemberId">' + item.MemberID + '</td>'
                        + '<td><span class="FirstName">' + item.FirstName + '</span> <span class="LastName">' + item.LastName + '</td>'
                        + '<td class="MobileNumber">' + item.MobileNumber + '</td>'
                        + "</tr>";
                    $('#tblSelectMembers tbody').append(rows);
                });

                if (!subpagerLoaded) {
                    subpagerLoaded = true;
                    $('#pagination-demo').twbsPagination({
                        totalPages: data.Pages,
                        visiblePages: 7,
                        onPageClick: function (event, page) {
                            if (subpageNo != page) {
                                subpageNo = page;
                                CustomerSearch(page)
                            }
                        }
                    });
                }
            }
            else {
                var norecords = "<tr>" +
                    '<td colspan="4">No data available</td>'
                    + "</tr>";
                $('#tblSelectMembers tbody').append(norecords);
            }

            $('#divSearchtable').show();
            HideLoader();
        },
        error: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });

}

//Searching Members
function SelectCustSearch() {
    ShowLoader();
    subpagerLoaded = false;
    if ($('#pagination-demo').data("twbs-pagination")) {
        $('#pagination-demo').twbsPagination('destroy');
    }
    subpageNo = 1;
    CustomerSearch(subpageNo);
}

function SelectCustClear() {
    $('#divSearchtable').hide();
    $('#tblSelectMembers tbody').find("tr").remove();
    $('#txtSelectCustomers').val("");
    subpagerLoaded = false;
    if ($('#pagination-demo').data("twbs-pagination")) {
        $('#pagination-demo').twbsPagination('destroy');
    }
}

//On Select Button Click
function SelectMember(Member) {
    $("#txtMemberId").val($(Member).parent().parent().find(".MemberId").text());
    $("#txtFirstName").val($(Member).parent().parent().find(".FirstName").text());
    $("#txtLastName").val($(Member).parent().parent().find(".LastName").text());
    $("#txtMobileNo").val($(Member).parent().parent().find(".MobileNumber").text());
    ClosingPopup("#modal-membersearch");
}

//Opening Popup
function OpeningPopup(modalId) {
    $(modalId).modal("show");
    $(modalId).show();
}

//Closing Popup
function ClosingPopup(modalId) {
    $(modalId).hide();
}

function AddProduct() {
    ShowLoader();
    var productOptions = "";
    if (productdata != null) {
        $.each(productdata, function (i, item) {
            productOptions += options.replace(/{{productId}}/g, item.ProductId)
                .replace(/{{ProductName}}/g, item.ProductName)
        });
        AddingProduct(productOptions);
    }
    else {
        $.ajax({
            url: GetProductsUrl,
            data: { branchId: $("#ddlBranch").val() },
            type: "GET",
            dataType: "json",
            success: function (data) {
                $.each(data, function (i, item) {
                    productdata = data
                    productOptions += options.replace(/{{productId}}/g, item.ProductId)
                        .replace(/{{ProductName}}/g, item.ProductName)
                });
                AddingProduct(productOptions);
            },
            error: function (errMsg) {
                HideLoader();
                alert(errMsg.responseText);
            },
            failure: function (errMsg) {
                HideLoader();
                alert(errMsg.responseText);
            }
        });
        return false;
    }


}

function GetRepresentative() {
    $.ajax({
        url: GetStaffUrl,
        data: { branchId: $("#ddlBranch").val() },
        type: "GET",
        dataType: "json",
        success: function (data) {
            $.each(data, function (i, item) {
                var option = options.replace(/{{productId}}/g, item.StaffId)
                    .replace(/{{ProductName}}/g, item.FirstName + " " + item.LastName);
                $("#ddlRepresentative").append(option);
            });
            isStaffLoaded = true;
        },
        error: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });
    return false;
}

function AddingProduct(productOptions) {
    var rows = '<tr class="ProductRow">'
        + '<td class="col-md-2"><select class="form-control bindproduct" id="ddlProduct" style="width:180px;" onchange="return ProductChange(this);"><option>--Select--</option>' + productOptions + '</select></td>'
        + '<td class="col-md-2"><input type="text" id="txtQty" value="1" class="form-control quantity" style="width:50px;" onblur="return Calcualting(this);" /></td>'
        + '<td class="col-md-2"><input type="text" id="txtPrice" class="form-control price" style="width:75px;" onblur="return Calcualting(this);"/></td>'
        + '<td class="col-md-2">'
        + '<select class="form-control discounttype" id="ddlDiscount" style="width:100px;" onchange="return Calcualting(this);">'
        + '<option value="0">--Select--</option>'
        + '<option value="1">Fix Discount</option>'
        + '<option value="2">Discount Percent</option>'
        + '</select>'
        + '</td>'
        + '<td class="col-md-2"><input type="text" id="txtDiscount" class="form-control discount" style="width:100px;" onblur="return Calcualting(this);" /></td>'
        + '<td class="col-md-2">'
        + '<select class="form-control gst" id="ddlGST" style="width:100px;" onchange="return Calcualting(this);">'
        + '<option value="0">--Select--</option>'
        + '<option value="5">5%</option>'
        + '<option value="12">12%</option>'
        + '<option value="18">18%</option>'
        + '<option value="28">28%</option>'
        + '</select>'
        + '</td>'
        + '<td class="col-md-2"><input type="text" id="txtGST" class="form-control gstamount" style="width:75px;" disabled="" /></td>'
        + '<td class="col-md-2"><input type="text" id="txtTotal" class="form-control total" style="width:100px;" disabled="" /></td>'
        + '<td class="col-md-2"><span class="fa fa-remove ng-scope" style="width:20px;" onclick="return Remove(this);"></span></td>'
        + "</tr>";
    $('#tblProduct tbody').append(rows);

    var rowCount = $('#tblProduct tr').length;

    if (rowCount <= 2) {
        $('.delete').hide();
    }
    else {
        $('.delete').show();
    }
    HideLoader();
}

//Removing Row
function Remove(deleteId) {
    $(deleteId).parent().parent().remove();

    var rowCount = $('#tblProduct tr').length;

    if (rowCount <= 2) {
        $('.delete').hide();
    }

    calculatingTotals();
}

function ProductChange(Product) {
    ShowLoader();
    var productId = $(Product).val();
    if (productId != "0") {
        $.ajax({
            cache: false,
            type: "GET",
            url: GetProductDetailsUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: { PrdouctId: productId },
            success: function (data) {
                $(Product).parent().parent().find(".price").val(data[0].Price);
                Calcualting(Product);
                HideLoader();
            },
            error: function (errMsg) {
                HideLoader();
                alert(errMsg.responseText);
            },
            failure: function (errMsg) {
                HideLoader();
                alert(errMsg.responseText);

            }
        });
    }
    else {
        $(Product).parent().parent().find(".price").val(0);
        Calcualting(Product);
        HideLoader();
        HideLoader();
    }

    return false;
}

function Calcualting(Id) {
    ShowLoader();
    var quantity = $(Id).parent().parent().find(".quantity").val();
    var price = $(Id).parent().parent().find(".price").val();
    var discountType = $(Id).parent().parent().find(".discounttype").val();
    var dicount = $(Id).parent().parent().find(".discount").val();
    var gst = $(Id).parent().parent().find(".gst").val();
    var total = 0, gstamount = 0;
    if (quantity != "" && price != "") {
        total = parseFloat(quantity) * parseFloat(price);
    }

    if (total != 0 && discountType != "0" && dicount != "") {
        if (discountType == "1") {
            if (dicount != "" && total != "") {
                total = parseFloat(total) - parseFloat(dicount);
            }
        }
        else {
            if (dicount != "" && total != "") {
                dicount = parseFloat(total) * (parseFloat(dicount) / 100);
                total = parseFloat(total) - parseFloat(dicount);
            }
        }
    }

    if (gst != "0") {
        gstamount = parseFloat(total) * (parseFloat(gst) / 100);
    }
    $(Id).parent().parent().find(".total").val((parseFloat(total) - parseFloat(gstamount)).toFixed(2));
    $(Id).parent().parent().find(".gstamount").val(parseFloat(gstamount).toFixed(2));

    calculatingTotals();

    HideLoader();

    return false;
}

function calculatingTotals() {
    var TotalAmount = 0;
    var GstAmount = 0;
    $("#tblProduct .total").each(function () {
        TotalAmount += parseFloat($(this).val());
    });

    $("#tblProduct .gstamount").each(function () {
        GstAmount += parseFloat($(this).val());
    });

    $("#lblSubtotal").text(parseFloat(TotalAmount).toFixed(2));
    $("#lbltotalgst").text(parseFloat(GstAmount).toFixed(2));
    $("#lbltotal").text(parseFloat(parseFloat(TotalAmount) + parseFloat(GstAmount)).toFixed(2));
}

function POSSave() {
    ShowLoader();
    var PurchaseOrderDetails = [];

    if ($('#HFPurchaseId').val() != 0) {
        $('#tblProduct tr.ProductRow').each(function () {
            PurchaseOrderDetails.push({
                "PurchaseId": $('#HFPurchaseId').val(),
                "ProductId": $(this).find(".bindproduct").val(),
                "Qty": $(this).find(".quantity").val(),
                "Rate": $(this).find(".price").val(),
                "DiscountType": $(this).find(".discounttype").val(),
                "Discount": $(this).find(".discount").val(),
                "GST": $(this).find(".gst").val()
            });
        });
    }
    else {
        $('#tblProduct tr.ProductRow').each(function () {
            PurchaseOrderDetails.push({
                "ProductId": $(this).find(".bindproduct").val(),
                "Qty": $(this).find(".quantity").val(),
                "Rate": $(this).find(".price").val(),
                "DiscountType": $(this).find(".discounttype").val(),
                "Discount": $(this).find(".discount").val(),
                "GST": $(this).find(".gst").val()
            });
        });
    }

    var PurchaseOrderHeader = {
        "MemberId": $('#txtMemberId').val(), "FirstName": $('#txtFirstName').val(), "LastName": $('#txtLastName').val(),
        "MobileNo": $('#txtMobileNo').val(), "OrderDate": $('#txtOrderDate').val(), "PaymentVia": $("#ddlPaymentType").val(),
        "Representative": $("#ddlRepresentative option:selected").text(), "PurchaseOrderDetails": PurchaseOrderDetails,
        "SubTotal": $("#lblSubtotal").text(), "GSTAmount": $("#lbltotalgst").text(), "TotalAmount": $("#lbltotal").text(),
        "BranchId": $("#ddlBranch").val(), "StaffId": $("#ddlRepresentative").val(), PurchaseId: $('#HFPurchaseId').val()
    };


    $.ajax({
        cache: false,
        type: "POST",
        url: PurchaseOrdersUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ objPurchaseOrderHeader: PurchaseOrderHeader }),
        success: function (data) {
            if (data.Success == "True") {
                toastr.success("Purchase Order Saved Successfully.");
                HideModel("#purchaseorder");
                LoadingOrders();
            }
            else {
                alert(data.Message);
            }
            HideLoader();
        },
        error: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        },
        failure: function (errMsg) {
            HideLoader();
            toastr.error(errMsg.responseText);
        }
    });

}

function gettingPurchaseOrderDetailsById() {
    ShowLoader();
    $.ajax({
        url: GetOrderByIdUrl,
        data: { id: _purchaseId },
        type: "GET",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $('#HFPurchaseId').val(data.PurchaseId);
                $("#txtMemberId").val(data.MemberId);
                $("#txtFirstName").val(data.FirstName);
                $("#txtLastName").val(data.LastName);
                $("#txtMobileNo").val(data.MobileNo);
                $('#txtOrderDate').datepicker('setDate', new Date(data.OrderDate.replace(/-/g, '/')));
                $("#ddlPaymentType").val(data.PaymentVia);
                $("#ddlRepresentative").val(data.StaffId);


                var orderItems = "";
                $('#tblProduct tbody').find('tr').remove();


                var GST = ["5", "12", "18", "28"];

                $.each(data.PurchaseOrderDetails, function (i, item) {
                    var productOptions = "";
                    var ddlDiscount = "";
                    var ddlGST = "";

                    $.each(productdata, function (i, item1) {
                        if (item1.ProductId == item.ProductId) {
                            productOptions += '<option value="' + item1.ProductId + '" selected>' + item1.ProductName + '</option>';
                        }
                        else {
                            productOptions += '<option value="' + item1.ProductId + '">' + item1.ProductName + '</option>';
                        }
                    });

                    $.each(GST, function (i, item1) {
                        if (item1 == item.GST) {
                            ddlGST += '<option value="' + item1 + '" selected>' + item1 + '%' + '</option>';
                        }
                        else {
                            ddlGST += '<option value="' + item1 + '">' + item1 + '%' + '</option>';
                        }
                    });

                    if (item.DiscountType == 1) {
                        ddlDiscount = '<option value="1" selected>Fix Discount</option>' + '<option value="2">Discount Percent</option>'
                    }
                    else if (item.DiscountType == 2) {
                        ddlDiscount = '<option value="1">Fix Discount</option>' + '<option value="2" selected>Discount Percent</option>'
                    }
                    else {
                        ddlDiscount = '<option value="1">Fix Discount</option>' + '<option value="2">Discount Percent</option>'
                    }
                    var total = (parseFloat(item.NetAmount) - parseFloat(item.GstAmount)).toFixed(2)

                    var rows = '<tr class="ProductRow">'
                        + '<td class="col-md-2"><select class="form-control bindproduct" id="ddlProduct" style="width:180px;" onchange="return ProductChange(this);"><option>--Select--</option>' + productOptions + '</select></td>'
                        + '<td class="col-md-2"><input type="text" id="txtQty" value="' + item.Qty + '" class="form-control quantity" style="width:50px;" onblur="return Calcualting(this);" /></td>'
                        + '<td class="col-md-2"><input type="text" id="txtPrice" class="form-control price" value="' + item.Rate + '" style="width:75px;" onblur="return Calcualting(this);"/></td>'
                        + '<td class="col-md-2">'
                        + '<select class="form-control discounttype" id="ddlDiscount" style="width:100px;" onchange="return Calcualting(this);">'
                        + '<option value="0">--Select--</option>' + ddlDiscount + '</select>'
                        + '</td>'
                        + '<td class="col-md-2"><input type="text" id="txtDiscount" value="' + item.Discount + '" class="form-control discount" style="width:100px;" onblur="return Calcualting(this);" /></td>'
                        + '<td class="col-md-2">'
                        + '<select class="form-control gst" id="ddlGST" style="width:100px;" onchange="return Calcualting(this);">'
                        + '<option value="0">--Select--</option>' + ddlGST + '</select>'
                        + '</td>'
                        + '<td class="col-md-2"><input type="text" id="txtGST" value="' + parseFloat(item.GstAmount).toFixed(2) + '"  class="form-control gstamount" style="width:75px;" disabled="" /></td>'
                        + '<td class="col-md-2"><input type="text" id="txtTotal" value="' +total  + '"  class="form-control total" style="width:100px;" disabled="" /></td>'
                        + '<td class="col-md-2"><span class="fa fa-remove ng-scope" style="width:20px;" onclick="return Remove(this);"></span></td>'
                        + "</tr>";
                    $('#tblProduct tbody').append(rows);
                });

                var rowCount = $('#tblProduct tr').length;

                if (rowCount <= 2) {
                    $('.delete').hide();
                }
                else {
                    $('.delete').show();
                }

                calculatingTotals();
            }
            HideLoader();
        },
        error: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });
}

function PrintOrderReceipt() {
    var printelem = document.getElementById('divOrderReceipt');
    var myWindow = window.open('', '', 'width=750,height=500', '_blank');
    myWindow.document.write(document.head.innerHTML);
    myWindow.document.write(printelem.innerHTML);
    myWindow.document.write("\x3Cscript>window.print(); window.close();\x3C/script>");
}

var row = '<tr class="ng-scope" style="">'
    + '<td class="col-md-2 text-center">'
    + '<div class="form-group ng-binding">'
    + '{{ProductCode}}'
    + '</div>'
    + '</td>'
    + '<td class="col-md-2 text-center">'
    + '<div class="form-group ng-binding">'
    + '{{Quantity}}'
    + '</div>'
    + '</td>'
    + '<td class="col-md-2">'
    + '<div class="form-group text-right ng-binding">'
    + '{{Price}}'
    + '</div>'
    + '</td>'
    + '<td class="col-md-3 text-right">'
    + '<div class="form-group ng-binding">'
    + '{{DiscountPrice}}'
    + '</div>'
    + '</td>'
    + '<td class="col-md-2 text-center">'
    + '<div class="form-group ng-binding">'
    + '{{GST}} %'
    + '</div>'
    + '</td>'
    + '<td class="col-md-2 text-right">'
    + '<div class="form-group ng-binding">'
    + '{{GSTAmount}}'
    + '</div>'
    + '</td>'
    + '<td class="col-md-2">'
    + '<div class="form-group ng-binding">'
    + '{{Total}}'
    + '</div>'
    + '</td>'
    + '</tr >';

function PrintOrder(purchaseId) {
    ShowLoader();
    $.ajax({
        url: GetOrderByIdUrl,
        data: { id: purchaseId },
        type: "GET",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $('#spnOrderNo').text(data.PurchaseId);
                $('#spnOrderDate').text(moment(new Date(parseInt(data.OrderDate.replace(/\D/g, "")))).format("DD/MM/YYYY"));
                $('#spnName').text(data.FirstName + " " + data.LastName);
                $('#spnSubTotal').text(parseFloat(data.SubTotal).toFixed(2));
                $('#spnGST').text(parseFloat(data.GSTAmount).toFixed(2));
                $('#spnTotal').text(parseFloat(data.TotalAmount).toFixed(2));

                var orderItems = "";
                $('#OrderItem tbody').find('tr').remove();
                $.each(data.PurchaseOrderDetails, function (i, item) {
                    orderItems = row.replace(/{{ProductCode}}/g, item.ProductName)
                        .replace(/{{Quantity}}/g, item.Qty)
                        .replace(/{{Price}}/g, parseFloat(item.Rate).toFixed(2))
                        .replace(/{{DiscountPrice}}/g, parseFloat((parseFloat(item.NetAmount) / parseFloat(item.Qty))).toFixed(2))
                        .replace(/{{GST}}/g, item.GST)
                        .replace(/{{GSTAmount}}/g, parseFloat(item.GstAmount).toFixed(2))
                        .replace(/{{Total}}/g, parseFloat(item.NetAmount).toFixed(2))
                    $('#OrderItem tbody').append(orderItems);
                });

                $("#modal-print").modal("show");

            }
            HideLoader();
        },
        error: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });

}