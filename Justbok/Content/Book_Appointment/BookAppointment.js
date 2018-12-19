//Variable Declaration
var defaultview = 'agendaWeek';
var calendarviewbuttons = 'month,agendaWeek,agendaDay';
var _date;
var _calEvent, Configs;
var _eventId = 0;
var option = '<option value="{{value}}">{{text}}</option>';
var checkboxId = "";
var ShowSubModel = 0, ShowConformationModel = 0;
var configs;
var startdate;

$(document).ready(function () {
    $('#calendar').fullCalendar({
        eventConstraint: {
            start: '00:00',
            end: '24:59'
        },
        allDaySlot: false,
        defaultView: defaultview,
        header: {
            left: 'prev,next today',
            center: 'title',
            right: calendarviewbuttons
        },
        events: function (start, end, timezone, callback) {
            Slots(start._d, end._d, callback);
        },
        editable: false,
        viewRender: function (view) {
            setTimeout(function () {
                $(".fc-time-grid-container").removeAttr("style");
                $(window).trigger("resize");
            });
        },
        dayClick: function (when, allDay, jsEvent, view) {
            _date = when._d;
            _eventId = 0;
            AddingSlot();
        },
        loading: function (isLoading, view) {
            if (isLoading)
                ShowLoader();
            else
                HideLoader();
        },
        eventClick: function (calEvent, jsEvent, view) {
            _calEvent = calEvent;
            if (!_calEvent.isbooked) {
                _eventId = calEvent.originalid;
                ShowAppointmentOptions();
            }
            else {
                _eventId = calEvent.originalid;
                ViewAppointment();
            }
        },
    });
});

function Slots(startvalue, endvalue, CallBack) {
    $.ajax({
        cache: false,
        type: "GET",
        url: AppointmentSlotsUrl,
        dataType: "json",
        data: {
            start: $.datepicker.formatDate('dd M, yy', new Date(startvalue)),
            end: $.datepicker.formatDate('dd M, yy', new Date(endvalue)),
            branchId : $("#ddlBranch").val()
        },
        success: function (data) {
            var events = [];
            if (data != null) {
                $.each(data, function (i, item) {
                    events.push({
                        id: item.SlotId,
                        originalid: item.AppointmentSlotId,
                        appslotiD: item.AppSlotID,
                        title: item.Title,
                        staffname: item.StaffName,
                        staffId: item.StaffId,
                        pattern: item.Pattern,
                        TotalDuration: item.TotalDuration,
                        MinDuration: item.MinDuration,
                        UsedDuration: item.UsedDuration,
                        isbooked: item.isBooked,
                        start: new Date(parseInt(item.StartDate.replace(/\D/g, ""))),
                        end: new Date(parseInt(item.EndDate.replace(/\D/g, ""))),
                        color: item.Color,
                        branchId: item.BranchId
                    });
                });
            }
            CallBack(events);
        },
        error: function (errMsg) {

        },
        failure: function (data) {
        }
    });
}

function AddingSlot() {
    ShowLoader();
    $.ajax({
        type: "GET",
        url: AddAppointmentSlotUrl,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#ModalBody').html(data);
            $('#HFSlotId').val("0")
            $('#bookAppointment').modal('show');
            HideLoader();
        },
        error: function () {
            HideLoader();
            alert("Content load failed.");
        }
    });
}

function ShowAppointmentOptions() {
    ShowLoader();
    $.ajax({
        type: "GET",
        url: ChooseSlotOptionUrl,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#ModalBody').html(data);
            $('#bookAppointment').modal('show');
            HideLoader();
        },
        error: function () {
            HideLoader();
            alert("Content load failed.");
        }
    });
}

function ViewAppointment() {
    ShowLoader();
    $.ajax({
        type: "GET",
        url: ViewAppointmentUrl,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#ModalBody').html(data);
            $('#bookAppointment').modal('show');
        },
        error: function () {
            HideLoader();
            alert("Content load failed.");
        }
    });
}

function CloseModel() {
    $('#subModal').hide();
    $('#modal_Conformation').hide();
    $('#bookAppointment').modal('hide');
    $('.modal-backdrop').hide();
}

function CloseSubModel() {
    ShowSubModel = 1;
    $('#subModal').hide();
}

function CloseConformationModel() {
    ShowConformationModel = 1;
    if ($("#HFValue").val() == "0") {
        $('#modal_Conformation').hide();
    }
    else {
        NoClicked();
    }
}

//Add Appointment

function AddAppointmentLoading() {
    GetStaff();
    $('#as_startdate, #as_repeats_enddate').datepicker({
        dateFormat: 'M d, yy',
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });

    $("#as_starttime, #as_endtime").datetimepicker({
        format: 'HH:mm',
        useCurrent: true,
        stepping: 15
    });

    $('.select2').select2();

    $('input[type="checkbox"].flat-red, input[type="radio"].flat-red').iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue',
    });

    $('#as_repeats').on('ifChanged', function (e) {
        $(this).trigger("onchange", e);
    });

    if (_eventId == "0") {
        $("#HFSlotId").val("0");
        $('#gridSystemModalLabel').html("Add Appointment Slot");
        $("#as_starttime").data("DateTimePicker").date(formatAMPMUTC(_date));
        $("#as_endtime").data("DateTimePicker").date(formatAMPMUTC(new Date(_date.getTime() + 30 * 60000)));
        $('#as_startdate').datepicker('setDate', new Date(_date));
        _date.setDate(_date.getDate() + 7);
        $('#as_repeats_enddate').datepicker('setDate', new Date(_date));

        var weekday = new Array("sun", "mon", "tue", "wed", "thu", "fri", "sat");

        checkboxId = "#as_repeats_" + weekday[new Date(_date).getDay()];

        $(checkboxId).iCheck("check");

        $("#btnDelete").hide();
    }
    else {
        $("#HFSlotId").val(_eventId);
        $('#gridSystemModalLabel').html("Edit Appointment Slot");
        $("#btnDelete").show();
        GettingSlotDetails();
    }
}

function GettingSlotDetails() {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: SlotDetailsByIdUrl,
        dataType: "json",
        data: { SlotId: _eventId },
        success: function (data) {
            if (data != null) {
                $("#HFStaffId").val(data.StaffId);
                $("#txtStaff").val(data.StaffName);
                GettingAppConfigByStaff($("#HFStaffId").val(), data.ConfigIds);
                var StartDate = isoDateReviver(data.strStartDate);
                $("#as_starttime").data("DateTimePicker").date(formatAMPMUTC(StartDate));
                $("#as_endtime").data("DateTimePicker").date(formatAMPMUTC(new Date(StartDate.getTime() + data.TotalDuration * 60000)));
                $('#as_startdate').datepicker('setDate', new Date(parseInt(data.StartDate.replace(/\D/g, ""))));

                if (data.IsRepeat) {
                    $("#as_repeats").iCheck("check");
                    if (data.EndDate != null) {
                        $('#as_repeats_enddate').datepicker('setDate', new Date(parseInt(data.EndDate.replace(/\D/g, ""))));
                        $("#as_repeats_end_on").iCheck("check")
                    }
                    else {
                        $('#as_repeats_enddate').val();
                        $("#as_repeats_end_never").iCheck("check")
                    }
                    $("#repeat").show();
                }
                else {
                    $("#as_repeats").iCheck("uncheck");
                    var _startdate = new Date(parseInt(data.StartDate.replace(/\D/g, "")));
                    $('#as_repeats_enddate').datepicker('setDate', new Date(_startdate.setDate(_startdate.getDate() + 7)));
                    $("#as_repeats_end_never").iCheck("check")
                    $("#repeat").hide();
                }

                var res = data.RepeatsOn.split(",");
                var weekday = new Array("sun", "mon", "tue", "wed", "thu", "fri", "sat");

                $.each(res, function (i, item) {
                    checkboxId = "#as_repeats_" + weekday[item];
                    $(checkboxId).iCheck("check");
                });

                if (data.SlotType == "0") {
                    $("#type_single").iCheck("check");
                    $("#as_minlength").val(data.MinDuration);
                    $("#as_slotlength").val("30");
                }
                else {
                    $("#type_multiple").iCheck("check");
                    $("#as_minlength").val("30");
                    $("#as_slotlength").val(data.MinDuration);
                }
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

function StaffBlur() {
    if ($("#txtStaff").val() == "") {
        $("#HFStaffId").val("");
        GettingAppConfigByStaff($("#HFStaffId").val(), "");
    }
}

function GetStaff() {
    $("#txtStaff").autocomplete({
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
            $("#HFStaffId").val(i.item.val);
            GettingAppConfigByStaff($("#HFStaffId").val(), "");
        },
        appendTo: "#bookAppointment",
        minLength: 2
    });
    return false;
}

function GettingAppConfigByStaff(StaffId, ConfigIds) {
    if (StaffId != "") {
        $("#loader").show();
        $('#divConfig').hide();
        $('#ddlAppConfig').find("option").remove();
        $("#ddlAppConfig").val('').trigger('change');
        $.ajax({
            cache: false,
            type: "GET",
            url: GetAppConfigByStaffIdUrl,
            dataType: "json",
            data: { staffId: StaffId, branchId: $("#ddlBranch").val() },
            success: function (data) {
                if (data != null) {
                    var config = [];
                    $.each(data, function (i, item) {
                        var rows = option.replace(/{{value}}/g, item.ConfigId)
                            .replace(/{{text}}/g, item.Title)
                        config.push(item.ConfigId)
                        $('#ddlAppConfig').append(rows);
                    });
                    if (ConfigIds != "") {
                        $("#ddlAppConfig").val(ConfigIds.split(",")).trigger('change');
                    }
                    else {
                        $("#ddlAppConfig").val(config).trigger('change');
                    }
                }
                $("#loader").hide();
                $('#divConfig').show();
            },
            error: function (errMsg) {
                $("#loader").hide();
                $('#divConfig').show();
            },
            failure: function (errMsg) {
                $("#loader").hide();
                $('#divConfig').show();
            }
        });
    }
    else {
        $('#ddlAppConfig').find("option").remove();
        $("#ddlAppConfig").val('').trigger('change');
    }
    return false;
}

function OnRepeat() {
    if ($("#as_repeats").is(":checked")) {
        $("#repeat").show();
    }
    else {
        $("#repeat").hide();
    }
    return false;
}

function SaveAppointmentSlot() {
    ShowLoader();
    if (ValidateSlot()) {
        var AppointmentSlotConfigs = [];
        var AppointmentSlotPatterns = [];

        $.each($("#ddlAppConfig").val(), function (i, item) {
            AppointmentSlotConfigs.push({
                "AppointmentId": item,
                "SlotId": $("#HFSlotId").val()
            });
        });

        var StarTime = $("#as_starttime").val();
        var Duration = $("#as_slotlength").val();
        var EndTime = $("#as_endtime").val();

        if ($("#type_multiple").is(":checked")) {
            var IsTrue = true;
            while (IsTrue) {
                var SlotEndTime = parseTime(StarTime);
                SlotEndTime = new Date(SlotEndTime);
                SlotEndTime = SlotEndTime.setTime(SlotEndTime.getTime() + (Duration * 60 * 1000));
                SlotEndTime = new Date(SlotEndTime);

                if (((parseTime(EndTime) - SlotEndTime) / (1000 * 60)) > 0) {
                    AppointmentSlotPatterns.push({
                        "SlotId": $("#HFSlotId").val(),
                        "Pattern": Pattern(StarTime),
                        "SlotDuration": Duration
                    });
                    StarTime = SlotEndTime.getHours() + ":" + SlotEndTime.getMinutes();
                }
                else {
                    IsTrue = false;
                    AppointmentSlotPatterns.push({
                        "SlotId": $("#HFSlotId").val(),
                        "Pattern": Pattern(StarTime),
                        "SlotDuration": ((parseTime(EndTime) - parseTime(StarTime)) / (1000 * 60))
                    });
                }
            }
        }
        else {
            AppointmentSlotPatterns.push({
                "SlotId": $("#HFSlotId").val(),
                "Pattern": Pattern(StarTime),
                "SlotDuration": ((parseTime(EndTime) - parseTime(StarTime)) / (1000 * 60))
            });
        }

        var AppointmentSlot = {
            SlotId: $("#HFSlotId").val(),
            StaffId: $("#HFStaffId").val(),
            StartDate: $("#as_startdate").val(),
            EndDate: $("#as_repeats").is(":checked") ? $("#as_repeats_end_never").is(":checked") ? null : $("#as_repeats_enddate").val() : $("#as_startdate").val(),
            IsRepeat: $("#as_repeats").is(":checked"),
            MinDuration: $("#type_multiple").is(":checked") ? $("#as_slotlength").val() : $("#as_minlength").val(),
            SlotType: $("#type_multiple").is(":checked") ? 1 : 0,
            IsActive: true,
            AppointmentSlotConfigs: AppointmentSlotConfigs,
            AppointmentSlotPatterns: AppointmentSlotPatterns,
            TotalDuaration: ((parseTime($("#as_endtime").val()) - parseTime($("#as_starttime").val())) / (1000 * 60)),
            UsedDuration: 0,
            BranchId : $("#ddlBranch").val()
        }

        $.ajax({
            cache: false,
            type: "POST",
            url: SavingAppointmentSlotUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ AppointmentSlots: AppointmentSlot }),
            success: function (data) {
                if (data.Success == "True") {
                    HideLoader();
                    toastr.success("Appointment Slot Infomration Saved Successfully.");
                    CloseModel();
                    $("#calendar").fullCalendar('refresh');
                    $('#calendar').fullCalendar('refetchEvents');
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
    return false;
}

function parseTime(cTime) {
    if (cTime == '') return null;
    var d = new Date();
    var time = cTime.match(/(\d+)(:(\d\d))?\s*(p?)/);
    d.setHours(parseInt(time[1]) + ((parseInt(time[1]) < 12 && time[4]) ? 12 : 0));
    d.setMinutes(parseInt(time[3]) || 0);
    d.setSeconds(0, 0);
    return d;
}

function Pattern(FromTime) {
    var res = FromTime.split(":");
    var Pattern = res[1] + " " + res[0] + " * * " + SelectedDays();
    return Pattern;
}

function SelectedDays() {
    var days = "";

    if ($("#as_repeats").is(":checked")) {
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
        days = $(checkboxId).val();
    }
    return days;
}

function ClearingSlotErrors() {
    $('#txtStaff').parent().find(".help-block-error").remove();
    $('#txtStaff').parent().removeClass("has-error");

    $('#ddlAppConfig').parent().parent().find(".help-block-error").remove();
    $('#ddlAppConfig').parent().parent().removeClass("has-error");

    $('#as_startdate').parent().find(".help-block-error").remove();
    $('#as_startdate').parent().removeClass("has-error");

    $('#as_repeats_wed').parent().parent().parent().parent().find(".help-block-error").remove();
    $('#as_repeats_wed').parent().parent().parent().parent().removeClass("has-error");

    $('#as_repeats_enddate').parent().find(".help-block-error").remove();
    $('#as_repeats_enddate').parent().removeClass("has-error");

    $('#as_startdate').parent().find(".help-block-error").remove();
    $('#as_startdate').parent().removeClass("has-error");

    $('#as_starttime').parent().find(".help-block-error").remove();
    $('#as_starttime').parent().removeClass("has-error");

    $('#as_endtime').parent().find(".help-block-error").remove();
    $('#as_endtime').parent().removeClass("has-error");

    $('#as_minlength').parent().find(".help-block-error").remove();
    $('#as_minlength').parent().removeClass("has-error");

    $('#as_slotlength').parent().find(".help-block-error").remove();
    $('#as_slotlength').parent().removeClass("has-error");

}

function ValidateSlot() {
    var IsValid = true;
    ClearingSlotErrors();
    if ($("#HFStaffId").val() == "" || $('#txtStaff').val() == "") {
        $('#txtStaff').parent().append("<span class='help-block help-block-error'>Please Select Staff Name</span>");
        $('#txtStaff').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#ddlAppConfig").val() == null) {
        $('#ddlAppConfig').parent().parent().append("<span class='help-block help-block-error'>Please Select Available For</span>");
        $('#ddlAppConfig').parent().parent().addClass("has-error");
        IsValid = false;
    }

    if (((parseTime($("#as_endtime").val()) - parseTime($("#as_starttime").val())) / (1000 * 60)) < 0) {
        $('#as_endtime').parent().append("<span class='help-block help-block-error'>Please Select Proper End Timing</span>");
        $('#as_endtime').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#as_repeats").is(":checked")) {
        if (!$(".Repeats_On").is(":checked")) {
            $('#as_repeats_wed').parent().parent().parent().parent().append("<span class='help-block help-block-error'>Please Select Repeats On</span>");
            $('#as_repeats_wed').parent().parent().parent().parent().addClass("has-error");
            IsValid = false;
        }
    }

    if ($("#as_repeats_end_on").is(":checked")) {
        if ($("#as_repeats_enddate").val() == "") {
            $('#as_repeats_enddate').parent().append("<span class='help-block help-block-error'>Please Select Ends On</span>");
            $('#as_repeats_enddate').parent().addClass("has-error");
            IsValid = false;
        }
    }

    if ($("#as_startdate").val() == "") {
        $('#as_startdate').parent().append("<span class='help-block help-block-error'>Please Select When</span>");
        $('#as_startdate').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#as_starttime").val() == "") {
        $('#as_starttime').parent().append("<span class='help-block help-block-error'>Please Select Start Timing</span>");
        $('#as_starttime').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#as_endtime").val() == "") {
        $('#as_endtime').parent().append("<span class='help-block help-block-error'>Please Select End Timing</span>");
        $('#as_endtime').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#type_single").is(":checked")) {
        if ($("#as_minlength").val() == "") {
            $('#as_minlength').parent().append("<span class='help-block help-block-error'>Please Enter Minutes</span>");
            $('#as_minlength').parent().addClass("has-error");
            IsValid = false;
        }
        else if ($("#as_minlength").val() == "0") {
            $('#as_minlength').parent().append("<span class='help-block help-block-error'>Minutes Must And Should Be Greater Than Zero</span>");
            $('#as_minlength').parent().addClass("has-error");
            IsValid = false;
        }
    }

    if ($("#type_multiple").is(":checked")) {
        if ($("#as_slotlength").val() == "") {
            $('#as_slotlength').parent().append("<span class='help-block help-block-error'>Please Enter Minutes</span>");
            $('#as_slotlength').parent().addClass("has-error");
            IsValid = false;
        }
        else if ($("#as_slotlength").val() == "0") {
            $('#as_slotlength').parent().append("<span class='help-block help-block-error'>Minutes Must And Should Be Greater Than Zero</span>");
            $('#as_slotlength').parent().addClass("has-error");
            IsValid = false;
        }
    }
    return IsValid;
}

function isoDateReviver(value) {
    if (typeof value === 'string') {
        var a = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)(?:([\+-])(\d{2})\:(\d{2}))?Z?$/.exec(value.replace(/\"/g, ""));
        if (a) {
            var utcMilliseconds = Date.UTC(+a[1], +a[2] - 1, +a[3], +a[4], +a[5], +a[6]);
            return new Date(utcMilliseconds);
        }
    }
    return value;
}

function DeleteSlot() {
    if (ShowSubModel == "0") {
        ShowLoader();
        $.ajax({
            type: "GET",
            url: DeleteAppointmentSlotUrl,
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (data) {
                $('#subModalContent').html(data);
                $('#subModal').css('z-index', '1230');
                $('#subModal').modal('show');
                $('#subModal').show();
                HideLoader();
            },
            error: function () {
                HideLoader();
                alert("Content load failed.");
            }
        });
    }
    else {
        $('#subModal').show();
    }
}

//Choose Slot Options
function bookAppointmentClick(BookSlotAppointmentUrl) {
    ShowLoader();
    $.ajax({
        type: "GET",
        url: BookSlotAppointmentUrl,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#ModalBody').html(data);
            HideLoader();
        },
        error: function () {
            HideLoader();
            alert("Content load failed.");
        }
    });
}

function EditAppSlotClick() {
    $.ajax({
        type: "GET",
        url: AddAppointmentSlotUrl,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#ModalBody').html(data);
        },
        error: function () {
            alert("Content load failed.");
        }
    });
}

//Delete Appointment Slot
function delteSlot(type) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "POST",
        url: DeleteSlotUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ SlotId: _calEvent.originalid, Type: type, SlotDate: _calEvent.start._d, branchId: _calEvent.branchId }),
        success: function (data) {
            if (data.Status == "success") {
                HideLoader();
                toastr.success("Appointment Slot Deleted Successfully.");
                CloseModel();
                $("#calendar").fullCalendar('refresh');
                $('#calendar').fullCalendar('refetchEvents');
            }
            else {
                HideLoader();
                toastr.error("Unable To Delete Appointment Slot.");
            }
        },
        error: function (errMsg) {
            HideLoader();
            toastr.error("Unable To Delete Appointment Slot.");
        },
        failure: function (errMsg) {
            HideLoader();
            toastr.error("Unable To Delete Appointment Slot.");
        }
    });
}

//Book Slot Appointment
function BookSlotAppLoading() {
    GetMembers();
    BindingAppointments();
    $('#gridSystemModalLabel').html("Book Appointment");
    $('#repeatsuntil').datepicker({
        dateFormat: 'M d, yy',
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100",
        onSelect: function (selected, evnt) {
            selectionchange();
        }
    });

    $('.select2').select2();

    $('input[type="checkbox"].flat-red, input[type="radio"].flat-red').iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue',
    });

    $('#as_repeats_slot').on('ifChanged', function (e) {
        $(this).trigger("onchange", e);
    });
    var _date = _calEvent.start._d, _enddate = new Date();
    _enddate.setDate(_date.getDate() + 7);
    configs = _calEvent.configs;
    $('#repeatsuntil').datepicker('setDate', new Date(_enddate));
    $('#spnWhen').html(moment(_date).format("ddd, MMM DD, hh:mm a"));
    startdate = moment(_date).format("MMM DD, YYYY");
    $('#spnWith').html(_calEvent.staffname);
    $("#emailuser").iCheck("check");
    $("#emailstaff").iCheck("check");
    BindingHowLong();
}

function OnRepeat_Slot() {
    if ($("#as_repeats_slot").is(":checked")) {
        $("#repeatbox").show();
    }
    else {
        $("#repeatbox").hide();
    }
    gettingBills();
    return false;
}

function BindingAppointments() {
    $.ajax({
        cache: false,
        type: "GET",
        url: AppointConfigBySlotIdUrl_1,
        dataType: "json",
        data: { SlotId: _calEvent.originalid },
        success: function (data) {
            if (data != null) {
                configs = data;
                $.each(data, function (i, item) {
                    var rows = option.replace(/{{value}}/g, item.AppointmentId)
                        .replace(/{{text}}/g, item.Title)
                    $('#ac_id').append(rows);
                });
            }
        },
        error: function (errMsg) {
        },
        failure: function (errMsg) {
        }
    });
    return false;
}

function BindingHowLong() {
    var MinDuration = _calEvent.MinDuration;
    var TotalDuration = _calEvent.TotalDuration;
    var UsedDuration = _calEvent.UsedDuration;

    var RemainDuration = TotalDuration - UsedDuration;
    var slots = Math.floor(RemainDuration / MinDuration);

    if (slots > 0) {
        for (var i = 1; i <= slots; i++) {
            var rows = option.replace(/{{value}}/g, MinDuration * i)
                .replace(/{{text}}/g, MinDuration * i + " minutes")
            $('#length').append(rows);
        }
    }
}

function GetMembers() {
    $("#txtMember").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: GetMembersForAutoCompleteUrl,
                data: { prefix: request.term,branchId : $("#ddlBranch").val() },
                dataType: "json",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item, id) {

                        return {
                            label: item.FirstName + " " + item.LastName,
                            val: item.MemberId
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
            $("#HFMemberId").val(i.item.val);
            selectionchange();
        },
        appendTo: "#bookAppointment",
        minLength: 2
    });
    return false;
    return false;
}

function BookClick() {
    ShowLoader();
    if (ValidateBook()) {
        var AppointmentBills = [];

        $.each($('#previewapptbills tbody').find("tr"), function (i, item) {
            AppointmentBills.push({
                "BookingId": $("#HFSBookingId").val(),
                "BillDate": $(this).find(".Date").html(),
                "Description": $(this).find(".Title").html(),
                "Amount": $(this).find(".Amount").html()
            });
        });

        var BookAppointment = {
            BookingId: $("#HFSBookingId").val(),
            MemberId: $("#HFMemberId").val(),
            StaffId: _calEvent.staffId,
            ConfigId: $("#ac_id").val(),
            StartDate: startdate,
            EndDate: $("#as_repeats").is(":checked") ? $("#repeatsuntil").val() : startdate,
            IsRepatable: $("#as_repeats").is(":checked"),
            Pattern: _calEvent.pattern,
            MeetingDuration: $("#length").val(),
            IsActive: true,
            AppointmentBills: AppointmentBills,
            AppSlotId: _calEvent.originalid,
            BranchId: $("#ddlBranch").val()
        }

        $.ajax({
            cache: false,
            type: "POST",
            url: BookAppointmentUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ objBookAppointment: BookAppointment }),
            success: function (data) {
                if (data.Success == "True") {
                    HideLoader();
                    toastr.success("Appointment Booked Successfully.");
                    CloseModel();
                    $("#calendar").fullCalendar('refresh');
                    $('#calendar').fullCalendar('refetchEvents');
                }
                else {
                    HideLoader();
                    toastr.error("Unable To Save Information.");
                }
            },
            error: function (errMsg) {
                HideLoader();
                toastr.error("Unable To Update Information.");
            },
            failure: function (errMsg) {
                HideLoader();
                toastr.error("Unable To Save Information.");
            }
        });
    } else {
        HideLoader();
    }
    return false;

}

function selectionchange() {
    gettingBills();
    return false;
}

function gettingBills() {
    if ($("#HFMemberId").val() != "" && $("#ac_id").val() != "0" && $("#length").val() != "0") {
        ShowLoader();
        $.ajax({
            cache: false,
            type: "GET",
            url: GenerateBillUrl,
            dataType: "json",
            data: { startDate: startdate, endDate: $("#as_repeats_slot").is(":checked") ? $("#repeatsuntil").val() : startdate, memberId: $('#HFMemberId').val(), configId: $('#ac_id').val(), duration: $('#length').val(), title: $('#ac_id option:selected').text() },
            success: function (data) {
                $('#previewapptbills tbody').find("tr").remove();
                if (data != null) {
                    if (data.Status == "Success") {
                        $.each(data.Data, function (i, item) {
                            var rows = '<tr role="row" class="odd">'
                                + '<td class="Date">' + item.Date + '</td>'
                                + '<td class="Title">' + item.Title + '</td>'
                                + '<td class="Amount">' + parseFloat(item.Amount).toFixed(2) + '</td>'
                                + '</tr >';
                            $('#previewapptbills tbody').append(rows);
                        });
                    }
                    else {
                        toastr.error(data.Message);
                    }
                    HideLoader();
                }
            },
            error: function (errMsg) {
                HideLoader();
            },
            failure: function (errMsg) {
                HideLoader();
            }
        });
    }
    else {
        $('#previewapptbills tbody').find("tr").remove();
    }
}

function ClearingBookErrors() {
    $('#txtMember').parent().find(".help-block-error").remove();
    $('#txtMember').parent().removeClass("has-error");

    $('#ac_id').parent().find(".help-block-error").remove();
    $('#ac_id').parent().removeClass("has-error");

    $('#length').parent().find(".help-block-error").remove();
    $('#length').parent().removeClass("has-error");

    $('#repeatsuntil').parent().find(".help-block-error").remove();
    $('#repeatsuntil').parent().removeClass("has-error");

}

function ValidateBook() {
    var IsValid = true;
    ClearingBookErrors();
    if ($("#HFMemberId").val() == "" || $('#txtMember').val() == "") {
        $('#txtMember').parent().append("<span class='help-block help-block-error'>Please Select Client</span>");
        $('#txtMember').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#ac_id").val() == "0") {
        $('#ac_id').parent().append("<span class='help-block help-block-error'>Please Select Appointment</span>");
        $('#ac_id').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#length").val() == "0") {
        $('#length').parent().append("<span class='help-block help-block-error'>Please Select How Long</span>");
        $('#length').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#as_repeats_slot").is(":checked")) {
        if ($("#repeatsuntil").val() == "") {
            $('#repeatsuntil').parent().append("<span class='help-block help-block-error'>Please Select Repeats Until</span>");
            $('#repeatsuntil').parent().addClass("has-error");
            IsValid = false;
        }
    }
    return IsValid;
}

//View Appointment
function BindingAppointmentDetails() {
    $('#gridSystemModalLabel').html("View Appointment");
    ShowConformationModel = 0;
    $.ajax({
        cache: false,
        type: "GET",
        url: GettingBookingDetailsByIdUrl,
        dataType: "json",
        data: { BookingId: _calEvent.originalid },
        success: function (data) {
            $('#previewapptbills tbody').find("tr").remove();
            if (data != null) {
                if (data.Status == "Success") {

                    $("#spnWho").html(data.Data.Client);
                    $("#spnWith").html(data.Data.With);
                    $("#spnAppointment").html(data.Data.Appointment);
                    $("#spnWhen").html(moment(data.Data.When).format("ddd, MMM DD, hh:mm a"));
                    $("#spnSlotDate").html(moment(_calEvent.start._d).format("ddd, MMM DD, hh:mm a"));


                    if ($(data.Data.IsRepatable)) {
                        $("#repeatbox").show();
                        $("#spnRepeatsUntil").html(moment(data.Data.RepeatsUntil).format("MMM DD, YYYY"));
                    }
                    $("#spnLength").html(data.Data.Length + " minutes");
                    if (data.Data.Bills != null) {
                        $.each(data.Data.Bills, function (i, item) {
                            var rows = '<tr role="row" class="odd">'
                                + '<td class="Date">' + item.Date + '</td>'
                                + '<td class="Title">' + item.Title + '</td>'
                                + '<td class="Amount">' + parseFloat(item.Amount).toFixed(2) + '</td>'
                                + '</tr >';
                            $('#previewapptbills tbody').append(rows);
                        });
                    }
                }
                else {
                    toastr.error(data.Message);
                }
                HideLoader();
            }
        },
        error: function (errMsg) {
            HideLoader();
            toastr.errMsg("Unable to view Appointment Details");
        },
        failure: function (errMsg) {
            HideLoader();
            toastr.errMsg("Unable to view Appointment Details");
        }
    });
}

function btnBookDeleteClicked() {
    if (ShowConformationModel == 0) {
        $('#modal_Conformation').css('z-index', '1230');
        $('#modal_Conformation').modal('show');
        $('#modal_Conformation').show();
    }
    else {
        $("#modal_Conformation").show();
    }
    $("#HFValue").val("0");
    return false;

}

function YesClicked() {
    ShowLoader();
    if ($("#HFValue").val() == "1") {
        $.ajax({
            cache: false,
            type: "POST",
            url: DeleteBookUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ BookId: _calEvent.originalid, BillToDelete: true }),
            success: function (data) {
                if (data.Status == "success") {
                    HideLoader();
                    toastr.success("Appointment Deleted Successfully.");
                    CloseModel();
                    $("#calendar").fullCalendar('refresh');
                    $('#calendar').fullCalendar('refetchEvents');
                }
                else {
                    HideLoader();
                    toastr.error("Unable To Delete Appointment.");
                }
            },
            error: function (errMsg) {
                HideLoader();
                toastr.error("Unable To Delete Appointment.");
            },
            failure: function (errMsg) {
                HideLoader();
                toastr.error("Unable To Delete Appointment.");
            }
        });
    }
    else {
        $('#modal_Conformation').hide();
        $("#HFValue").val("1");
        $("#Conformationbody").html("Do you want to cancel bills associated with this appointment");
        $('#modal_Conformation').show();
        HideLoader();
    }
}

function NoClicked() {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "POST",
        url: DeleteBookUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ BookId: _calEvent.originalid, BillToDelete: false }),
        success: function (data) {
            if (data.Status == "success") {
                HideLoader();
                toastr.success("Appointment Deleted Successfully.");
                CloseModel();
                $("#calendar").fullCalendar('refresh');
                $('#calendar').fullCalendar('refetchEvents');
            }
            else {
                HideLoader();
                toastr.error("Unable To Delete Appointment.");
            }
        },
        error: function (errMsg) {
            HideLoader();
            toastr.error("Unable To Delete Appointment.");
        },
        failure: function (errMsg) {
            HideLoader();
            toastr.error("Unable To Delete Appointment.");
        }
    });
}