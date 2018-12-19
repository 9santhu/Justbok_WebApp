var pagerLoaded = false, pageNo = 1, sortBy = "", sortDirection = "", HeaderId = "", classId = 0, timePagerLoaded = false, timingpageNo = 1, timingId = 0;
var deletionType = 0;

$(window).load(function () {
    ShowLoader();
});

$(document).ready(function () {
    ShowLoader();
    BindingClasses();
});

function BindingClasses() {
    pageNo = 1;
    sortBy = "classname";
    sortDirection = "asc";
    HeaderId = "classname";
    pagerLoaded = false;

    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }

    if ($("#tblClasses_length").val() != null && $("#tblClasses_length").val() != "") {
        GettingClasses(pageNo, $("#tblClasses_length").val())
    }
    else {
        GettingClasses(pageNo, 10)
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

        if ($("#tblClasses_length").val() != null && $("#tblClasses_length").val() != "") {
            GettingClasses(pageNo, $("#tblClasses_length").val())
        }
        else {
            GettingClasses(pageNo, 10)
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

function ShowChange() {
    ShowLoader();
    pageNo = 1;

    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    pagerLoaded = false;

    if ($("#tblClasses_length").val() != null && $("#tblClasses_length").val() != "") {
        GettingAppointConfig(pageNo, $("#tblClasses_length").val())
    }
    else {
        GettingAppointConfig(pageNo, 10)
    }
    return false;
}

function GettingClasses(pageno, pagesize) {
    $.ajax({
        cache: false,
        type: "GET",
        url: ClassesUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, classname: $('#searchClassName').val(), description: $('#searchDescription').val(), sortBy: sortBy, sortDirection: sortDirection, branchId: $("#ddlBranch").val() },
        success: function (data) {
            $('#tblClasses tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.ClassName + '</td>'
                        + '<td>' + item.Description + '</td>'
                        + '<td>' + item.AttendenceLimit + '</td>'
                        + '<td>' + item.ReservationLimit + '</td>'
                        + '<td>'
                        + '<button class="btn btn-success btn-xs btn-flat btnEdit" id="278" style="margin-right:10px;" title="Edit" onclick="return ClassViewShow(' + item.ClassId + ')">'
                        + '<span class="glyphicon glyphicon-edit"></span> Edit'
                        + '</button>'
                        + '<button class="btn btn-primary bg-purple btn-xs btn-flat btnTime" id="278" style="margin-right:10px;" title="Time" onclick="return ClassTimesViewShow(' + item.ClassId + ')">'
                        + '<span class="glyphicon glyphicon-time"></span> Time'
                        + '</button>'
                        + '<button class="btn btn-danger btn-xs btn-flat btnDelete" id="278" title="Delete" onclick="return Remove(' + item.ClassId + ')">'
                        + '<span class="glyphicon glyphicon-remove"></span> Delete'
                        + '</button>'
                        + '</td>'
                        + '</tr >';
                    $('#tblClasses tbody').append(rows);
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
                                if ($("#tblClasses_length").val() != null && $("#tblClasses_length").val() != "") {
                                    GettingClasses(pageNo, $("#tblClasses_length").val())
                                }
                                else {
                                    GettingClasses(pageNo, 10)
                                }
                            }
                        }
                    });
                }
            }
            else {
                var norecords = "<tr>" +
                    '<td colspan="5" class="Nodata">No data available</td>'
                    + "</tr>";
                $('#tblClasses tbody').append(norecords);
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

function ClassViewShow(id) {
    ShowLoader();
    classId = id;
    $.ajax({
        type: "GET",
        url: ClassViewUrl,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#modelBody').html(data);
            $(".classdialog").css("width", "700px");
            $('#c_id').val(id)
            $('#classView').modal('show');
        },
        error: function () {
            HideLoader();
            alert("Content load failed.");
        }
    });
}

function HideModel() {
    $('#submodel').hide();
    $('#classView').modal('hide');
    $('.modal-backdrop').hide();
}

//Class View related script
function DropInChange() {
    if ($("#ClassDropIn").is(":checked")) {
        $("#dropin").show()
    }
    else {
        $("#dropin").hide()
    }
}

function ClassDetailsById() {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: ClassDetailsByIdUrl,
        dataType: "json",
        data: { ClassId: classId },
        success: function (data) {
            if (data != null && data.Status == "success") {
                $("#class_name").val(data.Data.ClassName);
                $("#c_description").val(data.Data.Description);
                $("#c_attendance_limit").val(data.Data.AttendenceLimit);
                $("#c_registration_limit").val(data.Data.ReservationLimit);
                if (data.Data.DropInType == 0) {
                    $("#ClassDropIn").iCheck("check");
                    $("#dropin").show()
                    $("#c_dropinrate").val(data.Data.DropInRate);
                }
                else {
                    $("#DefaultDropIn").iCheck("check");
                    $("#dropin").hide()
                    $("#c_dropinrate").val("");
                }
                $("#c_color").val(data.Data.CalenderColor);
                document.getElementById('c_color').jscolor.importColor();
            }
            else {
                toastr.error("Unable to get class information.")
            }
            HideLoader();
        },
        error: function (errMsg) {
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
        }
    });
}

function SaveClass() {
    ShowLoader();
    if (ValidateClass()) {
        var Class = {
            ClassId: $("#c_id").val(),
            ClassName: $("#class_name").val(),
            Description: $("#c_description").val(),
            AttendenceLimit: $("#c_attendance_limit").val(),
            ReservationLimit: $("#c_registration_limit").val(),
            DropInType: $("#ClassDropIn").is(":checked") ? 0 : 1,
            DropInRate: $("#c_dropinrate").val(),
            IsActive: true,
            CalendarColor: $("#c_color").val(),
            BranchId : $("#ddlBranch").val()
        }
        $.ajax({
            cache: false,
            type: "POST",
            url: SaveClassUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ objClass: Class }),
            success: function (data) {
                if (data.Status == "success") {
                    HideModel();
                    toastr.success("Class Infomration Saved Successfully.");
                    BindingClasses();
                }
                else {
                    HideLoader();
                    toastr.error("Unable To Save Information.");
                }
            },
            error: function (errMsg) {
                HideLoader();
                toastr.error("Unable To Save Information.");
            },
            failure: function (errMsg) {
                HideLoader();
                toastr.error("Unable To Save Information.");
            }
        });
    }
    else {
        HideLoader();
    }
}

function ValidateClass() {
    var IsValid = true;
    ClearingErrorMsgs();
    if ($("#class_name").val() == "") {
        $('#class_name').parent().append("<span class='help-block help-block-error'>Please Enter Class Name</span>");
        $('#class_name').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#c_description").val() == "") {
        $('#c_description').parent().append("<span class='help-block help-block-error'>Please Enter Description</span>");
        $('#c_description').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#c_attendance_limit").val() == "") {
        $('#c_attendance_limit').parent().append("<span class='help-block help-block-error'>Please Enter Attendence Limit</span>");
        $('#c_attendance_limit').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#c_registration_limit").val() == "") {
        $('#c_registration_limit').parent().append("<span class='help-block help-block-error'>Please Enter Reservation Limit</span>");
        $('#c_registration_limit').parent().addClass("has-error");
        IsValid = false;
    }

    if (!$("#DefaultDropIn").is(":Checked") && !$("#ClassDropIn").is(":Checked")) {
        $('#DefaultDropIn').parent().parent().parent().append("<span class='help-block help-block-error'>Please Select Drop In Type</span>");
        $('#DefaultDropIn').parent().parent().parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#ClassDropIn").is(":Checked") && $("#c_dropinrate").val() == "") {
        $('#c_dropinrate').parent().append("<span class='help-block help-block-error'>Please Enter Drop In Amount</span>");
        $('#c_dropinrate').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#c_color").val() == "") {
        $('#c_color').parent().append("<span class='help-block help-block-error'>Please Enter Calendar Color</span>");
        $('#c_color').parent().addClass("has-error");
        IsValid = false;
    }

    if (($("#c_registration_limit").val() - $("#c_attendance_limit").val()) > 0) {
        $("#limitwarn").show();
        IsValid = false;
    }
    else {
        $("#limitwarn").hide();
    }

    return IsValid;
}

function ClearingErrorMsgs() {
    $('#class_name').parent().find(".help-block-error").remove();
    $('#class_name').parent().removeClass("has-error");
    $('#c_description').parent().find(".help-block-error").remove();
    $('#c_description').parent().removeClass("has-error");
    $('#c_attendance_limit').parent().find(".help-block-error").remove();
    $('#c_attendance_limit').parent().removeClass("has-error");
    $('#c_registration_limit').parent().find(".help-block-error").remove();
    $('#c_registration_limit').parent().removeClass("has-error");
    $('#c_dropinrate').parent().find(".help-block-error").remove();
    $('#c_dropinrate').parent().removeClass("has-error");
    $('#c_color').parent().find(".help-block-error").remove();
    $('#c_color').parent().removeClass("has-error");
    $('#DefaultDropIn').parent().parent().parent().find(".help-block-error").remove();
    $('#DefaultDropIn').parent().parent().parent().removeClass("has-error");
}

function limitblur() {
    if ($("#c_registration_limit").val() != "" && $("#c_attendance_limit").val() != "") {
        if (($("#c_registration_limit").val() - $("#c_attendance_limit").val()) > 0) {
            $("#limitwarn").show();
        }
        else {
            $("#limitwarn").hide();
        }
    }
}

//Deleting Class
function Remove(id) {
    if (id != 0) {
        classId = id;
        deletionType = 1;
        $('#modal_Conformation').modal('show');
    }
    return false;
}

function OnYesClick() {
    if (deletionType == 1) {
        deleteClass();
    }
    else if (deletionType == 2) {
        deleteClassTiming();
    }
}

function deleteClass() {
    ShowLoader();
    $('#modal_Conformation').modal('hide');
    $.ajax({
        cache: false,
        type: "POST",
        url: DeleteClassUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ ClassId: classId }),
        success: function (data) {
            if (data.Status == "success") {
                toastr.success("Class deletion successful.");
                pagerLoaded = false;
                if ($('.pagination').data("twbs-pagination")) {
                    $('.pagination').twbsPagination('destroy');
                }
                if ($("#tblClasses_length").val() != null && $("#tblClasses_length").val() != "") {
                    GettingClasses(pageNo, $("#tblClasses_length").val())
                }
                else {
                    GettingClasses(pageNo, 10)
                }
            }
            else {
                HideLoader();
                toastr.error("Unable to delete class");
            }
        },
        error: function (errMsg) {
            HideLoader();
            toastr.error("Unable to delete class");
        },
        failure: function (errMsg) {
            HideLoader();
            toastr.error("Unable to delete class");
        }
    });
}

//Searching And Reseting
function Search() {
    ShowLoader();
    pagerLoaded = false;
    pageNo = 1;
    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    if ($("#tblClasses_length").val() != null && $("#tblClasses_length").val() != "") {
        GettingClasses(pageNo, $("#tblClasses_length").val())
    }
    else {
        GettingClasses(pageNo, 10)
    }
}

function Reset() {
    ShowLoader();
    $('#searchClassName').val("");
    $('#searchDescription').val("");

    pagerLoaded = false;
    pageNo = 1;
    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }
    if ($("#tblClasses_length").val() != null && $("#tblClasses_length").val() != "") {
        GettingClasses(pageNo, $("#tblClasses_length").val())
    }
    else {
        GettingClasses(pageNo, 10)
    }
}

//Class Times View
function ClassTimesViewShow(id) {
    ShowLoader();
    classId = id;
    $.ajax({
        type: "GET",
        url: ClassTimesViewUrl,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#modelBody').html(data);
            $(".classdialog").css("width", "900px");
            $("#gridSystemModalLabel").html("Class Timings");
            $("#class_id").val(id);
            $('#classView').modal('show');
        },
        error: function () {
            HideLoader();
            alert("Content load failed.");
        }
    });
}

function LoadingClassTimes() {
    timePagerLoaded = false;
    timingpageNo = 1;
    if ($('#timingpagination').data("twbs-pagination")) {
        $('#timingpagination').twbsPagination('destroy');
    }
    GettingClassTiming();
}

function HideSubModel() {
    $('#submodel').hide();
}

function ShowAddEditClassTimeView(id) {
    ShowLoader();
    timingId = id;
    $.ajax({
        type: "GET",
        url: AddEditClassTimeUrl,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#submodelbody').html(data);
            $("#submodel").css("z-index", "1230");
            $('#submodel').modal('show');
            if (id == 0) {
                $("#submodallabel").html("Add Class Timing");
            }
            else {
                $("#submodallabel").html("Edit Class Timing");
            }
            $("#ct_id").val(id);
            $('#submodel').show();
        },
        error: function () {
            HideLoader();
            alert("Content load failed.");
        }
    });
}

//Add Edit Class Time
function AddEditClassTimeLoading() {
    $('#repeats').on('ifChanged', function (e) {
        $(this).trigger("onchange", e);
    });

    $('input[type="checkbox"].flat-red, input[type="radio"].flat-red').iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue',
    });

    $('#class_end_date, #class_start_date').datepicker({
        dateFormat: 'M d, yy',
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });

    $("#class_start_time, #class_end_time").datetimepicker({
        format: 'HH:mm',
        useCurrent: true,
        stepping: 15
    });

    GetInstructor();
    $("#btnSaveClassTiming").click(function () {
        SaveClassTiming();
    });

    if (timingId == 0) {
        $("#class_start_time").data("DateTimePicker").date(formatAMPMUTC(new Date()));
        $("#class_end_time").data("DateTimePicker").date(formatAMPMUTC(new Date(new Date().getTime() + 30 * 60000)));
        $('#class_start_date').datepicker('setDate', new Date());
        HideLoader();
    }
    else {
        TimingDetailsById();
    }
    
}

function classRepeat() {
    if ($('#repeats').is(":checked")) {
        $(".repeats-option").show();
        $('#class_end_date').datepicker('setDate', new Date());
        $('#class_end_date').val("");
        $("#class_end_date").removeAttr('disabled');
    }
    else {
        $(".repeats-option").hide();
        var _startdate = new Date($('#class_start_date').val());
        _startdate.setDate(_startdate.getDate() + 1);
        $('#class_end_date').datepicker('setDate', _startdate);
        $("#class_end_date").attr('disabled', 'disabled');
    }
}

function GetInstructor() {
    $("#instructor").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: GetStaffForAutoCompleteUrl,
                data: { prefix: request.term,branchId : $("#ddlBranch").val() },
                dataType: "json",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item, id) {

                        return {
                            label: item.FirstName + " " + item.LastName,
                            val: item.StaffId
                        }
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            $("#HFInstructorId").val(i.item.val);
        },
        appendTo: "#submodel",
        minLength: 2
    });
    return false;
}

function SaveClassTiming() {
    ShowLoader();
    if (ValidateClassTiming()) {
        var ClassTiming = {
            TimingId: $("#ct_id").val(),
            StaffId: $("#HFInstructorId").val(),
            IsRepeats: $("#repeats").is(":checked") ? true : false,
            Every: $("#divisor").val(),
            StartDate: $("#class_start_date").val(),
            EndDate: $("#class_end_date").val(),
            StartTime: $("#class_start_time").val(),
            EndTime: $("#class_end_time").val(),
            Pattern: Pattern($("#class_start_time").val()),
            Duration: ((parseTime($("#class_end_time").val()) - parseTime($("#class_start_time").val())) / (1000 * 60)),
            BranchId: $("#ddlBranch").val(),
            ClassId: $("#class_id").val(),
            DayNames: SelectedDayNames(),
            DayVals: SelectedDays(),
            IsActive: true,
        }
        $.ajax({
            cache: false,
            type: "POST",
            url: SaveClassTimingUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ objClassTiming: ClassTiming }),
            success: function (data) {
                if (data.Status == "success") {
                    HideSubModel();
                    toastr.success("Class Timing Saved Successfully.");
                    LoadingClassTimes();
                }
                else {
                    HideLoader();
                    toastr.error("Unable To Save Information.");
                }
            },
            error: function (errMsg) {
                HideLoader();
                toastr.error("Unable To Save Information.");
            },
            failure: function (errMsg) {
                HideLoader();
                toastr.error("Unable To Save Information.");
            }
        });
    }
    else {
        HideLoader();
    }
}

function ValidateClassTiming() {
    var IsValid = true;
    ClearTimingErrorrMsg();

    if ($("#repeats").is(":checked")) {
        if (SelectedDays() == "") {
            $('#day_sunday').parent().parent().parent().parent().parent().append("<span class='help-block help-block-error'>Please Select Days</span>");
            $('#day_sunday').parent().parent().parent().parent().parent().addClass("has-error");
            IsValid = false;
        }
    }

    if ($("#instructor").val() == "" || $("#HFInstructorId").val() == "") {
        $('#instructor').parent().append("<span class='help-block help-block-error'>Please Select Instructor</span>");
        $('#instructor').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#class_start_date").val() == "") {
        $('#class_start_date').parent().append("<span class='help-block help-block-error'>Please Select Start Date</span>");
        $('#class_start_date').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#class_end_date").val() == "" && !$("#repeats").is(":checked")) {
        $('#class_end_date').parent().append("<span class='help-block help-block-error'>Please Select End Date</span>");
        $('#class_end_date').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#class_start_time").val() == "") {
        $('#class_start_time').parent().append("<span class='help-block help-block-error'>Please Select Start Time</span>");
        $('#class_start_time').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#class_end_time").val() == "") {
        $('#class_end_time').parent().append("<span class='help-block help-block-error'>Please Select End Time</span>");
        $('#class_end_time').parent().addClass("has-error");
        IsValid = false;
    }

    return IsValid;
}

function ClearTimingErrorrMsg() {
    $('#day_sunday').parent().parent().parent().parent().parent().find(".help-block-error").remove();
    $('#day_sunday  ').parent().parent().parent().parent().parent().removeClass("has-error");
    $('#instructor').parent().find(".help-block-error").remove();
    $('#instructor').parent().removeClass("has-error");
    $('#class_start_date').parent().find(".help-block-error").remove();
    $('#class_start_date').parent().removeClass("has-error");
    $('#class_end_date').parent().find(".help-block-error").remove();
    $('#class_end_date').parent().removeClass("has-error");
    $('#class_start_time').parent().find(".help-block-error").remove();
    $('#class_start_time').parent().removeClass("has-error");
    $('#class_end_time').parent().find(".help-block-error").remove();
    $('#class_end_time').parent().removeClass("has-error");
}

function Pattern(FromTime) {
    var res = FromTime.split(":");
    var Pattern = "";
    if ($("#divisor").val() == 1) {
        Pattern = res[1] + " " + res[0] + " * * " + SelectedDaysPattern();
    }
    else if ($("#divisor").val() == 2) {
        Pattern = res[1] + " " + res[0] + " * * " + SelectedDaysPattern();
    }
    else if ($("#divisor").val() == 3) {
        Pattern = res[1] + " " + res[0] + " * * " + SelectedDaysPattern();
    }
    else if ($("#divisor").val() == 4) {
        Pattern = res[1] + " " + res[0] + " * * " + SelectedDaysPattern();
    }
    return Pattern;
}

function SelectedDaysPattern() {
    var days = "",divisor="";

    if ($("#divisor").val() == 1) {
        divisor="";
    }
    else if ($("#divisor").val() == 2) {
        divisor="#2";
    }
    else if ($("#divisor").val() == 3) {
        divisor="#3";
    }
    else if ($("#divisor").val() == 4) {
        divisor="#4";
    }

    if ($("#repeats").is(":checked")) {
        $(".Repeats_On").each(function () {
            if ($(this).is(":checked")) {
                days += $(this).val() + divisor + ",";
            }
        });
        if (days != "") {
            days = days.substring(0, days.lastIndexOf(","));
        }
    }
    else {
        days = new Date($("#class_start_date").val()).getDay();
    }
    return days;
}

function SelectedDays() {
    var days = "";

    if ($("#repeats").is(":checked")) {
        $(".Repeats_On").each(function () {
            if ($(this).is(":checked")) {
                days += $(this).val() + ",";
            }
        });
        if (days != "") {
            days = days.substring(0, days.lastIndexOf(","));
        }
    }
    else {
        days = new Date($("#class_start_date").val()).getDay();
    }
    return days;
}

function SelectedDayNames() {
    var days = "";

    if ($("#repeats").is(":checked")) {
        $(".Repeats_On").each(function () {
            if ($(this).is(":checked")) {
                days += $(this).attr("day") + ",";
            }
        });
        if (days != "") {
            days = days.substring(0, days.lastIndexOf(","));
        }
    }
    else {
        days = moment(new Date($("#class_start_date").val())).format("ddd")
    }
    return days;
}

function GettingClassTiming() {
    $.ajax({
        cache: false,
        type: "GET",
        url: ClassTimingsUrl,
        dataType: "json",
        data: { page: timingpageNo, pagesize: 10, classId: classId },
        success: function (data) {
            $('#tblClassTimes tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var EndDate = "";
                    if (item.EndDate == null) {
                        EndDate = "-";
                    }
                    else {
                        EndDate = moment(new Date(parseInt(item.EndDate.replace(/\D/g, "")))).format("MMM DD, YYYY");
                    }
                    var ever = "";
                    if (item.Every == 2) {
                        ever = '(every 2 weeks)';
                    }
                    else if (item.Every == 3) {
                        ever = '(every 3 weeks)';
                    }
                    else if (item.Every == 4) {
                        ever = '(every 4 weeks)';
                    }
                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + item.DayNames + '<br/>' + ever + '</td>'
                        + '<td>' + item.StaffName + '</td>'
                        + '<td>' + moment(new Date(parseInt(item.StartDate.replace(/\D/g, "")))).format("MMM DD, YYYY") + '</td>'
                        + '<td>' + EndDate + '</td>'
                        + '<td>' + item.StartTime + '</td>'
                        + '<td>' + item.EndTime + '</td>'
                        + '<td>'
                        + '<button class="btn btn-success btn-xs btn-flat btnEdit" id="278" style="margin-right:10px;" title="Edit" onclick="return ShowAddEditClassTimeView(' + item.TimingId + ')">'
                        + '<span class="glyphicon glyphicon-edit"></span>'
                        + '</button>'
                        + '<button class="btn btn-danger btn-xs btn-flat btnDelete" id="278" title="Delete" onclick="return RemoveTiming(' + item.TimingId + ')">'
                        + '<span class="glyphicon glyphicon-remove"></span>'
                        + '</button>'
                        + '</td>'
                        + '</tr >';
                    $('#tblClassTimes tbody').append(rows);
                });

                if (data.Pages < timePagerLoaded) {
                    timePagerLoaded = data.Pages;
                }
                if (!timePagerLoaded) {
                    timePagerLoaded = true;
                    $('#timingpagination').twbsPagination({
                        totalPages: data.Pages,
                        visiblePages: 7,
                        startPage: timingpageNo,
                        onPageClick: function (event, page) {
                            ShowLoader();
                            if (timingpageNo != page) {
                                timingpageNo = page;
                                GettingClasses()
                            }
                        }
                    });
                }
            }
            else {
                var norecords = "<tr>" +
                    '<td colspan="7" class="Nodata">No data available</td>'
                    + "</tr>";
                $('#tblClassTimes tbody').append(norecords);
            }
            HideLoader();
        },
        error: function (errMsg) {
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
        }
    });
}

function TimingDetailsById() {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: ClassTimingDetailsByIdUrl,
        dataType: "json",
        data: { TimingId: timingId},
        success: function (data) {
            if (data != null) {

                if (data.IsRepeats) {
                    $("#repeats").iCheck("check");
                }
                else {
                    $("#repeats").iCheck("uncheck");
                }

                $("#divisor").val(data.Every);

                var res = data.DayVals.split(",");
                var weekday = new Array("sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday");

                $.each(res, function (i, item) {
                    checkboxId = "#day_" + weekday[item];
                    $(checkboxId).iCheck("check");
                });

                $("#class_start_time").data("DateTimePicker").date(data.StartTime);
                $("#class_end_time").data("DateTimePicker").date(data.EndTime);
                $('#class_start_date').datepicker('setDate', new Date(parseInt(data.StartDate.replace(/\D/g, ""))));

                if (data.EndDate != null) {
                    $('#class_end_date').datepicker('setDate', new Date(parseInt(data.EndDate.replace(/\D/g, ""))));
                }

                $("#HFInstructorId").val(data.StaffId);
                $("#instructor").val(data.StaffName);
            }
            else {
                $("#class_start_time").data("DateTimePicker").date(formatAMPMUTC(new Date()));
                $("#class_end_time").data("DateTimePicker").date(formatAMPMUTC(new Date(new Date().getTime() + 30 * 60000)));
                $('#class_start_date').datepicker('setDate', new Date());
            }
            HideLoader();
        },
        error: function (errMsg) {
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
        }
    });
}

//Deleting Class Timing
function RemoveTiming(id) {
    if (id != 0) {
        timingId = id;
        deletionType = 2;
        $("#modal_Conformation").css("z-index", "1230");
        $('#modal_Conformation').modal('show');
    }
    return false;
}

function deleteClassTiming() {
    ShowLoader();
    $('#modal_Conformation').modal('hide');
    $.ajax({
        cache: false,
        type: "POST",
        url: DeleteClassTimingUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ TimingId: timingId }),
        success: function (data) {
            if (data.Status == "success") {
                toastr.success("Class timing deletion successful.");
                LoadingClassTimes();
            }
            else {
                HideLoader();
                toastr.error("Unable to delete class timing");
            }
        },
        error: function (errMsg) {
            HideLoader();
            toastr.error("Unable to delete class timing");
        },
        failure: function (errMsg) {
            HideLoader();
            toastr.error("Unable to delete class timing");
        }
    });
}
