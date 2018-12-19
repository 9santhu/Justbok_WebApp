var pageNo = 1, pagerLoaded = false;
var url = "";

$(document).ready(function () {
    ShowLoader();
    $('input[type=datetime]').datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "dd/mm/yy",
        yearRange: "-90:+00"
    });
    var date = new Date();
    var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
    var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);

   
    $("#txtFromDate").datepicker("setDate", firstDay);
    $("#txtToDate").datepicker("setDate", lastDay);

    BindMembership();
    BindStaff();

    if ($("#tblMembership_length").val() != null && $("#tblMembership_length").val() != "") {
        EnrolledMemberList(pageNo, $("#tblMembership_length").val())
    }
    else {
        EnrolledMemberList(pageNo, 10)
    }

    $('#btnSearch').click(function () {

        if ($("#tblMembership_length").val() != null && $("#tblMembership_length").val() != "") {
            SearchMember(pageNo, $("#tblMembership_length").val())
        }
        else {
            SearchMember(pageNo, 10)
        }
    });
   

});


function EnrolledMemberList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetMemberListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMembershipList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var note = (item.Note != null) ? item.Note : "";
                    var rows = '<tr role="row" class="odd">'
                        + '<td><a onclick="return EditMember(' + item.MemberID + ')">' + item.MemberID + '</a></td>'
                         + '<td style=display:none;>' + item.MembershipId + '</td>'
                        + '<td><a onclick="return EditMember(' + item.MemberID + ')">' + item.FirstName + '</a></td>'
                        + '<td>' + item.MobileNumber + '</td>'
                         + '<td>' + item.Package + '</td>'
                          + '<td>' + item.StartDate + '</td>'
                           + '<td>' + item.EndDate + '</td>'
                            + '<td>' + item.Amount + '</td>'
                             + '<td>' + item.PaymentAmount + '</td>'
                     + '<td>' + note + '</td>'
                      + '<td> <a href="" class="btn btn-success btn-xs btn-flat" onclick="return EditMember(' + item.MemberID + ')"><span class="glyphicon glyphicon-edit"></span> Edit </a> '
                     + '<a href="" class="btn btn-primary btn-xs btn-flat" onclick="return EditMember(' + item.MemberID + ')"><span class="glyphicon glyphicon-credit-card" ></span> Payment</a> </td>'
                     
                     
                              //+ '<td>' + item.Amount- item.PaymentAmount + '</td>'
                        + '</tr >';
                    $('#tblMembershipList tbody').append(rows);
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
                                if ($("#tblMembership_length").val() != null && $("#tblMembership_length").val() != "") {
                                    EnrolledMemberList(pageNo, $("#tblMembership_length").val())
                                }
                                else {
                                    EnrolledMemberList(pageNo, 10)
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
                $('#tblMembershipList tbody').append(norecords);
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

function SearchMember(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchMemberListUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { page: pageno, pagesize: pagesize, membername: $("#txtMember").val(), membershipid: $("#ddlMembership option:selected").text(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), representative: $("#ddlRepresentative option:selected").text(), category: $("#ddlCategory option:selected").text(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblMembershipList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var note = (item.Note != null) ? item.Note : "";
                    var rows = '<tr role="row" class="odd">'
                        + '<td><a onclick="return EditMember(' + item.MemberID + ')">' + item.MemberID + '</a></td>'
                         + '<td style=display:none;>' + item.MembershipId + '</td>'
                        + '<td><a onclick="return EditMember(' + item.MemberID + ')">' + item.FirstName + '</a></td>'
                        + '<td>' + item.MobileNumber + '</td>'
                         + '<td>' + item.Package + '</td>'
                          + '<td>' + item.StartDate + '</td>'
                           + '<td>' + item.EndDate + '</td>'
                            + '<td>' + item.Amount + '</td>'
                             + '<td>' + item.PaymentAmount + '</td>'
                     + '<td>' + note + '</td>'
                       + '<td> <a href="" class="btn btn-success btn-xs btn-flat" onclick="return EditMember(' + item.MemberID + ')"><span class="glyphicon glyphicon-edit"></span> Edit </a> '
                     + '<a href="" class="btn btn-primary btn-xs btn-flat" onclick="return EditMember(' + item.MemberID + ')"><span class="glyphicon glyphicon-credit-card" ></span> Payment</a> </td>'

                              //+ '<td>' + item.Amount- item.PaymentAmount + '</td>'
                        + '</tr >';
                    $('#tblMembershipList tbody').append(rows);
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
                                if ($("#tblMembershipList_length").val() != null && $("#tblMembershipList_length").val() != "") {
                                    SearchMember(pageNo, $("#tblMembershipList_length").val())
                                }
                                else {
                                    SearchMember(pageNo, 10)
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
                $('#tblMembershipList tbody').append(norecords);
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


function BindMembership() {
    $.ajax({
        cache: false,
        type: "GET",
        url: BindMembershipUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: "",
        success: function (data) {
            $.each(data, function (i, obj) {
                $("#ddlMembership").append($("<option></option>").val(obj.MembershipOfferId).html(obj.MemershipType));
            });
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });
}

function BindStaff() {
    $.ajax({
        cache: false,
        type: "GET",
        url: BindStaffUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $.each(data, function (i, obj) {
                $("#ddlRepresentative").append($("<option></option>").val(obj.StaffId).html(obj.FirstName + " " + obj.LastName));
            });
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });
}


function BindCategory() {
    $.ajax({
        cache: false,
        type: "GET",
        url: BindStaffUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $.each(data, function (i, obj) {
                $("#ddlRepresentative").append($("<option></option>").val(obj.StaffId).html(obj.FirstName + " " + obj.LastName));
            });
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });
}

function EditMember(id) {
    //var openTab = $(location.hash).filter("#Membership");
    //alert(JSON.stringify(openTab));
    //if (openTab.length) {
    //    $("a[href='" + location.hash + "']").click();
    //}
    LoadPage(EditMemberUrl + "/" + id, 'Justbok | Edit Member');
    return false;
}