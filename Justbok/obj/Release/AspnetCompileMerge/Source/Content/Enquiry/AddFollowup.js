$(document).ready(function () {
    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    $("#StartTime").datetimepicker({
        format: 'HH:mm',
        useCurrent: true,
        stepping: 15
    });
    $("#EndTime").datetimepicker({
        format: 'HH:mm',
        useCurrent: true,
        stepping: 15
    });

    //$('#StartTime').wickedpicker();
    //$('#EndTime').wickedpicker();
    UpdateFollowupHeader();
    UpdateFollowupTable();
    //$(document).on('click', '.btnEditEnquiry', function () { EditEnquiry(); });
    $(document).on('click', '#btnAddFollowup', function () { AddFollowup(); });

});

function UpdateFollowupHeader()
{
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetEnquiryDetailsUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: "",
    success: function (data) {
           
        $.each(data, function (i, obj) {
            var username = obj.FirstName + " " + obj.LastName;
            $('#lblUsername').text(username);
            $('#lblMobileNo').text(obj.MobileNumber);
            $('#lblEnqDate').text(obj.EnquiryDate);
        });
        HideLoader();
    },
    error: function () {
        HideLoader();
        alert("Failed! Please try again.");
    }
});

}

function UpdateFollowupTable()
{
    $.ajax({
        cache: false,
        type: "GET",
        url: GetFollowupDetailsUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: "",
    success: function (data) {
        $('#tblEnquiry tbody').empty();
        $.each(data, function (i, item) {
            var lastFollowupDate = "";
            var nextFollowupdate = "";
            var description = "";
            if (item.LastFollowUpDate != null) { lastFollowupDate = item.LastFollowUpDate }
            if (item.NextFollowUpDate != null) { nextFollowupdate = item.NextFollowUpDate }
            if (item.Description != null) { description = item.Description }
            var rows = "<tr>"
                + "<td style='display:none;'>" + item.FollowupId + "</td>"
+ "<td>" + lastFollowupDate + "</td>"
+ "<td>" + nextFollowupdate + "</td>"
+ "<td>" + item.StartTime + "</td>"
+ "<td>" + item.EndTime + "</td>"
+ "<td>" + item.EnqStatus + "</td>"
+ "<td>" + description + "</td>"
+ "<td><a class='btn btn-info btnEditEnquiry'  onclick='return EditEnquiry(this);' data-toggle='modal'>Edit</a></td>"
+ "</tr>";
            $('#tblEnquiry tbody').append(rows);
        });
    },
    failure: function (errMsg) {
        alert(errMsg.responseText);
    }
});


}

function EditEnquiry(id)
{
    var followupid = $(id).closest('tr').find('td').eq(0).html();
        $.ajax({
            cache: false,
            type: "GET",
            url: EnquiryDetailsUrl,
            dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { FollowupId: followupid },
        success: function (data) {
            $.each(data, function (i, obj) {
                if (obj.FollowupId == followupid) {
                    $('#txtfollowupid').val(obj.FollowupId);
                    $("#EnqStatus option").each(function () {
                        if ($(this).text() == obj.EnqStatus) {
                            $(this).prop("selected", true)
                        }
                    });
                    $('#LastFollowUpDate').val(obj.LastFollowUpDate);
                    $('#NextFollowUpDate').val(obj.NextFollowUpDate);
                    $('#StartTime').val(obj.StartTime);
                    $('#EndTime').val(obj.EndTime);
                    $('#Description').val(obj.Description);
                }
            });
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });

}

function AddFollowup()
{
   
    var jsonObject = {
        EnqStatus: $('#EnqStatus option:selected').val(), FollowupId: $('#txtfollowupid').val(), LastFollowUpDate: $('#txtLastFollowupDate').val(),
        NextFollowUpDate: $('#NextFollowUpDate').val(), StartTime: $('#StartTime').val(), EndTime: $('#EndTime').val(),
        Description: $('#Description').val()
    }

    $.ajax({
        cache: false,
        type: "POST",
        url: AddFollowupUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObject),
    success: function (data) {
        // UpdateMemberShipTable();

        $('#txtfollowupid').val("");
        UpdateFollowupTable();
    },
    failure: function (errMsg) {
        alert(errMsg.responseText);
    }
});
}