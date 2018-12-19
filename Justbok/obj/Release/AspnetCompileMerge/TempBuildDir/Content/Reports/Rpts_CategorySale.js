var pageNo = 1, pagerLoaded = false;
var url = "";

$(document).ready(function () {
    ShowLoader();
    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    CategoerySales();
    $('#btnSearch').click(function () { SearchCategoerySales(); });

    $('#dwnldPdf').click(function () { PDFMembershipList(); });

    $('#dwnldExcel').click(function () { ExcelMembershipList(); });

});


function CategoerySales() {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: CategoryWiseSaleUrl,
        dataType: "json",
        data:"",
        success: function (data) {
            $('#tblCategoryList tbody').find("tr").remove();

            var totalSale=0;
            var salAmount=0;

            $.each(data, function (i, item) {
                totalSale = totalSale + 1;
                salAmount = salAmount + item.Amount
            });
            var rows = "<tr>"
                  + "<td> Gym </td>"
                    + "<td>" +totalSale + "</td>"
                      + "<td>" + salAmount + "</td>"
   + "</tr>";
            $('#tblCategoryList tbody').append(rows);
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

function SearchCategoerySales() {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchCategoryWiseSaleUrl,
        dataType: "json",
        data: { startDate: $('#txtFromDate').val(), endDate: $('#txtToDate').val() },
        success: function (data) {
            $('#tblCategoryList tbody').find("tr").remove();

            var totalSale = 0;
            var salAmount = 0;

            $.each(data, function (i, item) {
                totalSale = totalSale + 1;
                salAmount = salAmount + item.Amount
            });
            var rows = "<tr>"
                  + "<td> Gym </td>"
                    + "<td>" + totalSale + "</td>"
                      + "<td>" + salAmount + "</td>"
   + "</tr>";
            $('#tblCategoryList tbody').append(rows);
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
        data: { startDate: $('#txtFromDate').val(), endDate: $('#txtToDate').val() },
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
        data: { startDate: $('#txtFromDate').val(), endDate: $('#txtToDate').val() },
        success: function (data) {
            window.location = DownloadPdfUrl + "?filename=" + "ReportsCategoryWiseSale.pdf";
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));

        }
    });
}