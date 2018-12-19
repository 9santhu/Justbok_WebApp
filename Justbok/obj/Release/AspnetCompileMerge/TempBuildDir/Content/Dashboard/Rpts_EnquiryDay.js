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
    var d = new Date();
    $("#txtFromDate").datepicker("setDate", d);
    $("#txtToDate").datepicker("setDate", d);
    BindMembership();
    BindStaff();
    if ($("#tblEnquiry_length").val() != null && $("#tblEnquiry_length").val() != "") {
        EnquiryDayList(pageNo, $("#tblEnquiry_length").val())
    }
    else {
        EnquiryDayList(pageNo, 10)
    }

    $('#btnSearch').click(function () {
        if ($("#tblEnquiry_length").val() != null && $("#tblEnquiry_length").val() != "") {
            SearchEnquiry(pageNo, $("#tblEnquiry_length").val())
        }
        else {
            SearchEnquiry(pageNo, 10)
        }
    });

});


function EnquiryDayList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: EnquiryDayUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblEnquiry tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
                        //+ '<td><a onclick="return EditMember(' + item.MemberID + ')">' + item.MemberID + '</a></td>'
                        // + '<td style=display:none;>' + item.MembershipId + '</td>'
                        //+ '<td><a onclick="return EditMember(' + item.MemberID + ')">' + item.FirstName + '</a></td>'
                          + '<td>' + item.Name + '</td>'
                        + '<td>' + item.MobileNumber + '</td>'
                         + '<td>' + item.EnquiryDate + '</td>'
                          + '<td>' + item.NextFollowUpDate + '</td>'
                           + '<td>' + item.LastFollowUpDate + '</td>'
                            + '<td>' + item.EnqStatus + '</td>'
                              + '<td>' + item.Note + '</td>'
                             + "<td><button class='btn btn-success btn-xs btn-flat btnEdit' type='button' onclick='return EditEnquiry(" + item.EnquiryId + ")'>Edit</button>  <a class='btn btn-info btn-xs btn-flat btnEnroll'  title='Enroll' href='/MemberShip/Membership' onclick = 'return NewMemberDetails(this);'><span class='glyphicon glyphicon-plus'></span> Enroll</a></td>"
                            
                     //  + '<td> <a href="" class="btn btn-success btn-xs btn-flat" onclick="return EditMember(' + item.MemberID + ')"><span class="glyphicon glyphicon-edit"></span> Edit </a> '
                     //+ '<a href="" class="btn btn-primary btn-xs btn-flat"><span class="glyphicon glyphicon-credit-card" ></span> Payment</a> </td>'

                              //+ '<td>' + item.Amount- item.PaymentAmount + '</td>'
                        + '</tr >';
                    $('#tblEnquiry tbody').append(rows);
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
                                if ($("#tblEnquiry_length").val() != null && $("#tblEnquiry_length").val() != "") {
                                    EnquiryDayList(pageNo, $("#tblEnquiry_length").val())
                                }
                                else {
                                    EnquiryDayList(pageNo, 10)
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
                $('#tblEnquiry tbody').append(norecords);
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
    LoadPage(EditMemberUrl + "/" + id + '#/Membership', 'Justbok | Edit Member');
    return false;
}

function SearchEnquiry(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchDayEnquiryListUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { page: pageno, pagesize: pagesize, membername: $("#txtMember").val(), gender: $('#ddlGender option:selected').text(), filter: $('#ddlFilter option:selected').text(), startdate: $('#txtFromDate').val(), todate: $('#txtToDate').val(), status: $('#ddlStatus option:selected').text(), membership: $('#ddlMembership option:selected').text(), recievedby: $('#ddlRepresentative option:selected').text(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblEnquiry tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
                          + '<td>' + item.Name + '</td>'
                        + '<td>' + item.MobileNumber + '</td>'
                         + '<td>' + item.EnquiryDate + '</td>'
                          + '<td>' + item.NextFollowUpDate + '</td>'
                           + '<td>' + item.LastFollowUpDate + '</td>'
                            + '<td>' + item.EnqStatus + '</td>'
                              + '<td>' + item.Note + '</td>'
                             + "<td><button class='btn btn-success btn-xs btn-flat btnEdit' type='button' onclick='return EditEnquiry(" + item.EnquiryId + ")'>Edit</button>  <a class='btn btn-info btn-xs btn-flat btnEnroll'  title='Enroll' href='/MemberShip/Membership' onclick = 'return NewMemberDetails(this);'><span class='glyphicon glyphicon-plus'></span> Enroll</a></td>"
                            
                     //  + '<td> <a href="" class="btn btn-success btn-xs btn-flat" onclick="return EditMember(' + item.MemberID + ')"><span class="glyphicon glyphicon-edit"></span> Edit </a> '
                     //+ '<a href="" class="btn btn-primary btn-xs btn-flat"><span class="glyphicon glyphicon-credit-card" ></span> Payment</a> </td>'
                        + '</tr >';
                    $('#tblEnquiry tbody').append(rows);
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
                                if ($("#tblEnquiry_length").val() != null && $("#tblEnquiry_length").val() != "") {
                                    SearchEnquiry(pageNo, $("#tblEnquiry_length").val())
                                }
                                else {
                                    SearchEnquiry(pageNo, 10)
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
                $('#tblEnquiry tbody').append(norecords);
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

function EditEnquiry(id) {
    LoadPage("/Enquiry/EditEnquiry/" +  + id, 'Justbok | Edit Enquiry');
    return false;
}

function NewMemberDetails(id) {
    var enquiryid = $(id).closest('tr').find('input[type="hidden"]').val();
    $.ajax({
        cache: false,
        type: "GET",
        url: NewMemberUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { EnquiryId: enquiryid },
        success: function (data) {

        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}