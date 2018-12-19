var pageNo = 1, pagerLoaded = false;
var url = "";

$(document).ready(function () {
    ShowLoader();
    if ($("#tblStockList_length").val() != null && $("#tblStockList_length").val() != "") {
        GettingStockList(pageNo, $("#tblStockList_length").val())
    }
    else {
        GettingStockList(pageNo, 10)
    }
    $(document).ready(function () { }).on('click', '#addStock', function () { AddStock(); });
    $(document).ready(function () { }).on('click', '#removeStock', function () { RemoveStock(); });
    $(document).ready(function () { }).on('click', '#StockList', function () { StockList(); });
    $(document).on("click", '#UpdateStock', function () { UpdateStock(); });
    $(document).on("click", '#UpdateRemoveStock', function () { DeleteStock(); });
});
    function GettingStockList(pageno, pagesize) {
        ShowLoader();
        $.ajax({
            cache: false,
            type: "GET",
            url: StockListUrl,
            dataType: "json",
            data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
            success: function (data) {
                $('#tblStockList tbody').find("tr").remove();
                if (data != null && data.Pages > 0) {
                    $.each(data.Result, function (i, item) {
                        var rows = '<tr role="row" class="odd">'
                            + '<td style=display:none;>' + item.ProductId + '</td>'
                            + '<td>' + item.BrandName + '</td>'
                            + '<td>' + item.ProductName + '</td>'
                           + '<td>' + item.Quantity + '</td>'
                           + ' <td class="sorting_4" style="display:none"><input type="Text" class="form-control" /></td>'
                            + '</tr >';
                        $('#tblStockList tbody').append(rows);
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
                                    if ($("#tblStockList_length").val() != null && $("#tblStockList_length").val() != "") {
                                        GettingStockList(pageNo, $("#tblStockList_length").val())
                                    }
                                    else {
                                        GettingStockList(pageNo, 10)
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
                    $('#tblStockList tbody').append(norecords);
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

    function AddStock()
    {
        $('.sorting_4').show();
        $('#removeStock').hide();
        $('#addStock').hide();
        $('#StockList').show();

        $('#tblStockList tr:last').after('<tr><td></td><td></td><td></td><td> <button type="button" class="btn btn-primary pull-right fa fa-plus" aria-hidden="true" id="UpdateStock" > Add Stock</button></td></tr>');

    }

    function RemoveStock()
    {
        $('.sorting_4').show();
        $('#removeStock').hide();
        $('#addStock').hide();
        $('#StockList').show();
        var text = $('#tblStockList tr:last-child td:last-child').html();
        if (text.includes('button') == true) {
            $('#tblStockList tr:last').empty;
        }
        if (text.includes('UpdateRemoveStock') == false) {
            $('#tblStockList tr:last').after('<tr><td></td><td></td><td></td><td> <button type="button" class="btn btn-primary pull-right fa fa-plus" aria-hidden="true" id="UpdateRemoveStock"> Remove Stock</button></td></tr>');
        }

    }

    function StockList()
    {
        $('#StockList').hide();
        $('.sorting_4').hide();
        $('#removeStock').show();
        $('#addStock').show();
        var text = $('#tblStockList tr:last-child td:last-child').html();
        if (text.includes('button') == true) {

            $('#tblStockList tr:last').remove();
        }
    }

    function UpdateStock()
    {
        ShowLoader();
        var table = $("table tbody");
        var rows = [];
        var jsonObject;
        //rows.push(StockType="StockIn")
        table.find('tr').each(function (i) {
            var $tds = $(this).find('td'),
                productId = $tds.eq(0).text(),
                manufacturename = $tds.eq(1).text(),
                productname = $tds.eq(2).text(),
            currentstock = $tds.eq(3).text(),
           stockcount = $tds.eq(4).find('input[type="text"]').val();
            rows.push({ "ProductId": productId, "Manufacture": manufacturename, "ProductName": productname, "Quantity": currentstock, "StockIn": stockcount })
        });

        $.ajax({
            cache: false,
            type: "POST",
            url: UpdateStockUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(rows),
            success: function (data) {

                //location.reload();
                if ($("#tblStockList_length").val() != null && $("#tblStockList_length").val() != "") {
                    GettingStockList(pageNo, $("#tblStockList_length").val());
                    StockList();
                }
                else {
                    GettingStockList(pageNo, 10);
                    StockList();
                }

            },
            failure: function (errMsg) {
                alert(errMsg.responseText);
            }
        });
    }

    function DeleteStock()
    {
        ShowLoader();
        var table = $("table tbody");
        var rows = [];
        var jsonObject;
        //rows.push(StockType="StockIn")
        table.find('tr').each(function (i) {
            var $tds = $(this).find('td'),
                productId = $tds.eq(0).text(),
                manufacturename = $tds.eq(1).text(),
                productname = $tds.eq(2).text(),
            currentstock = $tds.eq(3).text(),
           stockcount = $tds.eq(4).find('input[type="text"]').val();
            rows.push({ "ProductId": productId, "Manufacture": manufacturename, "ProductName": productname, "Quantity": currentstock, "StockOut": stockcount })
        });

        $.ajax({
            cache: false,
            type: "POST",
            url:DeleteStockUrl,
            dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(rows),
        success: function (data) {
              
            //location.reload();
            if ($("#tblStockList_length").val() != null && $("#tblStockList_length").val() != "") {
                GettingStockList(pageNo, $("#tblStockList_length").val())
                StockList();
            }
            else {
                GettingStockList(pageNo, 10)
                StockList();
            }

        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });


   
    }


    function ShowChange() {
        ShowLoader();
        pageNo = 1;

        if ($('.pagination').data("twbs-pagination")) {
            $('.pagination').twbsPagination('destroy');
        }
        pagerLoaded = false;

        if ($("#tblStockList_length").val() != null && $("#tblStockList_length").val() != "") {
            GettingStockList(pageNo, $("#tblStockList_length").val())
        }
        else {
            GettingStockList(pageNo, 10)
        }

        return false;
    }
       

   