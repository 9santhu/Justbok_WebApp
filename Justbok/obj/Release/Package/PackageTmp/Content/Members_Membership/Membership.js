var memberShipId, paymentId, PlanId;
var pageno = 1, pagerLoaded = false;


$(document).ready(function () {
    
    //  ShowLoader();

    
    $('input[type=datetime]').datepicker({
        dateFormat: "M dd, yy",
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
  
    $('#txtEnrollDate').datepicker('setDate', new Date());
    $('#txtMeasurementDate').datepicker('setDate', new Date());
    $('#txtSource1').hide();


    UpdatingDiet();
    BindPlanList();
   
    //$(document).on("click", "#btnSaveMember", function () { if (ValidateMeberInfo()) { SaveMember(); } });
    $('#btnSaveMember').click(function () { if (ValidateMeberInfo()) { SaveMember(); } });
    $("#txtDietPlanName").change(function () { WorkoutPlan(); });
    $("#txtMembershipType").change(function () { BindMembershipType(); });
    $('#btnEnroll').click(function () { if (ValidateMembership()) { AddMembership(); } });
    $(document).on('click', '#tblNewMembership tr', function () { memberShipId = $(this).closest('tr').find('input[type="hidden"]').val();});
    $('#paymentSave').click(function () { if (ValidatePayment()) { AddPayment(); } });
    //$(document).on('click', '#tblPaymentHistory tr', function () { BindPaymentHistory(); });
    $('#closepage').click(function () { $("#modal-printbody").dialog("close"); });
    $('#printpage').click(function () { PrintOrderReceipt(); });
    $("#imgInp").change(function () {readURL(this);});
    $("#btnUpload").click(function () { UploadImage(); });
    $('#addWorkout').click(function () {
        var wrapper = $("#AddworkoutContainer");
        var availableAttributes = [];
        availableAttributes = GetAutoCompleteOptions();
        AddWorkoutRow();
        $(wrapper).find('.txtwrkt').autocomplete({
            source: availableAttributes
        });

    });
    $("#txtWorkoutPlanName").change(function () { BindWorkoutPlan();});
    $('#btnSaveWorkout').click(function () { SaveWorkout(); });
    $('#addNewDiet').click(function () { AddNewDiet(); });
    $('#btnSaveDiet').click(function () { SaveDiet(); });
    $('#btnSaveMeasurement').click(function () { SaveMeasurement(); });

    //$(document).on('click', '.btnEditMembership', function () { EditMembership(); });
    $(document).ready(function () { }).on('click', '#btnYesPayment', function () { ConfirmDeleteMembership(); });
    $(document).ready(function () { }).on('click', '#btnYes', function () { ConfirmDeletePayment(); });
    $(document).on('click', '#showSelectMemberModal', function () { ShowReferenceModal(); });
    $(document).on('click', '#btnSelectCustSearch', function () { BindSelectMember(pageno, 10); });

    $("#txtSource").change(function () {
                var ddlSource = $("#txtSource option:selected").val();
                if (ddlSource == "Other") {
                    $("#txtSource1").show();
                    $("#txtSource1").val("");
                }
                else {
                    $("#txtSource1").hide();
                    $("#txtSource1").val("");
                }
    });

    
    if (enquiryId != "")
    {
        ShowLoader();
        EnquiryNewMember(enquiryId);
        
    }
    ShowWebcam();

    HideLoader();
    
});


function SaveMember() {
    ShowLoader();
    var jsonObject = {
         FirstName: $('#txtFirstName').val(), LastName: $('#txtLastName').val(), Dob: $('#txtDOB').val(),
        Email: $('#txtEmail').val(), MemberAddress: $('#txtAddress').val(), MobileNumber: $('#txtMobileno').val(), EnrollDate: $('#txtEnrollDate').val(),
        Gender: $("input[name='Gender']:checked").val(), MemberID: $('#txtMemberId').val(), MemberID: $('#txtMemberId').val(),
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
            toastr.success("Member Infomration Saved Successfully.");
            $('.nav-tabs a[href="#Membership"]').tab('show');
        },
        failure: function (errMsg) {
            HideLoader();
            console.log(errMsg.responseText);
        }
    });

}

function UpdatingDiet()
{
   
    $.ajax({
        url: DietPlanList,
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

function WorkoutPlan()
{
    PlanId = $("#txtDietPlanName option:selected").val();
               
    $.ajax({
        url: "BindDietWorkoutPlan",
        data: {palnid:PlanId},
    type: "GET",
    dataType: "json",
    success: function (data) {
        $('#tblDietPlan tbody').empty();
        $.each(data, function (i, item) {
            var rows = "<tr>"
                 + "<td style='display:none;'> <input type='Text' class='form-control'   id=DietPlanId" + i + "></td>"
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

            $('#DietPlanId' + i + '').val(item.DietPlanId);
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

function show(input) {
    if (input.files && input.files[0]) {
        var filerdr = new FileReader();
        filerdr.onload = function (e) {
            $('#user_img').attr('src', e.target.result);
        }
        filerdr.readAsDataURL(input.files[0]);
    }
}

function BindMembershipType()
{
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

function AddMembership()
{
    var jsonObject = {
        MembershipType: $('#txtMembershipType option:selected').val(), Months: $('#txtMonths').val(), Amount: $('#txtAmount').val(), StartDate: $('#txtStartDate').val(), Enddate: $('#txtEndate').val(),
        Note: $('#txtNote').val(), Status: "Active"
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: "AddMemberShip",
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObject),
    success: function (data) {
        onsuccess();
        HideLoader();
        SendMembershipSMS();
        //alert("Data Submitted");
        //toastr.success("Data Saved Successfully.");
    },
    failure: function (errMsg) {
        HideLoader();
        alert(errMsg.responseText);
    }
});

}

function SendMembershipSMS() {
    $.ajax({
        cache: false,
        type: "Get",
        url: GetMobileNumberUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { RequestType: "NewMembership" },
        success: function (data) {
            var msg = "";
            var mobileNumber = data[0].Mobile;
            var memberName = data[0].MemberName;
            var gender = data[0].GenderDetails;
            var enrollDate = data[0].EnrollDate;
            var membership = data[0].MembershipName;
            var gymName = data[0].GymName;
            var prefix = "";
            if (gender == "Male") {
                prefix = "Mr."
            }
            else {
                prefix = "Miss."
            }
            msg = "Dear " + prefix + memberName + ", your " + membership + " at " + gymName + "," + " on " + enrollDate + " registered."
            if (mobileNumber != "" && msg != "") {
                SendSMS(mobileNumber, msg);
            }
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });


}

function AddPayment()
{
    
    var jsonObject = {
        PaymentType: $('#ddlPaymentType option:selected').val(), PaymentAmount: $('#txtPaymentAmount').val(), PaymentDate: $('#txtPaymentDate').val(),
        PaymentDueDate: $('#txtDuedate').val(), ReferenceNumber: $('#txtReferenceNo').val(), MembershipId: memberShipId
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: "Payment",
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObject),
    success: function (data) {
        $('#modal-payment').modal('hide');
        onsuccess();
        onPaymentsuccess();
        HideLoader();
    },
    failure: function (errMsg) {
        HideLoader();
        alert(errMsg.responseText);
    }
});

}

function onsuccess()
{
    $.ajax({
        url: "BindMembership",
        data: "",
    type: "GET",
    dataType: "json",
    success: function (data) {
        $('#tblNewMembership tbody').empty();
        $.each(data, function (i, item) {
            var  dueAmount = 0;
            var membertype = item.MemershipType.split(' ');
            if (item.PaidAmount == undefined || item.PaidAmount == null) {
                item.PaidAmount = "-";
                dueAmount = parseInt(item.Amount) || 0;
            }
            else {
                var amount = parseInt(item.Amount)|| 0;
                var paidamount = parseInt(item.PaidAmount) || 0;
                dueAmount = amount-paidamount;
            }
            if (item.Status == undefined || item.Status == null) {
                item.Status = "-";
            }
            if (item.Notes == undefined || item.Notes == null) {
                item.Notes = "";
            }
            console.log(dueAmount);
            var rowno = i + 1;
            var rows = "<tr>"
                  + "<td style='display:none'>" + '<input type="hidden" name="hid" value=' + item.MembershipID + '>' + "</td>"
                + "<td>" + rowno + "</td>"
+ "<td>" + membertype[0] + "</td>"
+ "<td>" + item.StartDate + "</td>"
+ "<td>" + item.EndDate + "</td>"
+ "<td>" + item.Months + "</td>"
+ "<td>" + item.Amount + "</td>"
+ "<td ><span class='badge bg-red'>" + dueAmount + "</span></td>"
+ "<td><span class='badge bg-green'>" + item.PaidAmount  + "</span></td>"
+ "<td><span class='badge bg-green'>" + item.Status + "</span></td>"
+ "<td>" + item.Notes + "</td>"
+ "<td><a class='btn btn-info btnPayment' data-toggle='modal' data-target='#modal-payment' onclick='return BindPaymentData(this);'>Payment</a>&nbsp;<a class='btn btn-primary btnEditMembership' onclick='return EditMembership(this);'>Edit</a>&nbsp;<a class='btn btn-danger btnDeleteMembership' onclick='return DeleteMembership(this);'>Delete</a></td>"
+ "</tr>";
            $('#tblNewMembership tbody').append(rows);
                    
        });
    },
    error: function () {
        alert("Failed! Please try again.");
    }
});
}

function onPaymentsuccess()
{
    $.ajax({
        url: "BindPayment",
        data: {membershipid:memberShipId},
    type: "GET",
    dataType: "json",
    success: function (data) {
        $('#tblPaymentHistory tbody').empty();
        $.each(data, function (i, item) {
            var rows = "<tr>"
                  + "<td style='display:none'>" + '<input type="hidden" name="hid" value=' + item.RecieptNumber + '>' + "</td>"
+ "<td>" + item.PaymentDate + "</td>"
+ "<td>" + item.PaidAmount + "</td>"
+ "<td>" + item.PaymentType + "</td>"
+ "<td>" + item.RecieptNumber + "</td>"
+ "<td> <i class='fa fa-fw fa-edit'  onclick='return EditPayment(this);'></i></td></td>"
+ "<td><i class='fa fa-fw fa-close' data-toggle='modal' data-target='#modal_Conformation' onclick='return DeletePayment(this);'></i></td>"
+ "<td><a href='' data-toggle='modal' data-target='#modal-print' onclick='return PaymentHistory(this); ><i class='fa fa-fw fa-print'></i></a></td>"
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


function PrintOrderReceipt() {
    var printelem = document.getElementById('divMembership');
    var myWindow = window.open('', '', 'width=750,height=500', '_blank');
    myWindow.document.write(document.head.innerHTML);
    myWindow.document.write(printelem.innerHTML);
    myWindow.document.write("\x3Cscript>window.print(); window.close();\x3C/script>");
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

function UploadImage()
{
    var formData = new FormData();
    var totalFiles = document.getElementById("imgInp").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("imgInp").files[i];
        formData.append("imageUploadForm", file);
    }
    $.ajax({
        type: "POST",
        url: "UploadImage",
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
                alert("Error:Image not saved");
            }
        },
        error: function (error) {
            var obj = JSON.stringify(error);
            alert(obj);
            console.log(error.responseText);
        }
    });

}

function AddWorkoutRow()
{
    var i = $("#AddworkoutContainer select").length;

    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Workout</label><div class="input-group"><input type="Text" class="form-control"  id="txtworkout' + i + '"></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Sets</label><div class="input-group"><input type="Text" class="form-control" id="txtsets' + i + '"/><div class="input-group-btn"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Sets <span class="caret"></span></button><ul class="dropdown-menu pull-right"><li><a href="#">Sets</a></li><li><a href="#">Minutes</a></li></ul></div></div></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Repeats</label><input type="Text" class="form-control" id="txtRepeats' + i + '"/></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Days</label><select  class="form-control" id="txtdays' + i + '"><option>Mon-Tue-Wed-Thu-Fri-Sat</option><option>Mon-Wed-Fri</option><option>Mon-Wed</option><option>Mon-Thu</option><option>Mon</option><option>Wed</option><option>Fri</option><option>Tue-Thu-Sat</option><option>Tue-Fri</option><option>Wed-Sat</option><option>Tue</option><option>Thu</option><option>Sat</option><option>Sun</option></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Exercise Order</label><input type="Text" class="form-control" id="txtExcercise' + i + '"/></div>');
    //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Delete</label><span class="form-control" style="border-style: none;"> <a ><i class="fa fa-remove" aria-hidden="true"></i></a></span></div>');
    var workouts = '<div class="row">'
  + '<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control"  id="txtplanenameid' + i + '"></div></div>'
  + '<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control"  id="txtplaneid' + i + '"></div></div>'
  + '<div class="form-group col-md-2"><label>Workout</label><div class="input-group"><input type="Text" class="form-control txtwrkt"  id="txtworkout' + i + '"></div></div>'
  + '<div class="form-group col-md-2"><label>Sets</label><div class="input-group"><input type="Text" class="form-control" id="txtsets' + i + '"/><div class="input-group-btn"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Sets <span class="caret"></span></button><ul class="dropdown-menu pull-right"><li><a onclick="return Sets(this);">Sets</a></li><li><a onclick="return Minutes(this);">Minutes</a></li></ul></div></div></div>'
  + '<div class="form-group col-md-2"><label>Repeats</label><input type="Text" class="form-control" id="txtRepeats' + i + '"/></div>'
  + '<div class="form-group col-md-2"><label>Days</label><select  class="form-control" id="txtdays' + i + '"><option>Mon-Tue-Wed-Thu-Fri-Sat</option><option>Mon-Wed-Fri</option><option>Mon-Wed</option><option>Mon-Thu</option><option>Mon</option><option>Wed</option><option>Fri</option><option>Tue-Thu-Sat</option><option>Tue-Fri</option><option>Wed-Sat</option><option>Tue</option><option>Thu</option><option>Sat</option><option>Sun</option></select></div>'
  + '<div class="form-group col-md-2"><label>Exercise Order</label><input type="Text" class="form-control" id="txtExcercise' + i + '"/></div>'
  + '<div class="form-group col-md-2"><label>Delete</label><span class="form-control" style="border-style: none;"> <a class="text-danger btndeleteWorkout" onclick="return DeleteMemberWorkout(this);"><i class="fa fa-remove" aria-hidden="true"></i></a></span></div>'
  + '</div>'

    $("#AddworkoutContainer").append(workouts);
    i++;
}

function BindWorkoutPlan()
{
    PlanId = $("#txtWorkoutPlanName option:selected").val();
    $.ajax({
        url: "BindWorkoutPlan",
        data: {palnid:PlanId},
    type: "GET",
    dataType: "json",
    success: function (data) {
        $('#AddworkoutContainer').empty();

        $.each(data, function (i, item) {
            //$("#AddworkoutContainer").append('<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control"  id="txtplanenameid' + i + '"></div>');
            //$("#AddworkoutContainer").append('<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control"  id="txtplaneid' + i + '"></div>');
            //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Workout</label><div class="input-group"><input type="Text" class="form-control"  id="txtworkout' + i + '"></div>');
            //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Sets</label><div class="input-group"><input type="Text" class="form-control" id="txtsets' + i + '"/><div class="input-group-btn"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Sets <span class="caret"></span></button><ul class="dropdown-menu pull-right"><li><a href="#">Sets</a></li><li><a href="#">Minutes</a></li></ul></div></div></div>');
            //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Repeats</label><input type="Text" class="form-control" id="txtRepeats' + i + '"/></div>');
            //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Days</label><select  class="form-control" id="txtdays' + i + '"><option>Mon-Tue-Wed-Thu-Fri-Sat</option><option>Mon-Wed-Fri</option><option>Mon-Wed</option><option>Mon-Thu</option><option>Mon</option><option>Wed</option><option>Fri</option><option>Tue-Thu-Sat</option><option>Tue-Fri</option><option>Wed-Sat</option><option>Tue</option><option>Thu</option><option>Sat</option><option>Sun</option></div>');
            //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Exercise Order</label><input type="Text" class="form-control" id="txtExcercise' + i + '"/></div>');
            //$("#AddworkoutContainer").append('<div class="form-group col-md-2"><label>Delete</label><span class="form-control" style="border-style: none;"> <a ><i class="fa fa-remove" aria-hidden="true"></i></a></span></div>');


            var workouts = '<div class="row">'
           + '<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control planid"  id="txtplanenameid' + i + '"></div></div>'
           + '<div class="form-group col-md-2" style="display:none;"><label>WorkoutId</label><div class="input-group"><input type="Text" class="form-control"  id="txtMemberworkoutid' + i + '"></div></div>'
           + '<div class="form-group col-md-2"><label>Workout</label><div class="input-group"><input type="Text" class="form-control"  id="txtworkout' + i + '"></div></div>'
           + '<div class="form-group col-md-2"><label>Sets</label><div class="input-group"><input type="Text" class="form-control" id="txtsets' + i + '"/><div class="input-group-btn"><button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">Sets <span class="caret"></span></button><ul class="dropdown-menu pull-right"><li><a onclick="return Sets(this);">Sets</a></li><li><a onclick="return Minutes(this);">Minutes</a></li></ul></div></div></div>'
           + '<div class="form-group col-md-2"><label>Repeats</label><input type="Text" class="form-control" id="txtRepeats' + i + '"/></div>'
           + '<div class="form-group col-md-2"><label>Days</label><select  class="form-control" id="txtdays' + i + '"><option>Mon-Tue-Wed-Thu-Fri-Sat</option><option>Mon-Wed-Fri</option><option>Mon-Wed</option><option>Mon-Thu</option><option>Mon</option><option>Wed</option><option>Fri</option><option>Tue-Thu-Sat</option><option>Tue-Fri</option><option>Wed-Sat</option><option>Tue</option><option>Thu</option><option>Sat</option><option>Sun</option></select></div>'
           + '<div class="form-group col-md-2"><label>Exercise Order</label><input type="Text" class="form-control" id="txtExcercise' + i + '"/></div>'
           + '<div class="form-group col-md-2"><label>Delete</label><span class="form-control" style="border-style:none;"> <a class="text-danger btndeleteWorkout" onclick="return DeleteMemberWorkout(this);"><i class="fa fa-remove" aria-hidden="true"></i></a></span></div></div>';

            $("#AddworkoutContainer").append(workouts);

            $('#txtplanenameid' + i + '').val(item.PlaneNameId);
            $('#txtplaneid' + i + '').val(item.Planid);
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
                $('#txtsets' + i + '').parent().parent().next().hide();
            }
        })
    },
    failure: function (errMsg) {
        alert(errMsg.responseText);
    }
});
}

function DeleteMemberWorkout(id) {

    var $curr = $(id).closest('div');
    $curr = $curr.parent();
    $curr.remove();
}


function SaveWorkout()
{
    ShowLoader();
    var count = $("#AddworkoutContainer select").length;
    var jsonObj = [];
    jsonObj.push({ "PlanId":PlanId });
    for (i = 0; i <= count; i++)
    {
        var setMinValue = $('#txtsets' + i + '').parent().find("button").text();
        jsonObj.push(
            { "Workout": $('#txtworkout' + i + '').val(), "NumberOfSets": $('#txtsets' + i + '').val(), "Repeats": $('#txtRepeats' + i + '').val(), "NumberofDays": $('#txtdays' + i + '').val(), "ExcerciseOrder": $('#txtExcercise' + i + '').val(), "SetMin": setMinValue }
            );
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: "AddMemberWorkoutPlan",
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObj),
    success: function (data) {
        // onsuccess();
        HideLoader();
        toastr.success("Data Saved Successfully.");
       // alert("Data Submitted");
    },
    failure: function (errMsg) {
        alert(errMsg.responseText);
    }
});
}

function BindPlanList()
{
     $.ajax({
        url: GetPlanListUrl,
        data:{ BranchId: $('#ddlBranch option:selected').val() },
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

function AddNewDiet()
{
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

function SaveDiet()
{
    ShowLoader();
    var rowCount = $('#tblDietPlan tr').length;
           
    var jsonObj = [];
    jsonObj.push({ "DietPlanName1": $('#txtDietPlanName').val() });
    for (i = 0; i <= rowCount; i++) {
        jsonObj.push(
           {"DietPlanId": $('#txtdietplanid' + i + '').val(), "MealTime1": $('#txtMealTime' + i + '').val(), "MondayDiet": $('#txtMonday' + i + '').val(), "TuesdayDiet": $('#txtTuesday' + i + '').val(), "WednesdayDiet": $('#txtWednesday' + i + '').val(), "ThursdayDiet": $('#txtThursday' + i + '').val(), "FridayDiet": $('#txtFriday' + i + '').val(), "SaturdayDiet": $('#txtSaturday' + i + '').val(), "SundayDiet": $('#txtSunday' + i + '').val() }
           );
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: "AddMemberDietPlan",
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify(jsonObj),
    success: function (data) {
        HideLoader();
        toastr.success("Data Saved Successfully.");
        //alert("Data Submitted");
    },
    failure: function (errMsg) {
        HideLoader();
        alert(errMsg.responseText);
    },
    error: function (errMsg) { HideLoader(); },
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
        }
    });

}

function EditMembership(id) {
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var membershipId = $('#tblNewMembership tr').eq(rowIndex + 1).find('td').eq(0).find('input[type="hidden"]').val();
    alert
        $.ajax({
            cache: false,
            type: "GET",
            url: EditBindMembership,
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

}

function DeleteMembership(id) {
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var membershipId = $('#tblNewMembership tr').eq(rowIndex + 1).find('td').eq(0).find('input[type="hidden"]').val();
    $('#txtMembershipid').val(membershipId);
    $('#modal_ConformationPayment').modal('show');
}

function ConfirmDeleteMembership() {
    $('#modal_ConformationPayment').modal('hide');
    ShowLoader();
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
            HideLoader();
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
            $("#customerName").text(data.FirstName);
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

function EnquiryNewMember(enquiryid)
{
    
   
        $.ajax({
            cache: false,
            type: "GET",
            url: NewMemberUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: { EnquiryId: enquiryid },
            success: function (data) {
               
                $("#txtFirstName").val(data[0].FirstName);
                $("#txtLastName").val(data[0].LastName);
                $("#txtMobileno").val(data[0].MobileNumber);
                $("#txtDOB").val(data[0].DOB);
                $("#txtEmail").val(data[0].EmailId);
                $("input[name=Gender][value=" + data[0].Gender + "]").attr('checked', 'checked');
                //$("input[name='Gender']:checked").val(data[0].Gender);
                //$("#Gender").val(data[0].Gender);
                $("#txtAddress").val(data[0].Address);
                $("#txtPhoneResi").val(data[0].PhoneNumberOffice);
                $("#txtPhoneofc").val(data[0].PhoneNumberResidence);
                HideLoader();
            },
            failure: function (errMsg) {
                HideLoader();
                alert(errMsg.responseText);
            }
        });
        
}

function ShowWebcam() {
    $("#Camera").webcam({
        width: 320,
        height: 240,
        mode: "save",
        swffile: swf,
        onTick: function () { },
        onSave: function (data, ab) {

        },
        onCapture: function () {
            webcam.save(SaveWebcamPic);
        },
        debug: function (type, status) {
            $('#camStatus').append(type + ": " + status + '<br /><br />');
        },
        onLoad: function () { }
    });

}

function SaveMeasurement()
{
    var jsonObject = {
       MeasurementDate: $('#txtMeasurementDate').val(), NextMeasurementDate: $('#txtNextMeasurementDate').val(), Height: $('#txtHeight').val(), weight: $('#txtWeight').val(),
        UpperArm: $('#txtUpperArm').val(), ForeArm: $('#txtForeArm').val(), Calves: $('#txtCalves').val(), BMI: $('#txtBMI').val(),
        VFat: $('#txtVFat').val(), Shoulder: $('#txtShoulder').val(), Chest: $('#txtChest').val(), Arms: $('#txtArms').val(),
        UpperABS: $('#txtUpperABS').val(), WaistABS: $('#txtWaistABS').val(), LowerABS: $('#txtLowerABS').val(), Glutes: $('#txtGlutes').val(), Thighs: $('#txtThighes').val()
    }

    $.ajax({
        cache: false,
        type: "POST",
        url: AddMeasurementUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            toastr.success("Data Saved Successfully.");
            $('.nav-tabs a[href="#Workout"]').tab('show');
            //alert("Data Submitted");
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });

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


function ShowReferenceModal(pageno, pagesize) {
    $('#tblReferenceId').hide();
    $('#modal_SearchMemberId').modal('show');

}

function BindSelectMember(pageno, pagesize) {
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
                        + '<td>' + item.MemberName + '</td>'
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
                                BindSelectMember(pageno, 10);
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


function SelectCustomer(id) {
    $('#modal_SearchMemberId').modal('hide');
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var memberid = $('#tblReferenceId tr').eq(rowIndex + 1).find('td').eq(1).html();
    $('#txtReferenceBy').val(memberid);


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

    if ($('#txtEmail').val() != "")
    {
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
            || event.keyCode == 8  || event.keyCode == 9 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 46 || event.keyCode == 190) {

        } else {
            event.preventDefault();
        }
        if ($(this).val().indexOf('.') !== -1 && (event.keyCode == 190 || event.keyCode == 110))
            event.preventDefault();

    });
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

function GetSelectedCustMembershipData(Id, Type) {


    var subpagerLoaded = false, subpageNo = 1, freezeData = null;

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