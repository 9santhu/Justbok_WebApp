$(document).ready(function () {
    $('#txtAttendanceDate').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"

    });

    $("#txtTimeIn, #txtTimeOut").datetimepicker({
        format: 'hh:mm A',
        useCurrent: true,
        stepping: 15
    });

    BindLeaveDropdown();

    $('#txtAttendanceDate').datepicker('setDate', new Date());
    UpdateTable();
    $(document).ready(function () { }).on('click', '#btnManualSave', function (e) { SaveManualPresent(); });
    $('#txtAttendanceDate').on('change', function () { UpdateTable(); });
    $(document).ready(function () { }).on('click', '#btnsaveleave', function () { SaveLeave(); });
    $("#ddlBranch").change(function () { branch = $("#ddlBranch option:selected").val(); UpdateTable(); });


});

function UpdateTable()
{
    console.log("Bind Staff List called");
    ShowLoader();
    $.ajax({
        type: "GET",
        url: GetMemberListUrl,
        data: { Staffdate: $('#txtAttendanceDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (data) {

        $('#tblTimesheet tbody').empty();
        $.each(data, function (i, item) {

            var datematched = false;

            var rows = "<tr>"
                  + "<td style='display:none'>" + item.TimeSheetId + "</td>"
                + "<td style='display:none'>" + item.StaffId + "</td>"
                 + "<td >" + item.FirstName + " " + item.LastName + "</td>"
            //Present
           
            if (item.Present >0) {
                rows = rows + "<td > <i class='fa fa-check-circle' style='color:#00a65a;'></i><span style='display:none'>" + item.Present + "</span></td>"
            }

            else {

                rows = rows + "<td></td>"
            }

            // In
            var todaydate = new Date();
            var month = todaydate.getMonth() + 1;
            var day = todaydate.getDate();

            var convertTodayDate = todaydate.getFullYear() + '/' +
                (('' + month).length < 2 ? '0' : '') + month + '/' +
                (('' + day).length < 2 ? '0' : '') + day;
           
            var totaldate = convertDate(convertTodayDate);
        
            var gotDate = ConvertDateTime(item.AttendenceDate)
            if (totaldate.toString() === gotDate) {
                datematched = true;
            }
            //alert(JSON.stringify(item.InTime));
            if (item.InTime != null) {
                rows = rows + "<td >" + item.InTime + "</td>"
            }
            else if (datematched)
            {
                rows = rows + "<td > <a class='btn btn-success btn-xs btn-flat btnIn' onclick='return PresentIn(this);'>In</a></td>"
            }
            else {
                rows = rows + "<td ></td>"
            }
            // out
            
            if (item.OutTime != null) {
                rows = rows + "<td >" + item.OutTime + "</td>"
            }
            else if(datematched)
            {
                rows = rows + "<td > <a class='btn btn-danger btn-xs btn-flat btnOut' onclick='return PresentOut(this);'>Out</a></td>"
            }

            else {
                rows = rows + "<td ></td>"
            }
            //Leave
            if (item.LeaveDetails != null) {
                rows = rows + "<td >" + item.LeaveDetails + "<span style='display:none'>" + item.Leave + "</span>" + "</td>"
            }
            else {
                rows = rows + "<td ></td>"
            }
            rows = rows+ "<td >"
            if (item.Present != null && item.Present == "Yes")
            {
                rows = rows + "<a class='btn btn-xs btn-success btn-flat btnPresent' onclick='return Present(this);' style='display:none;'>Present</a>&nbsp;"
            }
            else
            {
                rows = rows + "<a class='btn btn-xs btn-success btn-flat btnPresent'  onclick='return Present(this);' >Present</a>&nbsp;"
            }
            rows = rows + "<a class='btn btn-primary btn-xs btn-flat btnManual' data-toggle='modal' data-target='#modal-manual'  onclick='return ManualPresent(this);'>Manual</a>&nbsp;<a class='btn btn-warning btn-xs btn-flat btnLeave' data-toggle='modal' data-target='#modal-leave'  onclick='return Leave(this);'>Absent</a></td>"
            + "</tr>";
            $('#tblTimesheet tbody').append(rows);
           
        })
        HideLoader();
    },
    error: function () {
        HideLoader();
        alert("Failed! Please try again.");
    }
});


}

function PresentIn(id)
{
    ShowLoader();
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
   var date1 = new Date();
    var date = ToDateDDMMYYFormat();
    var time = formatAMPM(date1);
    $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(4).html(time);
    var jsonObject = {
        TimeSheetId: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(0).html(), StaffId: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(1).html(), AttendenceDate: $('#txtAttendanceDate').val(),
        Present: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(3).find("span").text(), InTime: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(4).text(),
        OutTime: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(5).text(),
        Leave: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(6).find("span").text(), BranchId: $('#ddlBranch option:selected').val()
    }
    //$('#txtAttendanceDate').val()
   // alert($('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(4).html());
    $.ajax({
        cache: false,
        type: "POST",
        url: AddUpdateAttendenceUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObject),
    success: function (data) {
        UpdateTable();
        HideLoader();
    },
    failure: function (errMsg) {
        HideLoader();
        alert(errMsg.responseText);
    }
});
}

function PresentOut(id)
{
    ShowLoader();
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var date1 = new Date();
    var date = ToDateDDMMYYFormat();
    var time = formatAMPM(date1);
    $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(5).html(time);
    var jsonObject = {
        TimeSheetId: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(0).html(), StaffId: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(1).html(), AttendenceDate: $('#txtAttendanceDate').val(),
        Present: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(3).find("span").text(), InTime: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(4).text(),
        OutTime: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(5).text(),
        Leave: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(6).find("span").text(), BranchId: $('#ddlBranch option:selected').val()
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: AddUpdateAttendenceUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObject),
    success: function (data) {
        UpdateTable();
        HideLoader();
    },
    failure: function (errMsg) {
        HideLoader();
        alert(errMsg.responseText);
    }
});
}

function Present(id)
{
    ShowLoader();
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var date = ToDateDDMMYYFormat();
    var jsonObject = {
        TimeSheetId: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(0).html(), StaffId: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(1).html(), AttendenceDate: $('#txtAttendanceDate').val(),
        Present: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(3).find("span").text(), InTime: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(4).text(),
        OutTime: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(5).text(),
        BranchId: $('#ddlBranch option:selected').val(),
        Leave: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(6).find("span").text()
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: AddUpdateAttendenceUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObject),
    success: function (data) {
        UpdateTable();
        HideLoader();
    },
    failure: function (errMsg) {
        HideLoader();
        alert(errMsg.responseText);
    }
});
}

function ManualPresent(id)
{
    ShowLoader();
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    $('#txtManualTimesheetId').val($('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(0).html());
    $('#txtManualMemberId').val($('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(1).html());
    $('#txtmembername').text($('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(1).html());
    $('#txtManualPresent').val($('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(3).find("span").text());
    $('#txtInTime').text($('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(4).text());
    $('#txtOutTime').text($('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(5).text());
    $('#txtManualLeave').val($('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(6).find("span").text());

    HideLoader();
}

function SaveManualPresent()
{
    ShowLoader();
    var jsonObject = {
        TimeSheetId: $('#txtManualTimesheetId').val(),StaffId: $('#txtManualMemberId').val(), AttendenceDate: $('#txtAttendanceDate').val(), Present: $('#txtManualPresent').val(),
        InTime: $('#txtTimeIn').val(),
        OutTime: $('#txtTimeOut').val(), Leave: $('#txtManualLeave').val(),
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: AddUpdateAttendenceUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObject),
    success: function (data) {

        UpdateTable();
        $("#modal-manual").modal('hide');
        HideLoader();
    },
    failure: function (errMsg) {
        HideLoader();
        alert(errMsg.responseText);
    }
});
}

function Leave(id)
{
    
    ShowLoader();
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var timesheetId = $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(0).html();
    var staffId = $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(1).html();
    var isPresent =$('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(3).find("span").text()
    var inTime=$('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(4).text()
    var outTime=$('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(5).text()
    var leaveDetails = $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(6).text()
    var leaveID = $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(6).find("span").text()
    //alert(timesheetId+" "+staffId+" "+isPresent+" "+inTime+" "+outTime+" "+leaveDetails+" "+leaveID  );
    $('#txtTimeSheetId').val(timesheetId);
    $('#txtMmeberIdLeave').val(staffId);
    $('#txtIsPresent').val(isPresent);
    $('#txtInTime').val(inTime);
    $('#txtOutTime').val(outTime);
    $('#txtLeaveDetails').val(leaveDetails);
    $('#txtLeaveId').val(leaveID);
    HideLoader();
}

function SaveLeave()
{
   
    ShowLoader();
    var jsonObject = {
        TimeSheetId: $('#txtTimeSheetId').val(), StaffId: $('#txtMmeberIdLeave').val(), AttendenceDate: $('#txtAttendanceDate').val(),
        Present: $('#txtIsPresent').val(), InTime: $('#txtInTime').val(),
        OutTime: $('#txtOutTime').val(),
        LeaveDetails: $('#txtLeaveDetails').val(),
        Leave: $('#txtLeaveId').val(),
        LeaveId:   $('#ddlLeave  option:selected').val(),
        LeaveType: $('#ddlLeaveType  option:selected').text(),
        Reason: $('#txtreason').val(),
        BranchId: $('#ddlBranch option:selected').val()
       
        //StaffId: $('#txtMmeberIdLeave').val(), AttendenceDate: $('#txtAttendanceDate').val(), Present: $('#txtPresentLeave').val(),
        //InTime: $('#txtInTimeLeave').val(),
        //OutTime: $('#txtOutTimeLeave').val(), Leave: $('#ddlLeave  option:selected').val() + " | " + $('#ddlLeave  option:selected').text()
    }

    $.ajax({
        cache: false,
        type: "POST",
        url: AddAbsentUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObject),
    success: function (data) {
      
        UpdateTable();
        HideLoader();
        $("#modal-leave").modal("hide");
    },
    failure: function (errMsg) {
        HideLoader();
        alert(errMsg.responseText);
    }
});
}

function formatAMPM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}

function convertDate(selector) {
   
    //alert(selector);
    var from = selector.split("/");
    var gdate = from[2].toString() + "/" + from[1].toString() + "/" + from[0].toString();
    //alert(gdate);
    //var sel = new Date(gdate);
   // alert(sel);
    return gdate;
}

function ConvertDateTime(convertdate) {
    console.log(convertdate)
    var date = convertdate;
  
    var parsedDate = new Date(parseInt(date.substr(6)));
    //var parsedDate = new Date(Date.parse(date.substr(6)));
   
    //console.log("parsed date " + parsedDate.toUTCString());
    var newDate = new Date(parsedDate);
    console.log(newDate);

    //var getMonth = newDate.getMonth();
    var getDay = newDate.getDay();
    var getYear = newDate.getYear();
    var twoDigitDate = newDate.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
    var getMonth = ((newDate.getMonth().length + 1) === 1) ? (newDate.getMonth() + 1) :  + (newDate.getMonth() + 1);


    var startdate = twoDigitDate + '/' + getMonth + '/' + newDate.getFullYear();
    console.log(startdate);
    return startdate;
}

function GetDate(selector) {
    try {
        var from = selector.split("-");
        var gdate = from[1].toString() + "/" + from[0].toString() + "/" + from[2].toString();
        return gdate;
    }
    catch (err) {
        var from = selector.split("/");
        var gdate = from[1].toString() + "/" + from[0].toString() + "/" + from[2].toString();
        return gdate;
    }
}

function toDate(selector) {
    try {
        var from = selector.split("-");
        var gdate = from[0].toString() + "/" + from[1].toString() + "/" + from[2].toString();
        var sel = new Date(gdate);
        return sel;
    }
    catch (err) {
        var from = selector.split("/");
        var gdate = from[2].toString() + "/" + from[1].toString() + "/" + from[0].toString();
        var sel = new Date(gdate);
        return sel;
    }
}

function BindLeaveDropdown()
{
    $.ajax({
        cache: false,
        type: "GET",
        url: BindLeaveUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: "",
        success: function (data) {
            $.each(data, function
                (i, item)
            {
                $("#ddlLeave").append($("<option     />").val(item.LeaveTypeId).text(this.LeaveName));
            }
            );
           
        },
        failure: function (errMsg) {
          
        }
    });

}

function ToDateDDMMYYFormat()
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