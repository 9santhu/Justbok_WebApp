var pageNo = 1, pagerLoaded = false;
var url = "";

if ($("#tblSMSlog_length").val() != null && $("#tblSMSlog_length").val() != "") {
    BindSMSLog(pageNo, $("#tblSMSlog_length").val());
}
else {
    BindSMSLog(pageNo, $("#tblSMSlog_length").val());
}


function BindSMSLog(pageno, pagesize) {
    $.ajax({
        type: "GET",
        url: SMSLogUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize,BranchId: $('#ddlBranch option:selected').val() },
        success: function (data) {
            $('#tblSMSlog tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var sentDate = ConvertDate(item.SmsDate);
                    var rows = '<tr role="row" class="odd">'
                         + '<td>1</td>'
            + '<td>' + item.PhoneNumber + '</td>'
            + '<td>' + item.Message + '</td>'
            + '<td>' + sentDate + '</td>'
   + '</tr >';
                    $('#tblSMSlog tbody').append(rows);

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
                                if ($("#tblSMSlog_length").val() != null && $("#tblSMSlog_length").val() != "") {
                                    BindSMSLog(pageNo, $("#tblSMSlog_length").val());
                                }
                                else {
                                    BindSMSLog(pageNo, $("#tblSMSlog_length").val());
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
                $('#tblSMSlog tbody').append(norecords);
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