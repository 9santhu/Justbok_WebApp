var pagerLoaded = false, pageNo = 1, ExpenseTypes = null;

var option = '<option value="{{value}}">{{text}}</option>';

var sortBy = "", sortDirection = "", HeaderId = "";

$(document).ready(function () {
    ShowLoader();

    pageNo = 1;
    sortBy = "name";
    sortDirection = "ASC";
    HeaderId = "name";  

    if ($("#TBLExpenceList_length").val() != null && $("#TBLExpenceList_length").val() != "") {
        GettingExpenseType(pageNo, $("#TBLExpenceList_length").val())
    }
    else {
        GettingExpenseType(pageNo, 10)
    }

    $('#btnSearchExp').click(function () { SearchExpClick() });

    $('#btnSearchClear').click(function () { SearchClearClick() });

    $('#btnSave').click(function () { SaveClick() });
});

function ShowChange() {
    ShowLoader();
    pageNo = 1;

    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    pagerLoaded = false;

    if ($("#TBLExpenceList_length").val() != null && $("#TBLExpenceList_length").val() != "") {
        GettingExpenseType(pageNo, $("#TBLExpenceList_length").val())
    }
    else {
        GettingExpenseType(pageNo, 10)
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
            GettingExpenseType(pageNo, $("#TBLExpenceList_length").val())
        }
        else {
            GettingExpenseType(pageNo, 10)
        }
        HeaderId = header;
    }
}

function setClass() {
    $("th.Sortable").removeClass("sorting");
    $("th.Sortable").removeClass("sorting_asc");
    $("th.Sortable").removeClass("sorting_desc");
    $("th.Sortable").addClass("sorting");

    if (HeaderId != "") {
        $('#' + HeaderId).removeClass("sorting");
        var classname = (sortDirection == "ASC") ? "sorting_asc" : "sorting_desc";
        $('#' + HeaderId).addClass(classname);
    }
}

function GettingExpenseType(pageno, pagesize) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: ExpenseTypesUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, Name: $('#txtSearchName').val(), Description: $('#txtSearchDescription').val(), sortBy: sortBy, sortDirection: sortDirection, branchId: $("#ddlBranch").val() },
        success: function (data) {
            $('#TBLExpenceList tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.Name + '</td>'
                        + '<td>' + item.Description + '</td>'
                        + '<td>'
                        + '<button class="btn btn-success btn-xs btn-flat btnEdit" id="278" style="margin-right:10px;" title="Edit" onclick="return ShowModel(' + item.ExpenseTypeId + ')">'
                        + '<span class="glyphicon glyphicon-edit"></span> Edit'
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
                                    GettingExpenseType(pageNo, $("#TBLExpenceList_length").val())
                                }
                                else {
                                    GettingExpenseType(pageNo, 10)
                                }
                            }
                        }
                    });
                }
            }
            else {
                var norecords = "<tr>" +
                    '<td colspan="3" class="Nodata">No data available</td>'
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

function ShowModel(ExpenseTypeId) {
    $("#HFExpenseTypeId").val(ExpenseTypeId)
    if (ExpenseTypeId == 0) {
        $('#addExpenseType').modal('show');
    }
    else {
        ShowLoader();
        GettingExpenseTypeDetailsById(ExpenseTypeId);
    }
    return false;
}

function HideModel(id) {
    $(id).modal('hide');
    ClearAllRecord();
    return false;
}


function GettingExpenseTypeDetailsById(_ExpenseTypeId) {
    $.ajax({
        cache: false,
        type: "GET",
        url: ExpenseTypeDetailsByIdUrl,
        dataType: "json",
        data: { ExpenseTypeId: _ExpenseTypeId },
        success: function (data) {
            if (data != null) {
                $('#txtName').val(data.ExpenseTypeName);
                $('#txtDescription').val(data.ExpenseTypeDescription);
                $('#addExpenseType').modal('show');
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
        GettingExpenseType(pageNo, $("#TBLExpenceList_length").val())
    }
    else {
        GettingExpenseType(pageNo, 10)
    }
}

function SearchClearClick() {
    ShowLoader();
    $('#txtSearchName').val("");
    $('#txtSearchDescription').val("");
    pagerLoaded = false;
    pageNo = 1;
    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    if ($("#TBLExpenceList_length").val() != null && $("#TBLExpenceList_length").val() != "") {
        GettingExpenseType(pageNo, $("#TBLExpenceList_length").val())
    }
    else {
        GettingExpenseType(pageNo, 10)
    }
}

function Validate() {
    ClearErrorMessage();
    var IsValid = true;

    if ($('#txtName').val() == "0") {
        $('#txtName').parent().append(errorSpan.replace("<span class='help-block help-block-error'>Please enter expense type name</span>"));
        $('#txtName').parent().addClass("has-error");
        IsValid = false;
    }
    if ($('#txtDescription').val() == "") {
        $('#txtDescription').parent().append(errorSpan.replace("<span class='help-block help-block-error'>Please enter expense type description</span>"));
        $('#txtDescription').parent().addClass("has-error");
        IsValid = false;
    }

    return IsValid;
}

function SaveClick() {
    ShowLoader();
    if (Validate()) {
        var ExpenseType = {
            "ExpenseTypeId": $('#HFExpenseTypeId').val(), "ExpenseTypeName": $('#txtName').val(),
            "ExpenseTypeDescription": $('#txtDescription').val(), "BranchId": $('#ddlBranch').val()
        };

        $.ajax({
            cache: false,
            type: "POST",
            url: SaveExpenseTypeUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ objExpensesType: ExpenseType }),
            success: function (data) {
                if (data.Success == "True") {
                    toastr.success("Expense Type Infromation Saved Successfully.")
                    HideModel("#addExpenseType");
                    ClearAllRecord();
                    pagerLoaded = false;
                    if ($('.pagination').data("twbs-pagination")) {
                        $('.pagination').twbsPagination('destroy');
                    }
                    pageNo = 1;
                    if ($("#TBLExpenceList_length").val() != null && $("#TBLExpenceList_length").val() != "") {
                        GettingExpenseType(pageNo, $("#TBLExpenceList_length").val())
                    }
                    else {
                        GettingExpenseType(pageNo, 10)
                    }
                }
                else {
                    toastr.error(data.Message);
                }
            },
            error: function (errMsg) {
                toastr.error(errMsg.responseText);
            },
            failure: function (errMsg) {
                toastr.error(errMsg.responseText);
            }
        });
    }
    else {
        HideLoader();
    }
}

function ClearErrorMessage() {
    $('#txtDescription').datepicker('setDate', new Date());
    $('#txtDescription').parent().removeClass("has-error");
    $('#txtName').parent().find(".help-block-error").remove();
    $('#txtName').parent().removeClass("has-error");
}

function ClearAllRecord() {
    ClearErrorMessage();
    $('#txtDescription').val("");
    $('#txtName').val("");
    $('#addExpenseType').modal('hide');
}