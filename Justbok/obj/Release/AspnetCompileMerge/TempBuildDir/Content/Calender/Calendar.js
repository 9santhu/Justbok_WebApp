//Variable Declaration
var defaultview = 'agendaWeek';
var calendarviewbuttons = 'month,agendaWeek,agendaDay';
var _date = "", _calEvent = "";
var option = '<option value="{{value}}">{{text}}</option>';

var CalendarId = 0;
var r_membercount = 0;

$(document).ready(function () {
    ShowLoader();
    $('#calendar').fullCalendar({
        eventConstraint: {
            start: '00:00',
            end: '24:59'
        },
        allDaySlot: true,
        nowIndicator: true,
        defaultView: defaultview,
        header: {
            left: 'prev,next today',
            center: 'title',
            right: calendarviewbuttons
        },
        events: function (start, end, timezone, callback) {
            CalendarData(start._d, end._d, callback);
        },
        editable: false,
        viewRender: function (view) {
            setTimeout(function () {
                $(".fc-time-grid-container").removeAttr("style");
                $(window).trigger("resize");
            });
        },
        loading: function (isLoading, view) {
            if (isLoading)
                ShowLoader();
            else
                HideLoader();
        },
        dayClick: function (when, allDay, jsEvent, view) {
            _date = when._d;
            AddingClassTime();
        },
        eventClick: function (calEvent, jsEvent, view) {
            _calEvent = calEvent;
            if (_calEvent.isClass) {
                AddPeople();
            }
            else {
                AddEventPeople();
            }
        },
    });
});

function CalendarData(startvalue, endvalue, CallBack) {
    $.ajax({
        cache: false,
        type: "GET",
        url: GettingCalendarDataUrl,
        dataType: "json",
        data: {
            start: $.datepicker.formatDate('dd M, yy', new Date(startvalue)),
            end: $.datepicker.formatDate('dd M, yy', new Date(endvalue)),
            branchId: $("#ddlBranch").val()
        },
        success: function (data) {
            var events = [];
            if (data != null) {
                $.each(data, function (i, item) {
                    events.push({
                        id: item.Id,
                        originalid: item.ActualId,
                        title: item.Title,
                        isClass: item.isClass,
                        staffid: item.StaffId,
                        start: new Date(parseInt(item.StartDate.replace(/\D/g, ""))),
                        end: new Date(parseInt(item.EndDate.replace(/\D/g, ""))),
                        color: item.Color,
                        allDay: item.isAllDay,
                        reservationlimint: item.ReservationLimint,
                        attendencelimit: item.AttendenceLimit
                    });
                });
            }
            CallBack(events);
        },
        error: function (errMsg) {
            HideLoader();
        },
        failure: function (data) {
            HideLoader();
        }
    });
}

function AddingClassTime() {
    ShowLoader();
    $.ajax({
        type: "GET",
        url: AddClassTimeUrl,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#myModalBody').html(data);
            $('#myModal').modal('show');
        },
        error: function () {
            HideLoader();
            alert("Content load failed.");
        }
    });
}

function AddPeople() {
    ShowLoader();
    $.ajax({
        type: "GET",
        url: AddPeopleUrl,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#myModalBody').html(data);
            $('#myModal').modal('show');
        },
        error: function () {
            HideLoader();
            alert("Content load failed.");
        }
    });
}

function AddEventPeople() {
    ShowLoader();
    $.ajax({
        type: "GET",
        url: AddEventPeopleUrl,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#myModalBody').html(data);
            $('#myModal').modal('show');
        },
        error: function () {
            HideLoader();
            alert("Content load failed.");
        }
    });
}

//Add Class Timing View related
function AddClassTimeLoading() {
    $('input[type="checkbox"].flat-red, input[type="radio"].flat-red').iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue',
    });

    $("#c_endtime").datetimepicker({
        format: 'HH:mm',
        useCurrent: true,
        stepping: 15
    });

    $("#c_endtime").data("DateTimePicker").date(formatAMPMUTC(new Date(_date.getTime() + 60 * 60000)));

    $('#c_start_time').html(moment(_date).format("ddd, MMM DD") + "@" + formatAMPMUTC(_date));

    BindingClasses();
}

function BindingClasses() {
    $.ajax({
        cache: false,
        type: "GET",
        url: GettingClassesUrl,
        dataType: "json",
        data: { branchId: $("#ddlBranch").val() },
        success: function (data) {
            if (data != null) {
                var config = [];
                $.each(data, function (i, item) {
                    var rows = option.replace(/{{value}}/g, item.ClassId)
                        .replace(/{{text}}/g, item.ClassName)
                    $('#classes').append(rows);
                });
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
    return false;
}

function SaveClassTiming() {
    ShowLoader();
    if (ValidateClassTiming()) {
        var ClassTiming = {
            TimingId: 0,
            IsRepeats: $("#recurringyes").is(":checked") ? true : false,
            Every: 1,
            StartDate: moment(_date).format("MMM DD, YYYY"),
            EndDate: moment(_date).format("MMM DD, YYYY"),
            StartTime: formatAMPMUTC(_date),
            EndTime: $("#c_endtime").val(),
            Pattern: formatAMPMUTC(_date).split(":")[1] + " " + formatAMPMUTC(_date).split(":")[0] + " * * " + new Date(_date).getDay(),
            Duration: ((parseTime($("#c_endtime").val()) - parseTime(formatAMPMUTC(_date))) / (1000 * 60)),
            BranchId: $("#ddlBranch").val(),
            ClassId: $("#classes").val(),
            DayNames: moment(_date).format("ddd"),
            DayVals: new Date(_date).getDay(),
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
                    $("#myModal").modal("hide");
                    toastr.success("Class Timing Saved Successfully.");
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
}

function ValidateClassTiming() {
    var IsValid = true;
    ClearTimingErrorrMsg();

    if ($("#classes").val() == "0" || $("#classes").val() == "") {
        $('#classes').parent().append("<span class='help-block help-block-error'>Please select class</span>");
        $('#classes').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#c_end_time").val() == "") {
        $('#c_end_time').parent().append("<span class='help-block help-block-error'>Please select end tim</span>");
        $('#c_end_time').parent().addClass("has-error");
        IsValid = false;
    }

    return IsValid;
}

function ClearTimingErrorrMsg() {
    $('#classes').parent().find(".help-block-error").remove();
    $('#classes').parent().removeClass("has-error");
    $('#c_end_time').parent().find(".help-block-error").remove();
    $('#c_end_time').parent().removeClass("has-error");
}

//Add People Related
function AddPeopleLoading() {
    ShowLoader();
    r_membercount = 0;
    CalendarId = 0;
    _calEvent.start = _calEvent.start._d;
    if (!_calEvent.allDay) {
        _calEvent.end = _calEvent.end._d;
        $(".modal-title").html("<strong>" + _calEvent.title + " - " + moment(_calEvent.start).format("dddd") + ", " + moment(_calEvent.start).format("HH:mm") + " - " + moment(_calEvent.end).format("HH:mm") + "</strong>");
    }
    else {
        $(".modal-title").html("<strong>" + _calEvent.title + " - " + moment(_calEvent.start).format("dddd") + "</strong>");
    }
    GettingCalenderInstructor(true);
    GetMember();

}

function GetMember() {
    $("#reserve_member").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: GetMembersForAutoCompleteUrl,
                data: { prefix: request.term, branchId: $("#ddlBranch").val() },
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
            add_member(e, i, 1);
        },
        close: function (el) {
            el.target.value = '';
        },
        appendTo: "#myModal",
        minLength: 2
    });

    $("#wait_member").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: GetMembersForAutoCompleteUrl,
                data: { prefix: request.term, branchId: $("#ddlBranch").val() },
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
            add_member(e, i, 0);
        },
        close: function (el) {
            el.target.value = '';
        },
        appendTo: "#myModal",
        minLength: 2
    });

    $("#attend_member").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: GetMembersForAutoCompleteUrl,
                data: { prefix: request.term, branchId: $("#ddlBranch").val() },
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
            add_member(e, i, 2);
        },
        close: function (el) {
            el.target.value = '';
        },
        appendTo: "#myModal",
        minLength: 2
    });

    $("#instructor").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: GetStaffForAutoCompleteUrl,
                data: { prefix: request.term, branchId: $("#ddlBranch").val() },
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
            UpdatingCalendarInstructor(0, "", "", "");
        },
        appendTo: "#myModal",
        minLength: 2
    });
}

function add_member(e, i, type) {
    SavingCalendarMember(type, i.item.val, i.item.label);
}

function updateULSort(ul_id) {
    var mylist = $('#' + ul_id);
    var listitems = mylist.children('li').get();

    listitems.sort(function (a, b) {
        var compA = $(a).text().toUpperCase();
        var compB = $(b).text().toUpperCase();
        return (compA < compB) ? -1 : (compA > compB) ? 1 : 0;
    });

    $.each(listitems, function (idx, itm) { mylist.append(itm); });
}

function move_all_attended() {
    $("#reservelist li").each(function () {
        var id = $(this).attr('id').replace(/^m_/, '');
        var name = $(this).text();
        SavingCalendarMember(2, id, name);
    });
    return false;
}

function GettingCalenderInstructor(IsClass) {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: GettingCalendarInstructorUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { timingId: _calEvent.originalid, CalendarDate: moment(_calEvent.start).format("MM DD, YYYY"), StaffId: _calEvent.staffid, isClass: IsClass },
        success: function (data) {
            if (data != null) {
                CalendarId = data.CalendarId;
                $('#HFStaffId').val(data.StaffId);
                $('#instructor').val(data.FirstName + " " + data.LastName);
                GettingCalenderMembers(IsClass);
            }
            else {
                CalendarId = 0;
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

function GettingCalenderMembers(IsClass) {
    $.ajax({
        cache: false,
        type: "GET",
        url: GettingCalendarMembersUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { CalendarId: CalendarId },
        success: function (data) {
            if (data != null) {
                $.each(data, function (i, item) {
                    if (item.Type == 0) {
                        $('#waitlist').append('<li id="m_' + item.MemberId + '">' + item.FirstName + " " + item.LastName + '<span class="editatt" style="display: none;cursor:pointer;"><a onclick="return deletemember(' + item.CMemberId + ',this);"><i class="glyphicon glyphicon-trash"></i></a></span></li>');
                    }
                    else if (item.Type == 1) {
                        r_membercount = r_membercount + 1;
                        $('#reservelist').append('<li id="m_' + item.MemberId + '">' + item.FirstName + " " + item.LastName + '<span class="editatt" style="display: none;cursor:pointer;"><a onclick="return deletemember(' + item.CMemberId + ',this);"><i class="glyphicon glyphicon-trash"></i></a></span></li>');
                    }
                    else if (item.Type == 2) {
                        $('#signinlist').append('<li id="m_' + item.MemberId + '">' + item.FirstName + " " + item.LastName + '<span class="editatt" style="display: none;cursor:pointer;"><a onclick="return deletemember(' + item.CMemberId + ',this);"><i class="glyphicon glyphicon-trash"></i></a></span></li>');
                    }
                    else if (item.Type == 3) {
                        if (!item.isAttended) {
                            $('#e_reservelist').append('<li id="m_' + item.MemberId + '" cid="' + item.CMemberId + '">' + item.FirstName + " " + item.LastName + '<span class="copyatt" style="cursor:pointer;"> <a onclick="return eventdetails_attend_member(' + item.CMemberId + ',' + item.MemberId + ');" title = "Mark attendance for user."> <i class="glyphicon glyphicon-share-alt"></i></a></span> <span class="editatt" style="display: none;cursor:pointer;"><a onclick="return deleteeventmember(' + item.CMemberId + ',' + item.MemberId + ',1);"><i class="glyphicon glyphicon-trash"></i></a></span></li>');
                        }
                        else {
                            if (item.isReserved) {
                                $('#e_reservelist').append('<li id="m_' + item.MemberId + '" cid="' + item.CMemberId + '">' + item.FirstName + " " + item.LastName + '<span class="copyatt" style="display: none;cursor:pointer;"> <a onclick="return eventdetails_attend_member(' + item.CMemberId + ',' + item.MemberId + ');" title = "Mark attendance for user."> <i class="glyphicon glyphicon-share-alt"></i></a></span> <span class="editatt" style="display: none;cursor:pointer;"><a onclick="return deleteeventmember(' + item.CMemberId + ',' + item.MemberId + ',1);"><i class="glyphicon glyphicon-trash"></i></a></span></li>');
                            }
                            $('#e_signinlist').append('<li id="m_' + item.MemberId + '" cid="' + item.CMemberId + '">' + item.FirstName + " " + item.LastName + '<span class="editatt" style="display: none;cursor:pointer;"><a onclick="return deleteeventmember(' + item.CMemberId + ',' + item.MemberId + ',2);"><i class="glyphicon glyphicon-trash"></i></a></span></li>');
                        }
                    }
                });
                if (IsClass) {
                    updateULSort('waitlist');
                    updateULSort('reservelist');
                    updateULSort('signinlist');
                    if (_calEvent.reservationlimint <= r_membercount) {
                        toastr.error("reservation limit exceeded");
                        $("#waitlistrow").show();
                    }
                    else {
                        $("#waitlistrow").hide();
                    }
                }
                else {
                    updateULSort('e_reservelist');
                    updateULSort('e_signinlist');
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

function UpdatingCalendarInstructor(type, memberid, memberName, from) {
    ShowLoader();
    var Instructor = {
        CalendarId: CalendarId,
        TimeId: _calEvent.originalid,
        StaffId: $('#HFStaffId').val(),
        CalendarDate: _calEvent.start,
        IsActive: true,
        IsClass: true
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: UpdatingCalendarInstructorUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ instructor: Instructor }),
        success: function (data) {
            if (data.Status == "success") {
                CalendarId = data.CalendarId
                if (from == "member") {
                    SavingCalendarMember(type, memberid, memberName);
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

function SavingCalendarMember(type, memberid, memberName) {
    ShowLoader();
    var isValid = true;
    if (type == 3) {
        if ($("#e_signinlist").find("#m_" + memberid).length > 0 || $("#e_reservelist").find("#m_" + memberid).length > 0) {
            isValid = false;
            toastr.error("member already registerd")
        }
    }
    if (CalendarId != 0 && isValid) {
        var member = {
            CMemberId: 0,
            CalendarId: CalendarId,
            MemberId: memberid,
            Type: type,
            isAttended: false,
            isReserved: true
        }
        $.ajax({
            cache: false,
            type: "POST",
            url: SvaingCalendarMemberUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ CMember: member }),
            success: function (data) {
                if (data.Status == "success") {
                    CMemberId = data.CMemberId
                    if (type == 0) {
                        $('#waitlist').append('<li id="m_' + memberid + '">' + memberName + '<span class="editatt" style="display: none;cursor:pointer;"><a onclick="return deletemember(' + data.CMemberId + ',this);"><i class="glyphicon glyphicon-trash"></i></a></span></li>');
                        updateULSort('waitlist');
                    }
                    else if (type == 1) {
                        r_membercount = r_membercount + 1;
                        if (_calEvent.reservationlimint <= r_membercount) {
                            toastr.error("reservation limit exceeded");
                            $("#waitlistrow").show();
                        }
                        else {
                            $("#waitlistrow").hide();
                        }
                        $('#reservelist').append('<li id="m_' + memberid + '">' + memberName + '<span class="editatt" style="display: none;cursor:pointer;"><a onclick="return deletemember(' + data.CMemberId + ',this);"><i class="glyphicon glyphicon-trash"></i></a></span></li>');
                        updateULSort('reservelist');
                    }
                    else if (type == 2) {
                        $('#signinlist').append('<li id="m_' + memberid + '">' + memberName + '<span class="editatt" style="display: none;cursor:pointer;"><a onclick="return deletemember(' + data.CMemberId + ',this);"><i class="glyphicon glyphicon-trash"></i></a></span></li>');
                        updateULSort('signinlist');
                    }
                    else if (type == 3) {
                        $('#e_reservelist').append('<li id="m_' + memberid + '" cid="' + data.CMemberId + '">' + memberName + '<span class="copyatt"> <a onclick="return eventdetails_attend_member(' + data.CMemberId + ',' + memberid + ');" title = "Mark attendance for user."> <i class="glyphicon glyphicon-share-alt"></i></a></span> <span class="editatt" style="display: none;cursor:pointer;"><a onclick="return deleteeventmember(' + data.CMemberId + ',' + memberid + ',1);"><i class="glyphicon glyphicon-trash"></i></a></span></li>');
                        updateULSort('e_reservelist');
                        editatt(0);
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
    else {
        if (CalendarId == 0) {
            UpdatingCalendarInstructor(type, memberid, memberName, "member");
        }
        else {
            HideLoader();
        }
    }
}

function deletemember(CMemberId, id) {
    ShowLoader();
    if (CMemberId != 0) {
        $.ajax({
            cache: false,
            type: "POST",
            url: DeleteMemberUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ CMemberId: CMemberId }),
            success: function (data) {
                if (data.Status == "success") {
                    $(id).parent().parent().remove();
                }
                else {
                    toastr.error("Unable to delete member");
                }
                HideLoader();
            },
            error: function (errMsg) {
                toastr.error("Unable to delete member");
                HideLoader();
            },
            failure: function (errMsg) {
                toastr.error("Unable to delete member");
                HideLoader();
            }
        });
    }

    return false;
}

function editatt(type) {
    if (type == 0) {
        $(".editattdone").show();
        $(".editatt").hide();
    }
    else {
        $(".editattdone").hide();
        $(".editatt").show();
    }
}

var deletionType = 1;


//AddEvent People Related
function AddEventPeopleLoading() {
    ShowLoader();
    CalendarId = 0;
    _calEvent.start = _calEvent.start._d;
    if (!_calEvent.allDay) {
        _calEvent.end = _calEvent.end._d;
    }
    $(".modal-title").html("<strong>" + _calEvent.title + "</strong>");

    GettingCalenderInstructor(false);
    GetEventmember();
}

function GetEventmember() {
    $("#r_event_member").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: GetMembersForAutoCompleteUrl,
                data: { prefix: request.term, branchId: $("#ddlBranch").val() },
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
            add_member(e, i, 3);
        },
        close: function (el) {
            el.target.value = '';
        },
        appendTo: "#myModal",
        minLength: 2
    });
}

function deleteeventmember(CMemberId, memberid, type) {
    ShowLoader();
    if (CMemberId != 0) {
        $.ajax({
            cache: false,
            type: "POST",
            url: DeleteEventMemberUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ CMemberId: CMemberId, Type: type }),
            success: function (data) {
                if (data.Status == "success") {
                    if (type == 1) {
                        $("#e_reservelist").find("#m_" + memberid).remove();
                    }
                    else {
                        $("#e_signinlist").find("#m_" + memberid).remove();
                        $("#e_reservelist").find("#m_" + memberid).find(".copyatt").show()
                    }
                }
                else {
                    toastr.error("Unable to delete member");
                }
                HideLoader();
            },
            error: function (errMsg) {
                toastr.error("Unable to delete member");
                HideLoader();
            },
            failure: function (errMsg) {
                toastr.error("Unable to delete member");
                HideLoader();
            }
        });
    }

    return false;
}

function eventdetails_attend_member(CMemberId, memberid) {
    ShowLoader();
    if (CMemberId != 0) {
        $.ajax({
            cache: false,
            type: "POST",
            url: MarkAsAttendedUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ CMemberId: CMemberId }),
            success: function (data) {
                if (data.Status == "success") {
                    var membername = $("#e_reservelist").find("#m_" + memberid).text().trim();
                    $('#e_signinlist').append('<li id="m_' + memberid + '">' + membername + '<span class="editatt" style="display: none;cursor:pointer;"><a onclick="return deleteeventmember(' + CMemberId + ',' + memberid + ',2);"><i class="glyphicon glyphicon-trash"></i></a></span></li>');
                    $("#e_reservelist").find("#m_" + memberid).find(".copyatt").hide();
                    updateULSort('e_signinlist');
                }
                else {
                    toastr.error("Unable to delete member");
                }
                HideLoader();
            },
            error: function (errMsg) {
                toastr.error("Unable to delete member");
                HideLoader();
            },
            failure: function (errMsg) {
                toastr.error("Unable to delete member");
                HideLoader();
            }
        });
    }

    return false;
}

function e_move_all_attended() {
    $("#e_reservelist li").each(function () {
        var id = $(this).attr('id').replace(/^m_/, '');
        var cid = $(this).attr('cid')
        if ($("#e_signinlist").find("#m_" + id).length == 0) {
            eventdetails_attend_member(cid, id);
        }
    });
    return false;
}

//Deleting Class Or Event
function DeleteClass() {
    deletionType = 1;
    $("#Conformationbody").html("Are you sure you want to delete this class?")
    $('#modal_Conformation').modal('show');
    $('#modal_Conformation').show();
    return false;
}

function DeleteEvents() {
    deletionType = 2;
    $("#Conformationbody").html("Are you sure you want to delete this event?")
    $('#modal_Conformation').modal('show');
    $('#modal_Conformation').show();
    return false;
}

function YesClicked() {
    if (deletionType == 1) {
        deleteClassTiming();
    }
    else if (deletionType == 2) {
        deleteEvent();
    }
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
        data: JSON.stringify({ TimingId: _calEvent.originalid }),
        success: function (data) {
            if (data.Status == "success") {
                toastr.success("Class deletion successful.");
                $("#myModal").modal("hide");
                $("#calendar").fullCalendar('refresh');
                $('#calendar').fullCalendar('refetchEvents');
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

function deleteEvent() {
    ShowLoader();
    $('#modal_Conformation').modal('hide');
    $.ajax({
        cache: false,
        type: "POST",
        url: DeleteEventUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ EventId: _calEvent.originalid }),
        success: function (data) {
            if (data.Status == "success") {
                toastr.success("Event deletion successful.");
                $("#myModal").modal("hide");
                $("#calendar").fullCalendar('refresh');
                $('#calendar').fullCalendar('refetchEvents');
            }
            else {
                HideLoader();
                toastr.error("Unable to delete event");
            }
        },
        error: function (errMsg) {
            HideLoader();
            toastr.error("Unable to delete event");
        },
        failure: function (errMsg) {
            HideLoader();
            toastr.error("Unable to delete event");
        }
    });
}

function CloseConformationModel() {
    $('#modal_Conformation').hide();
}

function HideModel() {
    $('#modal_Conformation').hide();
    $('#myModal').modal('hide');
    $('.modal-backdrop').hide();
}