var pageNo = 1, pagerLoaded = false;
var url = "";

$(document).ready(function () {
    ShowLoader();
    $('input[type=datetime]').datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        yearRange: "-90:+00"
    });
    if ($("#tblTimesheetReports_length").val() != null && $("#tblTimesheetReports_length").val() != "") {
        TimesheetReports(pageNo, $("#tblTimesheetReports_length").val())
    }
    else {
        TimesheetReports(pageNo, 10)
    }

    $('#btnSearch').click(function () {

        if ($("#tblTimesheetReports_length").val() != null && $("#tblTimesheetReports_length").val() != "") {
            SearchPosSaleReportGym(pageNo, $("#tblTimesheetReports_length").val())
        }
        else {
            SearchPosSaleReportGym(pageNo, 10)
        }
    });

   
    $('#btnYes').click(function () { ShowSalarypopup(); });

    $('#dwnldPdf').click(function () { PDFMembershipList(); });
    $('#dwnldExcel').click(function () { ExcelMembershipList(); });
});

function TimesheetReports(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: TimesheetUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblTimesheetReports tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
                         + '<td style="display:none;">' + item.StaffId + '</td>'
                          + '<td style="display:none;">' + item.SalaryId + '</td>'
     + '<td>' + item.StaffName + '</td>'
       + '<td>' + item.WorkingDays + '</td>'
        + '<td>' + item.PresentDays + '</td>'
         + '<td>' + item.LeaveDays + '</td>'
          + '<td> <button type="button" class="btn btn-xs btn-success btn-flat pull-left btnSalary" data-toggle="modal" data-target="#modal_Conformation" title="Add" onclick="return BindStaffId(this);"><sapn class="glyphicon glyphicon-plus"></sapn> Add </button><sapn class="pull-right"> ' + item.TotalSalary + '</span></td>'
          + '<td> <button class="btn btn-primary btn-xs btn-flat btnCalendar" title="View Days" onclick="return CalenderBinding(this);"><span class="glyphicon glyphicon-calendar"></span> View Days</button></td>'
       + '</tr >';
                        $('#tblTimesheetReports tbody').append(rows);
                    
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
                                if ($("#tblTimesheetReports_length").val() != null && $("#tblTimesheetReports_length").val() != "") {
                                    TimesheetReports(pageNo, $("#tblTimesheetReports_length").val())
                                }
                                else {
                                    TimesheetReports(pageNo, 10)
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
                $('#tblTimesheetReports tbody').append(norecords);
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

function BindStaffId(id) {

    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    $('#SalaryUserName').text($('#tblTimesheetReports tr').eq(rowIndex + 1).find('td').eq(2).html());
    $('#txtSalaryStaffId').val($('#tblTimesheetReports tr').eq(rowIndex + 1).find('td').eq(0).html());
    $('#txtSalarySalaryId').val($('#tblTimesheetReports tr').eq(rowIndex + 1).find('td').eq(1).html());
    

}

function ShowSalarypopup()
{
    $("#modal_Conformation").modal('hide');
    $('#modal-Salary').modal('show');
}

   
function SaveSalary()
{
    ShowLoader();
    var jsonObject = {
        SalaryAmount: $('#txtSalaryAmount').val(), SalaryDate: $('#txtSalaryDate').val(), SalaryMode: $('#ddlSalaryMode option:selected').text(),
        ReferenceNumber: $('#txtReferenceNumber').val(), Comments: $('#txtComments').val(), StaffId: $('#txtSalaryStaffId').val(),SalaryId:$('#txtSalarySalaryId').val()
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: SaveSalaryUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            $('#modal-Salary').modal('hide');

            if ($("#tblTimesheetReports_length").val() != null && $("#tblTimesheetReports_length").val() != "") {
                TimesheetReports(pageNo, $("#tblTimesheetReports_length").val())
            }
            else {
                TimesheetReports(pageNo, 10)
            }
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
    
}

function CalenderBinding(id)
{
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var staffId = $('#tblTimesheetReports tr').eq(rowIndex + 1).find('td').eq(0).html();
    $.ajax({
        cache: false,
        type: "GET",
        url: CalenderUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: {StaffId:staffId, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
           
            $('#UserTimesheet tbody').find("tr").remove();
            if (data != null) {
               
                $.each(data, function (i, item) {
                    var workingHrs = "";
                    if (i == 0) {
                       
                        $('#CalUserName').text(item.StaffName);
                        $('#spStaffid').text(item.StaffId);
                    }
                    if (item.Date.indexOf("Sat") >= 0) {
                        var rows = '<tr role="row" class="odd" style="background-color: lightgray;">'
      + '<td>' + item.Date + '</td>'
                    }
                    else if (item.Date.indexOf("Sun") >= 0)
                    {
                        var rows = '<tr role="row" class="odd" style="background-color: lightpink;">'
         + '<td>' + item.Date + '</td>'
                    }
                    else {

                        var rows = '<tr role="row" class="odd">'
      + '<td>' + item.Date + '</td>'
                    }
                    if ((item.InTime != null && item.InTime != "") || (item.OutTime != null && item.OutTime != "")) {
                        rows += '<td>' + item.InTime + '</td>'
                        rows += '<td>' + item.OutTime + '</td>'
                        var inTime = ConvertAMPM(item.InTime);
                        var outTime = ConvertAMPM(item.OutTime);
                        workingHrs = CalculateTimediff(item.Date, inTime, outTime);
                            rows += '<td>' + workingHrs + '</td>';
                    }
                    else {
                        if (item.AbsentID != "") {
                            rows += '<td></td><td></td>'
                        }
                        else {
                            rows += '<td></td><td></td><td></td>'
                        }
                        
                    }
                    var gotDate = item.Date.split(' ');
                    var todaydate = ConvertTodayDateToDDMMYY();
                   
                    if ((item.InTime != null && item.InTime != "") || (item.OutTime != null && item.OutTime != "")) {
                        rows += '<td><span class="label label-success">Present</span></td></tr >';
                    }
                    else if (item.AbsentID != "") {
                        rows += '<td>0 Hrs 0 Mins</td><td><span class="label label-primary">Leave</span></td></tr >';
                    }
                    else if (gotDate[0] <= todaydate) {
                        rows += '<td><span class="label label-danger">Absent</span></td></tr >';
                    }
                    else {
                        rows += '<td></td></tr >';
                    }

                    $('#UserTimesheet tbody').append(rows);
                });
            }
            $('#modal_Calender').modal('show');
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function CalculateTimediff(date,startTime,endTime)
{
   
    var valuestart = startTime;
    var valuestop = endTime;
    var hourStart = new Date(date + valuestart).getHours();
    var hourEnd = new Date(date + valuestop).getHours();
   

    var minuteStart = new Date(date + valuestart).getMinutes();
    var minuteEnd = new Date(date + valuestop).getMinutes();

    var hourDiff = hourEnd - hourStart;
    var minuteDiff = minuteEnd - minuteStart;
    //if (minuteDiff < 0) {
    //    hourDiff = hourDiff - 1;
    //}
    if (minuteDiff < 10)
    {
        minuteDiff = "0" + minuteDiff;
    }
  
    return hourDiff+" Hrs "+minuteDiff+" Mins"
}

function ConvertAMPM(time)
{
    var hrs = Number(time.match(/^(\d+)/)[1]);
    var mnts = Number(time.match(/:(\d+)/)[1]);
    var format = time.match(/\s(.*)$/)[1];
    if (format == "PM" && hrs < 12) hrs = hrs + 12;
    if (format == "AM" && hrs == 12) hrs = hrs - 12;
    var hours = hrs.toString();
    var minutes = mnts.toString();
    if (hrs < 10) hours = "0" + hours;
    if (mnts < 10) minutes = "0" + minutes;

    return hours + ":" + minutes;
}

function ConvertTodayDateToDDMMYY()
{
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!

    var yyyy = today.getFullYear();
    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }
    var today = dd + '/' + mm + '/' + yyyy;
    return today;
}
