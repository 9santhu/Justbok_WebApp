var memberShipId = "";
var paymentId = "";
var paymentData = null;
var membershipdata = null;
var pageno = 1, pagerLoaded = false;

var saveMembership = true;


$(document).ready(function () {
    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });

    $('#txttransferStartDate, #txttransferEndDate, #txtFreezeStartDate, #txtFreezeEndDate').datepicker({
        dateFormat: "M dd, yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    
    ShowWebcam();

    $("#txtMembershipType").change(function () { MembershipType(); });
    //$(document).on("click", "#btnEnroll", function () { if (ValidateMembership()) { SaveMembership(); } });
    $("#btnEnroll").click(function () { if (ValidateMembership()) { SaveMembership(); } });
    //$(document).on("click", "#btnEditSaveMember", function () { if (ValidateMeberInfo()) { SaveMember(); } });
    $("#btnEditSaveMember").click(function () { if (ValidateMeberInfo()) { SaveMember(); } });
    $('#paymentSave').click(function () { if (ValidatePayment()) { SavePayment(); } });
    $(document).on('click', '.btnEditMembership', function () { EditMembership(); });
    // $(document).ready(function () { }).on('click', '.btnDeleteMembership', function () { DeleteMembership(); });
    $(document).ready(function () { }).on('click', '#btnYesPayment', function () { ConfirmDeleteMembership(); });
    // $(document).on('click', '.btnPaymentEdit', function () { EditPayment(); });
    //$(document).ready(function () { }).on('click', '.btnPaymentDelete', function () { DeletePayment(); });
    $(document).ready(function () { }).on('click', '#btnYes', function () { ConfirmDeletePayment(); });
    // $(document).on('click', '#tblPaymentHistory tr', function () { PaymentHistory(); });
    $('#printpage').click(function () { PrintOrderReceipt(); });
    //$(document).ready(function () { }).on('click', '#printpage', function () { PrintOrderReceipt(); });
    $("#txtDietPlanName").change(function () { BindDietWorkoutPlan(); });
    $("#imgInp").change(function () { readURL(this); });
    $("#btnUpload").click(function () { UploadImage(); });
    // $("#txtWorkoutPlanName").change(function () { BindWorkoutPlan(); });
    $('#addWorkout').click(function () {
        var wrapper = $("#AddworkoutContainer");
        var availableAttributes = [];
        availableAttributes = GetAutoCompleteOptions();
        AddWorkout();
        $(wrapper).find('.txtwrkt').autocomplete({
            source: availableAttributes
        });
    });
    $("#btnSaveWorkout").click(function () { SaveWorkout(); });
    $('#addNewDiet').click(function () { AddDiet(); });
    $('#btnSaveDiet').click(function () { SaveDiet(); });
    $(document).on('click', '#btnWorkoutPrint', function () { ShowLoader(); PrintWorkout(); });
    $(document).on('click', '#btnDietPrint', function () { PrintDiet(); });
    $(document).on('click', '#showSelectMemberModal', function () { ShowReferenceModal(); });
    $('#btnMeasurementSave').click(function () { UpdateMeasurements(); });
    $('#btnSaveNewMeasurements').click(function () { $('#txtMeasurementDate').datepicker().datepicker('setDate', new Date());  $('#modal_Measurement').modal('show'); });
    $('#btnClose').click(function () { $('#modal_Measurement').modal('hide'); });
    $('#PrintMeasurement').click(function () { PrintoutMeasurement(); });

    $("#txtSource").change(function () {
        var ddlSource = $("#txtSource option:selected").val();
        if (ddlSource == "Other") {
            $("#txtOther").show();
            $("#txtOther").val("");
        }
        else {
            $("#txtOther").hide();
            $("#txtOther").val("");
        }
    });

    $(document).on('click', '#tblNewMembership tr', function () {
        memberShipId = $(this).closest('tr').find('input[type="hidden"]').val();
    });
    $(document).on('click', '#btnSelectCustSearch', function () { BindSelectMember(pageno, 10); });
    BindMemberFullDetails();
    

});

//$(window).load(function () {
//    if(isNavigator)
//    {
//        console.log("true");
//    }
//});

function OnLoadBindMemberShip(membership) {
    debugger;
    $('#txtMembershipid').val("");
    membershipdata = membership;
            $('#tblNewMembership tbody').empty();
            $('#TransferMembership option').remove();
            $('#TransferMembership').append("<option value=''>---Select---</option>");
            $('#FreezeMembership option').remove();
            $('#FreezeMembership').append("<option value=''>---Select---</option>");

            $.each(membership, function (i, item) {
                var dueAmount = 0;
                var membertype = item.MemershipType.split(' ');
                if (item.PaidAmount == undefined || item.PaidAmount == null) {
                    item.PaidAmount = "-";
                    dueAmount = parseInt(item.Amount) || 0;
                }
                else {
                    var amount = parseInt(item.Amount) || 0;
                    var paidamount = parseInt(item.PaidAmount) || 0;
                    dueAmount = amount - paidamount;
                }
                if (item.Status == undefined || item.Status == null) {
                    item.Status = "-";
                }
                if (item.Note == undefined || item.Note == null) {
                    item.Note = "";
                }
                var status = '', statusclass = '', _disabled = ""

                if (item.Status == "Active") {
                    if (item.RemainingDays > 0) {
                        status = 'Active';
                        statusclass = 'label label-success';
                    }
                    else {
                        status = 'Expired';
                        statusclass = 'label label-danger';
                    }
                }
                else {
                    status = item.Status;
                    statusclass = 'label label-warning';
                    _disabled = "disabled"
                }
                var notes = "";
                if (item.Notes != null)
                {
                    notes = item.Notes;
                }
                var rowno = i + 1;
                var rows = "<tr>"
                    + "<td style='display:none'>" + '<input type="hidden" name="hid" value=' + item.MembershipID + '>' + "</td>"
                    + "<td>" + rowno + "</td>"
                    + "<td>" + membertype[0] + "</td>"
                    + "<td>" + item.StartDate + "</td>"
                    + "<td>" + item.EndDate + "</td>"
                    + "<td class='text-right'>" + item.Months + "</td>"
                    + "<td class='text-right'>" + parseFloat(item.Amount).toFixed(2) + "</td>"
                    + "<td class='text-right'><span class='label label-warning'>" + parseFloat(dueAmount).toFixed(2) + "</span></td>"
                    + "<td class='text-right'><span class='label label-success'>" + parseFloat(item.PaidAmount).toFixed(2) + "</span></td>"
                    + "<td><span class='label label-success'>" + item.Status + "</span></td>"
                    + "<td>" + notes + "</td>"
                    + "<td><a class='btn btn-info btnPayment btn-xs btn-flat' data-toggle='modal' data-target='#modal-payment' onclick='return BindPaymentData(this);'>"
                    + "<i class='fa fa-credit-card'></i>"
                    + " Payment</a > "
                    + "<a class='btn btn-primary btn-xs btn-flat btnEditMembership' onclick = 'return EditMembership(this);'>"
                    + "<i class='fa fa-edit'></i> Edit</a> "
                    + "<a class='btn btn-danger btn-xs btn-flat btnDeleteMembership' onclick = 'return DeleteMembership(this);'>"
                    + "<i class='fa fa-remove'></i> Delete</a></td>"
                    + "</tr>";
             
                $('#tblNewMembership tbody').append(rows);

                if (status.toLowerCase() == "active") {
                    $('#TransferMembership').append("<option value='" + item.MembershipID + "'>" + item.MemershipType + "</option>");
                    $('#FreezeMembership').append("<option value='" + item.MembershipID + "'>" + item.MemershipType + "</option>");
                }
                          });
      
}

function BindPaymentData(id) {
    ShowLoader();
    memberShipId = $(id).closest('tr').find('input[type="hidden"]').val();

    $.ajax({
        cache: false,
        url: BindPayment,
        data: { membershipid: memberShipId },
        type: "GET",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            paymentData = data;
            $('#tblPaymentHistory tbody').empty();
            $.each(data, function (i, item) {
                var rows = "<tr>"
                    + "<td style='display:none'>" + '<input type="hidden" name="hid" value=' + item.RecieptNumber + '>' + "</td>"
                    + "<td>" + item.PaymentDate + "</td>"
                    + "<td>" + item.PaidAmount + "</td>"
                    + "<td>" + item.PaymentType + "</td>"
                    + "<td>" + item.RecieptNumber + "</td>"
                    + "<td> <i class='fa fa-fw fa-edit btnPaymentEdit' onclick='return EditPayment(this);'></i></td>"
                    + "<td><i class='fa fa-fw fa-close btnPaymentDelete' data-toggle='modal' data-target='#modal_Conformation' onclick='return DeletePayment(this);'></i></td>"
                    + "<td><a href='' data-toggle='modal' data-target='#modal-print' onclick='return PaymentHistory(this);' ><i class='fa fa-fw fa-print'></i></a></td>"
                    + "</tr>";
                $('#tblPaymentHistory tbody').append(rows);

            });
            HideLoader();
           
        },
        error: function () {
            HideLoader();
            alert("Failed! Please try again (BindPayment).");
        },
        failure: function () { HideLoader();}
    });

}

function MembershipType() {
    var monthCount = 0;
    var ddlmembership = $("#txtMembershipType option:selected").val();
    var s = ddlmembership.split(" ");
    for (i = 0; i <= s.length; i++) {
        if (s[i] == "Month") {
            monthCount = i;
        }
    }
    var amount = s[monthCount + 1].substr(1).slice(0, -1);
    var days = s[1];
    $("#txtMonths").val(s[monthCount - 1]);
    $("#txtAmount").val(amount);
    var now = new Date();
    now.setMonth(now.getMonth() + parseInt(s[monthCount - 1]));
    $('#txtStartDate').datepicker('setDate', new Date());
    $('#txtEndate').datepicker('setDate', now);
}

function SaveMember() {
    ShowLoader();
    var jsonObject = {
        MemberID: $('#txtMemberId').val(), FirstName: $('#txtFirstName').val(), LastName: $('#txtLastName').val(), Dob: $('#txtDOB').val(),
        Email: $('#txtEmail').val(), MemberAddress: $('#txtAddress').val(), MobileNumber: $('#txtMobileno').val(), EnrollDate: $('#txtEnrollDate').val(),
        Gender: $("input[name='Gender']:checked").val(), MemberID: $('#txtMemberId').val(), MemberReference: $('#txtReferenceBy').val(),
        Representative: $('#ddlRepresentative option:selected').val(), Married: $("input[id='chkMarried']:checked").val(),
        SpouseName: $('#txtSpouseName').val(), SpouseBirthDate: $('#txtSpouseBday').val(), AnniversaryDate: $('#txtAnniversaryDate').val(),
        Occupation: $('#txtOccupation').val(), Designation: $('#txtDesignation').val(), Other: $('#txtOther').val(),
        MemberSource: $('#txtSource option:selected').val(), PhoneResidence: $('#txtPhoneResi').val(),
        PhoneOffice: $('#txtPhoneofc').val(), ReferredBy: $('#txtRefferedBy').val(), Programme: $('#txtProgramme').val(), BranchId: $('#ddlBranch option:selected').val()
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: SaveMemberUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            HideLoader();
            toastr.success("Data Saved Successfully.");
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });

}
var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';
function ValidateMeberInfo() {
    var IsValid = true;
    var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    $('#txtFirstName').parent().removeClass("has-error");
    $('#txtFirstName').parent().find(".help-block-error").remove();
    $('#txtLastName').parent().removeClass("has-error");
    $('#txtLastName').parent().find(".help-block-error").remove();
    $('#txtMobileno').parent().removeClass("has-error");
    $('#txtMobileno').parent().find(".help-block-error").remove();
    $('#txtEmail').parent().removeClass("has-error");
    $('#txtEmail').parent().find(".help-block-error").remove();

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

    if ($('#txtMobileno').val() == "") {

        $('#txtMobileno').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Mobile Number"));
        $('#txtMobileno').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtMobileno').parent().removeClass("has-error");
        $('#txtMobileno').parent().find(".help-block-error").remove();
    }

    if ($('#txtEmail').val() != "") {
        if (filter.test($('#txtEmail').val())) {

        }
        else {
            $('#txtEmail').parent().append(errorSpan.replace(/{{Message}}/g, "Not a valid format"));
            $('#txtEmail').parent().addClass("has-error");
            IsValid = false;
        }
    }


    return IsValid;
}

function ValidateMembership() {
    var IsValid = true;
    var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    $('#txtMembershipType').parent().removeClass("has-error");
    $('#txtMembershipType').parent().find(".help-block-error").remove();
    $('#txtMonths').parent().removeClass("has-error");
    $('#txtMonths').parent().find(".help-block-error").remove();

    $('#txtAmount').parent().removeClass("has-error");
    $('#txtAmount').parent().find(".help-block-error").remove();

    if ($('#txtMembershipType  option:selected').text() == "- Please select -") {

        $('#txtMembershipType').parent().append(errorSpan.replace(/{{Message}}/g, "Please select membership type"));
        $('#txtMembershipType').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtMembershipType').parent().removeClass("has-error");
        $('#txtMembershipType').parent().find(".help-block-error").remove();
    }
    if ($('#txtMonths').val() == "") {

        $('#txtMonths').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Last Name"));
        $('#txtMonths').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtMonths').parent().removeClass("has-error");
        $('#txtMonths').parent().find(".help-block-error").remove();
    }

    if ($('#txtAmount').val() == "") {

        $('#txtAmount').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Mobile Number"));
        $('#txtAmount').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtAmount').parent().removeClass("has-error");
        $('#txtAmount').parent().find(".help-block-error").remove();
    }



    return IsValid;

}

function ValidatePayment() {
    var IsValid = true;

    $('#ddlPaymentType').parent().removeClass("has-error");
    $('#ddlPaymentType').parent().find(".help-block-error").remove();
    $('#txtPaymentAmount').parent().removeClass("has-error");
    $('#txtPaymentAmount').parent().find(".help-block-error").remove();


    if ($('#ddlPaymentType  option:selected').text() == "--Select--") {

        $('#ddlPaymentType').parent().append(errorSpan.replace(/{{Message}}/g, "Please select payment type"));
        $('#ddlPaymentType').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#ddlPaymentType').parent().removeClass("has-error");
        $('#ddlPaymentType').parent().find(".help-block-error").remove();
    }
    if ($('#txtPaymentAmount').val() == "") {

        $('#txtPaymentAmount').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Amount"));
        $('#txtPaymentAmount').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtPaymentAmount').parent().removeClass("has-error");
        $('#txtPaymentAmount').parent().find(".help-block-error").remove();
    }




    return IsValid;
}

function AllowOnlyText(elementid) {
    $('#' + elementid).keydown(function (e) {
        if (e.ctrlKey || e.altKey) {
            e.preventDefault();
        } else {
            var key = e.keyCode;
            if (!((key == 8) || (key == 9) || (key == 32) || (key == 46) || (key >= 35 && key <= 40) || (key >= 65 && key <= 90))) {

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
            || event.keyCode == 8 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 46 || event.keyCode == 190) {

        } else {
            event.preventDefault();
        }
        if ($(this).val().indexOf('.') !== -1 && (event.keyCode == 190 || event.keyCode == 110))
            event.preventDefault();

    });
}


function SaveMembership() {
    ShowLoader();
    var jsonObject = {
        MembershipType: $('#txtMembershipType option:selected').val(), Months: $('#txtMonths').val(), Amount: $('#txtAmount').val(), StartDate: $('#txtStartDate').val(), Enddate: $('#txtEndate').val(),
        Note: $('#txtNote').val(), Status: "Active", MembershipId: $('#txtMembershipid').val(), MemberID: $('#txtMemberId').val()
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: AddMemberShipUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            onsuccess();
            $('#txtMembershipid').val("");
            HideLoader();
            toastr.success("Data Saved Successfully.");
            if (saveMembership)
            {
                SendMembershipSMS();
            }
            
            //alert("Data Submitted");

        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });

}

function SendMembershipSMS()
{
    $.ajax({
        cache: false,
        type: "Get",
        url: GetMobileNumberUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { RequestType: "EditMembership" },
        success: function (data) {
            var msg = "";
            var mobileNumber = data[0].Mobile;
            var memberName = data[0].MemberName;
            var gender = data[0].GenderDetails;
            var enrollDate = data[0].EnrollDate;
            var membership = data[0].MembershipName;
            var gymName=data[0].GymName;
            var prefix = "";
            if (gender == "Male") {
                prefix="Mr."
            }
            else {
                prefix="Miss."
            }
            msg = "Dear " + prefix + memberName + ", your " + membership + " at " + gymName + "," + " on " + enrollDate + " registered."
            if (mobileNumber != "" && msg != "")
            {
                SendSMS(mobileNumber, msg);
            }
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });
         

}

function SavePayment() {
    var jsonObject = {
        PaymentType: $('#ddlPaymentType option:selected').val(), PaymentAmount: $('#txtPaymentAmount').val(), PaymentDate: $('#txtPaymentDate').val(),
        PaymentDueDate: $('#txtDuedate').val(), RecieptNumber: $('#txtRecieptNumber').val(), ReferenceNumber: $('#txtReferenceNo').val(), MembershipId: memberShipId
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: Payment,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            $('#modal-payment').modal('hide');

            onsuccess()
            onPaymentsuccess();
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });

}

function onsuccess() {
    $.ajax({
        cache: false,
        url: BindMembershipUrl,
        data: "",
        type: "GET",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            membershipdata = data;
            $('#tblNewMembership tbody').empty();
            $('#TransferMembership option').remove();
            $('#TransferMembership').append("<option value=''>---Select---</option>");
            $('#FreezeMembership option').remove();
            $('#FreezeMembership').append("<option value=''>---Select---</option>");

            $.each(data, function (i, item) {
                var dueAmount = 0;
                var membertype = item.MemershipType.split(' ');
                if (item.PaidAmount == undefined || item.PaidAmount == null) {
                    item.PaidAmount = "-";
                    dueAmount = parseInt(item.Amount) || 0;
                }
                else {
                    var amount = parseInt(item.Amount) || 0;
                    var paidamount = parseInt(item.PaidAmount) || 0;
                    dueAmount = amount - paidamount;
                }
                if (item.Status == undefined || item.Status == null) {
                    item.Status = "-";
                }
                if (item.Note == undefined || item.Note == null) {
                    item.Note = "";
                }

                //Added Code From OnlOad 
                if (item.Status == "Active") {
                    if (item.RemainingDays > 0) {
                        status = 'Active';
                        statusclass = 'label label-success';
                    }
                    else {
                        status = 'Expired';
                        statusclass = 'label label-danger';
                    }
                }
                else {
                    status = item.Status;
                    statusclass = 'label label-warning';
                    _disabled = "disabled"
                }

                // Ends here 

                var notes = "";
                if (item.Notes != null) {
                    notes = item.Notes;
                }

                var rowno = i + 1;
                var rowno = i + 1;
                var rows = "<tr>"
                    + "<td style='display:none'>" + '<input type="hidden" name="hid" value=' + item.MembershipID + '>' + "</td>"
                    + "<td>" + rowno + "</td>"
                    + "<td>" + membertype[0] + "</td>"
                    + "<td>" + item.StartDate + "</td>"
                    + "<td>" + item.EndDate + "</td>"
                    + "<td class='text-right'>" + item.Months + "</td>"
                    + "<td class='text-right'>" + parseFloat(item.Amount).toFixed(2) + "</td>"
                    + "<td class='text-right'><span class='label label-warning'>" + parseFloat(dueAmount).toFixed(2) + "</span></td>"
                    + "<td class='text-right'><span class='label label-success'>" + parseFloat(item.PaidAmount).toFixed(2) + "</span></td>"
                    + "<td><span class='label label-success'>" + item.Status + "</span></td>"
                    + "<td>" + notes + "</td>"
                    + "<td><a class='btn btn-info btnPayment btn-xs btn-flat' data-toggle='modal' data-target='#modal-payment' onclick='return BindPaymentData(this);'>"
                    + "<i class='fa fa-credit-card'></i>"
                    + " Payment</a > "
                    + "<a class='btn btn-primary btn-xs btn-flat btnEditMembership' onclick = 'return EditMembership(this);'>"
                    + "<i class='fa fa-edit'></i> Edit</a> "
                    + "<a class='btn btn-danger btn-xs btn-flat btnDeleteMembership' onclick = 'return DeleteMembership(this);'>"
                    + "<i class='fa fa-remove'></i> Delete</a></td>"
                    + "</tr>";


                //var rows = "<tr>"
                //    + "<td style='display:none'>" + '<input type="hidden" name="hid" value=' + item.MembershipID + '>' + "</td>"
                //    + "<td>" + rowno + "</td>"
                //    + "<td>" + membertype[0] + "</td>"
                //    + "<td>" + item.StartDate + "</td>"
                //    + "<td>" + item.EndDate + "</td>"
                //    + "<td class='text-right'>" + item.Months + "</td>"
                //    + "<td class='text-right'>" + parseFloat(item.Amount).toFixed(2) + "</td>"
                //    + "<td class='text-right'><span class='badge bg-red'>" + dueAmount + "</span></td>"
                //    + "<td class='text-right'><span class='badge bg-green'>" + item.PaidAmount + "</span></td>"
                //    + "<td><span class='badge bg-green'>" + item.Status + "</span></td>"
                //    + "<td>" + item.Note + "</td>"
                //    //+ "<td><a class='btn btn-info btnPayment' data-toggle='modal' data-target='#modal-payment' onclick='return BindPaymentData(this);'>Payment</a>&nbsp;<a class='btn btn-primary btnEditMembership' onclick='return EditMembership(this);>Edit</a>&nbsp;<a class='btn btn-danger btnDeleteMembership' onclick='return DeleteMembership(this);'>Delete</a></td>"
                //      + "<td><a class='btn btn-info btnPayment btn-xs btn-flat' data-toggle='modal' data-target='#modal-payment' onclick='return BindPaymentData(this);'>"
                //    + "<i class='fa fa-credit-card'></i>"
                //    + " Payment</a > "
                //    + "<a class='btn btn-primary btn-xs btn-flat btnEditMembership' onclick = 'return EditMembership(this);'>"
                //    + "<i class='fa fa-edit'></i> Edit</a> "
                //    + "<a class='btn btn-danger btn-xs btn-flat btnDeleteMembership' onclick = 'return DeleteMembership(this);'>"
                //    + "<i class='fa fa-remove'></i> Delete</a></td>"
                //    + "</tr>";
                $('#tblNewMembership tbody').append(rows);
            });
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });
}

function onPaymentsuccess() {
    $.ajax({
        cache: false,
        url: BindPayment,
        data: { membershipid: memberShipId },
        type: "GET",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            paymentData = data;
            $('#tblPaymentHistory tbody').empty();
            $.each(data, function (i, item) {
                var rows = "<tr>"
                    + "<td style='display:none'>" + '<input type="hidden" name="hid" value=' + item.RecieptNumber + '>' + "</td>"
                    + "<td>" + item.PaymentDate + "</td>"
                    + "<td>" + item.PaidAmount + "</td>"
                    + "<td>" + item.PaymentType + "</td>"
                    + "<td>" + item.RecieptNumber + "</td>"
                    + "<td> <i class='fa fa-fw fa-edit btnPaymentEdit' onclick='return EditPayment(this);'></i></td>"
                    + "<td><i class='fa fa-fw fa-close btnPaymentDelete' data-toggle='modal' data-target='#modal_Conformation' onclick='return DeletePayment(this);'></i></td>"
                    + "<td><a href='' data-toggle='modal' data-target='#modal-print' onclick='return PaymentHistory(this);' ><i class='fa fa-fw fa-print'></i></a></td>"
                    + "</tr>";
                $('#tblPaymentHistory tbody').append(rows);
            });

            $('#modal-payment').modal('toggle');
            $('.nav-tabs a[href="#payment-history"]').tab('show');
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });
}

function EditMembership(id) {

    saveMembership = false;
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var membershipId = $('#tblNewMembership tr').eq(rowIndex + 1).find('td').eq(0).find('input[type="hidden"]').val();
    //if (membershipdata != null && membershipId != null) {
    //    $.each(membershipdata, function (i, obj) {
    //        if (obj.MembershipID == membershipId) {
    //            $('#txtMembershipid').val(obj.MembershipID);

    //            $("#txtMembershipType option").each(function () {
    //                if ($(this).text() == obj.MemershipType) {
    //                    $(this).prop("selected", true);
    //                }
    //            });
    //            $('#txtMonths').val(obj.Months);
    //            $('#txtAmount').val(obj.Amount);
    //            //$('#txtAmount').val(obj.Amount);
    //            $('#txtStartDate').val(obj.StartDate);
    //            $('#txtEndate').val(obj.EndDate);
    //            $('#txtNote').val(obj.Note);
    //        }
    //    });
    //}
    //else {
        $.ajax({
            cache: false,
            type: "GET",
            url: EditMembershipUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: { membershipId: membershipId },
            success: function (data) {
                $.each(data, function (i, obj) {
                    if (obj.MembershipID == membershipId) {
                        $('#txtMembershipid').val(obj.MembershipID);
                        $("#txtMembershipType option").each(function () {
                            if ($(this).text() == obj.MemershipType) {
                                $(this).prop("selected", true)
                            }
                        });
                        $('#txtMonths').val(obj.Months);
                        $('#txtAmount').val(obj.Amount);
                        $('#txtAmount').val(obj.Amount);
                        $('#txtStartDate').val(obj.StartDate);
                        $('#txtEndate').val(obj.EndDate);
                        $('#txtNote').val(obj.Note);
                    }
                });
            },
            error: function () {
                alert("Failed! Please try again.");
            }
        });
    //}

}

function DeleteMembership(id) {
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var membershipId = $('#tblNewMembership tr').eq(rowIndex + 1).find('td').eq(0).find('input[type="hidden"]').val();
    $('#txtMembershipid').val(membershipId);
    $('#modal_ConformationPayment').modal('show');
}

function ConfirmDeleteMembership() {
    $('#modal_ConformationPayment').modal('hide');
    membershipid = $('#txtMembershipid').val();
    $('#txtMembershipid').val("");
    $.ajax({
        cache: false,
        type: "POST",
        url: DeleteMembershipUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ MembershipId: membershipid }),
        success: function (data) {
            onsuccess();
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });
}

function EditPayment(id) {
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var RecieptNo = $('#tblPaymentHistory tr').eq(rowIndex + 1).find('td').eq(4).html();
    if (paymentData != null && RecieptNo != null) {
        $.each(paymentData, function (i, obj) {
            if (obj.RecieptNumber == RecieptNo) {
                $("#ddlPaymentType option").each(function () {
                    if ($(this).text() == obj.PaymentType) {
                        $(this).text() == obj.PaymentType;
                        $(this).attr('selected', 'selected');
                    }
                });
                $('#txtPaymentAmount').val(obj.PaidAmount);
                $('#txtPaymentDate').val(obj.PaymentDate);
                $('#txtDuedate').val(obj.PaymentDueDate);
                $('#txtRecieptNumber').val(obj.RecieptNumber);
                $('.nav-tabs a[href="#add-payment"]').tab('show');
            }
        });
    }
    else {
        $.ajax({
            cache: false,
            url: BindPayment,
            data: { membershipid: memberShipId },
            type: "GET",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                paymentData = data;
                $.each(paymentData, function (i, obj) {
                    if (obj.RecieptNumber == RecieptNo) {
                        //select dropdown payment type
                        $("#ddlPaymentType option").each(function () {
                            if ($(this).text() == obj.PaymentType) {
                                $(this).text() == obj.PaymentType;
                                $(this).attr('selected', 'selected');
                            }
                        });
                        //payment amount 
                        $('#txtPaymentAmount').val(obj.PaidAmount);
                        $('#txtPaymentDate').val(obj.PaymentDate);
                        $('#txtDuedate').val(obj.PaymentDueDate);
                        $('#txtRecieptNumber').val(obj.RecieptNumber);
                        $('.nav-tabs a[href="#add-payment"]').tab('show');
                    }
                });
            },

            error: function () {
                alert("Failed! Please try again.");
            }
        });
    }
}

function DeletePayment(id) {

    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var RecieptNo = $('#tblPaymentHistory tr').eq(rowIndex + 1).find('td').eq(4).html();

    $('#txtPaymentRecieptNo').val(RecieptNo);
}

function ConfirmDeletePayment() {
    $('#modal_Conformation').modal('hide');
    recieptNumber = $('#txtPaymentRecieptNo').val()
    $.ajax({
        cache: false,
        type: "GET",
        url: DeletePaymentUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { RecieptNumber: recieptNumber },
        success: function (data) {
            onPaymentsuccess();
            onsuccess();
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function PaymentHistory(id) {
    paymentId = $(id).closest('tr').find('input[type="hidden"]').val();
    $.ajax({
        cache: false,
        url: GetInvoice,
        data: { membershipid: memberShipId, receiptnumber: paymentId },
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var rate = data.Amount / 1.15;
            var taxamount = data.Amount - rate;
            var gstamt = taxamount / 2;
            var gstRate = 15 / 2;
            $("#printcustomerName").text(data.FirstName);
            $("#address").text(data.Address);
            $("#package").text(data.Package);
            $("#startdate").text(data.StartDate);
            $("#enddate").text(data.EndDate);
            $("#totalAmount").text(data.Amount);
            $("#rate").text(rate.toFixed(2));
            $("#Total").text(rate.toFixed(2));
            $("#taxableValue").text(rate.toFixed(2));
            $("#cgstrate").text(gstRate);
            $("#cgstamount").text(gstamt.toFixed(2));
            $("#sgstrate").text(gstRate);
            $("#sgstamount").text(gstamt.toFixed(2));
        },
        error: function () {
            alert("Failed! Please try again. PaymentHistory");
        }
    });
}

function PrintOrderReceipt() {
  
    var printelem = document.getElementById('divMembership');
    var myWindow = window.open('', '', 'width=750,height=500', '_blank');
    myWindow.document.write(document.head.innerHTML);
    myWindow.document.write(printelem.innerHTML);
    myWindow.document.write("\x3Cscript>window.print(); window.close();\x3C/script>");
}



function BindDietPlan() {
    $.ajax({
        url: GetDietPlanList,
        data: { BranchId: $('#ddlBranch option:selected').val() },
        type: "GET",
        dataType: "json",
        success: function (data) {
            $.each(data, function (i, item) {
                $("#txtDietPlanName").append($("<option></option>").val(item.PlaneNameId).html(item.PlanName));
            })
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function BindDietWorkoutPlan() {

    PlanId = $("#txtDietPlanName option:selected").val();
    if (PlanId != "---Select Diet---") {

        $.ajax({
            cache: false,
            url: BindDietWorkoutPlanUrl,
            contentType: "application/json; charset=utf-8",
            data: { palnid: PlanId },
            type: "GET",
            dataType: "json",
            success: function (data) {
                $('#tblDietPlan tbody').empty();
                $.each(data, function (i, item) {
                    var rows = "<tr>"
                        + "<td style='display:none;'> <input type='Text' class='form-control'   id=txtdietplanid" + i + "></td>"
                        + "<td style='display:none'> <input type='Text' class='form-control'   id=DietId" + i + "></td>"
                        + "<td> <input type='Text' class='form-control' readonly id=txtMealTime" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtMonday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtTuesday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtWednesday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtThursday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtFriday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtSaturday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtSunday" + i + "></td>"
                        + "</tr>";
                    $('#tblDietPlan tbody').append(rows);
                    $('#txtdietplanid' + i + '').val(item.DietPlanId);
                    $('#DietId' + i + '').val(item.DietId);
                    $('#txtMealTime' + i + '').val(item.DietTime);
                    $('#txtMonday' + i + '').val(item.MondayDiet);
                    $('#txtTuesday' + i + '').val(item.TuesdayDiet);
                    $('#txtWednesday' + i + '').val(item.WednesdayDiet);
                    $('#txtThursday' + i + '').val(item.ThursdayDiet);
                    $('#txtFriday' + i + '').val(item.FridayDiet);
                    $('#txtSaturday' + i + '').val(item.SaturdayDiet);
                    $('#txtSunday' + i + '').val(item.SundayDiet);
                })
            },
            failure: function (errMsg) {
                alert(errMsg.responseText);
            }
        });
    }
    else {
        $('#tblDietPlan tbody').empty();
    }

}

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#userpic').attr('src', e.target.result);

        }
        reader.readAsDataURL(input.files[0]);
    }
}

function UploadImage() {
    var formData = new FormData();
    var totalFiles = document.getElementById("imgInp").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("imgInp").files[i];
        formData.append("imageUploadForm", file);
    }
    $.ajax({
        type: "POST",
        url: UpdateUploadImageUrl,
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {
            if (response == "success") {
                toastr.success("Image Saved Successfully.");
                //alert('Image saved successfully!!');
            }
            else {
                alert(response);
            }

        },
        error: function (error) {
            alert("error");
        }
    });
}

function UpdateMemberInfoImage(image) {
    $.each(image, function (i, item) {
        $("#customerimage").attr('src', item.ImageData);
    });
}

function UpdateHeaderMembership(details) {
   
    var dueAmount = 0;
    if (details.PaymentAmount == undefined || details.PaymentAmount == null) {
        details.PaymentAmount = "-";
        dueAmount = parseInt(details.Amount) || 0;
            }
    else {
        var amount = parseInt(details.Amount) || 0;
        var paidamount = parseInt(details.PaymentAmount) || 0;
                dueAmount = amount - paidamount;
            }
    if (details.MembershipType != null) {
        var membertype = details.MembershipType.split(' ');
                if (membertype[1] == "1") {
                    $("#subscriptiontime").text(membertype[1] + " Month");
                }
                else {
                    $("#subscriptiontime").text(membertype[1] + " Months");
                }
            }

            $("#expires").text(details.EndDate);
            $("#dueamount").text(dueAmount);
}

function BindPlan() {
    $.ajax({
        url: GetPlanList,
        data: { BranchId: $('#ddlBranch option:selected').val() },
        type: "GET",
        dataType: "json",
        success: function (data) {
            $.each(data, function (i, item) {
                $("#txtWorkoutPlanName").append($("<option></option>").val(item.PlaneNameId).html(item.PlanName));
            })
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function EditMemberWorkoutPlan(workout) {
  
            $('#AddworkoutContainer').empty();
            var wrapper = $("#AddworkoutContainer");
            var availableAttributes = [];
            availableAttributes = GetAutoCompleteOptions();
            $('#txtWorkoutPlanName').val(workout[0].PlaneNameId);
            if ($('#txtWorkoutPlanName').val() == null)
            {
                var length = $('#txtWorkoutPlanName > option').length;
                if (length < 2)
                {
                    BindPlan();
                    $('#txtWorkoutPlanName').val(workout[0].PlaneNameId);
                }
                //$('#txtWorkoutPlanName').val();
                
            }
           
            $.each(workout, function (i, item) {
                var workouts = '<div class="row">'
                    + '<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control planid"  id="txtplanenameid' + i + '"></div></div>'
                    + '<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control"  id="txtMemberworkoutid' + i + '"></div></div>'
                    + '<div class="form-group col-md-2"><label>Workout</label><div class="input-group"><input type="Text" class="form-control txtwrkt"  id="txtworkout' + i + '"></div></div>'
                    + '<div class="form-group col-md-2"><label>Sets</label><div class="input-group"><input type="Text" class="form-control" id="txtsets' + i + '"/><div class="input-group-btn"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Sets <span class="caret"></span></button><ul class="dropdown-menu pull-right"><li><a onclick="return Sets(this);">Sets</a></li><li><a onclick="return Minutes(this);">Minutes</a></li></ul></div></div></div>'
                    + '<div class="form-group col-md-2"><label>Repeats</label><input type="Text" class="form-control" id="txtRepeats' + i + '"/></div>'
                    + '<div class="form-group col-md-2"><label>Days</label><select  class="form-control" id="txtdays' + i + '"><option>Mon-Tue-Wed-Thu-Fri-Sat</option><option>Mon-Wed-Fri</option><option>Mon-Wed</option><option>Mon-Thu</option><option>Mon</option><option>Wed</option><option>Fri</option><option>Tue-Thu-Sat</option><option>Tue-Fri</option><option>Wed-Sat</option><option>Tue</option><option>Thu</option><option>Sat</option><option>Sun</option></select></div>'
                    + '<div class="form-group col-md-2"><label>Exercise Order</label><input type="Text" class="form-control" id="txtExcercise' + i + '"/></div>'
                    + '<div class="form-group col-md-2"><label>Delete</label><span class="form-control" style="border-style:none;"> <a class="text-danger btndeleteWorkout" onclick="return DeleteMemberWorkout(this);"><i class="fa fa-remove" aria-hidden="true"></i></a></span></div></div>';
                $("#AddworkoutContainer").append(workouts);
                $('#txtplanenameid' + i + '').val(item.PlaneNameId);
                $('#txtMemberworkoutid' + i + '').val(item.MemberWorkoutPlanid);
                $('#txtworkout' + i + '').val(item.Workout);
                //$('#txtsets' + i + '').val(item.NumberOfSets);
                $('#txtRepeats' + i + '').val(item.Repeats);
                $('#txtdays' + i + '').val(item.NumberofDays);
                $('#txtExcercise' + i + '').val(item.ExcerciseOrder)
                if (item.NumberOfSets != null) {
                    $('#txtsets' + i + '').val(item.NumberOfSets);
                }
                else if (item.NumberOfMinutes != null) {
                    $('#txtsets' + i + '').val(item.NumberOfMinutes);
                    // var setMinValue = $('#txtsets' + i + '').parent().find("button").text();
                    $('#txtsets' + i + '').parent().find("button").text("");
                    $('#txtsets' + i + '').parent().find("button").append("Minutes" + " <span class='caret'></span>");
                    $('#txtsets' + i + '').parent().parent().find("label").text("Minutes");
                    $('#txtsets' + i + '').parent().parent().next().find("label").hide();
                    $('#txtsets' + i + '').parent().parent().next().find("input").hide();
                }
            })
            $(wrapper).find('.txtwrkt').autocomplete({
                source: availableAttributes
            });

}

function DeleteMemberWorkout(id) {
    var $curr = $(id).closest('div');
    $curr = $curr.parent();
    $curr.remove();
}

function BindWorkoutPlan() {
    PlanId = $("#txtWorkoutPlanName option:selected").val();

    if (PlanId != "---Select Workout---") {
        $.ajax({
            cache: false,
            type: "GET",
            url: BindWorkoutPlanUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: { palnid: PlanId},
            success: function (data) {
                $('#AddworkoutContainer').empty();
                var wrapper = $("#AddworkoutContainer");
                var availableAttributes = [];
                availableAttributes = GetAutoCompleteOptions();
                $.each(data, function (i, item) {

                    //$("#AddworkoutContainer").append('<div class="form-group col-md-2" style="display:none;"><label>PlanNameId</label><div class="input-group"><input type="Text" class="form-control"  id="txtplanenameid' + i + '"></div>');
                    //$("#AddworkoutContainer").append('<div class="form-group col-md-2" style="display:none;"><label>PlanId</label><div class="input-group"><input type="Text" class="form-control"  id="txtplaneid' + i + '"></div>');
                    //$("#AddworkoutContainer").append('<div class="form-group col-md-2" style="display:none;"><label>MemberWorkPlanId</label><div class="input-group"><input type="Text" class="form-control"  id="txtmemberworkoutplanid' + i + '"></div>');
                    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Workout</label><div class="input-group"><input type="Text" class="form-control"  id="txtworkout' + i + '"></div>');
                    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Sets</label><div class="input-group"><input type="Text" class="form-control" id="txtsets' + i + '"/><div class="input-group-btn"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Sets <span class="caret"></span></button><ul class="dropdown-menu pull-right"><li><a href="#">Sets</a></li><li><a href="#">Minutes</a></li></ul></div></div></div>');
                    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Repeats</label><input type="Text" class="form-control" id="txtRepeats' + i + '"/></div>');
                    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Days</label><select  class="form-control" id="txtdays' + i + '"><option>Mon-Tue-Wed-Thu-Fri-Sat</option><option>Mon-Wed-Fri</option><option>Mon-Wed</option><option>Mon-Thu</option><option>Mon</option><option>Wed</option><option>Fri</option><option>Tue-Thu-Sat</option><option>Tue-Fri</option><option>Wed-Sat</option><option>Tue</option><option>Thu</option><option>Sat</option><option>Sun</option></div>');
                    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Exercise Order</label><input type="Text" class="form-control" id="txtExcercise' + i + '"/></div>');
                    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Delete</label><span class="form-control" style="border-style:none;"> <a class="text-danger btndeleteWorkout"><i class="fa fa-remove" aria-hidden="true"></i></a></span></div>');

                    var workouts = '<div class="row">'
                   + '<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control planid"  id="txtplanenameid' + i + '"></div></div>'
                   + '<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control"  id="txtMemberworkoutid' + i + '"></div></div>'
                   + '<div class="form-group col-md-2"><label>Workout</label><div class="input-group"><input type="Text" class="form-control txtwrkt"  id="txtworkout' + i + '"></div></div>'
                   + '<div class="form-group col-md-2"><label>Sets</label><div class="input-group"><input type="Text" class="form-control" id="txtsets' + i + '"/><div class="input-group-btn"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Sets <span class="caret"></span></button><ul class="dropdown-menu pull-right"><li><a  onclick="return Sets(this);">Sets</a></li><li><a onclick="return Minutes(this);">Minutes</a></li></ul></div></div></div>'
                   + '<div class="form-group col-md-2"><label>Repeats</label><input type="Text" class="form-control" id="txtRepeats' + i + '"/></div>'
                   + '<div class="form-group col-md-2"><label>Days</label><select  class="form-control" id="txtdays' + i + '"><option>Mon-Tue-Wed-Thu-Fri-Sat</option><option>Mon-Wed-Fri</option><option>Mon-Wed</option><option>Mon-Thu</option><option>Mon</option><option>Wed</option><option>Fri</option><option>Tue-Thu-Sat</option><option>Tue-Fri</option><option>Wed-Sat</option><option>Tue</option><option>Thu</option><option>Sat</option><option>Sun</option></select></div>'
                   + '<div class="form-group col-md-2"><label>Exercise Order</label><input type="Text" class="form-control" id="txtExcercise' + i + '"/></div>'
                   + '<div class="form-group col-md-2"><label>Delete</label><span class="form-control" style="border-style:none;"> <a class="text-danger btndeleteWorkout" onclick="return DeleteMemberWorkout(this);"><i class="fa fa-remove" aria-hidden="true"></i></a></span></div></div>';

                    $("#AddworkoutContainer").append(workouts);

                    $('#txtplanenameid' + i + '').val(item.PlaneNameId);
                    $('#txtplaneid' + i + '').val(item.Planid);
                    $('#txtmemberworkoutplanid' + i + '').val(item.txtmemberworkoutplanid);
                    $('#txtworkout' + i + '').val(item.Workout);
                    //$('#txtsets' + i + '').val(item.NumberOfSets);
                    $('#txtRepeats' + i + '').val(item.Repeats);
                    $('#txtdays' + i + '').val(item.NumberofDays);
                    $('#txtExcercise' + i + '').val(item.ExcerciseOrder)

                    if (item.NumberOfSets != null) {
                        $('#txtsets' + i + '').val(item.NumberOfSets);

                    }
                    else if (item.NumberOfMinutes != null) {
                        $('#txtsets' + i + '').val(item.NumberOfMinutes);
                        // var setMinValue = $('#txtsets' + i + '').parent().find("button").text();
                        $('#txtsets' + i + '').parent().find("button").text("");
                        $('#txtsets' + i + '').parent().find("button").append("Minutes" + " <span class='caret'></span>");
                        $('#txtsets' + i + '').parent().parent().find("label").text("Minutes");
                        $('#txtsets' + i + '').parent().parent().next().find("label").hide();
                        $('#txtsets' + i + '').parent().parent().next().find("input").hide();
                    }

                })
                $(wrapper).find('.txtwrkt').autocomplete({
                    source: availableAttributes
                });
            },
            failure: function (errMsg) {
                alert(errMsg.responseText);
            }
        });

    }
    else {
        $('#AddworkoutContainer').empty();
    }
}

function AddWorkout() {
    var i = $("#AddworkoutContainer select").length;



    //$("#AddworkoutContainer").append('<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control"  id="txtplanenameid' + i + '"></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control"  id="txtplaneid' + i + '"></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Workout</label><div class="input-group"><input type="Text" class="form-control"  id="txtworkout' + i + '"></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Sets</label><div class="input-group"><input type="Text" class="form-control" id="txtsets' + i + '"/><div class="input-group-btn"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Sets <span class="caret"></span></button><ul class="dropdown-menu pull-right"><li><a href="#">Sets</a></li><li><a href="#">Minutes</a></li></ul></div></div></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Repeats</label><input type="Text" class="form-control" id="txtRepeats' + i + '"/></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Days</label><select  class="form-control" id="txtdays' + i + '"><option>Mon-Tue-Wed-Thu-Fri-Sat</option><option>Mon-Wed-Fri</option><option>Mon-Wed</option><option>Mon-Thu</option><option>Mon</option><option>Wed</option><option>Fri</option><option>Tue-Thu-Sat</option><option>Tue-Fri</option><option>Wed-Sat</option><option>Tue</option><option>Thu</option><option>Sat</option><option>Sun</option></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Exercise Order</label><input type="Text" class="form-control" id="txtExcercise' + i + '"/></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Delete</label><span class="form-control" style="border-style: none;"> <a class="text-danger btndeleteWorkout"><i class="fa fa-remove" aria-hidden="true"></i></a></span></div>');

    var workouts = '<div class="row">'
    + '<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control"  id="txtplanenameid' + i + '"></div></div>'
    + '<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control"  id="txtplaneid' + i + '"></div></div>'
    + '<div class="form-group col-md-2"><label>Workout</label><div class="input-group"><input type="Text" class="form-control txtwrkt"  id="txtworkout' + i + '"></div></div>'
    + '<div class="form-group col-md-2"><label>Sets</label><div class="input-group"><input type="Text" class="form-control" id="txtsets' + i + '"/><div class="input-group-btn"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Sets <span class="caret"></span></button><ul class="dropdown-menu pull-right"><li><a  onclick="return Sets(this);">Sets</a></li><li><a onclick="return Minutes(this);">Minutes</a></li></ul></div></div></div>'
    + '<div class="form-group col-md-2"><label>Repeats</label><input type="Text" class="form-control" id="txtRepeats' + i + '"/></div>'
    + '<div class="form-group col-md-2"><label>Days</label><select  class="form-control" id="txtdays' + i + '"><option>Mon-Tue-Wed-Thu-Fri-Sat</option><option>Mon-Wed-Fri</option><option>Mon-Wed</option><option>Mon-Thu</option><option>Mon</option><option>Wed</option><option>Fri</option><option>Tue-Thu-Sat</option><option>Tue-Fri</option><option>Wed-Sat</option><option>Tue</option><option>Thu</option><option>Sat</option><option>Sun</option></select></div>'
    + '<div class="form-group col-md-2"><label>Exercise Order</label><input type="Text" class="form-control" id="txtExcercise' + i + '"/></div>'
    + '<div class="form-group col-md-2"><label>Delete</label><span class="form-control" style="border-style: none;"> <a class="text-danger btndeleteWorkout"><i class="fa fa-remove" aria-hidden="true"></i></a></span></div>'
    + '</div>'

    $("#AddworkoutContainer").append(workouts);

    i++;
}

function SaveWorkout() {
    ShowLoader();
    var count = $("#AddworkoutContainer select").length;
    var jsonObj = [];
    if ($("#txtMemberworkoutid0") != null) {
        for (i = 0; i < count; i++) {
            var setMinValue = $('#txtsets' + i + '').parent().find("button").text();
            jsonObj.push(
                { "Planid": $('#txtplaneid' + i + '').val(), "PlaneNameId": $('#txtplanenameid' + i + '').val(), "MemberWorkoutPlanid": $('#txtMemberworkoutid' + i + '').val(), "Workout": $('#txtworkout' + i + '').val(), "NumberOfSets": $('#txtsets' + i + '').val(), "Repeats": $('#txtRepeats' + i + '').val(), "NumberofDays": $('#txtdays' + i + '').val(), "ExcerciseOrder": $('#txtExcercise' + i + '').val(),"SetMin": setMinValue  }
            );
        }
    }
    else {
        for (i = 0; i <= count; i++) {
            var setMinValue = $('#txtsets' + i + '').parent().find("button").text();
            jsonObj.push(
                { "Planid": $('#txtplaneid' + i + '').val(), "PlaneNameId": $('#txtplanenameid' + i + '').val(), "Workout": $('#txtworkout' + i + '').val(), "NumberOfSets": $('#txtsets' + i + '').val(), "Repeats": $('#txtRepeats' + i + '').val(), "NumberofDays": $('#txtdays' + i + '').val(), "ExcerciseOrder": $('#txtExcercise' + i + '').val(), "SetMin": setMinValue }
            );
        }
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: UpdateWorkout,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObj),
        success: function (data) {
            HideLoader();
            toastr.success("Data Saved Successfully.");
           // alert("Data Submitted");
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function EditMemberDietPlan(diet) {
            $('#tblDietPlan tbody').empty();
            if (diet != null)
            {
                $('#txtDietPlanName').val(diet[0].DietPlanId);
                if ($('#txtDietPlanName').val() == null) {
                    var length = $('#txtDietPlanName > option').length;
                    if (length < 2) {
                        BindDietPlan();
                        $('#txtDietPlanName').val(data[0].DietPlanId);
                    }
                }
                $.each(diet, function (i, item) {
                    var rows = "<tr>"
                        + "<td  style='display:none;'> <input type='Text' class='form-control' id=txtMemberdietplan" + i + "></td>"
                        + "<td  style='display:none;'> <input type='Text' class='form-control' id=txtdietplanid" + i + "></td>"
                        + "<td> <input type='Text' class='form-control' readonly id=txtMealTime" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtMonday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtTuesday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtWednesday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtThursday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtFriday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtSaturday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtSunday" + i + "></td>"
                        + "</tr>";

                    $('#tblDietPlan tbody').append(rows);
                    $('#txtMemberdietplan' + i + '').val(item.MemberDietPlanid);
                    $('#txtdietplanid' + i + '').val(item.DietPlanId);
                    $('#txtMealTime' + i + '').val(item.DietTime);
                    $('#txtMonday' + i + '').val(item.MondayDiet);
                    $('#txtTuesday' + i + '').val(item.TuesdayDiet);
                    $('#txtWednesday' + i + '').val(item.WednesdayDiet);
                    $('#txtThursday' + i + '').val(item.ThursdayDiet);
                    $('#txtFriday' + i + '').val(item.FridayDiet);
                    $('#txtSaturday' + i + '').val(item.SaturdayDiet);
                    $('#txtSunday' + i + '').val(item.SundayDiet);
                })
            }
}

function AddDiet() {
    var i = $('#tblDietPlan tr').length - 1;
    var rows = "<tr>"
        //+ "<td  style='display:none;'> <input type='Text' class='form-control' id=txtdietid" + i + "></td>"
        + "<td  style='display:none;'> <input type='Text' class='form-control' id=txtdietplanid" + i + "></td>"
        + "<td> <input type='Text' class='form-control'  id=txtMealTime" + i + "></td>"
        + "<td> <input type='Text' class='form-control'  id=txtMonday" + i + "></td>"
        + "<td> <input type='Text' class='form-control'  id=txtTuesday" + i + "></td>"
        + "<td> <input type='Text' class='form-control'  id=txtWednesday" + i + "></td>"
        + "<td> <input type='Text' class='form-control'  id=txtThursday" + i + "></td>"
        + "<td> <input type='Text' class='form-control'  id=txtFriday" + i + "></td>"
        + "<td> <input type='Text' class='form-control'  id=txtSaturday" + i + "></td>"
        + "<td> <input type='Text' class='form-control'  id=txtSunday" + i + "></td>"
        + "</tr>";
    $('#tblDietPlan tbody').append(rows);
}

function SaveDiet() {
    ShowLoader();
    var rowCount = $('#tblDietPlan tr').length;
    var jsonObj = [];
    for (i = 0; i <= rowCount; i++) {
        jsonObj.push(
            { "DietPlanId": $('#txtdietplanid' + i + '').val(), "MemberDietPlanid": $('#txtMemberdietplan' + i + '').val(), "MealTime1": $('#txtMealTime' + i + '').val(), "MondayDiet": $('#txtMonday' + i + '').val(), "TuesdayDiet": $('#txtTuesday' + i + '').val(), "WednesdayDiet": $('#txtWednesday' + i + '').val(), "ThursdayDiet": $('#txtThursday' + i + '').val(), "FridayDiet": $('#txtFriday' + i + '').val(), "SaturdayDiet": $('#txtSaturday' + i + '').val(), "SundayDiet": $('#txtSunday' + i + '').val() }
        );
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: UpdateDiet,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObj),
        success: function (data) {
            HideLoader();
            toastr.success("Data Saved Successfully.");
            //alert("Data Submitted");
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });

}

function PrintWorkout() {
   
    WorkoutPrintMemberDetails();
    $('#modal_Workout').modal('show');
}

function WorkoutPrintMemberDetails() {
    
    GetMemberDetail();
    GetGymDetail()
    WorkoutMembershipDetails();
    BindWorkoutPrint();
    HideLoader();
}

function GetMemberDetail() {

    $("#WorkoutCustomerName").html("");
    $("#WorkoutMemberid").text = "";
    $.ajax({
        url: GetDetailsMemberUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $('#WorkoutCustomerName').html("<b>"+data[0].FirstName + " " + data[0].LastName+"</b>");
            $('#WorkoutMemberid').text(data[0].MemberID);
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function GetGymDetail() {
    $("#gymNameWorkout").html("");
    $("#gymAddress").html("");
    $("#invoiceLogo").attr('src', "");

    $.ajax({
        url: GetDetailsGymUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $('#gymNameWorkout').html("<b>"+data[0].GymName+"</b>");
            $('#gymAddress').html(data[0].GymAddress);
            $("#invoiceLogo").attr('src', data[0].ImageData);
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function WorkoutMembershipDetails() {
    
    $("#WorkoutmembershipName").html("");
    $("#bindWorkoutEndDate").html("");
    $("#bindWorkoutStarDate").html("");

    $.ajax({
        url: WorkoutMembershipUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            var startDate = ConvertToDate(data[0].StartDate);
            var endDate = ConvertToDate(data[0].EndDate)
            var membertype = data[0].MembershipType.split(' ');
            if (membertype[0] == "1") {
                $("#WorkoutmembershipName").html("<b>Membership :</b>"+membertype[0] + " Month");
            }
            else {
                $("#WorkoutmembershipName").html("<b>Membership :</b>" + membertype[0] + " Months");
            }
            $("#bindWorkoutEndDate").html("<b>Start Date :</b>"+endDate);
            $("#bindWorkoutStarDate").html("<b>End Date :</b>"+startDate);
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
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

function BindWorkoutPrint()
{
    $("#AddWorkoutPrint").empty();

    $.ajax({
        cache: false,
        type: "GET",
        url: BindMemberWorkoutPlanUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: "",
        success: function (data) {

            var allweeks = false;
            var mwf = false;
            var sun = false;
            var mw = false;
            var mt = false;
            var m = false;
            var w = false;
            var f = false;
            var tts = false;
            var tf = false;
            var ws = false;
            var tue = false;
            var thu = false;
            var sat = false;
            var tts = false;
            var noNumberOfdays = false;
            $.each(data, function (i, item) {

                var workout = "";
                var numberOfSets = ""
                var repeats = "";

                if (item.Workout != null)
                {
                    workout = item.Workout;
                }
                if (item.NumberOfSets != null)
                {
                    numberOfSets=item.NumberOfSets
                }
                if (item.NumberOfMinutes != null)
                {
                    numberOfSets = item.NumberOfMinutes
                }

                if (item.Repeats != null)
                {
                    repeats = item.Repeats;
                }
                if (item.NumberofDays == "Mon-Tue-Wed-Thu-Fri-Sat") {

                    if (allweeks == false) {
                        allweeks = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong>Mon-Tue-Wed-Thu-Fri-Sat</strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table workoutTable" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:250px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');
                    }
                 
                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets  + "</td>"
                   + "<td class='text-center'>" + repeats  + "</td>"
                   + "</tr>";
                    $('.workoutTable tbody').append(rows);
                }

                //Mon-Wed-Fri
                if (item.NumberofDays == "Mon-Wed-Fri") {
                    
                    if (mwf == false) {
                        mwf = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong>Mon-Wed-Fri</strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table mwf" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:280px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');
                    }

                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets + "</td>"
                   + "<td class='text-center'>" + item.Repeats + "</td>"
                   + "</tr>";
                    $('.mwf tbody').append(rows);
                }
                //Sun
                if (item.NumberofDays == "Sun") {
                   
                    if (sun == false) {
                        sun = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong>Sun</strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table sun" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:280px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');
                    }
                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets + "</td>"
                   + "<td class='text-center'>" + repeats + "</td>"
                   + "</tr>";
                    $('.sun tbody').append(rows);
                }

                //Mon-Wed
                if (item.NumberofDays == "Mon-Wed") {
                   
                    if (mw == false) {
                        mw = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong>Mon-Wed</strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table mw" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:280px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');
                    }
                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets + "</td>"
                   + "<td class='text-center'>" + repeats + "</td>"
                   + "</tr>";
                    $('.mw tbody').append(rows);
                }

                //Mon-Thu
                if (item.NumberofDays == "Mon-Thu") {

                    if (mt == false) {
                        mt = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong>Mon-Thu</strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table mt" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:280px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');
                    }
                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets + "</td>"
                   + "<td class='text-center'>" + repeats + "</td>"
                   + "</tr>";
                    $('.mt tbody').append(rows);
                }
                //Mon
                if (item.NumberofDays == "Mon") {

                    if (m == false) {
                        m = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong>Mon</strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table m" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:280px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');
                    }
                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets + "</td>"
                   + "<td class='text-center'>" + repeats + "</td>"
                   + "</tr>";
                    $('.m tbody').append(rows);
                }
                //Wed
                if (item.NumberofDays == "Wed") {

                    if (w == false) {
                        w = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong>Wed</strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table w" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:280px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');
                    }
                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets + "</td>"
                   + "<td class='text-center'>" +repeats + "</td>"
                   + "</tr>";
                    $('.w tbody').append(rows);
                }
                //Fri
                if (item.NumberofDays == "Fri") {

                    if (f == false) {
                        f = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong>Fri</strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table f" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:280px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');
                    }
                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets + "</td>"
                   + "<td class='text-center'>" + repeats + "</td>"
                   + "</tr>";
                    $('.f tbody').append(rows);
                }

                //Tue-Thu-Sat
                if (item.NumberofDays == "Tue-Thu-Sat") {

                    if (tts == false) {
                        tts = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong>Tue-Thu-Sat</strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table tts" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:280px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');
                    }
                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets + "</td>"
                   + "<td class='text-center'>" + repeats + "</td>"
                   + "</tr>";
                    $('.tts tbody').append(rows);
                }
                //Tue-Fri
                if (item.NumberofDays == "Tue-Fri") {

                    if (tf == false) {
                        tf = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong>Tue-Fri</strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table tf" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:280px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');
                    }
                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets + "</td>"
                   + "<td class='text-center'>" + repeats + "</td>"
                   + "</tr>";
                    $('.tf tbody').append(rows);
                }
                //Wed-Sat
                if (item.NumberofDays == "Wed-Sat") {

                    if (ws == false) {
                        ws = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong>Wed-Sat</strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table ws" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:280px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');
                    }
                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets + "</td>"
                   + "<td class='text-center'>" + repeats + "</td>"
                   + "</tr>";
                    $('.ws tbody').append(rows);
                }
                //Tue
                if (item.NumberofDays == "Tue") {

                    if (tue == false) {
                        tue = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong>Tue</strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table tue" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:280px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');
                    }
                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets + "</td>"
                   + "<td class='text-center'>" + repeats + "</td>"
                   + "</tr>";
                    $('.tue tbody').append(rows);
                }
                //Thu
                if (item.NumberofDays == "Thu") {

                    if (thu == false) {
                        thu = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong>Thu</strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table thu" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:280px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');
                    }
                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets + "</td>"
                   + "<td class='text-center'>" + repeats + "</td>"
                   + "</tr>";
                    $('.thu tbody').append(rows);
                }

                //Sat
                if (item.NumberofDays == "Sat") {

                    if (sat == false) {
                        sat = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong>Sat</strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table sat" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:280px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');
                    }
                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets  + "</td>"
                   + "<td class='text-center'>" + repeats + "</td>"
                   + "</tr>";
                    $('.sat tbody').append(rows);
                }

                if (item.NumberofDays == null)
                {
                    if (noNumberOfdays == false)
                    {
                        noNumberOfdays = true;
                        $("#AddWorkoutPrint").append('<div class="text-center" style="margin:3px;font-size:15px;"> <strong> </strong></div>');
                        $("#AddWorkoutPrint").append('<table class="table default" style="font-size: 15px;" align="center"><thead><tr style="border-bottom:1px dotted #000000 !important;"><th class="workout" style="width:280px;">WorkOut</th><th class="text-center ng-binding" style="width:103px;">Minutes/Sets</th><th class="text-center">Repeats</span></th> </tr> </thead><tbody></tbody></table>');

                    }
                       
                    var rows = "<tr style='border-bottom: 1px dotted rgb(0, 0, 0) !important;'>"
                   + "<td>" + workout + "</td>"
                   + "<td class='text-center'>" + numberOfSets + "</td>"
                   + "<td class='text-center'>" + repeats + "</td>"
                   + "</tr>";
                    $('.default tbody').append(rows);

                }

              


            });
            //$("#Signaturecalender").append('<div class="table-responsive"><table id="tblCalander" class="table"><tbody><tr><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td>'
            //       + '<td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td>'
            //       + '<td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td></tr>'
            //       + '<tr><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td>'
            //       + '<td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td></tr>'
            //       + '<tr><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td>'
            //       + '<td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td></tr>'
            //       + '<tr><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td>'
            //       + '<td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td></tr>'
            //       + '<tr><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td>'
            //       + '<td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td><td style="padding:15px;border: #929191 solid 1px;">&nbsp;</td></tr>'
            //       + '</tbody></table></div>'
            //      );

           
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });


}

function PrintoutWorkout() {
    var printelem = document.getElementById('divWorkoutCard');
    var myWindow = window.open('', '', 'width=750,height=500', '_blank');
    myWindow.document.write(document.head.innerHTML);
    myWindow.document.write(printelem.innerHTML);
    myWindow.document.write("\x3Cscript>window.print(); window.close();\x3C/script>");
}


function PrintDiet() {

    DietPrintMemberDetails();
    $('#modal_Diet').modal('show');
}

function DietPrintMemberDetails() {

    DietGetMemberDetail();
    DietGetGymDetail();
    DietMembershipDetails()
    BindDietDetails();
}

function DietGetGymDetail() {

    $.ajax({
        url: GetDetailsGymUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {

            //customerName  txtMemberid  gymName  membership startdate enddate
            $('#DietgymName').text(data[0].GymName);
            $('#Dietgymaddress').text(data[0].GymAddress);
            $("#DietinvoiceLogo").attr('src', data[0].ImageData);
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function DietGetMemberDetail() {

    $.ajax({
        url: GetDetailsMemberUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {

            //customerName  txtMemberid  gymName  membership startdate enddate
            $('#DietCustomerName').text(data[0].FirstName + " " + data[0].LastName);
            $('#DietMemberid').text(data[0].MemberID);
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function DietMembershipDetails() {
    $.ajax({
        url: WorkoutMembershipUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            var membertype = data[0].MembershipType.split(' ');
            if (membertype[1] == "1") {
                $("#Dietmembership").text(membertype[1] + " Month");
            }
            else {
                $("#Dietmembership").text(membertype[1] + " Months");
            }

            $("#Dietenddate").text(data[0].EndDate);
            $("#Dietstartdate").text(data[0].StartDate);
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function BindDietDetails()
{
    $.ajax({
        url: EditMemberDietPlanUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {

            $('.tbldietPlan tbody').empty();
            $.each(data, function (i, item) {

                var rows = "<tr>"
                  + "<td>" + item.DietTime + "</td>"
                  + "<td class='text-center'>" + item.MondayDiet + "</td>"
                  + "<td class='text-center'>" + item.TuesdayDiet + "</td>"
                   + "<td class='text-center'>" + item.WednesdayDiet + "</td>"
                    + "<td class='text-center'>" + item.ThursdayDiet + "</td>"
                     + "<td class='text-center'>" + item.FridayDiet + "</td>"
                      + "<td class='text-center'>" + item.SaturdayDiet + "</td>"
                       + "<td class='text-center'>" + item.SundayDiet + "</td>"


                  + "</tr>";
                $('.tbldietPlan tbody').append(rows);
            })
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function PrintoutDiet() {
    var printelem = document.getElementById('divDietCard');
    var myWindow = window.open('', '', 'width=750,height=500', '_blank');
    myWindow.document.write(document.head.innerHTML);
    myWindow.document.write(printelem.innerHTML);
    myWindow.document.write("\x3Cscript>window.print(); window.close();\x3C/script>");
}
//
function BindMemberDetailsMeasurements()
{
    $.ajax({
        url: BindMemberDetailMeasurementUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $('#customerName').text(data[0].FirstName + " " + data[0].LastName);
            $('#joiningDate').text(data[0].EnrollDate);
            $('#gender').text(data[0].Gender);
            $('#measurementDate').text(data[0].NextMeasurementDate);
            
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        },
        error: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function BindImageMeasurements() {
    $.ajax({
        url: ImageMeasurementsUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $('#gymImage').attr('src', data[0].ImageData);
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        },
        error: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function BindMeasurement(measurement)
{
    if (measurement.length > 0)
                {
            $('#tblMemMeasurement tbody').empty();
            var rows;
            var i;
            var lastValue = measurement.length - 1;
            rows = "<tr><td></td>"
            for (i = 0; i <= measurement.length-1; i++)
            {
                rows += "<th>" + measurement[i].MeasurementDate + " <span class='glyphicon glyphicon-edit' onclick = 'return ShowMeasurements(" + measurement[i].MeasurementId + ");'></span></th>";
            }
            rows += "</tr>"
            $('#tblMemMeasurement tbody').append(rows);

            //height
            height = "<tr><td><b>Height</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {
                var firstValue = measurement[lastValue].Height;
                var diffFirst = measurement[i].Height;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue)
                {
                    var j = i + 1;
                    var prev = measurement[j].Height;
                    var present = measurement[i].Height;
                  diffPrev = present - prev;
                }
               
                if(diffPrev>0)
                {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0)
                {
                    differnceValue = "+" + differnceValue;
                }
                height += "<td><b>" + measurement[i].Height + "<span style='color:blue'>[" + differnceValue + "]</span> <span style='color:orange'> ["+diffPrev+"]</span></b></td>";
            }
            height += "</tr>"
            $('#tblMemMeasurement tbody').append(height);

            //weight
            weight = "<tr><td><b>Weight</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {
                var firstValue = measurement[lastValue].weight;
                var diffFirst = measurement[i].weight;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue) {
                    var j = i + 1;
                    var prev = measurement[j].weight;
                    var present = measurement[i].weight;
                    diffPrev = present - prev;
                }

                if (diffPrev > 0) {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0) {
                    differnceValue = "+" + differnceValue;
                }
                weight += "<td><b>" + measurement[i].weight + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
            }
            weight += "</tr>"
            $('#tblMemMeasurement tbody').append(weight);

           //Upper Arm
            upperArm = "<tr><td><b>Upper Arm</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {
                var firstValue = measurement[lastValue].UpperArm;
                var diffFirst = measurement[i].UpperArm;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue) {
                    var j = i + 1;
                    var prev = measurement[j].UpperArm;
                    var present = measurement[i].UpperArm;
                    diffPrev = present - prev;
                }

                if (diffPrev > 0) {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0) {
                    differnceValue = "+" + differnceValue;
                }
                upperArm += "<td><b>" + measurement[i].UpperArm + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
            }
            upperArm += "</tr>"
            $('#tblMemMeasurement tbody').append(upperArm);

            //ForeArm
            foreArm = "<tr><td><b>ForeArm</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {

                var firstValue = measurement[lastValue].ForeArm;
                var diffFirst = measurement[i].ForeArm;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue) {
                    var j = i + 1;
                    var prev = measurement[j].ForeArm;
                    var present = measurement[i].ForeArm;
                    diffPrev = present - prev;
                }

                if (diffPrev > 0) {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0) {
                    differnceValue = "+" + differnceValue;
                }
                foreArm += "<td><b>" + measurement[i].ForeArm + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
            }
            foreArm += "</tr>"
            $('#tblMemMeasurement tbody').append(foreArm);


            //calves
            calves = "<tr><td><b>Calves</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {
                var firstValue = measurement[lastValue].Calves;
                var diffFirst = measurement[i].Calves;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue) {
                    var j = i + 1;
                    var prev = measurement[j].Calves;
                    var present = measurement[i].Calves;
                    diffPrev = present - prev;
                }

                if (diffPrev > 0) {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0) {
                    differnceValue = "+" + differnceValue;
                }
                calves += "<td><b>" + measurement[i].Calves + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
            }
            calves += "</tr>"
            $('#tblMemMeasurement tbody').append(calves);

            //BMI
            bmi = "<tr><td><b>BMI</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {
                var firstValue = measurement[lastValue].BMI;
                var diffFirst = measurement[i].BMI;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue) {
                    var j = i + 1;
                    var prev = measurement[j].BMI;
                    var present = measurement[i].BMI;
                    diffPrev = present - prev;
                }

                if (diffPrev > 0) {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0) {
                    differnceValue = "+" + differnceValue;
                }
                bmi += "<td><b>" + measurement[i].BMI + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
            }
            bmi += "</tr>"
            $('#tblMemMeasurement tbody').append(bmi);

            //BMI
            vfat = "<tr><td><b>V. Fat</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {
                var firstValue = measurement[lastValue].VFat;
                var diffFirst = measurement[i].VFat;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue) {
                    var j = i + 1;
                    var prev = measurement[j].VFat;
                    var present = measurement[i].VFat;
                    diffPrev = present - prev;
                }

                if (diffPrev > 0) {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0) {
                    differnceValue = "+" + differnceValue;
                }
                vfat += "<td><b>" + measurement[i].VFat + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
            }
            vfat += "</tr>"
            $('#tblMemMeasurement tbody').append(vfat);

            //Shoulder
            shoulder = "<tr><td><b>Shoulder</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {
                var firstValue = measurement[lastValue].Shoulder;
                var diffFirst = measurement[i].Shoulder;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue) {
                    var j = i + 1;
                    var prev = measurement[j].Shoulder;
                    var present = measurement[i].Shoulder;
                    diffPrev = present - prev;
                }

                if (diffPrev > 0) {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0) {
                    differnceValue = "+" + differnceValue;
                }
                shoulder += "<td><b>" + measurement[i].Shoulder + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
            }
            shoulder += "</tr>"
            $('#tblMemMeasurement tbody').append(shoulder);

            //chest
            chest = "<tr><td><b>Chest</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {
                var firstValue = measurement[lastValue].Chest;
                var diffFirst = measurement[i].Chest;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue) {
                    var j = i + 1;
                    var prev = measurement[j].Chest;
                    var present = measurement[i].Chest;
                    diffPrev = present - prev;
                }

                if (diffPrev > 0) {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0) {
                    differnceValue = "+" + differnceValue;
                }
                chest += "<td><b>" + measurement[i].Chest + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
            }
            chest += "</tr>"
            $('#tblMemMeasurement tbody').append(chest);

            //Arms
            arms = "<tr><td><b>Arms</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {
                var firstValue = measurement[lastValue].Arms;
                var diffFirst = measurement[i].Arms;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue) {
                    var j = i + 1;
                    var prev = measurement[j].Arms;
                    var present = measurement[i].Arms;
                    diffPrev = present - prev;
                }

                if (diffPrev > 0) {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0) {
                    differnceValue = "+" + differnceValue;
                }
                arms += "<td><b>" + measurement[i].Arms + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
            }
            arms += "</tr>"
            $('#tblMemMeasurement tbody').append(arms);

            //Upper ABS
            upperabs = "<tr><td><b>Upper ABS</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {
                var firstValue = measurement[lastValue].UpperABS;
                var diffFirst = measurement[i].UpperABS;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue) {
                    var j = i + 1;
                    var prev = measurement[j].UpperABS;
                    var present = measurement[i].UpperABS;
                    diffPrev = present - prev;
                }

                if (diffPrev > 0) {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0) {
                    differnceValue = "+" + differnceValue;
                }
                upperabs += "<td><b>" + measurement[i].UpperABS + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
            }
            upperabs += "</tr>"
            $('#tblMemMeasurement tbody').append(upperabs);

            //Waist ABS
            waistabs = "<tr><td><b>Waist ABS</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {
                var firstValue = measurement[lastValue].WaistABS;
                var diffFirst = measurement[i].WaistABS;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue) {
                    var j = i + 1;
                    var prev = measurement[j].WaistABS;
                    var present = measurement[i].WaistABS;
                    diffPrev = present - prev;
                }

                if (diffPrev > 0) {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0) {
                    differnceValue = "+" + differnceValue;
                }
                waistabs += "<td><b>" + measurement[i].WaistABS + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
            }
            waistabs += "</tr>"
            $('#tblMemMeasurement tbody').append(waistabs);

            //Lower ABS
            lowerabs = "<tr><td><b>Lower ABS</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {
                var firstValue = measurement[lastValue].LowerABS;
                var diffFirst = measurement[i].LowerABS;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue) {
                    var j = i + 1;
                    var prev = measurement[j].LowerABS;
                    var present = measurement[i].LowerABS;
                    diffPrev = present - prev;
                }

                if (diffPrev > 0) {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0) {
                    differnceValue = "+" + differnceValue;
                }
                lowerabs += "<td><b>" + measurement[i].LowerABS + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
            }
            lowerabs += "</tr>"
            $('#tblMemMeasurement tbody').append(lowerabs);

            //Glutes
            glutes = "<tr><td><b>Glutes</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {
                var firstValue = measurement[lastValue].Glutes;
                var diffFirst = measurement[i].Glutes;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue) {
                    var j = i + 1;
                    var prev = measurement[j].Glutes;
                    var present = measurement[i].Glutes;
                    diffPrev = present - prev;
                }

                if (diffPrev > 0) {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0) {
                    differnceValue = "+" + differnceValue;
                }
                glutes += "<td><b>" + measurement[i].Glutes + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
            }
            glutes += "</tr>"
            $('#tblMemMeasurement tbody').append(glutes);

            //Thighes
            thighes = "<tr><td><b>Thighes</b></td>"
            for (i = 0; i <= measurement.length - 1; i++) {
                var firstValue = measurement[lastValue].Thighs;
                var diffFirst = measurement[i].Thighs;
                var differnceValue = diffFirst - firstValue;
                var diffPrev = 0;
                if (i < lastValue) {
                    var j = i + 1;
                    var prev = measurement[j].Thighs;
                    var present = measurement[i].Thighs;
                    diffPrev = present - prev;
                }

                if (diffPrev > 0) {
                    diffPrev = "+" + diffPrev;
                }
                if (differnceValue > 0) {
                    differnceValue = "+" + differnceValue;
                }
                thighes += "<td><b>" + measurement[i].Thighs + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
            }
            thighes += "</tr>"
            $('#tblMemMeasurement tbody').append(thighes);
            }
}

function ShowMeasurements(measurementid)
{
    var id = measurementid;
  
    $.ajax({
        url: editMeasurementUrl,
        data: {measurementid:id},
        type: "GET",
        dataType: "json",
        success: function (data) {
           
                $('#MeasurementId').val(data[0].MeasurementId);
                $('#txtMeasurementDate').val(data[0].MeasurementDate);
                $('#txtNextMeasurementDate').val(data[0].NextMeasurementDate);
                $('#txtHeight').val(data[0].Height);
                $('#txtWeight').val(data[0].weight);
                $('#txtUpperArm').val(data[0].UpperArm);
                $('#txtForeArm').val(data[0].ForeArm);
                $('#txtCalves').val(data[0].Calves);
                $('#txtBMI').val(data[0].BMI);
                $('#txtVFat').val(data[0].VFat);
                $('#txtShoulder').val(data[0].Shoulder);
                $('#txtChest').val(data[0].Chest);
                $('#txtArms').val(data[0].Arms);
                $('#txtUpperABS').val(data[0].UpperABS);
                $('#txtWaistABS').val(data[0].WaistABS);
                $('#txtLowerABS').val(data[0].LowerABS);
                $('#txtGlutes').val(data[0].Glutes);
                $('#txtThighes').val(data[0].Thighs);
           
            $('#modal_Measurement').modal('show');
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        },
        error: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function UpdateMeasurements()
{
    var jsonObject = {
        MeasurementId: $('#MeasurementId').val(), MeasurementDate: $('#txtMeasurementDate').val(), NextMeasurementDate: $('#txtNextMeasurementDate').val(), Height: $('#txtHeight').val(), weight: $('#txtWeight').val(),
        UpperArm: $('#txtUpperArm').val(), ForeArm: $('#txtForeArm').val(), Calves: $('#txtCalves').val(), BMI: $('#txtBMI').val(),
        VFat: $('#txtVFat').val(), Shoulder: $('#txtShoulder').val(), Chest: $('#txtChest').val(), Arms: $('#txtArms').val(),
        UpperABS: $('#txtUpperABS').val(), WaistABS: $('#txtWaistABS').val(), LowerABS: $('#txtLowerABS').val(), Glutes: $('#txtGlutes').val(), Thighs: $('#txtThighes').val()
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: UpdateMeasurementsUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            $('#MeasurementId').val("");
            $('#modal_Measurement').modal('hide');
            toastr.success("Data Saved Successfully.");
            BindMeasurement();
            BindImageMeasurements();
            BindMemberDetailsMeasurements();
           

            //alert("Data Submitted");
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function PrintoutMeasurement() {

    $("#details").show();
    var printelem = document.getElementById('printMeasurement');
    var myWindow = window.open('', '', 'width=750,height=500', '_blank');
    myWindow.document.write(document.head.innerHTML);
    myWindow.document.write(printelem.outerHTML);
    myWindow.document.write("\x3Cscript>window.print(); window.close();\x3C/script>");
    $("#details").hide();
}

function ShowWebcam()
{
   
    
    $("#Camera").webcam({
        width: 320,
        height: 240,
        mode: "save",
        swffile: swf,
        onTick: function () { },
        onSave: function (data, ab) {
            //UploadPic();
        },
        onCapture: function () {
            webcam.save(SaveWebcamPic);
        },
        debug: function (type, status) {
           
            //$('#camStatus').append(type + ": " + status + '<br /><br />');
            var status = type + ": " + status;
            $('#camStatus').append(status);
            if (status == "notify: Capturing finished.")
            {
                toastr.success("Image Saved Successfully.");

            }
            
        },
        onLoad: function () { }
    });

}

function Capture() {
    document.getElementById('XwebcamXobjectX').capture();
}

function Sets(id) {
    //change label text
    var $currdiv = $(id).parent().parent().parent().parent().parent();
    $currdiv.find("label").text("Sets");
    //change button value
    $(id).closest("div").find("button").text("")
    $(id).closest("div").find("button").append("Sets" + " <span class='caret'></span>");
    //hide repeat textbox
    var $curr = $(id).closest('div');
    $curr = $curr.parent().parent();
    //$curr.next().show();
    $curr = $curr.next();
    $curr.find("input").show();
    $curr.find("label").show();
}

function Minutes(id) {
    //alert($(".setMin").text());
    //change label text
    var $currdiv = $(id).parent().parent().parent().parent().parent();
    $currdiv.find("label").text("Minutes");
    //change button value
    $(id).closest("div").find("button").text("")
    $(id).closest("div").find("button").append("Minutes" + " <span class='caret'></span>");
    //hide repeat textbox
    var $curr = $(id).closest('div');
    $curr = $curr.parent().parent();
    $curr = $curr.next();
    $curr.find("input").hide();
    $curr.find("label").hide();
}

function ShowReferenceModal(pageno, pagesize)
{
    $('#tblReferenceId').hide();
    $('#modal_SearchMemberId').modal('show');

}


function BindSelectMember(pageno, pagesize)
{
    ShowLoader();
    //var customer = $('input[id="txtSelectCustomers"]').val();
    var customer = $('#txtcustomerReference').val();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetCustomerUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val(), CustomerName: customer },
        success: function (data) {
            $('#tblReferenceId tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = "<tr>" +
                        '<td><button type="button" class="btn btn-sm btn-primary btn-flat" id="btnSelect" onclick="return SelectCustomer(this)">Select</button></td>'
                        + '<td>' + item.MemberID + '</td>'
                        + '<td>' + item.MemberName+'</td>'
                        + '<td >' + item.MobileNumber + '</td>'
                        + "</tr>";
                    
                    $('#tblReferenceId tbody').append(rows);
                    $('#tblReferenceId').show();
                 
                });
                HideLoader();
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
                                BindSelectMember(pageno,10);
                            }
                        }
                    });
                }
            }
            else {
                var norecords = "<tr>" +
                    '<td colspan="4">No data available</td>'
                    + "</tr>";
                $('#tblReferenceId tbody').append(norecords);
                HideLoader();
            }

            $('#tblReferenceId').show();
            HideLoader();
        },
        error: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });
}


function SelectCustomer(id)
{
    $('#modal_SearchMemberId').modal('hide');
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var memberid = $('#tblReferenceId tr').eq(rowIndex + 1).find('td').eq(1).html();
    $('#txtReferenceBy').val(memberid);
    

}

AllowOnlyText("txtFirstName");
AllowOnlyText("txtLastName");
AllowOnlyText("txtOccupation");
AllowOnlyText("txtSpouseName");
AllowOnlyText("txtDesignation");
AllowOnlyText("txtProgramme");
AllowOnlyText("txtOther");
AllowOnlyNumbers("txtMobileno");
AllowOnlyNumbers("txtPhoneResi");
AllowOnlyNumbers("txtPhoneofc");
AllowOnlyNumbers("txtMonths");
AllowOnlyNumbers("txtAmount");
AllowOnlyNumbers("txtPaymentAmount");

var subpagerLoaded = false, subpageNo = 1, freezeData = null;

//Transfer & Freeze Related
function GetSelectedCustMembershipData(Id, Type) {


    var CustMembership = $.grep(membershipdata, function (v) {
        return v.MembershipID === $(Id).val();
    })[0];

    if (CustMembership !== undefined && CustMembership !== null) {

        var m = moment(new Date(CustMembership.EndDate));
        var years = m.diff(new Date(), 'years');
        m.add(-years, 'years');
        var months = m.diff(new Date(), 'months');
        m.add(-months, 'months');
        var days = m.diff(new Date(), 'days');

        var remainingadays = '';

        if (years > 0) {
            remainingadays = years + ' year ';
        }

        if (months > 0) {
            remainingadays = remainingadays + months + ' month ';
        }

        if (days > 0) {
            remainingadays = remainingadays + days + ' days';
        }


        if (Type === 'Transfer') {
            $("#txttransferStartDate").datepicker('setDate', new Date());
            $("#txttransferEndDate").datepicker('setDate', CustMembership.EndDate);
            $("#transferremianingDays").html(remainingadays);
        }
        else {
            $("#txtFreezeStartDate").datepicker('setDate', new Date());
            $("#freezeremianingDays").html(remainingadays);
        }
    }
}

function CustomerSearch(subpageNo) {
    $.ajax({
        cache: false,
        type: "GET",
        url: GetMemberDetailsUrl,
        dataType: "json",
        data: { searchCharacter: $('#txtSelectCustomers').val(), page: subpageNo, branchId: $("#ddlBranch").val() },
        success: function (data) {
            $('#tblSelectMembers tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = "<tr>" +
                        '<td><button type="button" class="btn btn-sm btn-primary btn-flat" id="btnSelect" onclick="SelectMember(this)">Select</button></td>'
                        + '<td class="MemberId">' + item.MemberID + '</td>'
                        + '<td><span class="FirstName">' + item.FirstName + '</span> <span class="LastName">' + item.LastName + '</td>'
                        + '<td class="MobileNumber">' + item.MobileNumber + '</td>'
                        + "</tr>";
                    $('#tblSelectMembers tbody').append(rows);
                });

                if (!subpagerLoaded) {
                    subpagerLoaded = true;
                    $('#pagination-demo').twbsPagination({
                        totalPages: data.Pages,
                        visiblePages: 7,
                        onPageClick: function (event, page) {
                            if (subpageNo != page) {
                                subpageNo = page;
                                CustomerSearch(page)
                            }
                        }
                    });
                }
            }
            else {
                var norecords = "<tr>" +
                    '<td colspan="4">No data available</td>'
                    + "</tr>";
                $('#tblSelectMembers tbody').append(norecords);
            }

            $('#divSearchtable').show();
            HideLoader();
        },
        error: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });

}

//Searching Members
function SelectCustSearch() {
    ShowLoader();
    subpagerLoaded = false;
    if ($('#pagination-demo').data("twbs-pagination")) {
        $('#pagination-demo').twbsPagination('destroy');
    }
    subpageNo = 1;
    CustomerSearch(subpageNo);
}

function SelectCustClear() {
    $('#divSearchtable').hide();
    $('#tblSelectMembers tbody').find("tr").remove();
    $('#txtSelectCustomers').val("");
    subpagerLoaded = false;
    if ($('#pagination-demo').data("twbs-pagination")) {
        $('#pagination-demo').twbsPagination('destroy');
    }
}

//On Select Button Click
function SelectMember(Member) {
    $("#TransferMemberId").val($(Member).parent().parent().find(".MemberId").text());
    $("#TransferMemberName").val($(Member).parent().parent().find(".FirstName").text() + " " + $(Member).parent().parent().find(".LastName").text());
    $("#modal-membersearch").modal("hide");
}

function SaveTransfer() {
    debugger;
    var flag = ValidateTransferForm();
    if (flag) {
        ShowLoader();
        $.ajax({
            cache: false,
            type: "POST",
            url: TransferMemberShipUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ membershipId: $('#TransferMembership').val(), memberId: $("#TransferMemberId").val(), Notes: "Transfered From - " + $("#memberdetail").html(), StartDate: $("#txttransferStartDate").val(), EndDate: $("#txttransferEndDate").val() }),
            success: function (data) {
                if (data.Status == "Success") {
                    toastr.success('Transfered membership successfully');
                    ClearTransfer();
                    OnLoadBindMemberShip();
                }
                HideLoader();
            },
            failure: function (errMsg) {
                toastr.error('An error occured while saving data!');
                HideLoader();
            },
            error: function (errMsg) {
                toastr.error('An error occured while saving data!');
                HideLoader();
            }
        });
    }
}

function ValidateTransferForm() {
    var flag = true;

    if (!ValidateRequiredField($("#TransferMembership"), ' Please Select Membership', 'after')) {
        flag = false;
    }
    if (!ValidateRequiredField($("#TransferMemberName"), ' Please Select Member', 'after')) {
        flag = false;
    }
    if (!ValidateRequiredField($("#txttransferStartDate"), ' Please Enter Start Date', 'after')) {
        flag = false;
    }
    if (!ValidateRequiredField($("#txttransferEndDate"), ' Please Enter End Date', 'after')) {
        flag = false;
    }

    return flag;
}

function ClearTransfer() {
    $("#TransferMembership").val("");
    $("#TransferMemberId").val("");
    $("#TransferMemberName").val("");
    $("#txttransferStartDate").datepicker('setDate', '');
    $("#txttransferEndDate").datepicker('setDate', '');
    $("#transferremianingDays").html("");
}

function SaveFreeze() {
    var flag = ValidateFreezeform();
    if (flag) {
        ShowLoader();

        var freezemember = {
            FreezeId: $("#HFreezeMembership").val(),
            MemberShipId: $("#FreezeMembership").val(),
            StartDate: $("#txtFreezeStartDate").val(),
            EndDate: $("#txtFreezeEndDate").val(),
            Member_Id: $('#txtMemberId').val()
        };

        $.ajax({
            cache: false,
            type: "POST",
            url: FreezeMemberShipUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ objMemberFreeze: freezemember }),
            success: function (data) {
                if (data.Status == "Success") {
                    toastr.success('Freeze membership successfully');
                    ClearFreeze();
                    OnLoadBindMemberShip();
                    GetMembershipFreezeList();
                }
                HideLoader();
            },
            failure: function (errMsg) {
                toastr.error('An error occured while saving data!');
                HideLoader();
            },
            error: function (errMsg) {
                toastr.error('An error occured while saving data!');
                HideLoader();
            }
        });
    }
}

function ValidateFreezeform() {
    var flag = true;

    if (!ValidateRequiredField($("#FreezeMembership"), ' Please Select Membership', 'after')) {
        flag = false;
    }
    if (!ValidateRequiredField($("#txtFreezeStartDate"), ' Please Enter Start Date', 'after')) {
        flag = false;
    }
    if (!ValidateRequiredField($("#txtFreezeEndDate"), ' Please Enter End Date', 'after')) {
        flag = false;
    }

    if ($("#txtFreezeStartDate").datepicker("getDate") !== null && $("#txtFreezeEndDate").datepicker("getDate") !== null) {
        var startdate = new Date($("#txtFreezeStartDate").datepicker("getDate"));
        var enddate = new Date($("#txtFreezeEndDate").datepicker("getDate"));

        if (startdate > enddate) {
            toastr.error("Please ensure that the End Date is greater than Start Date.");
            flag = false;
        }
    }

    return flag;
}

function ClearFreeze() {
    $("#freezeremianingDays").html("");
    $("#FreezeMembership").val("");
    $("#txtFreezeStartDate").datepicker('setDate', '');
    $("#txtFreezeEndDate").datepicker('setDate', '');
    $("#HFreezeMembership").val(0);
}

function EditFreezeMembership(Id) {
    var Freeze = $.grep(freezeData, function (v) {
        return v.FreezeId == Id;
    })[0];

    if (Freeze != null && Freeze != undefined) {
        var CustMembership = $.grep(membershipdata, function (x) {
            return x.MembershipID == Freeze.MemberShipId;
        })[0];

        if (CustMembership !== null && CustMembership !== undefined) {
            if (CustMembership.Status != "Active" || CustMembership.RemainingDays <= 0) {
                $('#FreezeMembership').append("<option value='" + CustMembership.MembershipID + "'>" + CustMembership.MemershipType + "</option>");
            }
            $("#HFreezeMembership").val(Id);
            $('#FreezeMembership').val(CustMembership.MembershipID)
            $("#txtFreezeStartDate").datepicker('setDate', new Date(Freeze.StartDate));
            $("#txtFreezeEndDate").datepicker('setDate', new Date(Freeze.EndDate));
        }
    }
}

var deleteFreezeId = 0;
function DeleteFreezeMembership(Id) {
    deleteFreezeId = Id;
    $("#modal_Conformation_Freeze").modal("show");
}

function DeleteFreeze() {
    ShowLoader();
    $("#modal_Conformation_Freeze").modal("hide");
    $.ajax({
        cache: false,
        type: "POST",
        url: DeleteFreezeUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ FreezeId: deleteFreezeId }),
        success: function (data) {
            if (data.Status == "Success") {
                toastr.success('Freeze deletion successfully');
                ClearFreeze();
                OnLoadBindMemberShip();
                GetMembershipFreezeList();
            }
            HideLoader();
        },
        failure: function (errMsg) {
            toastr.error('An error occured while deleting!');
            HideLoader();
        },
        error: function (errMsg) {
            toastr.error('An error occured while deleting!');
            HideLoader();
        }
    });
}

function GetMembershipFreezeList(freeze) {
            $('#FreezeMembershipList tbody').empty();
            freezeData = null;
            if (freeze != null && freeze.length > 0) {
                freezeData = freeze;
                $.each(freeze, function (i, item) {
                    var rowno = i + 1;
                    var rows = '<tr>'
                        + '<td class="text-left">' + rowno + '</td>'
                        + '<td class="text-left">' + item.MemershipType.split(' ')[0] + '</td>'
                        + '<td class="text-left">' + item.StartDate + '</td>'
                        + '<td class="text-left">' + item.EndDate + '</td>'
                        + '<td class="text-right">' + item.Month + '</td>'
                        + '<td class="text-right">' + parseFloat(item.Amount).toFixed(2) + '</td>'
                        + '<td class="text-center">' + item.Status + '</td>'
                        + '<td class="text-center">'
                        + '<a href="javascript:void(0)" onclick="EditFreezeMembership(' + item.FreezeId + ')" title="Edit" class="btn btn-success btn-xs"><i class="fa fa-edit"></i> Edit</a> &nbsp;&nbsp;'
                        + '<a href="javascript:void(0)" onclick="DeleteFreezeMembership(' + item.FreezeId + ')" title="Delete" class="btn btn-danger btn-xs"><i class="fa fa-remove"></i> Delete</a>'
                        + '</td>';
                    +'</tr>';
                    $('#FreezeMembershipList tbody').append(rows);
                });
            }
           
}

function SendSMS(listphonenumber, content) {
    var encoded = encodeURI(content);
    //var urlString = SendSMSUrl.replace(/&amp;/g, '&');
    var urlString = "http://bulksms.smsroot.com/app/smsapi/index.php?key=35B759B645F85F&campaign=0&routeid=18&type=text&contacts=" + listphonenumber + "&senderid=SMSDMO&msg=" + encoded + "";
    //alert(urlString);
    ////urlString = "'" + urlString + "';";
    $.ajax({
        cache: false,
        type: "GET",
        url: urlString + "&callback=?",
        crossDomain: true,
        dataType: 'jsonp',
        data: "",
        success: function (jqXHR, textStatus) {
           
        },
        error: function (errMsg) {
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
        }
    });


}

function GetAutoCompleteOptions() {
    var availableAttributes = [];
    $.ajax({
        cache: false,
        type: "POST",
        contentType: "application/json",
        url: AutoCompleteUrl,
        dataType: "json",
        data: "",
        success: function (data) {

            for (var i in data)
                availableAttributes.push(data[i].WorkoutName);

        },
        error: function (result) {
            console.log(JSON.stringify(result));
        }
    });


    return availableAttributes;
}

function BindMemberFullDetails()
{
    ShowLoader();
    $.ajax({
        type: "GET",
        url: BindMembershipfullDetails,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data:"",
        success: function (data) {
            //Membership Table Binding
            if (data.BindMembership.Result.length > 0) {
                OnLoadBindMemberShip(data.BindMembership.Result);
            }
           
            //Memebrship detail table binding
            if (data.MembershipHeader.Result!=null) {
                
                UpdateHeaderMembership(data.MembershipHeader.Result);
            }
           

            //Mmeberinfo
            if (data.UpdateMemberInfo.Result.length > 0) {
                UpdateMemberInfoImage(data.UpdateMemberInfo.Result);
            }
           
            //Workout
            if (data.Workoutplan.Result.length > 0) {
                EditMemberWorkoutPlan(data.Workoutplan.Result);
            }
           
            //Diet
            if (data.DietPlan.Result.length > 0) {
                EditMemberDietPlan(data.DietPlan.Result);
            }
           
            //Freeze
            if (data.Freeze.Result.length > 0) {
                GetMembershipFreezeList(data.Freeze.Result);
            }
           
            //Measurement
            if (data.MeasurementList.Result.length > 0) {
                BindMeasurement(data.MeasurementList.Result);
            }
            
           // Image Measurement
            if (data.ImageMeasurement.Result.length > 0) {
                MeasurementImage(data.ImageMeasurement.Result);
            }
           

            //Info Measurement
            if (data.InfoMeasurement.Result.length > 0) {
                MeasurementMemberDetails(data.InfoMeasurement.Result);
            }
            //sms
            if (data.SMS.Result.length > 0) {
                BindSMSLog(data.SMS.Result);
            }
            else {
                var norecords = "<tr>" +
                    '<td colspan="4">No data available</td>'
                    + "</tr>";
                $('#tblsms tbody').append(norecords);
            }

            
          
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
        },
        error: function () {
            HideLoader();
        }
    });



}

function MeasurementImage(img)
{
    $('#gymImage').attr('src', img[0].ImageData);
}

function MeasurementMemberDetails(details)
{
    $('#customerName').text(details[0].FirstName + " " + details[0].LastName);
    $('#joiningDate').text(details[0].EnrollDate);
    $('#gender').text(details[0].Gender);
    $('#measurementDate').text(details[0].NextMeasurementDate);
}

function OnLoadBindMemberShip() {
    debugger;
    ShowLoader();
    $('#txtMembershipid').val("");
   

    $.ajax({
        cache: false,
        type: "GET",
        url: EditBindMembershipUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { MemberId: $('#txtMemberId').val() },
        success: function (data) {

            membershipdata = data;
            $('#tblNewMembership tbody').empty();
            $('#TransferMembership option').remove();
            $('#TransferMembership').append("<option value=''>---Select---</option>");
            $('#FreezeMembership option').remove();
            $('#FreezeMembership').append("<option value=''>---Select---</option>");

            $.each(data, function (i, item) {
                var dueAmount = 0;
                var membertype = item.MemershipType.split(' ');
                if (item.PaidAmount == undefined || item.PaidAmount == null) {
                    item.PaidAmount = "-";
                    dueAmount = parseInt(item.Amount) || 0;
                }
                else {
                    var amount = parseInt(item.Amount) || 0;
                    var paidamount = parseInt(item.PaidAmount) || 0;
                    dueAmount = amount - paidamount;
                }
                if (item.Status == undefined || item.Status == null) {
                    item.Status = "-";
                }
                if (item.Note == undefined || item.Note == null) {
                    item.Note = "";
                }
                var status = '', statusclass = '', _disabled = ""

                if (item.Status == "Active") {
                    if (item.RemainingDays > 0) {
                        status = 'Active';
                        statusclass = 'label label-success';
                    }
                    else {
                        status = 'Expired';
                        statusclass = 'label label-danger';
                    }
                }
                else {
                    status = item.Status;
                    statusclass = 'label label-warning';
                    _disabled = "disabled"
                }
                var notes = "";
                if (item.Notes != null) {
                    notes = item.Notes;
                }
                var rowno = i + 1;
                var rows = "<tr>"
                    + "<td style='display:none'>" + '<input type="hidden" name="hid" value=' + item.MembershipID + '>' + "</td>"
                    + "<td>" + rowno + "</td>"
                    + "<td>" + membertype[0] + "</td>"
                    + "<td>" + item.StartDate + "</td>"
                    + "<td>" + item.EndDate + "</td>"
                    + "<td class='text-right'>" + item.Months + "</td>"
                    + "<td class='text-right'>" + parseFloat(item.Amount).toFixed(2) + "</td>"
                    + "<td class='text-right'><span class='label label-warning'>" + parseFloat(dueAmount).toFixed(2) + "</span></td>"
                    + "<td class='text-right'><span class='label label-success'>" + parseFloat(item.PaidAmount).toFixed(2) + "</span></td>"
                    + "<td><span class='label label-success'>" + item.Status + "</span></td>"
                    + "<td>" + notes + "</td>"
                    + "<td><a class='btn btn-info btnPayment btn-xs btn-flat' data-toggle='modal' data-target='#modal-payment' onclick='return BindPaymentData(this);'>"
                    + "<i class='fa fa-credit-card'></i>"
                    + " Payment</a > "
                    + "<a class='btn btn-primary btn-xs btn-flat btnEditMembership' onclick = 'return EditMembership(this);'>"
                    + "<i class='fa fa-edit'></i> Edit</a> "
                    + "<a class='btn btn-danger btn-xs btn-flat btnDeleteMembership' onclick = 'return DeleteMembership(this);'>"
                    + "<i class='fa fa-remove'></i> Delete</a></td>"
                    + "</tr>";

                $('#tblNewMembership tbody').append(rows);

                if (status.toLowerCase() == "active") {
                    $('#TransferMembership').append("<option value='" + item.MembershipID + "'>" + item.MemershipType + "</option>");
                    $('#FreezeMembership').append("<option value='" + item.MembershipID + "'>" + item.MemershipType + "</option>");
                }

                HideLoader();
            });
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        },
        error: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });
}



function UpdateHeaderMembershipDetails() {
    $.ajax({
        url: UpdateMembershipUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {

            var dueAmount = 0;

            var membertype = data.MembershipType.split(' ');
            if (data.PaymentAmount == undefined || data.PaymentAmount == null) {
                data.PaymentAmount = "-";
                dueAmount = parseInt(data.Amount) || 0;
            }
            else {
                var amount = parseInt(data.Amount) || 0;
                var paidamount = parseInt(data.PaymentAmount) || 0;
                dueAmount = amount - paidamount;
            }
            if (membertype[1] == "1") {
                $("#subscriptiontime").text(membertype[1] + " Month");
            }
            else {
                $("#subscriptiontime").text(membertype[1] + " Months");
            }

          
            $("#expires").text(data.EndDate);
            $("#dueamount").text(dueAmount);


        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}


function UpdateMemberInformation() {
    ShowLoader();
    $.ajax({
        url: UpdateMemberInformationUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $.each(data, function (i, item) {
                $("#customerimage").attr('src',item.ImageData);
            })
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });
}




function EditMemberWorkoutPlan() {
    $.ajax({
        url: EditMemberWorkoutPlanUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $('#AddworkoutContainer').empty();
            var wrapper = $("#AddworkoutContainer");
            var availableAttributes = [];
            availableAttributes = GetAutoCompleteOptions();

            $('#txtWorkoutPlanName').val(data[0].PlaneNameId);
            if ($('#txtWorkoutPlanName').val() == null) {
                var length = $('#txtWorkoutPlanName > option').length;
                if (length < 2) {
                    BindPlan();
                    $('#txtWorkoutPlanName').val(data[0].PlaneNameId);
                }
                //$('#txtWorkoutPlanName').val();

            }

            $.each(data, function (i, item) {


                //$("#AddworkoutContainer").append('<div class="row"><div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control planid"  id="txtplanenameid' + i + '"></div>');
                //$("#AddworkoutContainer").append('<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control"  id="txtMemberworkoutid' + i + '"></div>');
                //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Workout</label><div class="input-group"><input type="Text" class="form-control"  id="txtworkout' + i + '"></div>');
                //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Sets</label><div class="input-group"><input type="Text" class="form-control" id="txtsets' + i + '"/><div class="input-group-btn"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Sets <span class="caret"></span></button><ul class="dropdown-menu pull-right"><li><a href="#">Sets</a></li><li><a href="#">Minutes</a></li></ul></div></div></div>');
                //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Repeats</label><input type="Text" class="form-control" id="txtRepeats' + i + '"/></div>');
                //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Days</label><select  class="form-control" id="txtdays' + i + '"><option>Mon-Tue-Wed-Thu-Fri-Sat</option><option>Mon-Wed-Fri</option><option>Mon-Wed</option><option>Mon-Thu</option><option>Mon</option><option>Wed</option><option>Fri</option><option>Tue-Thu-Sat</option><option>Tue-Fri</option><option>Wed-Sat</option><option>Tue</option><option>Thu</option><option>Sat</option><option>Sun</option></div>');
                //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Exercise Order</label><input type="Text" class="form-control" id="txtExcercise' + i + '"/></div>');
                //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Delete</label><span class="form-control" style="border-style:none;"> <a class="text-danger btndeleteWorkout" onclick="return DeleteMemberWorkout(this);"><i class="fa fa-remove" aria-hidden="true"></i></a></span></div></div>');

                var workouts = '<div class="row">'
                    + '<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control planid"  id="txtplanenameid' + i + '"></div></div>'
                    + '<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control"  id="txtMemberworkoutid' + i + '"></div></div>'
                    + '<div class="form-group col-md-2"><label>Workout</label><div class="input-group"><input type="Text" class="form-control txtwrkt"  id="txtworkout' + i + '"></div></div>'
                    + '<div class="form-group col-md-2"><label>Sets</label><div class="input-group"><input type="Text" class="form-control" id="txtsets' + i + '"/><div class="input-group-btn"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Sets <span class="caret"></span></button><ul class="dropdown-menu pull-right"><li><a onclick="return Sets(this);">Sets</a></li><li><a onclick="return Minutes(this);">Minutes</a></li></ul></div></div></div>'
                    + '<div class="form-group col-md-2"><label>Repeats</label><input type="Text" class="form-control" id="txtRepeats' + i + '"/></div>'
                    + '<div class="form-group col-md-2"><label>Days</label><select  class="form-control" id="txtdays' + i + '"><option>Mon-Tue-Wed-Thu-Fri-Sat</option><option>Mon-Wed-Fri</option><option>Mon-Wed</option><option>Mon-Thu</option><option>Mon</option><option>Wed</option><option>Fri</option><option>Tue-Thu-Sat</option><option>Tue-Fri</option><option>Wed-Sat</option><option>Tue</option><option>Thu</option><option>Sat</option><option>Sun</option></select></div>'
                    + '<div class="form-group col-md-2"><label>Exercise Order</label><input type="Text" class="form-control" id="txtExcercise' + i + '"/></div>'
                    + '<div class="form-group col-md-2"><label>Delete</label><span class="form-control" style="border-style:none;"> <a class="text-danger btndeleteWorkout" onclick="return DeleteMemberWorkout(this);"><i class="fa fa-remove" aria-hidden="true"></i></a></span></div></div>';



                $("#AddworkoutContainer").append(workouts);

                $('#txtplanenameid' + i + '').val(item.PlaneNameId);
                $('#txtMemberworkoutid' + i + '').val(item.MemberWorkoutPlanid);
                $('#txtworkout' + i + '').val(item.Workout);
                //$('#txtsets' + i + '').val(item.NumberOfSets);
                $('#txtRepeats' + i + '').val(item.Repeats);
                $('#txtdays' + i + '').val(item.NumberofDays);
                $('#txtExcercise' + i + '').val(item.ExcerciseOrder)
                if (item.NumberOfSets != null) {
                    $('#txtsets' + i + '').val(item.NumberOfSets);
                }
                else if (item.NumberOfMinutes != null) {
                    $('#txtsets' + i + '').val(item.NumberOfMinutes);
                    // var setMinValue = $('#txtsets' + i + '').parent().find("button").text();
                    $('#txtsets' + i + '').parent().find("button").text("");
                    $('#txtsets' + i + '').parent().find("button").append("Minutes" + " <span class='caret'></span>");
                    $('#txtsets' + i + '').parent().parent().find("label").text("Minutes");
                    $('#txtsets' + i + '').parent().parent().next().find("label").hide();
                    $('#txtsets' + i + '').parent().parent().next().find("input").hide();
                }
            })
            $(wrapper).find('.txtwrkt').autocomplete({
                source: availableAttributes
            });
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });

}



function EditMemberDietPlan() {
    $.ajax({
        url: EditMemberDietPlanUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $('#tblDietPlan tbody').empty();
            if (data != null) {
                $('#txtDietPlanName').val(data[0].DietPlanId);
                if ($('#txtDietPlanName').val() == null) {
                    var length = $('#txtDietPlanName > option').length;
                    if (length < 2) {
                        BindDietPlan();
                        $('#txtDietPlanName').val(data[0].DietPlanId);
                    }
                }
                $.each(data, function (i, item) {
                    var rows = "<tr>"
                        + "<td  style='display:none;'> <input type='Text' class='form-control' id=txtMemberdietplan" + i + "></td>"
                        + "<td  style='display:none;'> <input type='Text' class='form-control' id=txtdietplanid" + i + "></td>"
                        + "<td> <input type='Text' class='form-control' readonly id=txtMealTime" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtMonday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtTuesday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtWednesday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtThursday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtFriday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtSaturday" + i + "></td>"
                        + "<td> <input type='Text' class='form-control'  id=txtSunday" + i + "></td>"
                        + "</tr>";

                    $('#tblDietPlan tbody').append(rows);
                    $('#txtMemberdietplan' + i + '').val(item.MemberDietPlanid);
                    $('#txtdietplanid' + i + '').val(item.DietPlanId);
                    $('#txtMealTime' + i + '').val(item.DietTime);
                    $('#txtMonday' + i + '').val(item.MondayDiet);
                    $('#txtTuesday' + i + '').val(item.TuesdayDiet);
                    $('#txtWednesday' + i + '').val(item.WednesdayDiet);
                    $('#txtThursday' + i + '').val(item.ThursdayDiet);
                    $('#txtFriday' + i + '').val(item.FridayDiet);
                    $('#txtSaturday' + i + '').val(item.SaturdayDiet);
                    $('#txtSunday' + i + '').val(item.SundayDiet);
                })
            }

        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}



function GetMembershipFreezeList() {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetFreezeListUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { MemberId: $('#txtMemberId').val() },
        success: function (data) {
            $('#FreezeMembershipList tbody').empty();
            freezeData = null;
            if (data != null && data.length > 0) {
                freezeData = data;
                $.each(data, function (i, item) {
                    var rowno = i + 1;
                    var rows = '<tr>'
                        + '<td class="text-left">' + rowno + '</td>'
                        + '<td class="text-left">' + item.MemershipType.split(' ')[0] + '</td>'
                        + '<td class="text-left">' + item.StartDate + '</td>'
                        + '<td class="text-left">' + item.EndDate + '</td>'
                        + '<td class="text-right">' + item.Month + '</td>'
                        + '<td class="text-right">' + parseFloat(item.Amount).toFixed(2) + '</td>'
                        + '<td class="text-center">' + item.Status + '</td>'
                        + '<td class="text-center">'
                        + '<a href="javascript:void(0)" onclick="EditFreezeMembership(' + item.FreezeId + ')" title="Edit" class="btn btn-success btn-xs"><i class="fa fa-edit"></i> Edit</a> &nbsp;&nbsp;'
                        + '<a href="javascript:void(0)" onclick="DeleteFreezeMembership(' + item.FreezeId + ')" title="Delete" class="btn btn-danger btn-xs"><i class="fa fa-remove"></i> Delete</a>'
                        + '</td>';
                    +'</tr>';
                    $('#FreezeMembershipList tbody').append(rows);
                });
            }
            else {
                var norecords = "<tr>" +
                    '<td colspan="8" class="Nodata">No data available</td>'
                    + "</tr>";
                $('#FreezeMembershipList tbody').append(norecords);
            }
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
            toastr.error(errMsg.responseText);
        },
        error: function (errMsg) {
            HideLoader();
            toastr.error("Suresh");
        }
    });
}



function BindMeasurement() {
    $.ajax({
        url: MeasurementUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            if (data.length > 0) {
                $('#tblMemMeasurement tbody').empty();
                var rows;
                var i;
                var lastValue = data.length - 1;
                rows = "<tr><td></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    rows += "<th>" + data[i].MeasurementDate + " <span class='glyphicon glyphicon-edit' onclick = 'return ShowMeasurements(" + data[i].MeasurementId + ");'></span></th>";
                }
                rows += "</tr>"
                $('#tblMemMeasurement tbody').append(rows);

                //height
                height = "<tr><td><b>Height</b></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    var firstValue = data[lastValue].Height;
                    var diffFirst = data[i].Height;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].Height;
                        var present = data[i].Height;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    height += "<td><b>" + data[i].Height + "<span style='color:blue'>[" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                height += "</tr>"
                $('#tblMemMeasurement tbody').append(height);

                //weight
                weight = "<tr><td><b>Weight</b></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    var firstValue = data[lastValue].weight;
                    var diffFirst = data[i].weight;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].weight;
                        var present = data[i].weight;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    weight += "<td><b>" + data[i].weight + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                weight += "</tr>"
                $('#tblMemMeasurement tbody').append(weight);

                //Upper Arm
                upperArm = "<tr><td><b>Upper Arm</b></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    var firstValue = data[lastValue].UpperArm;
                    var diffFirst = data[i].UpperArm;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].UpperArm;
                        var present = data[i].UpperArm;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    upperArm += "<td><b>" + data[i].UpperArm + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                upperArm += "</tr>"
                $('#tblMemMeasurement tbody').append(upperArm);

                //ForeArm
                foreArm = "<tr><td><b>ForeArm</b></td>"
                for (i = 0; i <= data.length - 1; i++) {

                    var firstValue = data[lastValue].ForeArm;
                    var diffFirst = data[i].ForeArm;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].ForeArm;
                        var present = data[i].ForeArm;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    foreArm += "<td><b>" + data[i].ForeArm + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                foreArm += "</tr>"
                $('#tblMemMeasurement tbody').append(foreArm);


                //calves
                calves = "<tr><td><b>Calves</b></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    var firstValue = data[lastValue].Calves;
                    var diffFirst = data[i].Calves;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].Calves;
                        var present = data[i].Calves;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    calves += "<td><b>" + data[i].Calves + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                calves += "</tr>"
                $('#tblMemMeasurement tbody').append(calves);

                //BMI
                bmi = "<tr><td><b>BMI</b></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    var firstValue = data[lastValue].BMI;
                    var diffFirst = data[i].BMI;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].BMI;
                        var present = data[i].BMI;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    bmi += "<td><b>" + data[i].BMI + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                bmi += "</tr>"
                $('#tblMemMeasurement tbody').append(bmi);

                //BMI
                vfat = "<tr><td><b>V. Fat</b></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    var firstValue = data[lastValue].VFat;
                    var diffFirst = data[i].VFat;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].VFat;
                        var present = data[i].VFat;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    vfat += "<td><b>" + data[i].VFat + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                vfat += "</tr>"
                $('#tblMemMeasurement tbody').append(vfat);

                //Shoulder
                shoulder = "<tr><td><b>Shoulder</b></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    var firstValue = data[lastValue].Shoulder;
                    var diffFirst = data[i].Shoulder;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].Shoulder;
                        var present = data[i].Shoulder;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    shoulder += "<td><b>" + data[i].Shoulder + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                shoulder += "</tr>"
                $('#tblMemMeasurement tbody').append(shoulder);

                //chest
                chest = "<tr><td><b>Chest</b></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    var firstValue = data[lastValue].Chest;
                    var diffFirst = data[i].Chest;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].Chest;
                        var present = data[i].Chest;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    chest += "<td><b>" + data[i].Chest + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                chest += "</tr>"
                $('#tblMemMeasurement tbody').append(chest);

                //Arms
                arms = "<tr><td><b>Arms</b></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    var firstValue = data[lastValue].Arms;
                    var diffFirst = data[i].Arms;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].Arms;
                        var present = data[i].Arms;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    arms += "<td><b>" + data[i].Arms + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                arms += "</tr>"
                $('#tblMemMeasurement tbody').append(arms);

                //Upper ABS
                upperabs = "<tr><td><b>Upper ABS</b></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    var firstValue = data[lastValue].UpperABS;
                    var diffFirst = data[i].UpperABS;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].UpperABS;
                        var present = data[i].UpperABS;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    upperabs += "<td><b>" + data[i].UpperABS + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                upperabs += "</tr>"
                $('#tblMemMeasurement tbody').append(upperabs);

                //Waist ABS
                waistabs = "<tr><td><b>Waist ABS</b></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    var firstValue = data[lastValue].WaistABS;
                    var diffFirst = data[i].WaistABS;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].WaistABS;
                        var present = data[i].WaistABS;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    waistabs += "<td><b>" + data[i].WaistABS + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                waistabs += "</tr>"
                $('#tblMemMeasurement tbody').append(waistabs);

                //Lower ABS
                lowerabs = "<tr><td><b>Lower ABS</b></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    var firstValue = data[lastValue].LowerABS;
                    var diffFirst = data[i].LowerABS;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].LowerABS;
                        var present = data[i].LowerABS;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    lowerabs += "<td><b>" + data[i].LowerABS + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                lowerabs += "</tr>"
                $('#tblMemMeasurement tbody').append(lowerabs);

                //Glutes
                glutes = "<tr><td><b>Glutes</b></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    var firstValue = data[lastValue].Glutes;
                    var diffFirst = data[i].Glutes;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].Glutes;
                        var present = data[i].Glutes;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    glutes += "<td><b>" + data[i].Glutes + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                glutes += "</tr>"
                $('#tblMemMeasurement tbody').append(glutes);

                //Thighes
                thighes = "<tr><td><b>Thighes</b></td>"
                for (i = 0; i <= data.length - 1; i++) {
                    var firstValue = data[lastValue].Thighs;
                    var diffFirst = data[i].Thighs;
                    var differnceValue = diffFirst - firstValue;
                    var diffPrev = 0;
                    if (i < lastValue) {
                        var j = i + 1;
                        var prev = data[j].Thighs;
                        var present = data[i].Thighs;
                        diffPrev = present - prev;
                    }

                    if (diffPrev > 0) {
                        diffPrev = "+" + diffPrev;
                    }
                    if (differnceValue > 0) {
                        differnceValue = "+" + differnceValue;
                    }
                    thighes += "<td><b>" + data[i].Thighs + "<span style='color:blue'> [" + differnceValue + "]</span> <span style='color:orange'> [" + diffPrev + "]</span></b></td>";
                }
                thighes += "</tr>"
                $('#tblMemMeasurement tbody').append(thighes);
            }

        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        },
        error: function (errMsg) {
            alert(errMsg.responseText);
        }

    });
}



function BindImageMeasurements() {
    $.ajax({
        url: ImageMeasurementsUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $('#gymImage').attr('src', data[0].ImageData);
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        },
        error: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}



function BindMemberDetailsMeasurements() {
    $.ajax({
        url: BindMemberDetailMeasurementUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $('#customerName').text(data[0].FirstName + " " + data[0].LastName);
            $('#joiningDate').text(data[0].EnrollDate);
            $('#gender').text(data[0].Gender);
            $('#measurementDate').text(data[0].NextMeasurementDate);

        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        },
        error: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function BindSMSLog(details)
{
    $('#tblsms tbody').find("tr").remove();
    $.each(details, function (i, item) {
        var sentDate = ConvertDate(item.SmsDate);
        var rows = '<tr role="row" class="odd">'
            + '<td>' + sentDate + '</td>'
+ '<td>' + item.PhoneNumber + '</td>'
+ '<td>' + item.Message + '</td>'

+ '</tr >';
        $('#tblsms tbody').append(rows);

    });
    
}