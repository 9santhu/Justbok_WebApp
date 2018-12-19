var pagerLoaded = false, pageNo = 1, ExpenseTypes = null;

var option = '<option value="{{value}}">{{text}}</option>';

var sortBy = "", sortDirection = "", HeaderId = "", StaffType = 1;

var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';

$(window).load(function () { ShowLoader(); });
$(document).ready(function () {
    $('.select2').select2();
    ShowLoader();
    //GetTaxTypes();
    //GetStaff();

    $(".rate").keydown(function (event) {
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

    $(".minutes").keydown(function (event) {
        if (event.shiftKey == true) {
            event.preventDefault();
        }
        if ((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105)
            || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 46) {

        } else {
            event.preventDefault();
        }
    });

    pageNo = 1;
    sortBy = "TITLE";
    sortDirection = "ASC";
    HeaderId = "TITLE";

    if ($("#TblAppConfig_length").val() != null && $("#TblAppConfig_length").val() != "") {
        GettingAppointConfig(pageNo, $("#TblAppConfig_length").val())
    }
    else {
        GettingAppointConfig(pageNo, 10)
    }

    $('#btnSave').click(function () { btnSaveClick() });
    $('#btnSearch').click(function () { btnSearchClick() });
    $('#btnClear').click(function () { btnClearClick() });
    $('#btnYes').click(function () { btnYesClick() });
});

function GetTaxTypes() {
    $('#ddlTax').find("option").remove();
    $.ajax({
        cache: false,
        type: "GET",
        url: GetTaxTypesUrl,
        dataType: "json",
        data: "",
        success: function (data) {
            if (data != null) {
                $.each(data, function (i, item) {
                    var rows = option.replace(/{{value}}/g, item.TaxTypeId)
                        .replace(/{{text}}/g, item.Description)
                    $('#ddlTax').append(rows);
                });
            }
            isTaxLoaded = true;
            LoadingStaffBranch();
        },
        error: function (errMsg) {
        },
        failure: function (errMsg) {
        }
    });
    return false;
}

function GetStaff() {
    $('#ddlStaff').find("option").remove();;
    $.ajax({
        cache: false,
        type: "GET",
        url: GetStaffUrl,
        dataType: "json",
        data: { branchId: $('#ddlBranch').val() },
        success: function (data) {
            if (data != null) {
                $.each(data, function (i, item) {
                    var rows = option.replace(/{{value}}/g, item.StaffId)
                        .replace(/{{text}}/g, item.FirstName + " " + item.LastName)
                    $('#ddlStaff').append(rows);
                });
            }
            isStaffLoaded = true;
            LoadingStaffBranch();
        },
        error: function (errMsg) {
        },
        failure: function (errMsg) {
        }
    });
    return false;
}

var isStaffLoaded = false; isTaxLoaded = false;
function ShowModel(ConfigId) {
    $('body').addClass("modal-open");
    if (ConfigId == 0) {
        $("#HFConfigId").val(ConfigId)
        $('#addAppConfig').modal('show');
    }
    else {
        ShowLoader();
        $("#HFConfigId").val(ConfigId)
        GetStaff();
        GetTaxTypes();
    }
    return false;
}

function LoadingStaffBranch() {
    if (isStaffLoaded && isTaxLoaded) {
        GettingAppointmentConfigById($("#HFConfigId").val());
    }
}

function HideModel(id) {
    $('body').removeClass("modal-open");
    ClearAllRecord();
    $(id).modal('hide');
    return false;
}

function StaffChange(id) {
    if ($(id).val() == "1") {
        $('#Staff').hide();
        StaffType = 1;
    }
    else {
        $('#Staff').show();
        StaffType = 0;
    }
}

function TaxChange(id) {
    if ($(id).val() == "1") {
        $('#Tax').show();
    }
    else {
        $('#Tax').hide();
    }
    return false;
}

function Validate() {
    var IsValid = true;
    ClearErrorMsg();

    if ($('#txtTitle').val() == "") {
        $('#txtTitle').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Title"));
        $('#txtTitle').parent().addClass("has-error");
        IsValid = false;
    }


    if ($('#ddlTax').val() == "1" && $('#txtTax').val() == "") {
        $('#txtTax').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Tax Percentage"));
        $('#txtTax').parent().addClass("has-error");
        IsValid = false;
    }

    if (StaffType != "1" && $('#ddlStaff').val() == null) {
        $('#ddlStaff').parent().append(errorSpan.replace(/{{Message}}/g, "Please Select Staff"));
        $('#ddlStaff').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#Pricing_1").find(".rate").val() != "" && $('#Pricing_1').find(".minutes").val() == "") {
        $('#Pricing_1').find(".minutes").parent().parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter min"));
        $('#Pricing_1').find(".minutes").parent().parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#Pricing_1").find(".rate").val() == "" && $('#Pricing_1').find(".minutes").val() != "") {
        $('#Pricing_1').find(".rate").parent().parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Rs"));
        $('#Pricing_1').find(".rate").parent().parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#Pricing_2").find(".rate").val() != "" && $('#Pricing_2').find(".minutes").val() == "") {
        $('#Pricing_2').find(".minutes").parent().parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter min"));
        $('#Pricing_2').find(".minutes").parent().parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#Pricing_2").find(".rate").val() == "" && $('#Pricing_2').find(".minutes").val() != "") {
        $('#Pricing_2').find(".rate").parent().parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Rs"));
        $('#Pricing_2').find(".rate").parent().parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#Pricing_3").find(".rate").val() != "" && $('#Pricing_3').find(".minutes").val() == "") {
        $('#Pricing_3').find(".minutes").parent().parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter min"));
        $('#Pricing_3').find(".minutes").parent().parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#Pricing_3").find(".rate").val() == "" && $('#Pricing_3').find(".minutes").val() != "") {
        $('#Pricing_3').find(".rate").parent().parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Rs"));
        $('#Pricing_3').find(".rate").parent().parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#Pricing_4").find(".rate").val() != "" && $('#Pricing_4').find(".minutes").val() == "") {
        $('#Pricing_4').find(".minutes").parent().parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter min"));
        $('#Pricing_4').find(".minutes").parent().parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#Pricing_4").find(".rate").val() == "" && $('#Pricing_4').find(".minutes").val() != "") {
        $('#Pricing_4').find(".rate").parent().parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Rs"));
        $('#Pricing_4').find(".rate").parent().parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#Pricing_5").find(".rate").val() != "" && $('#Pricing_5').find(".minutes").val() == "") {
        $('#Pricing_5').find(".minutes").parent().parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter min"));
        $('#Pricing_5').find(".minutes").parent().parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#Pricing_5").find(".rate").val() == "" && $('#Pricing_5').find(".minutes").val() != "") {
        $('#Pricing_5').find(".rate").parent().parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Rs"));
        $('#Pricing_5').find(".rate").parent().parent().addClass("has-error");
        IsValid = false;
    }

    return IsValid;
}

function btnSaveClick() {
    if (Validate()) {
        ShowLoader();
        var IsAllStaff = false;

        var AppointmentStaffs = [];
        var AppointmentSlabs = [];

        if (StaffType == "1") {
            IsAllStaff = true;
        }
        else {
            var Staff = $('#ddlStaff').val();

            $.each(Staff, function (i, item) {
                AppointmentStaffs.push({
                    "StaffId": item,
                    "AppointmentId": $("#HFConfigId").val()
                });
            });
        }

        $('.Pricing').each(function () {
            if ($(this).find(".minutes").val() != "" && $(this).find(".rate").val() != "") {
                AppointmentSlabs.push({

                    "Minutes": $(this).find(".minutes").val(),
                    "Price": $(this).find(".rate").val(),
                    "AppointmentId": $("#HFConfigId").val()
                });
            }
        });

        var Appointment = {
            "Title": $('#txtTitle').val(), "IsAllStaff": IsAllStaff, "TaxType": $('#ddlTax').val(),
            "TaxPercentage": $('#txtTax').val(), "IsActive": true, "AppointmentStaffs": AppointmentStaffs,
            "AppointmentSlabs": AppointmentSlabs, "AppointmentId": $("#HFConfigId").val(), "BranchId": $("#ddlBranch").val()
        };

        $.ajax({
            cache: false,
            type: "POST",
            url: SaveAppointmentConfigUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ Appointment: Appointment }),
            success: function (data) {
                if (data.Success == "True") {
                    HideModel("#addAppConfig");
                    toastr.success("Appointment Configuration Infomration Saved Successfully.");
                    ClearAllRecord();
                    pagerLoaded = false;
                    if ($('.pagination').data("twbs-pagination")) {
                        $('.pagination').twbsPagination('destroy');
                    }
                    pageNo = 1;
                    if ($("#TblAppConfig_length").val() != null && $("#TblAppConfig_length").val() != "") {
                        GettingAppointConfig(pageNo, $("#TblAppConfig_length").val())
                    }
                    else {
                        GettingAppointConfig(pageNo, 10)
                    }
                }
                else {
                    toastr.error("Unable to save information.");
                    HideLoader();
                }
            },
            error: function (errMsg) {
                toastr.error("Unable to save information.");
                HideLoader();
            },
            failure: function (errMsg) {
                toastr.error("Unable to save information.");
                HideLoader();
            }
        });
    }
    else {
        HideLoader();
    }
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

        if ($("#TblAppConfig_length").val() != null && $("#TblAppConfig_length").val() != "") {
            GettingAppointConfig(pageNo, $("#TblAppConfig_length").val())
        }
        else {
            GettingAppointConfig(pageNo, 10)
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
        $('#' + HeaderId).removeClass();
        var classname = (sortDirection == "ASC") ? "sorting_asc" : "sorting_desc";
        $('#' + HeaderId).addClass(classname);
    }
}

function GettingAppointConfig(pageno, pagesize) {
    $.ajax({
        cache: false,
        type: "GET",
        url: AppointmentConfigListUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, Title: $('#txtSearchTitle').val(), sortBy: sortBy, sortDirection: sortDirection, branchId: $("#ddlBranch").val() },
        success: function (data) {
            $('#TblAppConfig tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.Title + '</td>'
                        + '<td>' + item.AppointmentStaffs + '</td>'
                        + '<td>' + item.AppointmentSlabs + '</td>'
                        + '<td>'
                        + '<button class="btn btn-success btn-xs btn-flat btnEdit" id="278" style="margin-right:10px;" title="Edit" onclick="return ShowModel(' + item.AppointmentId + ')">'
                        + '<span class="glyphicon glyphicon-edit"></span> Edit'
                        + '</button>'
                        + '<button class="btn btn-danger btn-xs btn-flat btnDelete" id="278" title="Delete" onclick="return Remove(' + item.AppointmentId + ')">'
                        + '<span class="glyphicon glyphicon-remove"></span> Delete'
                        + '</button>'
                        + '</td>'
                        + '</tr >';
                    $('#TblAppConfig tbody').append(rows);
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
                                if ($("#TblAppConfig_length").val() != null && $("#TblAppConfig_length").val() != "") {
                                    GettingAppointConfig(pageNo, $("#TblAppConfig_length").val())
                                }
                                else {
                                    GettingAppointConfig(pageNo, 10)
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
                $('#TblAppConfig tbody').append(norecords);
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

function ShowChange() {
    ShowLoader();
    pageNo = 1;

    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    pagerLoaded = false;

    if ($("#TblAppConfig_length").val() != null && $("#TblAppConfig_length").val() != "") {
        GettingAppointConfig(pageNo, $("#TblAppConfig_length").val())
    }
    else {
        GettingAppointConfig(pageNo, 10)
    }
    return false;
}

function btnSearchClick() {
    ShowLoader();
    pagerLoaded = false;
    pageNo = 1;
    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    if ($("#TblAppConfig_length").val() != null && $("#TblAppConfig_length").val() != "") {
        GettingAppointConfig(pageNo, $("#TblAppConfig_length").val())
    }
    else {
        GettingAppointConfig(pageNo, 10)
    }
}

function btnClearClick() {
    ShowLoader();
    $('#txtSearchTitle').val("");
    pagerLoaded = false;
    pageNo = 1;
    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    if ($("#TblAppConfig_length").val() != null && $("#TblAppConfig_length").val() != "") {
        GettingAppointConfig(pageNo, $("#TblAppConfig_length").val())
    }
    else {
        GettingAppointConfig(pageNo, 10)
    }
}

function ClearAllRecord() {
    $('#txtTitle').val("");


    $('#AllStaff').prop('checked', true);
    $('#SpecificStaff').prop('checked', false);

    $('#Staff').hide();
    $('#Tax').show();
    $('#txtTax').val("");

    $('#ddlTax').val("1");
    $("#ddlStaff").val('').trigger('change')
    $('.minutes').val("");
    $('.rate').val("");

    ClearErrorMsg();

    $("#HFConfigId").val("")
}

function ClearErrorMsg() {
    $('#ddlStaff').parent().removeClass("has-error");
    $('#ddlStaff').parent().find(".help-block-error").remove();
    $('#txtTax').parent().removeClass("has-error");
    $('#txtTax').parent().find(".help-block-error").remove();
    $('.rate').parent().parent().removeClass("has-error");
    $('.rate').parent().parent().find(".help-block-error").remove();
    $('.minutes').parent().parent().removeClass("has-error");
    $('.minutes').parent().parent().find(".help-block-error").remove();
    $('#txtTitle').parent().removeClass("has-error");
    $('#txtTitle').parent().find(".help-block-error").remove();
}

var varAppointmentId = 0;
function Remove(AppointmentId) {
    if (AppointmentId != 0) {
        varAppointmentId = AppointmentId;
        $('#modal_Conformation').modal('show');
    }
    return false;
}

function btnYesClick() {
    ShowLoader();
    $('#modal_Conformation').modal('hide');
    $.ajax({
        cache: false,
        type: "POST",
        url: DeactivateUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ AppointmentId: varAppointmentId }),
        success: function (data) {
            if (data.Success == "True") {
                toastr.success("Appointment Configuration Infomration Deleted Successfully.");
                pagerLoaded = false;
                if ($('.pagination').data("twbs-pagination")) {
                    $('.pagination').twbsPagination('destroy');
                }
                if ($("#TblAppConfig_length").val() != null && $("#TblAppConfig_length").val() != "") {
                    GettingAppointConfig(pageNo, $("#TblAppConfig_length").val())
                }
                else {
                    GettingAppointConfig(pageNo, 10)
                }
            }
            else {
                toastr.error("Unable To Delete Appointment Configuration Infomration.");
                HideLoader();
            }
        },
        error: function (errMsg) {
            toastr.error("Unable To Delete Appointment Configuration Infomration.");
            HideLoader();
        },
        failure: function (errMsg) {
            toastr.error("Unable To Delete Appointment Configuration Infomration.");
            HideLoader();
        }
    });
}

function GettingAppointmentConfigById(AppointmentId)     {
    $.ajax({
        cache: false,
        type: "GET",
        url: GetAppointmentConfigByIdUrl,
        dataType: "json",
        data: { AppointmentId: AppointmentId },
        success: function (data) {
            if (data != null) {
                $('#txtTitle').val(data.Title);
                if (data.IsAllStaff) {
                    $('#AllStaff').prop('checked', true);
                    $('#SpecificStaff').prop('checked', false);
                    $('#Staff').hide();
                    $("#ddlStaff").val('').trigger('change')
                }
                else {
                    $('#AllStaff').prop('checked', false);
                    $('#SpecificStaff').prop('checked', true);
                    $('#Staff').show();
                    if (data.ArrAppointmentStaffs != null && data.ArrAppointmentStaffs.length > 0) {
                        $("#ddlStaff").val(data.ArrAppointmentStaffs).trigger('change')
                    }
                    else {
                        $("#ddlStaff").val("").trigger('change')
                    }
                }

                if (data.TaxType == "1") {
                    $('#Tax').show();
                    $('#txtTax').val(data.TaxPercentage);
                    $('#ddlTax').val("1");
                }
                else {
                    $('#Tax').hide();
                    $('#txtTax').val("");
                    $('#ddlTax').val(data.TaxType);
                }

                var PriceId = 1;

                if (data.ArrAppointmentSlabs != null && data.ArrAppointmentSlabs.length > 0) {
                    $.each(data.ArrAppointmentSlabs, function (i, item) {

                        $("#Pricing_" + PriceId).find(".minutes").val(item.Minutes)
                        $("#Pricing_" + PriceId).find(".rate").val(item.Price)
                        PriceId = PriceId + 1;
                    });
                }
                $('#addAppConfig').modal('show');
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