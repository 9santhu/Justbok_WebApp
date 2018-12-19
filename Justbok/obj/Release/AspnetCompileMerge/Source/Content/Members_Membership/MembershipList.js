var pageNo = 1, pagerLoaded = false;
var url = "";

$(document).ready(function () {
   
    $('input[type=datetime]').datepicker({
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });
    ShowLoader();
    if ($("#tblMembershipList_length").val() != null && $("#tblMembershipList_length").val() != "") {
        GettingMembersList(pageNo, $("#tblMembershipList_length").val())
    }
    else {
        GettingMembersList(pageNo, 10)
    }
});



function GettingMembersList(pageno, pagesize) {
   
    $.ajax({
        cache: false,
        type: "GET",
        url: GetMembershipListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, BranchId: $('#ddlBranch').val() },
        success: function (data) {
            $('#tblMembershipList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
              
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
                        
                        + '<td>' + item.MemberID + '</td>'
                        + '<td>' + item.FirstName + '</td>'
                        + '<td>' + item.MobileNumber + '</td>'
                        + '<td>' + item.MembershipType + '</td>'
                        + '<td>' + item.StartDate + '</td>'
                        + '<td>' + item.EndDate + '</td>'
                        + '<td>' + item.Amount + '</td>'
                        + '<td>' + item.PaymentAmount + '</td>'
                        
                        //+ '<td>'
                        //+ '<a class="btn btn-info EditMember" onclick="return EditMember(' + item.MemberID + ')">Edit</a>&nbsp;'
                        //+ '<a class="btn btn-info" href="">Delete</a>'
                        //+ '</td>'
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
                                    GettingMembersList(pageNo, $("#tblMembershipList_length").val())
                                   
                                }
                                else {
                                    GettingMembersList(pageNo, 10)
                                  
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

