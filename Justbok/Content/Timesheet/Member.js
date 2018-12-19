$(document).ready(function () {
    $('#txtAttendanceDate').datepicker({
        dateFormat: "m/dd/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"

    });

    $('#txtAttendanceDate').datepicker('setDate', new Date());
    UpdateTable();

    $('#txtAttendanceDate').on('change', function () { UpdateTable(); });
    $('.chkmaster').click(function (e) { $(this).closest('table').find('td input:checkbox').prop('checked', this.checked);});
    $('#btnAllPresent').click(function (e) { PresentAll(); });
    $("#ddlBranch").change(function () { branch = $("#ddlBranch option:selected").val(); UpdateTable(); });

});

function UpdateTable()
{
    ShowLoader();
    $.ajax({
        url:MembersAttendanceListUrl,
        data: { date: $('#txtAttendanceDate').val(), BranchId: $('#ddlBranch option:selected').val()},
    type: "GET",
    dataType: "json",
    success: function (data) {

        $('#tblTimesheet tbody').empty();
        $.each(data, function (i, item) {
            var rows = "<tr>"
                + "<td>" + item.MemberID + "</td>"
                 + "<td >" + item.FirstName + " " + item.LastName + "</td>"
            //Present
            if (item.Present !=null && item.Present == "Yes") {
                rows = rows + "<td > <label class='label label-success' >" + item.Present + "</label></td>"
            }
            else if (item.Present != null && $.trim(item.Present) == "No")
            {
                rows = rows + "<td > <label class='label label-warning' >" + item.Present + "</label></td>"
            }
            else {
                rows = rows + "<td></td>"
            }
            rows = rows + "<td >" + item.MobileNumber + "</td>"
                   
            if (item.Present != null && item.Present == "Yes")
            {
                rows = rows + "<td><strong><a class='text-danger link btnAbsent' onclick='return MarkAbsent(this);'>Mark Absent</a></strong></td>"
            }
            else
            {
                rows = rows + "<td><strong><a class='text-success link btnPresent'  onclick='return MarkPresent(this);'>Mark Present</a></strong></td>"
            }
            rows = rows + "<td><input type='checkbox' class='multi-checkbox' /></td>"
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

function MarkPresent(id)
{
    ShowLoader();
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(2).html("<label class='label label-success' >Yes</label>");
    var jsonObject = {
        MemberID: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(0).html(),AttendenceDate: $('#txtAttendanceDate').val(),
        IsPresent: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(2).find("label").text()
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: AddUpdateMemberAttendenceUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObject),
    success: function (data) {
        UpdateTable();
        HideLoader();
    },
    failure: function (errMsg) {
        //HideLoader();
        alert(errMsg.responseText);
    }
});
           
}

function MarkAbsent(id)
{
    ShowLoader();
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(2).html("<label class='label label-warning' >No</label>");
    var jsonObject = {
        MemberID: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(0).html(),AttendenceDate: $('#txtAttendanceDate').val(),
        IsPresent: $('#tblTimesheet tr').eq(rowIndex + 1).find('td').eq(2).find("label").text()
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: AddUpdateMemberAttendenceUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObject),
    success: function (data) {
        UpdateTable();
        //HideLoader();
    },
    failure: function (errMsg) {
        HideLoader();
        alert(errMsg.responseText);
    }
});
}

function PresentAll()
{
    ShowLoader();
    var memberid="";
    var ispresent="";
    var jsonObj = [];
    $('#tblTimesheet').find('tr').each(function (i, el) {
        var chkbox = $(this).find("input[type='checkbox']");
        if(chkbox.prop('checked')==true)
        {
            var $tds = $(this).find('td');
            var memberid = $tds.eq(0).text();
            jsonObj.push(
          { "MemberID": memberid, "IsPresent": "Yes", AttendenceDate: $('#txtAttendanceDate').val()}
          );
        }
    });

    $.ajax({
        cache: false,
        type: "POST",
        url: MulitpleMemberAttendanceUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObj),
    success: function (data) {
        UpdateTable();
        //HideLoader();
    },
    failure: function (errMsg) {
        HideLoader();
        alert(errMsg.responseText);
    }
});
}
