var pageNo = 1, pagerLoaded = false;
var url = "";

$(document).ready(function () {
    $('#tblQuickPrint tbody').empty();
    $('#btnSelectCustSearch').click(function () { QuickPrintSearchMember(pageNo,10); });
    $('#printpageMemo').click(function () { QuickPrintOrderReceipt(); });
   
});

function ShowPrintModal()
{
    $('#modal_Print').modal('show');
}

function QuickPrintSearchMember(pageno, pagesize) {
    ShowLoader();
    var memberInfo = $('#txtQuickprint').val();
    $.ajax({
        cache: false,
        type: "GET",
        url: QuickPrintSearchMemberUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, Member: memberInfo },
        success: function (data) {
            $('#tblQuickPrint tbody').empty();
            $('#tblMemberDetails').show();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = "<tr>"
        + "<td>" + item.MemberID + "</td>"
         + "<td style='display:none'>" + item.MembershipId + "</td>"
        + "<td style='display:none'>" + item.RecieptNumber + "</td>"
        + "<td>" + item.FirstName + " " + item.LastName + "</td>"
        + "<td>" + item.MembershipType + "</td>"
        + "<td>" + item.PaymentAmount + "</td>"
         + "<td>" + item.PaymentDate + "</td>"

        + "<td><a class='btn btn-primary btn-xs btn-flat btnQuickPrint' onclick='return PrintMemo(this)'>Print</a></td>"
        + "</tr>";
                    $('#tblQuickPrint tbody').append(rows);

                });
              
                HideLoader();
                
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
                                    QuickPrintSearchMember(pageNo, 10)
                            }
                        }
                    });
                }
            }
            else {
                var norecords = "<tr>" +
                    '<td colspan="4">No data available</td>'
                    + "</tr>";
                $('#tblDuePayment tbody').append(norecords);
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

function PrintMemo(id)
{
    $('#modal_Print').modal('hide');
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var memberid = $('#tblQuickPrint tr').eq(rowIndex + 1).find('td').eq(0).html();
    var memberShipId = $('#tblQuickPrint tr').eq(rowIndex + 1).find('td').eq(1).html();
    var RecieptNo = $('#tblQuickPrint tr').eq(rowIndex + 1).find('td').eq(2).html();
    EditMember(memberid);
  
    
    $.ajax({
        cache: false,
        async:true,
        url: GetInvoice,
        data: { membershipid: memberShipId, receiptnumber: RecieptNo, Memberid: memberid },
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            
            var rate = data.Amount / 1.15;
            var taxamount = data.Amount - rate;
            var gstamt = taxamount / 2;
            var gstRate = 15 / 2;
            $("#customerNameQuickPrint").text(data.FirstName);
            $("#addressQuickPrint").text(data.Address);
            $("#packageQuickPrint").text(data.Package);
            $("#startdateQuickPrint").text(data.StartDate);
            $("#enddateQuickPrint").text(data.EndDate);
            $("#totalAmountQuickPrint").text(data.Amount);
            $("#rateQuickPrint").text(rate.toFixed(2));
            $("#TotalQuickPrint").text(rate.toFixed(2));
            $("#taxableValueQuickPrint").text(rate.toFixed(2));
            $("#cgstrateQuickPrint").text(gstRate);
            $("#cgstamountQuickPrint").text(gstamt.toFixed(2));
            $("#sgstrateQuickPrint").text(gstRate);
            $("#sgstamountQuickPrint").text(gstamt.toFixed(2));
           
            $('#modal-printMemo').modal('show');
      
            $('.nav-tabs a[href="#Membership"]').tab('show');
          
           
        //    setTimeout(function () {
             
              
        //}, 2000);
        },
           
            //$('.nav-tabs a[href="#Membership"]').tab('show');
         
        
        error: function () {
            //alert("Failed! Please try again. PaymentHistory");
        }
    
    });

}


function EditMember(id) {
    LoadPage(EditMemberUrl + "/" + id, 'Justbok | Edit Member');
    
    return false;
}

function QuickPrintOrderReceipt() {
    var printelem = document.getElementById('divQuickPrintMembershipMemo');
    var myWindow = window.open('', '', 'width=750,height=500', '_blank');
    myWindow.document.write(document.head.innerHTML);
    myWindow.document.write(printelem.innerHTML);
    myWindow.document.write("\x3Cscript>window.print(); window.close();\x3C/script>");
}


//function QuickPrintSearchMember(pageno) {
//    ShowLoader();
//    var memberInfo = $('#txtQuickprint').val();
//    $.ajax({
//        cache: false,
//        type: "GET",
//        url: QuickPrintSearchMemberUrl,
//        dataType: "json",
//        data: { page: pageno, Member: memberInfo },
//        success: function (data) {
//            $('#tblQuickPrint tbody').find("tr").remove();
//            if (data != null && data.Pages > 0) {
//                $.each(data.Result, function (i, item) {
//                    var active = "No";
//                    if (item.Active == true) {
//                        active = "Yes";
//                    }
//                      var rows = "<tr>"
//    + "<td>" + item.MemberID + "</td>"
//    + "<td>" + item.FirstName + " " + item.LastName + "</td>"
//    + "<td>" + item.MembershipType + "</td>"
//    + "<td>" + item.PaymentAmount + "</td>"
//     + "<td>" + item.PaymentDate + "</td>"
//    + "<td><a class='btn btn-primary btn-xs btn-flat btnQuickPrint' onclick='return EditMember(" + item.MemberID + ")' >Print</a></td>"
//    + "</tr>";
//                    $('#tblQuickPrint tbody').append(rows);
//                });

//                if (data.Pages < pageNo) {
//                    pageNo = data.Pages;
//                }
//                if (!pagerLoaded) {
//                    pagerLoaded = true;
//                    $('#pagination').twbsPagination({
//                        totalPages: data.Pages,
//                        visiblePages: 7,
//                        startPage: pageNo,
//                        onPageClick: function (event, page) {
//                            ShowLoader();
//                            if (pageNo != page) {
//                                pageNo = page;
//                                QuickPrintSearchMember(pageNo);
//                            }
//                        }
//                    });
//                }
//            }
//            else {
//                var norecords = "<tr>" +
//                    '<td colspan="4">No data available</td>'
//                    + "</tr>";
//                $('#tblQuickPrint tbody').append(norecords);
//            }

//            HideLoader();
//            setClass();
//        },
//        error: function (errMsg) {
//            HideLoader();
//        },
//        failure: function (errMsg) {
//            HideLoader();
//        }
//    });


//}

function setClass() {
    $("th").removeClass();
    $("th").addClass("sorting");

    if (HeaderId != "") {
        $('#' + HeaderId).removeClass();
        var classname = (sortDirection == "ASC") ? "sorting_asc" : "sorting_desc";
        $('#' + HeaderId).addClass(classname);
    }
}



//$.ajax({
//    cache: false,
//    type: "GET",
//    url: QuickPrintSearchMemberUrl,
//    dataType: "json",
//    contentType: "application/json; charset=utf-8",
//    data: { Member: memberInfo },
//    success: function (data) {
//        $('#tblQuickPrint tbody').empty();
//        $('#tblMemberDetails').show();

//        $.each(data, function (i, item) {
//            var rows = "<tr>"
//+ "<td>" + item.MemberID + "</td>"
// + "<td style='display:none'>" + item.MembershipId + "</td>"
//+ "<td style='display:none'>" + item.RecieptNumber + "</td>"
//+ "<td>" + item.FirstName + " " + item.LastName + "</td>"
//+ "<td>" + item.MembershipType + "</td>"
//+ "<td>" + item.PaymentAmount + "</td>"
// + "<td>" + item.PaymentDate + "</td>"

//+ "<td><a class='btn btn-primary btn-xs btn-flat btnQuickPrint' onclick='return PrintMemo(this)'>Print</a></td>"
//+ "</tr>";
//            $('#tblQuickPrint tbody').append(rows);

//        });
//        HideLoader();
//    },
//    failure: function (errMsg) {
//        HideLoader();
//        alert(errMsg.responseText);
//    }
//});