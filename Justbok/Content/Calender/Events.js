var pagerLoaded = false, pageNo = 1, sortBy = "", sortDirection = "", HeaderId = "", eventId = 0, createdOn = "";

$(document).ready(function () {
    ShowLoader();
    $('#searchFromDate, #searchToDate').datepicker({
        dateFormat: 'M d, yy',
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });

    var today_date = new Date()
    today_date.setDate(today_date.getDate() + 7)
    $('#searchFromDate').datepicker('setDate', new Date());
    $('#searchToDate').datepicker('setDate', today_date);


    BindingEvents();
});

function BindingEvents() {
    pageNo = 1;
    sortBy = "name";
    sortDirection = "asc";
    HeaderId = "name";
    pagerLoaded = false;

    if ($('.pagination').data("twbs-pagination")) {
        $('.pagination').twbsPagination('destroy');
    }

    if ($("#tblEvents_length").val() != null && $("#tblEvents_length").val() != "") {
        GettingEvents(pageNo, $("#tblEvents_length").val())
    }
    else {
        GettingEvents(pageNo, 10)
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

        if ($("#tblEvents_length").val() != null && $("#tblEvents_length").val() != "") {
            GettingEvents(pageNo, $("#tblEvents_length").val())
        }
        else {
            GettingEvents(pageNo, 10)
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

    if ($("#tblEvents_length").val() != null && $("#tblEvents_length").val() != "") {
        GettingEvents(pageNo, $("#tblEvents_length").val())
    }
    else {
        GettingEvents(pageNo, 10)
    }
    return false;
}

function GettingEvents(pageno, pagesize) {
    $.ajax({
        cache: false,
        type: "GET",
        url: EventsUrl,
        dataType: "json",
        data: { page: pageno, pagesize: pagesize, eventname: $('#searchName').val(), description: $('#searchDescription').val(), fromDate: $('#searchFromDate').val(), toDate: $('#searchToDate').val(), branchId: $('#ddlBranch').val(), sortBy: sortBy, sortDirection: sortDirection },
        success: function (data) {
            $('#tblEvents tbody').find("tr").remove();
            if (data != null && data.Pages > 0) {
                $.each(data.Result, function (i, item) {
                    var imageurl = item.PhotoUrl == null ? "" : item.PhotoUrl;
                    var rows = '<tr role="row" class="odd">'
                        + '<td>' + moment(new Date(parseInt(item.StartDate.replace(/\D/g, "")))).format("MMM DD, YYYY HH:mm") + '</td>'
                        + '<td>' + moment(new Date(parseInt(item.EndDate.replace(/\D/g, "")))).format("MMM DD, YYYY HH:mm") + '</td>'
                        + '<td>' + item.Name + '</td>'
                        + '<td>' + item.Description + '</td>'
                        + '<td>' + item.Price + '</td>'
                        + '<td> <img class="product-table-image" src="' + imageurl + '"></td>'
                        + '<td>'
                        + '<button class="btn btn-success btn-xs btn-flat btnEdit" id="278" style="margin-right:10px;" title="Edit" onclick="return EventsView(' + item.EventId + ')">'
                        + '<span class="glyphicon glyphicon-edit"></span> Edit'
                        + '</button>'
                        + '<button class="btn btn-danger btn-xs btn-flat btnDelete" id="278" title="Delete" onclick="return Remove(' + item.EventId + ')">'
                        + '<span class="glyphicon glyphicon-remove"></span> Delete'
                        + '</button>'
                        + '</td>'
                        + '</tr >';
                    $('#tblEvents tbody').append(rows);
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
                                if ($("#tblEvents_length").val() != null && $("#tblEvents_length").val() != "") {
                                    GettingEvents(pageNo, $("#tblEvents_length").val())
                                }
                                else {
                                    GettingEvents(pageNo, 10)
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
                $('#tblEvents tbody').append(norecords);
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

function EventsView(id) {
    ShowLoader();
    eventId = id;
    $.ajax({
        type: "GET",
        url: EventViewUrl,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#modelBody').html(data);
            $('#e_id').val(id)
            $('#eventView').modal('show');
        },
        error: function () {
            HideLoader();
            alert("Content load failed.");
        }
    });
}

function HideModel() {
    $('#submodel').modal('hide');
    $('#eventView').modal('hide');
}

//Event View related script
function EventViewLoading() {
    $("#e_limit").keydown(function (event) {
        allownumbers(event);
    });

    $("#e_cost").keydown(function (event) {
        allownumeric(event);
    });

    $('#e_all_day').on('ifChanged', function (e) {
        $(this).trigger("onchange", e);
    });

    $('input[type="checkbox"].flat-red, input[type="radio"].flat-red').iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue',
    });

    $('#e_end_date, #e_start_date').datepicker({
        dateFormat: 'M d, yy',
        changeMonth: true,
        changeYear: true,
        yearRange: "-60:+100"
    });

    $("#e_start_time, #e_end_time").datetimepicker({
        format: 'HH:mm',
        useCurrent: true,
        stepping: 15
    });

    if (eventId != 0) {
        EventDetailsById();
    }
    else {
        $("#e_start_time").data("DateTimePicker").date("09:00");
        $("#e_end_time").data("DateTimePicker").date("17:00");
        $('#e_end_date, #e_start_date').datepicker('setDate', new Date());
        HideLoader();
    }

    $("#e_start_date").change(function () {
        $('#e_end_date').val($("#e_start_date").val());
    });
}

function allday() {
    if ($("#e_all_day").is(':checked')) {
        $("#end_time_row, #start_time_row").fadeOut();
    } else {
        $("#end_time_row, #start_time_row").fadeIn();
    }
}

function EventDetailsById() {
    ShowLoader();
    $.ajax({
        cache: false,
        type: "GET",
        url: EventDetailsByIdUrl,
        dataType: "json",
        data: { EventId: eventId },
        success: function (data) {
            if (data != null && data.Status == "success") {
                $("#e_name").val(data.Data.Name);
                $("#e_description").val(data.Data.Description);
                $('#e_start_date').datepicker('setDate', new Date(parseInt(data.Data.StartDate.replace(/\D/g, ""))));
                $('#e_end_date').datepicker('setDate', new Date(parseInt(data.Data.EndDate.replace(/\D/g, ""))));


                if (data.Data.IsAllDay) {
                    $("#e_start_time").data("DateTimePicker").date(new Date(parseInt(data.Data.StartDate.replace(/\D/g, ""))));
                    $("#e_end_time").data("DateTimePicker").date(new Date(parseInt(data.Data.EndDate.replace(/\D/g, ""))));
                    $("#e_all_day").iCheck("check");
                }
                else {
                    $("#e_start_time").data("DateTimePicker").date("09:00");
                    $("#e_end_time").data("DateTimePicker").date("17:00");
                    $("#e_all_day").iCheck("uncheck");
                }

                if (data.Data.IncludedTax) {
                    $("#e_includes_tax").iCheck("check");
                }
                else {
                    $("#e_includes_tax").iCheck("uncheck");
                }
                $("#e_cost").val(data.Data.Price)
                $("#e_limit").val(data.Data.RegistrationLimit)

                if (data.Data.PhotoUrl != null && data.Data.PhotoUrl != "") {
                    var html = "<img class='img-responsive' src='" + data.Data.PhotoUrl + "' />";
                    $('#photoplace').html(html);
                    $("#e_photo").val(data.Data.PhotoUrl);
                }
                else {
                    var html = "<img class='img-responsive' src='' />";
                    $('#photoplace').html(html);
                    $("#e_photo").val("");
                }

                createdOn = new Date(parseInt(data.Data.CreatedOn.replace(/\D/g, "")));
            }
            else {
                toastr.error("Unable to get event information.")
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

function SaveEvent() {
    ShowLoader();
    if (ValidateEvent()) {
        var Event = {
            EventId: $("#e_id").val(),
            Name: $("#e_name").val(),
            Description: $("#e_description").val(),
            StartDate: !$("#e_all_day").is(":checked") ? new Date($("#e_start_date").val() + " " + $("#e_start_time").val()) : $("#e_start_date").val(),
            EndDate: !$("#e_all_day").is(":checked") ? new Date($("#e_end_date").val() + " " + $("#e_end_time").val()) : $("#e_end_date").val(),
            IsAllDay: $("#e_all_day").is(":checked") ? 1 : 0,
            IncludedTax: $("#e_includes_tax").is(":checked") ? 1 : 0,
            Price: $("#e_cost").val(),
            RegistrationLimit: $("#e_limit").val(),
            IsActive: true,
            BranchId: $("#ddlBranch").val(),
            PhotoUrl: $("#e_photo").val(),
            CreatedOn: createdOn
        }
        $.ajax({
            cache: false,
            type: "POST",
            url: SaveEventUrl,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ objEvent: Event }),
            success: function (data) {
                if (data.Status == "success") {
                    HideModel();
                    toastr.success("Events information saved successfully.");
                    BindingEvents();
                }
                else {
                    HideLoader();
                    toastr.error("Unable to save information.");
                }
            },
            error: function (errMsg) {
                HideLoader();
                toastr.error("Unable to save information.");
            },
            failure: function (errMsg) {
                HideLoader();
                toastr.error("Unable to save information.");
            }
        });
    }
    else {
        HideLoader();
    }
}

function ValidateEvent() {
    var IsValid = true;
    ClearingErrorMsgs();
    if ($("#e_name").val() == "") {
        $('#e_name').parent().append("<span class='help-block help-block-error'>Please enter name</span>");
        $('#e_name').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#e_description").val() == "") {
        $('#e_description').parent().append("<span class='help-block help-block-error'>Please enter description</span>");
        $('#e_description').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#e_start_date").val() == "") {
        $('#e_start_date').parent().append("<span class='help-block help-block-error'>Please select start date</span>");
        $('#e_start_date').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#e_end_date").val() == "") {
        $('#e_end_date').parent().append("<span class='help-block help-block-error'>Please select end date</span>");
        $('#e_end_date').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#e_start_time").val() == "") {
        $('#e_start_time').parent().append("<span class='help-block help-block-error'>Please select start time</span>");
        $('#e_start_time').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#e_end_time").val() == "") {
        $('#e_end_time').parent().append("<span class='help-block help-block-error'>Please select end time</span>");
        $('#e_end_time').parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#e_cost").val() == "") {
        $('#e_cost').parent().parent().append("<span class='help-block help-block-error'>Please enter price</span>");
        $('#e_cost').parent().parent().addClass("has-error");
        IsValid = false;
    }

    if ($("#e_limit").val() == "") {
        $('#e_limit').parent().append("<span class='help-block help-block-error'>Please enter registration limit</span>");
        $('#e_limit').parent().addClass("has-error");
        IsValid = false;
    }

    return IsValid;
}

function ClearingErrorMsgs() {
    $('#e_limit').parent().find(".help-block-error").remove();
    $('#e_limit').parent().removeClass("has-error");
    $('#e_name').parent().find(".help-block-error").remove();
    $('#e_name').parent().removeClass("has-error");
    $('#e_description').parent().find(".help-block-error").remove();
    $('#e_description').parent().removeClass("has-error");
    $('#e_start_date').parent().find(".help-block-error").remove();
    $('#e_start_date').parent().removeClass("has-error");
    $('#e_end_date').parent().find(".help-block-error").remove();
    $('#e_end_date').parent().removeClass("has-error");
    $('#e_start_time').parent().find(".help-block-error").remove();
    $('#e_start_time').parent().removeClass("has-error");
    $('#e_end_time').parent().find(".help-block-error").remove();
    $('#e_end_time').parent().removeClass("has-error");
    $('#e_cost').parent().parent().find(".help-block-error").remove();
    $('#e_cost').parent().parent().removeClass("has-error");
}

//Deleting Class
function Remove(id) {
    if (id != 0) {
        eventId = id;
        $('#modal_Conformation').modal('show');
    }
    return false;
}

function OnYesClick() {
    deleteEvent();
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
        data: JSON.stringify({ EventId: eventId }),
        success: function (data) {
            if (data.Status == "success") {
                toastr.success("Event deletion successful.");
                pagerLoaded = false;
                BindingEvents();
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

//Searching And Reseting
function Search() {
    ShowLoader();
    BindingEvents();
}

function Reset() {
    ShowLoader();
    $('#searchName').val("");
    $('#searchDescription').val("");
    var today_date = new Date()
    today_date.setDate(today_date.getDate() + 7)
    $('#searchFromDate').datepicker('setDate', new Date());
    $('#searchToDate').datepicker('setDate', today_date);
    BindingEvents();
}


//Event Logo
function ShowEventLogo() {
    ShowLoader();
    $.ajax({
        type: "GET",
        url: EventLogoUrl,
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            $('#subModelBody').html(data);
            $('#submodel').modal('show');
            $('#submodel').show();
            HideLoader();
        },
        error: function () {
            HideLoader();
            alert("Content load failed.");
        }
    });
}

function UploadImage() {

    $('#fileupload').fileupload({
        url: UploadEventImageUrl,
        dataType: 'json',
        autoUpload: true,
        cache: false,
        formData: {
            filetype: 'event'
        },
        start: function (e, data) {
            ShowLoader();
        },
        done: function (e, data) {
            debugger;
            if (data.jqXHR.responseJSON) {
                if (data.formData.filetype == 'event') {
                    var fileurl = "" + data.jqXHR.responseJSON + "";
                    var html = "<img class='img-responsive' src='" + fileurl + "' />";
                    $('#photoplace').html(html);
                    $("#e_photo").val(fileurl); //This will have issues...
                    $('#submodel').hide(); // This is a secondary modal
                }
            } else {
                toastr.error(data.jqXHR.responseJSON[0].error);
            }
            HideLoader()
        },
        fail: function (e, data) {
            HideLoader()
            toastr.error("An unknown failure has occurred");
        },
        progressall: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#progress .progress-bar').css(
                'width',
                progress + '%'
            );
        }
    }).prop('disabled', !$.support.fileInput)
        .parent().addClass($.support.fileInput ? undefined : 'disabled');

}

function HideSubModel() {
    $('#submodel').hide();
}

function RemovePhoto() {
    var html = "<img class='img-responsive' src='' />";
    $('#photoplace').html(html);
    $("#e_photo").val("");
}