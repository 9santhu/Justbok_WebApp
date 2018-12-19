$(document).ready(function () {
    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    $('#txtEqnuiryDate').datepicker('setDate', new Date());
    $('#btnSaveEnquiry').click(function () { if (ValidateEnquiryForm()) { SaveEnquiry(); } });
    if (enroll)
    {
        var enqId = TempData["EnquiryMemberInfo"] = EnquiryId;
        alert(enqId);
    }

    HideLoader();
});

function SaveEnquiry()
{
    
    ShowLoader();
    var jsonObject = {

        FirstName: $('#txtFirstName').val(), LastName: $('#txtLastName').val(), MobileNumber: $('#txtMobileNo').val(),
        DOB: $('#txtDob').val(), EnquiryDate: $('#txtEqnuiryDate').val(), EmailId: $('#txtEmail').val(),
        PhoneNumberResidence: $('#txtPhResidence').val(), PhoneNumberOffice: $('#txtPhOffice').val(), Gender: $('#ddlGender option:selected').val(),
        Address: $('#txtAddress').val(), Age: $('#txtAge').val(), Intention: $('#ddlIntension option:selected').val(),
        AmountOffered: $('#txtAmtOffered').val(),
        ProgramSuggested: $('#ProgramSuggested  option:selected').val(), Category: $('#ddlCategory option:selected').val(), TrailOffered: $('#ddlTrailOffered option:selected').val(),
        TrailDate: $('#txtTrailDate').val(), HowDidYouKnow: $('#ddlHowDidKNow option:selected').val(), RecievedBy: $('#ddlRepresentative option:selected').val(),
        Notes: $('#txtNotes').val(), SMS: $('#chkSMS').val(), call: $('#chkCall').val(), Email: $('#chkEmail').val(),
        BranchId: $('#ddlBranch option:selected').val()
    }

    $.ajax({
        cache: false,
        type: "POST",
        url: NewEnquiryFormUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
          
            RedirectEnquiry();
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });


}
function RedirectEnquiry() {
    LoadPage('/Followup/AddFollowup/', 'Justbok | Add Followup');
    return false;
}

function EnrollEnquiry()
{

    
}


var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';


function ValidateEnquiryForm() {
    var IsValid = true;
    $('#txtFirstName').parent().removeClass("has-error");
    $('#txtFirstName').parent().find(".help-block-error").remove();
    $('#txtLastName').parent().removeClass("has-error");
    $('#txtLastName').parent().find(".help-block-error").remove();
    $('#txtMobileNo').parent().removeClass("has-error");
    $('#txtMobileNo').parent().find(".help-block-error").remove();
    $('#txtEqnuiryDate').parent().removeClass("has-error");
    $('#txtEqnuiryDate').parent().find(".help-block-error").remove();
    $('#ddlGender').parent().removeClass("has-error");
    $('#ddlGender').parent().find(".help-block-error").remove();

    if ($('#txtFirstName').val() == "") {

        $('#txtFirstName').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter First Name"));
        $('#txtFirstName').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtFirstName').parent().removeClass("has-error");
        $('#txtFirstName').parent().find(".help-block-error").remove();
    }
    if ($('#txtLastName').val() == "") {

        $('#txtLastName').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Last Name"));
        $('#txtLastName').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtLastName').parent().removeClass("has-error");
        $('#txtLastName').parent().find(".help-block-error").remove();
    }

    if ($('#txtMobileNo').val() == "") {

        $('#txtMobileNo').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Mobile Number"));
        $('#txtMobileNo').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtMobileNo').parent().removeClass("has-error");
        $('#txtMobileNo').parent().find(".help-block-error").remove();
    }

    if ($('#txtEqnuiryDate').val() == "") {

        $('#txtEqnuiryDate').parent().append(errorSpan.replace(/{{Message}}/g, "Please Select Enquiry Date"));
        $('#txtEqnuiryDate').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtEqnuiryDate').parent().removeClass("has-error");
        $('#txtEqnuiryDate').parent().find(".help-block-error").remove();
    }

    if ($('#ddlGender option:selected').text() == "--Select--") {

        $('#ddlGender').parent().append(errorSpan.replace(/{{Message}}/g, "Please Select Gender"));
        $('#ddlGender').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#ddlGender').parent().removeClass("has-error");
        $('#ddlGender').parent().find(".help-block-error").remove();
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

AllowOnlyText("txtFirstName");
AllowOnlyText("txtLastName");
AllowOnlyText("txtNotes");

AllowOnlyNumbers("txtMobileNo");
AllowOnlyNumbers("txtPhResidence");
AllowOnlyNumbers("txtPhOffice");
AllowOnlyNumbers("txtAge");
AllowOnlyNumbers("txtAmtOffered");



