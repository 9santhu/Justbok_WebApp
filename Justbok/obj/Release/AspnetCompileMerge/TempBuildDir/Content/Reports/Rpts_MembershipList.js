var pageNo = 1, pagerLoaded = false;

$(document).ready(function () {
   
    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    if ($("#tblMembershipList_length").val() != null && $("#tblMembershipList_length").val() != "") {
        GettingMemberList(pageNo, $("#tblMembershipList_length").val())
    }
    else {
        GettingMemberList(pageNo, 10)
    }
    BindMembership();

    $('#btnSearch').click(function () {
        if ($("#tblMembershipList_length").val() != null && $("#tblMembershipList_length").val() != "") {
            SearchMember(pageNo, $("#tblMembershipList_length").val())
        }
        else {
            SearchMember(pageNo, 10)
        }
    });

    $('#dwnldPdf').click(function () { PDFMembershipList(); });

    $('#dwnldExcel').click(function () { ExcelMembershipList(); });

   


   
});

function SearchMember(pageno, pagesize)  {
    ShowLoader();
    $('#pagination').twbsPagination('destroy');
    // ChnangePaginationId();
    pagerLoaded = false;
    var status = $('input[type=checkbox]').prop('checked');
    //var from = $("#txtFromDate").val().split("/")
    //var f = new Date(from[2], from[1] - 1, from[0])
    //console.log(f);
    
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchMemberListUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { page: pageno, pagesize: pagesize, membername: $("#txtSearch").val(), membershiptype: $("#ddlMembership option:selected").text(), startdate: $("#txtFromDate").val(), todate: $('#txtToDate').val(), category: $("#ddlCategory").val(), active: status, branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMembershipList tbody').find("tr").remove();
            //$('#tblMembershipList tbody').empty();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;
                    if (item.Amount != "") {

                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                    }
                    var dob = "";
                    var enrolldate = "";
                    var startdate = "";
                    var enddate = "";
                    if (item.Dob != null)
                    {
                        dob = ConvertBackEndDate(item.Dob);
                    }
                    if (item.EnrollDate != null)
                    {
                        enrolldate = ConvertBackEndDate(item.EnrollDate);
                    }
                    console.log(item.StartDate);
                    if (item.StartDate != null) {
                        startdate=  ConvertBackEndDate(item.StartDate);
                    }
                    if (item.EndDate != null) {
                        enddate= ConvertBackEndDate(item.EndDate);
                    }
                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.MemberID + '</td>'
                        + '<td>' + item.FirstName + '</td>'
                        + '<td>' + item.MobileNumber + '</td>'
                        + '<td>' + item.Email + '</td>'
                        + '<td>' + dob+ '</td>'
                         + '<td>' + item.MemberAddress + '</td>'
                        + '<td>' + enrolldate + '</td>'
                         + '<td>' + item.MembershipType + '</td>'
                          + '<td>' + startdate + '</td>'
                           + '<td>' + enddate + '</td>'
                            + '<td>' + item.Amount + '</td>'
                             + '<td>' + item.PaymentAmount + '</td>'
                    + '<td>' + pendingamt + '</td>'
                     + '<td>' + item.Status + '</td>'
                        + '</tr >';
                    $('#tblMembershipList tbody').append(rows);
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
                                if ($("#tblMembershipList_length").val() != null && $("#tblMembershipList_length").val() != "") {
                                    SearchMember(pageNo, $("#tblMembershipList_length").val())
                                }
                                else {
                                    SearchMember(pageNo, 10)
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
                $('#tblMembershipList tbody').append(norecords);
            }

            HideLoader();
            //setClass();
        },
        error: function (errMsg) {
            HideLoader();
           
        },
        failure: function (errMsg) {
            HideLoader();
            
        }
    });
}


function ChnangePaginationId()
{
    $('#pagination').attr('id', 'searchpagination');
}

function GettingMemberList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetMemberListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMembershipList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var pendingamt = 0;
                    if (item.Amount != "") {
                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                    }

                    var dob = "";
                    var enrolldate = "";
                    var startdate = "";
                    var enddate = "";
                    if (item.Dob != null) {
                        dob = ConvertToDate(item.Dob);
                    }
                    if (item.EnrollDate != null) {
                        enrolldate = ConvertToDate(item.EnrollDate);
                    }

                    if (item.StartDate != null) {
                        startdate = ConvertToDate(item.StartDate);
                    }
                    if (item.EndDate != null) {
                        enddate = ConvertToDate(item.EndDate);
                    }


                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.MemberID + '</td>'
                        + '<td>' + item.FirstName + '</td>'
                        + '<td>' + item.MobileNumber + '</td>'
                        + '<td>' + item.Email + '</td>'
                        + '<td>' + dob + '</td>'
                         + '<td>' + item.Address + '</td>'
                        + '<td>' + enrolldate + '</td>'
                         + '<td>' + item.Package + '</td>'
                          + '<td>' + startdate + '</td>'
                           + '<td>' + enddate + '</td>'
                            + '<td>' + item.Amount + '</td>'
                             + '<td>' + item.PaymentAmount + '</td>'

                    + '<td>' + pendingamt + '</td>'
                     + '<td>' + item.Status + '</td>'
                    
                              //+ '<td>' + item.Amount- item.PaymentAmount + '</td>'
                        + '</tr >';
                    $('#tblMembershipList tbody').append(rows);
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
                                if ($("#tblMembershipList_length").val() != null && $("#tblMembershipList_length").val() != "") {
                                    GettingMemberList(pageNo, $("#tblMembershipList_length").val())
                                }
                                else {
                                    GettingMemberList(pageNo, 10)
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
                $('#tblMembershipList tbody').append(norecords);
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

function BindMembership() {
    $.ajax({
        cache: false,
        type: "GET",
        url: BindMembershipUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: "",
        success: function (data) {
            $.each(data, function (i, obj) {
                $("#ddlMembership").append($("<option></option>").val(obj.MembershipOfferId).html(obj.MemershipType));
            });
        },
        error: function () {
            alert("Failed! Please try again.");
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
       
        data: { membername: $("#txtSearch").val(), membershipid: $("#ddlMembership option:selected").val(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), category: $("#ddlCategory").val(), active: $("#chkActive").val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
           // var response = JSON.parse(data);

            window.location = DownloadExcelUrl + "?fileGuid='" + data.FileGuid
                              + '&filename=' + data.FileName;
            //toastr.success("Excel Report Generated.");
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
        data: { membername: $("#txtSearch").val(), membershipid: $("#ddlMembership option:selected").val(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), category: $("#ddlCategory").val(), active: $("#chkActive").val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
           
           
            console.log(data.FileName);
            window.location = DownloadPdfUrl + "?filename=" + "ReportsMembership.pdf";
            //toastr.success("PDF Report Generated.");
            HideLoader();
        },
        error: function (data) {

            HideLoader();

            alert(JSON.stringify(data));
            
        }
    });
}


function ConvertBackEndDate(date)
{
    var dt = new Date(parseInt(date.substr(6)));
    var twoDigitDate = dt.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
    var getMonth = ((dt.getMonth().length + 1) === 1) ? (dt.getMonth() + 1) : '0' + (dt.getMonth() + 1);
    var year = dt.getFullYear();
    var fulldate = twoDigitDate + "/" + getMonth + "/" + year;
    return fulldate;
}

function ConvertToDate(convertdate) {
    var date = convertdate.split("-");
    //var date = convertdate;
    if (date.length > 0 && convertdate != "") {
        var startdate = date[2] + '/' + date[1] + '/' + date[0];
        return startdate;
    }
    else { return ""; }
}





