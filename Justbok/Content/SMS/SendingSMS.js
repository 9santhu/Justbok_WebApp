var pageNo = 1, pagerLoaded = false;
var url = "";

$(document).ready(function () {
    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
        BindAllMembers(pageNo, $("#tblMemberList_length").val());
    }
    else {
        BindAllMembers(pageNo, $("#tblMemberList_length").val());
    }
    if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
        BindAllEnquiry(pageNo, $("#tblMemberList_length").val());
    }
    else {
        BindAllEnquiry(pageNo, $("#tblMemberList_length").val());
    }
    if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
        BindDueMembership(pageNo, $("#tblMemberList_length").val());
    }
    else {
        BindDueMembership(pageNo, $("#tblMemberList_length").val());
    }
    if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
        BindExpiredMembership(pageNo, $("#tblMemberList_length").val());
    }
    else {
        BindExpiredMembership(pageNo, $("#tblMemberList_length").val());
    }
    if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
        BindPaymentDue(pageNo, $("#tblMemberList_length").val());
    }
    else {
        BindPaymentDue(pageNo, $("#tblMemberList_length").val());
    }
    var radbtn_ischecked = $('.AllMembers').find('input[type="radio"]').is(':checked');
    if (radbtn_ischecked != true)
    {
        $('.AllMembers').find('input[type="radio"]').prop('checked', true);
    }
    $('.chkmaster').click(function (e) { $(this).closest('table').find('td input:checkbox').prop('checked', this.checked); });
    $(".results").on("click", ".AllMembers", function (event) {
        $(".btnMembers").html("All Members");
        $('#divDueMembership').hide();
        $('#divExpiredMembership').hide();
        $('#divAllEnquiry').hide();
        $('#divPaymentDue').hide();
        $('#divMemberList').show();
        $('#divDates').hide();
    }); 
    $(".results").on("click", ".DueMembership", function (event) {
        $(".btnMembers").text("Due Membership");
        $('#divMemberList').hide();
        $('#divExpiredMembership').hide();
        $('#divAllEnquiry').hide();
        $('#divPaymentDue').hide();
        $('#divDueMembership').show();
        $('#divDates').show();
    });
    $(".results").on("click", ".ExpiredMembership", function (event) {
        $(".btnMembers").text("Expired Membership");
        $('#divMemberList').hide();
        $('#divDueMembership').hide();
        $('#divAllEnquiry').hide();
        $('#divPaymentDue').hide();
        $('#divExpiredMembership').show();
        $('#divDates').show();
    });
    $(".results").on("click", ".AllEnquiry", function (event) {
        $(".btnMembers").text("All Enquiry");
        $('#divMemberList').hide();
        $('#divDueMembership').hide();
        $('#divExpiredMembership').hide();
        $('#divPaymentDue').hide();
        $('#divAllEnquiry').show();
        $('#divDates').show();
    });
    $(".results").on("click", ".PaymentDue", function (event) {
        $(".btnMembers").text("Payment Due");
        $('#divMemberList').hide();
        $('#divDueMembership').hide();
        $('#divExpiredMembership').hide();
        $('#divPaymentDue').show();
        $('#divAllEnquiry').hide();
        $('#divDates').show();
    });
    $('#btnSearch').click(function () {
        var searchType = $('.btnMembers').text();
        if (searchType == "Due Membership")
        {
            if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
                BindDueMembership(pageNo, $("#tblMemberList_length").val());
            }
            else {
                BindDueMembership(pageNo, $("#tblMemberList_length").val());
            }
        }
        else if (searchType == "Expired Membership")
        {
            if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
                BindExpiredMembership(pageNo, $("#tblMemberList_length").val());
            }
            else {
                BindExpiredMembership(pageNo, $("#tblMemberList_length").val());
            }
        }
        else if (searchType == "All Enquiry")
{
            if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
                BindAllEnquiry(pageNo, $("#tblMemberList_length").val());
            }
            else {
                BindAllEnquiry(pageNo, $("#tblMemberList_length").val());
            }
}

else
{
            if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
                BindPaymentDue(pageNo, $("#tblMemberList_length").val());
            }
            else {
                BindPaymentDue(pageNo, $("#tblMemberList_length").val());
            }
}


        if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
            BindDueMembership(pageNo, $("#tblMemberList_length").val());
        }
        else {
            BindDueMembership(pageNo, $("#tblMemberList_length").val());
        }
    });
    $('#btnSendAllMem').click(function () { GetMobileNumberList(); });
    var maxLength = 160;
    $('#txtAllMemMessage').keyup(function () {
        var length = $(this).val().length;
        var length = maxLength - length;
        $('#countTextArea').text("Character "+length);
    });
    $('#btnOutsideContact').click(function () { SendOuterSMS(); });

});
function SendSMS(listphonenumber,content)
{
    var encoded = encodeURI(content);
    //var urlString = SendSMSUrl.replace(/&amp;/g, '&');

    var urlString = "http://bulksms.smsroot.com/app/smsapi/index.php?key=35B759B645F85F&campaign=0&routeid=18&type=text&contacts=" + listphonenumber + "&senderid=JUSTBK&msg=" + encoded + "";
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
function BindSMSTable(pageno, pagesize,tableType)
{
    $.ajax({
        cache: false,
        type: "GET",
        url: AllMemberUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize,TableType:tableType ,BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMemberList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                        var rows = '<tr role="row" class="odd">'
                + '<td><input type="checkbox" class="multi-checkbox" /></td>'
                + '<td style=display:none;>' + item.MemberId + '</td>'
                + '<td>' + item.MemberName + '</td>'
                + '<td>' + item.Email + '</td>'
                + '<td>' + item.Address + '</td>'
                + '<td>' + item.PhoneNo + '</td>'
       + '</tr >';
                        $('#tblMemberList tbody').append(rows);
                    
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
                                if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
                                    YearPerformanceReport(pageNo, $("#tblMemberList_length").val())
                                }
                                else {
                                    YearPerformanceReport(pageNo, 10)
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
                $('#tblYearPerformance tbody').append(norecords);
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
function BindAllMembers(pageno, pagesize)
{
    $.ajax({
        cache: false,
        type: "GET",
        url: OnloadAllMemberUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize,BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMemberList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
            + '<td><input type="checkbox" class="multi-checkbox" /></td>'
            + '<td style=display:none;>' + item.MemberId + '</td>'
            + '<td>' + item.MemberName + '</td>'
            + '<td>' + item.Email + '</td>'
            + '<td>' + item.Address + '</td>'
            + '<td>' + item.PhoneNo + '</td>'
   + '</tr >';
                    $('#tblMemberList tbody').append(rows);

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
                                if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
                                    BindAllMembers(pageNo, $("#tblMemberList_length").val())
                                }
                                else {
                                    BindAllMembers(pageNo, 100)
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
                $('#tblMemberList tbody').append(norecords);
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
function BindDueMembership(pageno, pagesize) {
    $.ajax({
        cache: false,
        type: "GET",
        url: OnloadDueMemberUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, fromdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblDueMembership tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var StartDate = ConvertDate(item.StartDate);
                    var EndDate = ConvertDate(item.EndDate);
                    var rows = '<tr role="row" class="odd">'
            + '<td><input type="checkbox" class="multi-checkbox" /></td>'
            + '<td style=display:none;>' + item.MemberId + '</td>'
            + '<td>' + item.MemberName + '</td>'
            + '<td>' + item.MembershipType + '</td>'
            + '<td>' + StartDate + '</td>'
            + '<td>' + EndDate + '</td>'
              + '<td>' + item.MobileNumber + '</td>'
   + '</tr >';
                    $('#tblDueMembership tbody').append(rows);

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
                                if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
                                    BindDueMembership(pageNo, $("#tblMemberList_length").val())
                                }
                                else {
                                    BindDueMembership(pageNo, 100)
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
                $('#tblDueMembership tbody').append(norecords);
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
function BindExpiredMembership(pageno, pagesize) {
    $.ajax({
        cache: false,
        type: "GET",
        url: OnloadExpiredMemberUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, fromdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblExpiredMembership tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var StartDate = ConvertDate(item.StartDate);
                    var EndDate = ConvertDate(item.EndDate);
                    var rows = '<tr role="row" class="odd">'
            + '<td><input type="checkbox" class="multi-checkbox" /></td>'
            + '<td style=display:none;>' + item.MemberId + '</td>'
            + '<td>' + item.MemberName + '</td>'
            + '<td>' + item.MembershipType + '</td>'
            + '<td>' + StartDate + '</td>'
            + '<td>' + EndDate + '</td>'
              + '<td>' + item.MobileNumber + '</td>'
   + '</tr >';
                    $('#tblExpiredMembership tbody').append(rows);

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
                                if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
                                    BindExpiredMembership(pageNo, $("#tblMemberList_length").val())
                                }
                                else {
                                    BindExpiredMembership(pageNo, 100)
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
                $('#tblExpiredMembership tbody').append(norecords);
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
function BindAllEnquiry(pageno, pagesize) {
    $.ajax({
        cache: false,
        type: "GET",
        url: OnloadAllEnquiryUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, fromdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblAllEnquiry tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var enquirydate = ConvertDate(item.EnquiryDate);
                    var lastfollowupdate = ConvertDate(item.LastFollowUpDate);
                    var nextfollowupdate = ConvertDate(item.NextFollowUpDate);
                    var rows = '<tr role="row" class="odd">'
            + '<td><input type="checkbox" class="multi-checkbox" /></td>'
            + '<td>' + item.MemberName + '</td>'
            + '<td>' + item.Address + '</td>'
            + '<td>' + item.MobileNumber + '</td>'
            + '<td>' + enquirydate + '</td>'
              + '<td>' + lastfollowupdate + '</td>'
               + '<td>' + nextfollowupdate + '</td>'
              + '<td>' + item.EnqStatus + '</td>'
   + '</tr >';
                    $('#tblAllEnquiry tbody').append(rows);

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
                                if ($("#tblMemberList_length").val() != null && $("#tblMemberList_length").val() != "") {
                                    BindAllEnquiry(pageNo, $("#tblMemberList_length").val())
                                }
                                else {
                                    BindAllEnquiry(pageNo, 100)
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
                $('#tblAllEnquiry tbody').append(norecords);
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
function BindPaymentDue(pageno, pagesize) {
    $.ajax({
        cache: false,
        type: "GET",
        url: OnloadDuepaymentListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, fromdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblPaymentDue tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {

                $.each(data.Result, function (i, item) {
                  
                    var pendingamt = 0;

                    if (item.Amount != "") {

                        var amount = parseInt(item.Amount);
                        var paid = parseInt(item.PaymentAmount);
                        pendingamt = amount - paid
                    }
                    if (pendingamt > 0)
                    {
                        var StartDate = ConvertDate(item.StartDate);
                        var EndDate = ConvertDate(item.EndDate);
                        var rows = '<tr role="row" class="odd">'
                + '<td><input type="checkbox" class="multi-checkbox" /></td>'
                 + '<td style=display:none;>' + item.MemberId + '</td>'
                + '<td>' + item.FirstName + '</td>'
                + '<td>' + item.Package + '</td>'
                + '<td>' + StartDate + '</td>'
                + '<td>' + EndDate + '</td>'
                  + '<td>' + item.PaymentAmount + '</td>'
                   + '<td>' + pendingamt + '</td>'
                          + '<td>' + item.MobileNumber + '</td>'
                        
       + '</tr >';
                    }
                    $('#tblPaymentDue tbody').append(rows);
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
                                if ($("#tblPaymentsDueList_length").val() != null && $("#tblPaymentsDueList_length").val() != "") {
                                    BindPaymentDue(pageNo, $("#tblPaymentsDueList_length").val())
                                }
                                else {
                                    BindPaymentDue(pageNo, 10)
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
                $('#tblPaymentDue tbody').append(norecords);
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
function GetMobileNumberList()
{
   
    var searchType = $('.btnMembers').text();
    var PhoneNumbers = "";
    var memberid = "";
    var msg = "";
    if (searchType.trim() == "All Members")
    {
        $('#tblMemberList').find('tr').each(function (i, el) {
            var chkbox = $(this).find("input[type='checkbox']");
            if (chkbox.prop('checked') == true) {
                var $tds = $(this).find('td');
                var phoneNumber = $tds.eq(5).text();
                var memberID = $tds.eq(1).text();
                if (phoneNumber != "undefined") {
                    PhoneNumbers = PhoneNumbers + phoneNumber + ',';
                    memberid = memberid + memberID + ',';
                }

            }
        });
    }
    else if (searchType == "Due Membership")
    {
        $('#tblDueMembership').find('tr').each(function (i, el) {
            var chkbox = $(this).find("input[type='checkbox']");
            if (chkbox.prop('checked') == true) {
                var $tds = $(this).find('td');
                var memberID = $tds.eq(1).text();
                var phoneNumber = $tds.eq(6).text();
                if (phoneNumber != "undefined")
                {
                    PhoneNumbers = PhoneNumbers + phoneNumber + ',';
                    memberid = memberid + memberID + ',';
                }
                
            }
        });
    }
    else if (searchType == "Expired Membership") {
        $('#tblExpiredMembership').find('tr').each(function (i, el) {
            var chkbox = $(this).find("input[type='checkbox']");
            if (chkbox.prop('checked') == true) {
                var $tds = $(this).find('td');
                var memberID = $tds.eq(1).text();
                var phoneNumber = $tds.eq(6).text();
                if (phoneNumber != "undefined") {
                    PhoneNumbers = PhoneNumbers + phoneNumber + ',';
                    memberid = memberid + memberID + ',';
                }
            }
        });
    }
    else if (searchType == "All Enquiry") {

        $('#tblAllEnquiry').find('tr').each(function (i, el) {
            var chkbox = $(this).find("input[type='checkbox']");
            if (chkbox.prop('checked') == true) {
                var $tds = $(this).find('td');
                var phoneNumber = $tds.eq(3).text();
                if (phoneNumber != "undefined") {
                    PhoneNumbers = PhoneNumbers + phoneNumber + ',';
                }
            }
        });
    }
    else if (searchType.trim() == "Payment Due") {
        $('#tblPaymentDue').find('tr').each(function (i, el) {
            var chkbox = $(this).find("input[type='checkbox']");
            if (chkbox.prop('checked') == true) {
                var $tds = $(this).find('td');
                var memberID = $tds.eq(1).text();
                var phoneNumber = $tds.eq(8).text();
                if (phoneNumber != "undefined") {
                    PhoneNumbers = PhoneNumbers + phoneNumber + ',';
                    memberid = memberid + memberID + ',';
                }
            }
        });
    }

    if (PhoneNumbers != "")
    {
        PhoneNumbers = PhoneNumbers.slice(0, -1);
        msg = $('#txtAllMemMessage').val();
        SaveMessage(PhoneNumbers, msg,memberid);
        SendSMS(PhoneNumbers,msg)
       
    }
  



}
function ConvertDate(convertdate) {
    var date = convertdate;
    var parsedDate = new Date(parseInt(date.substr(6)));
    var newDate = new Date(parsedDate);

    //var getMonth = newDate.getMonth();
    var getDay = newDate.getDay();
    var getYear = newDate.getYear();

    var twoDigitDate = newDate.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
    var getMonth = ((newDate.getMonth().length + 1) === 1) ? (newDate.getMonth() + 1) : '0' + (newDate.getMonth() + 1);

    var startdate = twoDigitDate + '/' + getMonth + '/' + newDate.getFullYear();

    return startdate;
}

function SendOuterSMS()
{
    var PhoneNumbers = "";
    var msg = "";
    var contacts = $('#txtAllContactNo').val();
    var listContacts = contacts.split("\n");
    $.each(listContacts, function (i, contact) {
        if (contact != "" && contact.length == 10)
        {
            PhoneNumbers = PhoneNumbers + contact + ',';
        }
    })
   
    if (PhoneNumbers != "") {
        PhoneNumbers = PhoneNumbers.slice(0, -1);
        msg = $('#txtOutsideMessage').val();
        SaveMessage(PhoneNumbers, msg,"");
        SendSMS(PhoneNumbers, msg);
       
    }
}

function SaveMessage(contactNumber,message,memberid)
{
        $.ajax({
            type: "GET",
            url: messageUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: { phoneNumber: contactNumber, Message: message, memberid: memberid, BranchId: $('#ddlBranch option:selected').val() },
            success: function (data) {
                toastr.success("Message Sent Successfully.");
            },
            error: function () {
            }
        });
}


var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';
//function ValidateProduct() {
//    var IsValid = true;
//    $('#txtProductname').parent().removeClass("has-error");
//    $('#txtProductname').parent().find(".help-block-error").remove();
//    if ($('#txtProductname').val() == "") {

//        $('#txtProductname').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Product Name"));
//        $('#txtProductname').parent().addClass("has-error");
//        IsValid = false;
//    }
//    else {
//        $('#txtProductname').parent().removeClass("has-error");
//        $('#txtProductname').parent().find(".help-block-error").remove();
//    }
//    if ($('#txtPrice').val() == "") {

//        $('#txtPrice').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Price"));
//        $('#txtPrice').parent().addClass("has-error");
//        IsValid = false;
//    }
//    else {
//        $('#txtPrice').parent().removeClass("has-error");
//        $('#txtPrice').parent().find(".help-block-error").remove();
//    }

//    if ($('#txtQuantity').val() == "") {

//        $('#txtQuantity').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Quantity"));
//        $('#txtQuantity').parent().addClass("has-error");
//        IsValid = false;
//    }
//    else {
//        $('#txtQuantity').parent().removeClass("has-error");
//        $('#txtQuantity').parent().find(".help-block-error").remove();
//    }

//    if ($('#txtLowStockQuantity').val() == "") {

//        $('#txtLowStockQuantity').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Low Stock Quantity"));
//        $('#txtLowStockQuantity').parent().addClass("has-error");
//        IsValid = false;
//    }
//    else {
//        $('#txtLowStockQuantity').parent().removeClass("has-error");
//        $('#txtLowStockQuantity').parent().find(".help-block-error").remove();
//    }
//    return IsValid;
//}


