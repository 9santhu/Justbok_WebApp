var pageNo = 1, pagerLoaded = false;
$(document).ready(function () {

    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    if ($("#tblMembershipWiseSale_length").val() != null && $("#tblMembershipWiseSale_length").val() != "") {
        GetMembershipWiseSale(pageNo, $("#tblMembershipWiseSale_length").val())
    }
    else {
        GetMembershipWiseSale(pageNo, 10)
    }
  

    $('#btnSearch').click(function () {
        if ($("#tblMembershipWiseSale_length").val() != null && $("#tblMembershipWiseSale_length").val() != "") {
            SearchMembershipWiseSale(pageNo, $("#tblMembershipWiseSale_length").val())
        }
        else {
            SearchMembershipWiseSale(pageNo, 10)
        }
    });

    $('#dwnldPdf').click(function () { PDFMembershipList(); });
    $('#dwnldExcel').click(function () { ExcelMembershipList(); });

});

function GetMembershipWiseSale(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetMembershipwiseListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMembershipWiseSale tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {

                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
                         + '<td>' + item.Package + '</td>'
                           + '<td>' + item.ToatlSale + '</td>'
                            + '<td>' + item.Amount + '</td>'
                  
                        + '</tr >';
                    $('#tblMembershipWiseSale tbody').append(rows);
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
                                if ($("#tblMembershipWiseSale_length").val() != null && $("#tblMembershipWiseSale_length").val() != "") {
                                    GetMembershipWiseSale(pageNo, $("#tblMembershipWiseSale_length").val())
                                }
                                else {
                                    GetMembershipWiseSale(pageNo, 10)
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
                $('#tblMembershipWiseSale tbody').append(norecords);
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

function SearchMembershipWiseSale(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchMembershipwiseListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMembershipWiseSale tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {

                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
                         + '<td>' + item.Package + '</td>'
                           + '<td>' + item.ToatlSale + '</td>'
                            + '<td>' + item.Amount + '</td>'

                        + '</tr >';
                    $('#tblMembershipWiseSale tbody').append(rows);
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
                                if ($("#tblMembershipWiseSale_length").val() != null && $("#tblMembershipWiseSale_length").val() != "") {
                                    SearchMembershipWiseSale(pageNo, $("#tblMembershipWiseSale_length").val())
                                }
                                else {
                                    SearchMembershipWiseSale(pageNo, 10)
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
                $('#tblMembershipWiseSale tbody').append(norecords);
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

function ExcelMembershipList() {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "POST",
        url: ExcelUrl,
        dataType: "json",
        data: { startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            window.location = DownloadExcelUrl + "?fileGuid='" + data.FileGuid
                              + '&filename=' + data.FileName;
            HideLoader();
        },
        error: function (data) {
            HideLoader();
            alert("Failed! Please try again.");
        }
    });
}

function PDFMembershipList() {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "POST",
        url: PDFUrl,
        dataType: "json",
        data: { startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            window.location = DownloadPdfUrl + "?filename=" + "Reports Membershipwise Sale.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}