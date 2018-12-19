var DueData = 0;
var branch = "";

$(document).ready(function () {
    Dashboard();
    $(document).ready(function () { }).on('click', '#btnShowStats', function () { RemoveError(); ShowStaticsPopup(); });
    $(document).ready(function () { }).on('click', '#btnValidatePwd', function () { if (ValidatePassword()) { ShowStatistics(); } });
    
});

function Dashboard() {
    branch = $("#ddlBranch option:selected").val();
    NewDashboard();
    //HideLoader();
}

function ShowStaticsPopup()
{
    var btnValue = $("#btnShowStats").val();
    if (btnValue == "Show") {
        $('#modal_ShowStatistics').modal('show');
        $('#txtPassword').val("");
    }
else if (btnValue == "Hide") {
    $('#showStats').hide();
    $("#btnShowStats").html('Show Stats');
    $("#btnShowStats").add("<i>").addClass("fa fa-search");
    $("#btnShowStats").prop('value', 'Show');
}
    
}

function ShowStatistics() {
    var pwd = $('#txtPassword').val();
        $.ajax({
            cache: false,
            type: "GET",
            url: ValidatePwdUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: { pwd: pwd },
            success: function (data) {
                if (data == "success") {
                    $('#modal_ShowStatistics').modal('hide');
                    $('#showStats').show();
                    $("#btnShowStats").html('Hide Stats');
                    $("#btnShowStats").add("<i>").addClass("fa fa-sign-out");
                    $("#btnShowStats").prop('value', 'Hide');
                }
                else {
                    toastr.error("Invalid password.Please try again");
                }

            },
            error: function () {
               
            }
        });
    
}
var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';
function ValidatePassword() {
    var IsValid = true;
    $('#txtPassword').parent().removeClass("has-error");
    $('#txtPassword').parent().find(".help-block-error").remove();
       
    if ($('#txtPassword').val() == "") {

        $('#txtPassword').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Password"));
        $('#txtPassword').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtPassword').parent().removeClass("has-error");
        $('#txtPassword').parent().find(".help-block-error").remove();
    }
    return IsValid;
}

function RemoveError()
{
    $('#txtPassword').parent().removeClass("has-error");
    $('#txtPassword').parent().find(".help-block-error").remove();
}

window.onhashchange = function () {
    if (window.innerDocClick) {
        window.innerDocClick = false;
    } else {
        if (window.location.hash != '#undefined') {
            goBack();
        } else {
            history.pushState("", document.title, window.location.pathname);
            location.reload();
        }
    }
}

function goBack() {
    window.location.hash = window.location.lasthash[window.location.lasthash.length - 1];
    //blah blah blah
    window.location.lasthash.pop();
}

function NewDashboard()
{
    ShowLoader();
    $.ajax({
        type: "GET",
        url: ListOfMeasurementUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { BranchId: branch },
        success: function (data) {
            $('#tblMembershipMeasurement tbody').empty();
            $('#tblFollowups tbody').empty();
            $('#tblLatestMember tbody').empty();
            $('#tblAnniversies tbody').empty();
            $('#tblMembershipDue tbody').empty();
            $('#tblMembershipExpired tbody').empty();
            $('#tblPaymentsDue tbody').empty();
            $('#tblBirthday tbody').empty();
            $('#ttlDueAmount tbody').empty();
          
            //Measurement Table Binding
            if (data.NewMeasurement.Result.length > 0) {
                BindMeasurement(data.NewMeasurement.Result);
            }
            else {
                var rows = "<tr><td colspan='4' align='center'> No Measurements found.</td></tr>";
                $('#tblMembershipMeasurement tbody').append(rows);
            }

            //FollowupToday table binding

            if (data.NewFollowup.Result.length > 0) {
                TotalFollowup(data.NewFollowup.Result);
            }
            else {
                var rows = "<tr><td colspan='4' align='center'> No Followups found.</td></tr>";
                $('#tblFollowups tbody').append(rows);
            }

            //Latest member list
            if (data.LatestMember.Result.length > 0) {
                TotalMemberList(data.LatestMember.Result);
            }
            else {
                var rows = "<tr><td colspan='4' align='center'> No Latest Members found.</td></tr>";
                $('#tblLatestMember tbody').append(rows);
            }
            //Anniversary List
            if (data.Anniversary.Result.length > 0) {
                AnniversaryList(data.Anniversary.Result);
            }
            else {
                var rows = "<tr><td colspan='4' align='center'> No Anniversary found.</td></tr>";
                $('#tblAnniversies tbody').append(rows);
            }
            //Membership due
            if (data.MembershipDue.Result.length > 0) {
                MembershipDue(data.MembershipDue.Result);
            }
            else {
                var rows = "<tr><td colspan='4' align='center'> No Due Membership found.</td></tr>";
                $('#tblMembershipDue tbody').append(rows);
            }
            //Expired Membership
            if (data.MembersExpired.Result.length > 0) {
                MembershipExpiredList(data.MembersExpired.Result);
            }
            else {
                var rows = "<tr><td colspan='4' align='center'> No Expired Membership found.</td></tr>";
                $('#tblMembershipExpired tbody').append(rows);
            }
            //Due Payment
            if (data.MembershipDuePayment.Result.length > 0) {
                MembershipDuePaymentList(data.MembershipDuePayment.Result);
            }
            else {
                var rows = "<tr><td colspan='4' align='center'> No Due Payments found.</td></tr>";
                $('#tblPaymentsDue tbody').append(rows);
            }
            //Birthday
            if (data.Birthday.Result.length > 0) {
                BirthDay(data.Birthday.Result);
            }
            else {
                var rows = "<tr><td colspan='4' align='center'> No Birthdays found.</td></tr>";
                $('#tblBirthday tbody').append(rows);
            }

            //Total Due Payment
            if (data.TotalDueAmount.Result.length > 0) {
                GetTotalDueAmountList(data.TotalDueAmount.Result);
            }
            else {
                var rows = "<tr><td colspan='4' align='center'> No Due Payments found.</td></tr>";
                $('#ttlDueAmount tbody').append(rows);
            }

            //Today Due Amount
            if (data.TodayDueAmount.Result.length > 0) {
                TodayDueAmountList(data.TodayDueAmount.Result);
            }
          

            //week Due Amount
            if (data.WeekDueAmount.Result.length > 0) {
                WeekDueAmountList(data.WeekDueAmount.Result);
            }
           

            //month Due Amount
            if (data.MonthDueAmount.Result.length > 0) {
                MonthDueAmountList(data.MonthDueAmount.Result);
            }

            //Today Sold Membership
            if (data.TodaySoldMenership.Result.length > 0) {
                TodaySoldMembershipList(data.TodaySoldMenership.Result);
            }

            //Week Sold Membership
            if (data.WeekSoldMenership.Result.length > 0) {
               WeekSoldMembershipList(data.WeekSoldMenership.Result);
            }

            //Month Sold Membership
            if (data.MonthSoldMenership.Result.length > 0) {
                MonthSoldMembershipList(data.MonthSoldMenership.Result);
            }
          
            //Enquiry Details day
            if (data.EnquiryDayList.Result.length > 0) {
                EnquiryDetailsTodayList(data.EnquiryDayList.Result);
            }
            
            //Enquiry Details week
            if (data.EnquiryWeekList.Result.length > 0) {
                EnquiryDetailsWeekList(data.EnquiryWeekList.Result);
            }
            
            //Enquiry Details Month
            if (data.EnquiryMonthList.Result.length > 0) {
                EnquiryDetailsMonthList(data.EnquiryMonthList.Result);
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

function BindMeasurement(measurement)
{
    $.each(measurement, function (i, item) {
        if (i <= 10) {
            var measurementDate="";
            var nextMeasurementDate="";
            if (item.MeasurementDate != null) {
                measurementDate = ConvertDate(item.MeasurementDate);
            }
          
            if (item.NextMeasurementDate) {
                nextMeasurementDate = ConvertDate(item.NextMeasurementDate);
            }
           
            var rows = "<tr>"
                 + "<td style='display:none'>" + '<input type="text" value=' + item.MemberID + '>' + "</td>"
+ '<td> <a onclick="return EditMember(' + item.MemberID + ')">' + item.FirstName + " " + item.LastName + "</a></td>"
+ "<td>" + item.MobileNumber + "</td>"
+ "<td>" + measurementDate + "</td>"
+ "<td>" + nextMeasurementDate + "</td>"

+ "</tr>";
            $('#tblMembershipMeasurement tbody').append(rows);
        }
    });

}

function TotalFollowup(followup)
{
    $.each(followup, function (i, item) {
        if (i <= 10) {
            var nextFolluwupDate = "";
            var mobilenumber = "";
            var gender = "";
            if (item.NextFollowUpDate != null) {
                nextFolluwupDate = ConvertDate(item.NextFollowUpDate);
            }
            if(item.MobileNumber!=null)
            {
                mobilenumber=item.MobileNumber;
            }
            if (item.Gender != null) {
                gender = item.Gender;
            }
            var rows = "<tr>"
          + "<td style='display:none'>" + '<input type="text" value=' + item.EnquiryId + '>' + "</td>"
          + "<td style='display:none'>" + '<input type="text" value=' + item.FollowupId + '>' + "</td>"
          + "<td><a onclick='return EditEnquiry(" + item.EnquiryId + ")'>" + item.FirstName + " " + item.LastName + "</td>"
          + "<td>" +mobilenumber + "</td>"
          + "<td>" + gender + "</td>"
          + "<td>" + nextFolluwupDate + "</td>"
          + "</tr>";
            $('#tblFollowups tbody').append(rows);
            // HideLoader();
        }
    });
}

function TotalMemberList(memberlist)
{
    $.each(memberlist, function (i, item) {
        if (i <= 10) {

            var email = "";
            var mobilenum = "";
            var gender = "";
            if (item.Email != null)
            {
                email = item.Email;
            }
            
            if (item.MobileNumber)
            {
                mobilenum = item.MobileNumber;
            }
            if (item.Gender) {
                gender = item.Gender;
            }
            var rows = "<tr>"
                 + "<td style='display:none'>" + '<input type="text" value=' + item.MemberID + '>' + "</td>"
+ '<td> <a onclick="return EditMember(' + item.MemberID + ')">' + item.FirstName + " " + item.LastName + "</a></td>"
+ "<td>" + email + "</td>"
+ "<td>" + mobilenum + "</td>"
+ "<td>" + gender + "</td>"
+ "</tr>";
            $('#tblLatestMember tbody').append(rows);
        }
    });
}

function AnniversaryList(anniversary)
{
    $.each(anniversary, function (i, item) {
        if (i <= 10) {
            var anniversaryDate = "";
            var mobileNumber = "";
            var gender = "";
            if (item.AnniversaryDate!=null)
            {
                anniversaryDate = ConvertDate(item.AnniversaryDate);
            }
            if (item.MobileNumber != null)
            {
                mobileNumber = item.MobileNumber;
            }
            if (item.Gender != null)
            {
                gender = item.Gender;
            }
            var rows = "<tr>"
                 + "<td style='display:none'>" + '<input type="text" value=' + item.MemberID + '>' + "</td>"
+ '<td> <a onclick="return EditMember(' + item.MemberID + ')">' + item.FirstName + " " + item.LastName + "</a></td>"
+ "<td>" + anniversaryDate + "</td>"
+ "<td>" + mobileNumber + "</td>"
+ "<td>" + gender + "</td>"

+ "</tr>";
            $('#tblAnniversies tbody').append(rows);
        }
    });
}

function MembershipDue(duelist)
{
    DueData = duelist;
    $.each(duelist, function (i, item) {
        if (i <= 10) {
            var membership = "";
            var startDate = "";
            var endDate = "";
            if (item.MembershipType != null)
            {
                var membertype = item.MembershipType.split(' ');
                membership = membertype[1] + " " + membertype[2];
            }
            if (item.StartDate != null)
            {
                startDate = ConvertDate(item.StartDate);
            }

            if (item.EndDate != null) {
                endDate = ConvertDate(item.EndDate);
            }
               
            var rows = "<tr>"
                 + "<td style='display:none'>" + '<input type="text" value=' + item.MemberID + '>' + "</td>"
                  + "<td style='display:none'>" + '<input type="text" value=' + item.MembershipID + '>' + "</td>"
+ '<td> <a onclick="return EditMember(' + item.MemberID + ')">' + item.FirstName + " " + item.LastName + "</a></td>"
+ "<td>" + membership + "</td>"
+ "<td>" + startDate + "</td>"
+ "<td>" + endDate + "</td>"
+ "</tr>";
            $('#tblMembershipDue tbody').append(rows);
        }
    });
}

function MembershipExpiredList(expired)
{
    $.each(expired, function (i, item) {
        if (i <= 10) {
            var membership = "";
            var startDate = "";
            var endDate = "";
            if (item.MembershipType != null) {
                var membertype = item.MembershipType.split(' ');
                membership = membertype[1] + " " + membertype[2];
            }
            if (item.StartDate != null) {
                startDate = ConvertDate(item.StartDate);
            }

            if (item.EndDate != null) {
                endDate = ConvertDate(item.EndDate);
            }
                var rows = "<tr>"
                     + "<td style='display:none'>" + '<input type="text" value=' + item.MemberID + '>' + "</td>"
                      + "<td style='display:none'>" + '<input type="text" value=' + item.MembershipID + '>' + "</td>"
    + "<td><a onclick='return EditMember(" + item.MemberID + ")'>" + item.FirstName + " " + item.LastName + "</td>"
    + "<td>" + membership + "</td>"
    + "<td>" + startDate + "</td>"
    + "<td>" + endDate + "</td>"
    + "</tr>";
                $('#tblMembershipExpired tbody').append(rows);
                // HideLoader();
            
            }
           
    });
}

function MembershipDuePaymentList(duePayment)
{
    $.each(duePayment, function (i, item) {
        if (i <= 10) {

            var membership = "";
            var paymentDueDate = "";
            var endDate = "";
            if (item.MembershipType != null) {
                var membertype = item.MembershipType.split(' ');
                membership = membertype[1] + " " + membertype[2];
            }
            if (item.PaymentDueDate != null) {
                paymentDueDate = ConvertDate(item.PaymentDueDate);
            }
         
            var dueAmount = item.Amount - item.PaymentAmount;
            var rows = "<tr>"
                 + "<td style='display:none'>" + '<input type="text" value=' + item.MemberID + '>' + "</td>"
                  + "<td style='display:none'>" + '<input type="text" value=' + item.MembershipID + '>' + "</td>"
+ "<td><a onclick='return EditMember(" + item.MemberID + ")'>" + item.FirstName + " " + item.LastName + "</td>"
+ "<td>" + membership + "</td>"
+ "<td>" + paymentDueDate + "</td>"
+ "<td>" + dueAmount + "</td>"
+ "<td>" + item.PaymentAmount + "</td>"
+ "</tr>";
            $('#tblPaymentsDue tbody').append(rows);
        }
    });
}

function BirthDay(birthday)
{
    $.each(data, function (i, item) {
        if (i <= 10) {
            var dob = "";
            var mobile = "";
            var gender = "";

            if (item.Dob != null)
            {
                dob = ConvertDate(item.dob);
            }

            if (item.MobileNumber != null) {
                mobile = item.MobileNumber;
            }

            if (item.Gender != null) {
                gender = item.Gender;
            }

            var rows = "<tr>"
                 + "<td style='display:none'>" + '<input type="text" value=' + item.MemberID + '>' + "</td>"
+ '<td> <a onclick="return EditMember(' + item.MemberID + ')">' + item.FirstName + " " + item.LastName + "</a></td>"
+ "<td>" + dob + "</td>"
+ "<td>" + mobile + "</td>"
+ "<td>" + gender + "</td>"
+ "</tr>";
            $('#tblBirthday tbody').append(rows);
            // HideLoader();
        }
    });
}

function GetTotalDueAmountList(dueamount)
{
    var totalDueAmt = 0;
    if (DueData != null) {
        $.each(DueData, function (i, item) {
            var dueAmount = item.Amount - item.PaymentAmount;
            totalDueAmt = totalDueAmt + dueAmount;
        });
        $('#ttlDueAmount').text(totalDueAmt);
    }
    else
    {
        MembershipDue(dueamount);
    }

}

function TodayDueAmountList(dueamount)
{
    var duedate = 0;
    $.each(dueamount, function (i, item) {
            var dueAmount = item.Amount - item.PaymentAmount;
            duedate = duedate + dueAmount;
        });
        $('#tdaydueamount').text(duedate);
}

function WeekDueAmountList(dueamount) {
    var duedate = 0;
    $.each(dueamount, function (i, item) {
            var dueAmount = item.Amount - item.PaymentAmount;
            duedate = duedate + dueAmount;
        });
        $('#weekdueamount').text(duedate);
}

function MonthDueAmountList(dueamount) {
    var duedate = 0;
    $.each(dueamount, function (i, item) {
            var dueAmount = item.Amount - item.PaymentAmount;
            duedate = duedate + dueAmount;
        });
        $('#monthdueamount').text(duedate);
}

function TodaySoldMembershipList(sold)
{
    $('#daymembershipsold').text(data.length);
    var amount = 0;
    $.each(sold, function (i, obj) {
        cAmount = parseInt(obj.Amount);
        amount = amount + cAmount;
    });
    $('#amountDay').text(amount);
}

function WeekSoldMembershipList(sold) {
    $('#weekMembershipSold').text(sold.length);
    var amount = 0;
    $.each(sold, function (i, obj) {
        cAmount = parseInt(obj.Amount);
        amount = amount + cAmount;
    });
    $('#amountWeek').text(amount);
}

function MonthSoldMembershipList(sold) {
    $('#monthMembershipSold').text(sold.length);
    var amount = 0;
    $.each(sold, function (i, obj) {
        cAmount = parseInt(obj.Amount);
        amount = amount + cAmount;
    });
    $('#amountMonth').text(amount);
}

function EnquiryDetailsTodayList(enquiry)
{
    $('#enquiryDay').text(enquiry.length);
}

function EnquiryDetailsWeekList(enquiry) {
    $('#enquiryWeek').text(enquiry.length);
}

function EnquiryDetailsMonthList(enquiry) {
    $('#enquiryMonth').text(enquiry.length);
}



