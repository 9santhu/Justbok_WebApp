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
    
    BindHeaderDetails();
    BindFollowupDetails();

    $(document).on('click', '#btnAddFollowup', function () { if (ValidateEnquiryForm()) { AddFollowup();} });
    $(document).on('click', '#lnkEditEnquiry', function () { RedirectEnquiry();});

    HideLoader();
   
});

function BindHeaderDetails()
{
   
    $.ajax({
        cache: false,
        type: "GET",
        url: GetMemberDetailsUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: "",
        success: function (data) {
            
                $("#customerName").text(data[0].Name);
                $("#phNumber").text(data[0].MobileNumber);
                $("#enquiryDate").text(data[0].EnquiryDate);
                
        },
        failure: function (errMsg) {
            HideLoader();
        }
    });
}

function BindFollowupDetails()
{
    
    $.ajax({
        cache: false,
        type: "GET",
        url: GetFollowup,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: "",
        success: function (data) {
            $('#tblEnquiry tbody').find("tr").remove();
            $.each(data, function (i, item) {
                var lastfollowupdate = item.LastFollowUpDate != null ? item.LastFollowUpDate : "";
                var nextfollowupdate = item.NextFollowUpDate != null ? item.NextFollowUpDate : "";
                var starttime = item.StartTime != null ? item.StartTime : "";
                var endtime = item.EndTime != null ? item.EndTime : "";
                var status = item.EnqStatus != null ? item.EnqStatus : "";
                var description = item.Description != null ? item.Description : "";

                
                var rows = '<tr role="row" class="odd">'
                      + '<td style=display:none;>' + item.FollowupId + '</td>'
                      + '<td>' + lastfollowupdate + '</td>'
                      + '<td>' + nextfollowupdate + '</td>'
                      + '<td>' + starttime + '</td>'
                      + '<td>' + endtime + '</td>'
                       + '<td>' + status + '</td>'
                        + '<td>' + description + '</td>'
                      + '<td>'
                      + '<span class="glyphicon glyphicon-edit" style="font-size: 16px;" onclick="return EditFollowup(this);" ></span>'
                      + '</td>'
                      + '</tr >';
                $('#tblEnquiry tbody').append(rows);

            });

        },
        failure: function (errMsg) {
            HideLoader();
        }
    });
}



function AddFollowup() {
    ShowLoader();
    var jsonObject = {
        EnqStatus: $('#EnqStatus option:selected').val(), FollowupId: $('#txtfollowupid').val(), LastFollowUpDate: $('#LastFollowUpDate').val(),
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
            BindFollowupDetails();
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
            
        }
    });
}

function EditFollowup(id)
{
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var FollowupId = $('#tblEnquiry tr').eq(rowIndex + 1).find('td').eq(0).html();
    $.ajax({
        cache: false,
        type: "GET",
        url: FollowupDetail,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: { FollowupId: FollowupId },
    success: function (data) {
        $.each(data, function (i, obj) {
            
                $('#txtfollowupid').val(obj.FollowupId);
                $("#EnqStatus option").each(function () {
                    if ($(this).text() == obj.EnqStatus) {
                        $(this).text() == obj.EnqStatus;
                        $(this).attr('selected', 'selected');
                    }
                });
                $('#LastFollowUpDate').val(obj.LastFollowUpDate);
                $('#NextFollowUpDate').val(obj.NextFollowUpDate);
                $('#StartTime').val(obj.StartTime);
                $('#EndTime').val(obj.EndTime);
                $('#Description').val(obj.Description);
            
        });
    },
    error: function () {
        alert("Failed! Please try again.");
    }
});
}

function RedirectEnquiry()
{
    LoadPage('/Enquiry/EditEnquiryDetails/', 'Justbok | Edit Enquiry Details'); return false;
}

var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';


function ValidateEnquiryForm() {
    var IsValid = true;
    $('#LastFollowUpDate').parent().removeClass("has-error");
    $('#LastFollowUpDate').parent().find(".help-block-error").remove();
    $('#EnqStatus').parent().removeClass("has-error");
    $('#EnqStatus').parent().find(".help-block-error").remove();
   

    if ($('#LastFollowUpDate').val() == "") {

        $('#LastFollowUpDate').parent().append(errorSpan.replace(/{{Message}}/g, "Please select follow up date"));
        $('#LastFollowUpDate').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#LastFollowUpDate').parent().removeClass("has-error");
        $('#LastFollowUpDate').parent().find(".help-block-error").remove();
    }
    

    if ($('#EnqStatus option:selected').text() == "--Select--") {

        $('#EnqStatus').parent().append(errorSpan.replace(/{{Message}}/g, "Please select status"));
        $('#EnqStatus').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#EnqStatus').parent().removeClass("has-error");
        $('#EnqStatus').parent().find(".help-block-error").remove();
    }
    return IsValid;
}

function AllowOnlyText(elementid) {
    $('#' + elementid).keydown(function (e) {
        if (e.shiftKey || e.ctrlKey || e.altKey) {
            e.preventDefault();
        } else {
            var key = e.keyCode;
            if (!((key == 8) || (key == 32) || (key == 46) || (key >= 35 && key <= 40) || (key >= 65 && key <= 90))) {
                e.preventDefault();
            }
        }
    });
}

function AllowOnlyNumbers(elementid) {
    $("#" + elementid).keydown(function (event) {
        if (event.shiftKey == true) {
            event.preventDefault();
        }
        if ((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105) || event.keyCode == 110
            || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 46 || event.keyCode == 190) {

        } else {
            event.preventDefault();
        }
        if ($(this).val().indexOf('.') !== -1 && (event.keyCode == 190 || event.keyCode == 110))
            event.preventDefault();

    });
}

AllowOnlyText("Description");


