var pagerLoaded = false, pageNo = 1, ExpenseTypes = null;

var option = '<option value="{{value}}">{{text}}</option>';

var sortBy = "", sortDirection = "", HeaderId = "";

$(document).ready(function () {
    ShowLoader();
    $('input[type=datetime]').datepicker({
        dateFormat: "M dd, yy",
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });

    $('input[type=datetime]').datepicker('setDate', new Date());

    $("#txtExpenseAmount").keydown(function (event) {
        if (event.shiftKey == true) {
            event.preventDefault();
        }
        if ((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105) || event.keyCode == 110
            || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 46 || event.keyCode == 190) {

        } else {
            event.preventDefault();
        }
        if ($(this).val().indexOf('.') !== -1 && (event.keyCode == 190 || event.keyCode == 110))
            event.preventDefault();

    });

    pageNo = 1;
    sortBy = "EXPENSESAMOUNT";
    sortDirection = "ASC";
    HeaderId = "EXPENSESAMOUNT";

    if ($("#TBLExpenceList_length").val() != null && $("#TBLExpenceList_length").val() != "") {
        GettingExpenses(pageNo, $("#TBLExpenceList_length").val())
    }
    else {
        GettingExpenses(pageNo, 10)
    }

    GettingExpensTypes();
    GetExpenseModes();

    $('#btnSearchExp').click(function () { SearchExpClick() });

    $('#btnSearchClear').click(function () { SearchClearClick() });

    $('#btnSave').click(function () { SaveClick() });
    $('#btnYes').click(function () { YesClick() });
});

function ShowChange() {
    ShowLoader();
    pageNo = 1;

    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    pagerLoaded = false;

    if ($("#TBLExpenceList_length").val() != null && $("#TBLExpenceList_length").val() != "") {
        GettingExpenses(pageNo, $("#TBLExpenceList_length").val())
    }
    else {
        GettingExpenses(pageNo, 10)
    }
    return false;
}

function SortData(obj) {
    if (obj) {
        ShowLoader();
        var orderBy = obj.getAttribute('key');
        var header = obj.getAttribute('headerid')
        IsHeaderBindingRequired = true;
        if (sortBy.toUpperCase() == orderBy.toUpperCase()) {
            sortDirection = sortDirection.toUpperCase() == "ASC" ? "DESC" : "ASC";
        }
        else {
            sortBy = orderBy;
            sortDirection = "ASC";
        }

        if ($("#TBLExpenceList_length").val() != null && $("#TBLExpenceList_length").val() != "") {
            GettingExpenses(pageNo, $("#TBLExpenceList_length").val())
        }
        else {
            GettingExpenses(pageNo, 10)
        }
        HeaderId = header;
    }
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

function pad(num) {
    num = "0" + num;
    return num.slice(-2);
}

function GettingExpenses(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: ExpensesListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, ExpenseTypeId: $('#ddlSearchExpenseType').val(), Search: $('#txtSearch').val(), FromDate: $('#txtDateFirst').val(), ToDate: $('#txtDateTo').val(), sortBy: sortBy, sortDirection: sortDirection,branchId : $("#ddlBranch").val() },
        success: function (data) {
            $('#TBLExpenceList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var str = item.ExpensesDate.replace(/\D/g, "")
                    var description = item.Description == null ? "" : item.Description;
                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.ExpensesType + '</td>'
                        + '<td>' + parseFloat(item.ExpensesAmount).toFixed(2) + '</td>'
                        + '<td>' + pad(new Date(parseInt(str)).getDate()) + '/' + pad(new Date(parseInt(str)).getMonth() + 1) + '/' + new Date(parseInt(str)).getFullYear() + '</td>'
                        + '<td>' + description + '</td>'
                        + '<td>'
                        + '<button class="btn btn-success btn-xs btn-flat btnEdit" id="278" style="margin-right:10px;" title="Edit" onclick="return ShowModel(' + item.ExpensesId + ')">'
                        + '<span class="glyphicon glyphicon-edit"></span> Edit'
                        + '</button>'
                        + '<button class="btn btn-danger btn-xs btn-flat btnDelete" id="278" title="Delete" onclick="return Remove(' + item.ExpensesId + ')">'
                        + '<span class="glyphicon glyphicon-remove"></span> Delete'
                        + '</button>'
                        + '</td>'
                        + '</tr >';
                    $('#TBLExpenceList tbody').append(rows);
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
                                if ($("#TBLExpenceList_length").val() != null && $("#TBLExpenceList_length").val() != "") {
                                    GettingExpenses(pageNo, $("#TBLExpenceList_length").val())
                                }
                                else {
                                    GettingExpenses(pageNo, 10)
                                }
                            }
                        }
                    });
                }
            }
            else {
                var norecords = "<tr>" +
                    '<td colspan="4" class="Nodata">No data available</td>'
                    + "</tr>";
                $('#TBLExpenceList tbody').append(norecords);
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

function GettingExpensTypes() {
    $.ajax({
        cache: false,
        type: "GET",
        url: ExpenseTypeListUrl,
        dataType: "json",
        data: {branchId : $("#ddlBranch").val()},
        success: function (data) {
            if (data != null) {
                ExpenseTypes = data;
                $.each(data, function (i, item) {
                    var rows = option.replace(/{{value}}/g, item.ExpensesTypeId)
                        .replace(/{{text}}/g, item.ExpenseTypeName)
                    $('#ddlSearchExpenseType').append(rows);
                    $('#ddlExpenseType').append(rows);
                });
            }
        },
        error: function (errMsg) {
        },
        failure: function (errMsg) {
        }
    });

}

function GetExpenseModes() {
    $.ajax({
        cache: false,
        type: "GET",
        url: ExpenseModesUrl,
        dataType: "json",
        data: "",
        success: function (data) {
            if (data != null) {
                $.each(data, function (i, item) {
                    var rows = option.replace(/{{value}}/g, item.ModeId)
                        .replace(/{{text}}/g, item.Mode)
                    $('#ddlExpenseMode').append(rows);
                });
            }
        },
        error: function (errMsg) {
        },
        failure: function (errMsg) {
        }
    });
}

function ShowModel(ExpenseId) {
    $('body').addClass("modal-open");
    if (ExpenseId == 0) {
        $('#addExpense').modal('show');
    }
    else {
        ShowLoader();
        GettingExpenseDetailsById(ExpenseId);
    }
    return false;
}

function HideModel(id) {
    $('body').removeClass("modal-open");
    $(id).modal('hide');
    ClearAllRecord();
    return false;
}


function GettingExpenseDetailsById(ExpenseId) {
    $.ajax({
        cache: false,
        type: "GET",
        url: ExpenseDetailsByIdUrl,
        dataType: "json",
        data: { ExpensesId: ExpenseId },
        success: function (data) {
            if (data != null) {
                $.each(data, function (i, item) {
                    var str = item.ExpensesDate.replace(/\D/g, "")
                    $('#ddlExpenseType').val(item.ExpensesTypeId);
                    $('#txtExpenseAmount').val(parseFloat(item.ExpensesAmount).toFixed(2));
                    $('#ddlExpenseMode').val(item.ExpensesMode);
                    $('#txtReferenceNumber').val(item.ReferenceNumber == null ? "" : item.ReferenceNumber);
                    $('#txtDescription').val(item.Description == null ? "" : item.Description);
                    $('#HFExpenseId').val(item.ExpensesId);
                    $('#txtExpenseDate').datepicker('setDate', new Date(parseInt(str)));
                    $('#addExpense').modal('show');
                });
            }
            HideLoader()
        },
        error: function (errMsg) {
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader()
        }
    });
}

function SearchExpClick() {
    ShowLoader();
    pagerLoaded = false;
    pageNo = 1;
    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    if ($("#TBLExpenceList_length").val() != null && $("#TBLExpenceList_length").val() != "") {
        GettingExpenses(pageNo, $("#TBLExpenceList_length").val())
    }
    else {
        GettingExpenses(pageNo, 10)
    }
}

function SearchClearClick() {
    ShowLoader();
    $('input[type=datetime]').datepicker('setDate', new Date());
    $('#ddlSearchExpenseType').val("0");
    $('#txtSearch').val("");
    pagerLoaded = false;
    pageNo = 1;
    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    if ($("#TBLExpenceList_length").val() != null && $("#TBLExpenceList_length").val() != "") {
        GettingExpenses(pageNo, $("#TBLExpenceList_length").val())
    }
    else {
        GettingExpenses(pageNo, 10)
    }
}

var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';

function Validate() {
    var IsValid = true;
    clearErrormessage();

    if ($('#ddlExpenseType').val() == "0") {
        $('#ddlExpenseType').parent().append(errorSpan.replace(/{{Message}}/g, "Please Select Expense Mode"));
        $('#ddlExpenseType').parent().addClass("has-error");
        IsValid = false;
    }

    if ($('#txtExpenseAmount').val() == "") {
        $('#txtExpenseAmount').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Expense Amount"));
        $('#txtExpenseAmount').parent().addClass("has-error");
        IsValid = false;
    }

    if ($('#ddlExpenseMode').val() == "0") {
        $('#ddlExpenseMode').parent().append(errorSpan.replace(/{{Message}}/g, "Please Select Expense Mode"));
        $('#ddlExpenseMode').parent().addClass("has-error");
        IsValid = false;
    }

    if ($('#txtExpenseDate').val() == "") {
        $('#txtExpenseDate').parent().append(errorSpan.replace(/{{Message}}/g, "Please Select Date"));
        $('#txtExpenseDate').parent().addClass("has-error");
        IsValid = false;
    }

    return IsValid;
}

function SaveClick() {
    ShowLoader();
    if (Validate()) {
        var Expenses = {
            "ExpensesId": $('#HFExpenseId').val(), "ExpenseTypeId": $('#ddlExpenseType').val(),
            "ExpenseAmount": $('#txtExpenseAmount').val(), "ExpenseDate": new Date($('#txtExpenseDate').val()),
            "Description": $('#txtDescription').val(), "IsActive": true,
            "ReferenceNumber": $("#txtReferenceNumber").val(), "ExpenseMode": $('#ddlExpenseMode').val(),"BranchId":$("#ddlBranch").val()
        };

        $.ajax({
            cache: false,
            type: "POST",
            url: SaveExpenseUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ obExpens: Expenses }),
            success: function (data) {
                if (data.Success == "True") {
                    HideModel("#addExpense");
                    ClearAllRecord();
                    pagerLoaded = false;
                    if ($('.pagination').data("twbs-pagination")) {
                        $('.pagination').twbsPagination('destroy');
                    }
                    pageNo = 1;
                    if ($("#TBLExpenceList_length").val() != null && $("#TBLExpenceList_length").val() != "") {
                        GettingExpenses(pageNo, $("#TBLExpenceList_length").val())
                    }
                    else {
                        GettingExpenses(pageNo, 10)
                    }
                }
                else {
                    alert(data.Message);
                }
            },
            error: function (errMsg) {
                alert(errMsg.responseText);
            },
            failure: function (errMsg) {
                alert(errMsg.responseText);
            }
        });
    }
    else {
        HideLoader();
    }
}

function clearErrormessage() {
    $('#ddlExpenseType').parent().removeClass("has-error");
    $('#ddlExpenseType').parent().find(".help-block-error").remove();
    $('#txtExpenseAmount').parent().removeClass("has-error");
    $('#txtExpenseAmount').parent().find(".help-block-error").remove();
    $('#ddlExpenseMode').parent().removeClass("has-error");
    $('#ddlExpenseMode').parent().find(".help-block-error").remove();
    $('#txtExpenseDate').parent().removeClass("has-error");
    $('#txtExpenseDate').parent().find(".help-block-error").remove();
}

function ClearAllRecord() {
    clearErrormessage();
    $('#ddlExpenseType').val("0");
    $('#txtExpenseAmount').val("");
    $('#ddlExpenseMode').val("");
    $('#txtReferenceNumber').val("");
    $('#txtDescription').val("");
    $('#HFExpenseId').val("");
    $('#txtExpenseDate').datepicker('setDate', new Date());
    

    $('#addExpense').modal('hide');
}
var expenseId = 0;
function Remove(ExpenseId) {
    if (ExpenseId != 0) {
        expenseId = ExpenseId;
        $('#modal_Conformation').modal('show');
    }
    return false;
}

function YesClick() {
    ShowLoader();
    $('#modal_Conformation').modal('hide');
    $.ajax({
        cache: false,
        type: "POST",
        url: RemovingExpenseUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ ExpenseId: expenseId }),
        success: function (data) {
            if (data.Success == "True") {
                pagerLoaded = false;
                if ($('.pagination').data("twbs-pagination")) {
                    $('.pagination').twbsPagination('destroy');
                }
                if ($("#TBLExpenceList_length").val() != null && $("#TBLExpenceList_length").val() != "") {
                    GettingExpenses(pageNo, $("#TBLExpenceList_length").val())
                }
                else {
                    GettingExpenses(pageNo, 10)
                }
            }
            else {
                HideLoader();
                alert(data.Message);
            }
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

