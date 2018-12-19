var pageNo = 1, pagerLoaded = false;
var url = "";
$(document).ready(function () {
    ShowLoader();
    $('.datepicker').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    var d = new Date();
    $("#txtFromdate").datepicker("setDate", d);
    $("#txtTodate").datepicker("setDate", d);
    GetRepresentatives();
    GetMemberOffers();
   

    if ($("#tblEnquiryList_length").val() != null && $("#tblEnquiryList_length").val() != "") {
        GettingEnquiryList(pageNo, $("#tblEnquiryList_length").val())
    }
    else {
        GettingEnquiryList(pageNo, 10)
    }

    $("#ddlBranch").change(function () {
        if ($("#tblEnquiryList_length").val() != null && $("#tblEnquiryList_length").val() != "") {
            GettingEnquiryList(pageNo, $("#tblEnquiryList_length").val())
        }
        else {
            GettingEnquiryList(pageNo, 10)
        }
    });

    $('#btnSearchEnquiry').click(function () {
        if ($("#tblEnquiryList_length").val() != null && $("#tblEnquiryList_length").val() != "") {
            SearchEnquiry(pageNo, $("#tblEnquiryList_length").val())
        }
        else {
            SearchEnquiry(pageNo, 10)
        }
        
    });
    //$('.btnEnroll').click(function () { NewMemberDetails(); });

});

function GettingEnquiryList(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: true,
        type: "GET",
        url: GetEnquieryListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblEnquiryList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {

                    var rows = '<tr role="row" class="odd">'
                        + "<td style='display:none'>" + '<input type="hidden" name="hid" value=' + item.EnquiryId + '>' + "</td>"
   + "<td>" + item.FirstName + " " + item.LastName + "</td>"
   + "<td>" + item.MobileNumber + "</td>"
   + "<td>" + item.EnquiryDate + "</td>"
   + "<td>" + item.LastFollowUpDate + "</td>"
     + "<td>" + item.NextFollowUpDate + "</td>"
     + "<td>" + item.EnqStatus + "</td>"
   + "<td><button class='btn btn-success btn-xs btn-flat btnEdit' type='button' onclick='return EditEnquiry(" + item.EnquiryId + ")'>Edit</button>  <a class='btn btn-info btn-xs btn-flat btnEnroll'  title='Enroll' href='/MemberShip/Membership' onclick = 'return NewMemberDetails(this);'><span class='glyphicon glyphicon-plus'></span> Enroll</a></td>"
   //<button class='btn btn-info btnGoEnroll' type='button'>Enroll</button> 
                        + '</tr >';
                    $('#tblEnquiryList tbody').append(rows);
                    //" onclick = "return EditMember(' + item.ProductId + ')"
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
                                if ($("#tblEnquiryList_length").val() != null && $("#tblEnquiryList_length").val() != "") {
                                    GettingEnquiryList(pageNo, $("#tblEnquiryList_length").val())
                                }
                                else {
                                    GettingEnquiryList(pageNo, 10)
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
                $('#tblEnquiryList tbody').append(norecords);
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

function setClass() {
    $("th").removeClass();
    $("th").addClass("sorting");

    if (HeaderId != "") {
        $('#' + HeaderId).removeClass();
        var classname = (sortDirection == "ASC") ? "sorting_asc" : "sorting_desc";
        $('#' + HeaderId).addClass(classname);
    }
}

function GetRepresentatives()
{
    $.ajax({
        cache: false,
        type: "GET",
        url: GetRepresentativeList,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: "",
    success: function (data) {
        $.each(data, function (key, value) {
            $("#ddlRecievedby").append($("<option></option>").val(value.FirstName + " " + value.LastName).html(value.FirstName + " " + value.LastName));
        });
    },
    failure: function (errMsg) {
        alert(errMsg.responseText);
    }
});
}

function GetMemberOffers()
{

    $.ajax({
        cache: false,
        type: "GET",
        url: GetMembershipOffersUrl,
        dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: "",
    success: function (data) {
        $.each(data, function (key, value) {
            $("#ddlMembership").append($("<option></option>").val(value.OfferName).html(value.OfferName));
        });
    },
    failure: function (errMsg) {
        alert(errMsg.responseText);
    }
});
}

function EditEnquiry(id) {
    LoadPage(EditEnquiryUrl + "/" + id, 'Justbok | Edit Enquiry');
    return false;
}

function SearchEnquiry(pageno, pagesize)
{
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SearchMemberListEnquiryUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { page: pageno, pagesize: pagesize, membername: $("#txtMemberNamePhone").val(), gender: $('#ddlGenderSearch option:selected').text(), filter: $('#ddlFilterSearch option:selected').text(), startdate: $('#txtFromdate').val(), todate: $('#txtTodate').val(), status: $('#ddlStatus option:selected').text(), membership: $('#ddlMembership option:selected').text(), recievedby: $('#ddlRecievedby option:selected').text(), branchid: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblEnquiryList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
                        + "<td style='display:none'>" + '<input type="hidden" name="hid" value=' + item.EnquiryId + '>' + "</td>"
            + "<td>" + item.Name + "</td>"
            + "<td>" + item.MobileNumber + "</td>"
            + "<td>" + item.EnquiryDate + "</td>"
            + "<td>" + item.LastFollowUpDate + "</td>"
              + "<td>" + item.NextFollowUpDate + "</td>"
              + "<td>" + item.EnqStatus + "</td>"
             + "<td><button class='btn btn-success btn-xs btn-flat btnEdit' type='button' onclick='return EditEnquiry(" + item.EnquiryId + ")'>Edit</button>  <a class='btn btn-info btn-xs btn-flat btnEnroll'  title='Enroll' href='/MemberShip/Membership' onclick = 'return NewMemberDetails(this);'><span class='glyphicon glyphicon-plus'></span> Enroll</a> </td>"
                     //  + '<td> <a href="" class="btn btn-success btn-xs btn-flat" onclick="return EditMember(' + item.MemberID + ')"><span class="glyphicon glyphicon-edit"></span> Edit </a> '
                     //+ '<a href="" class="btn btn-primary btn-xs btn-flat"><span class="glyphicon glyphicon-credit-card" ></span> Payment</a> </td>'
                        + '</tr >';
                    $('#tblEnquiryList tbody').append(rows);
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
                                if ($("#tblEnquiryList_length").val() != null && $("#tblEnquiryList_length").val() != "") {
                                    SearchEnquiry(pageNo, $("#tblEnquiryList_length").val())
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
                $('#tblEnquiryList tbody').append(norecords);
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

function NewMemberDetails(id) {
    var enquiryid = $(id).closest('tr').find('input[type="hidden"]').val();
    enroll = true;
    $.ajax({
        cache: false,
        type: "GET",
        url: NewMemberUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: {EnquiryId:enquiryid},
        success: function (data) {
           
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}
