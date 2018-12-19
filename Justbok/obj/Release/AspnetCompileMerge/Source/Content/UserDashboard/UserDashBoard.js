$(document).ready(function () {
    GetUserDetails();
    GetMemberImage();
    BindMemberShip();
});

function GetUserDetails()
{
    ShowLoader();
    var x = $('#myHiddenVar').val();
    var userid = x.toString();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetMemberInfoUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { MemberId: userid },
        success: function (data) {
            $.each(data, function (i, item) {
                $('#lblMemberId').html(item.MemberID);
                $('#lblMemberName').html(item.FirstName)
                $('#lblPhoneNumber').html(item.MobileNumber)
                $('#lblEnrollDate').html(item.EnrollDate)
                //$('#lblMembershipType').html(item.Package)
                //$('#lblStatus').html(item.Status)
                $('#customerimage').attr("src", item.ImageData);
                $('#lblUserName').html(item.FirstName);
                $('#userimage').attr("src", item.ImageData);
               
            })

        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });


}

function GetMemberImage() {
    var x = $('#myHiddenVar').val();
    var userid = x.toString();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetMemberImageUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { MemberId: userid },
        success: function (data) {
            $.each(data, function (i, item) {
                $('#customerimage').attr("src", item.ImageData);
            })

        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });


}

function BindMemberShip() {
    ShowLoader();
    var x = $('#myHiddenVar').val();
    var userid = x.toString();
    $.ajax({
        cache: false,
        type: "GET",
        url: EditBindMembershipUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { MemberId: userid },
        success: function (data) {

            membershipdata = data;
            $('#tblMembership tbody').empty();
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
                if (item.Notes == undefined || item.Notes == null) {
                    item.Notes = "";
                }
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
    + "<td><span class='badge bg-green'>" + item.PaidAmount + "</span></td>"
    + "<td><span class='badge bg-green'>" + item.Status + "</span></td>"
    + "<td>" + item.Notes + "</td>"
    + "<td><a class='btn btn-info btnPayment' data-toggle='modal' data-target='#modal-payment' onclick='return BindPaymentData(this);'>View Payments</a></td>"
    + "</tr>";
                $('#tblMembership tbody').append(rows);
               
            });
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
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
            $('#tblPaymentHistory tbody').empty();
            if (data != "") {
                $.each(data, function (i, item) {
                    var rows = "<tr>"
                    + "<td style='display:none'>" + '<input type="hidden" name="hid" value=' + item.RecieptNumber + '>' + "</td>"
                    + "<td>" + item.PaymentDate + "</td>"
                    + "<td>" + item.PaidAmount + "</td>"
                    + "<td>" + item.PaymentType + "</td>"
                    + "<td>" + item.RecieptNumber + "</td>"
                    + "</tr>";
                    $('#tblPaymentHistory tbody').append(rows);
                });
            }
            else {
                var rows = "<tr>"
                     + "<td colspan='4'>No data available</td>"
                 + "</tr>";
                $('#tblPaymentHistory tbody').append(rows);
            }

            HideLoader();
        },
        error: function () {
            HideLoader();
            alert("Failed! Please try again (BindPayment).");
        }
    });

}