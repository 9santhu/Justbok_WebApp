Date.prototype.monthNames = [
    "January", "February", "March",
    "April", "May", "June",
    "July", "August", "September",
    "October", "November", "December"
];

Date.prototype.weekNames = [
    "Sunday", "Monday", "Tuesday",
    "Wednesday", "Thursday", "Friday", "Saturday"
];

Date.prototype.getMonthName = function () {
    return this.monthNames[this.getMonth()];
};
Date.prototype.getShortMonthName = function () {
    return this.getMonthName().substr(0, 3);
};

Date.prototype.getWeekName = function () {
    return this.weekNames[this.getDay()];
};
Date.prototype.getShortMonthName = function () {
    return this.getWeekName().substr(0, 3);
};


function allownumbers (event) {
    if (event.shiftKey == true) {
        event.preventDefault();
    }
    if ((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105)
        || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 46) {

    } else {
        event.preventDefault();
    }
}

function allownumeric(event) {
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
}

function formatAMPMUTC(date) {
    date = new Date(date);
    var hours = date.getUTCHours();
    var minutes = date.getUTCMinutes();
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes;
    return strTime;
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

function GetDateFormat(date) {

    var dd = new Date(date);

    var d = dd.getDate();
    var m = dd.getMonth() + 1;
    var y = dd.getFullYear();

    if (m < 10) {
        m = '0' + m;
    }
    if (d < 10) {
        d = '0' + d;
    }
    return d + '/' + m + '/' + y;
}

function ValidateRequiredField(ctrl, ValidationMessage, ValidationPosition, defValue) {
    try {
        ClearMessage(ctrl);
        if ($(ctrl).val() == '' || $(ctrl).val() == defValue) {
            $(ctrl).closest('.form-group').addClass('has-error');
            PrintMessage(ctrl, ValidationMessage, ValidationPosition);
            return false;
        }

        return true;

    } catch (e) {
        //alert(e);
    }
    return true;
}

function ClearMessage(ctrl) {
    $(ctrl).closest('.form-group').removeClass('has-error');
    $(ctrl).next('.help-block').remove();
    $(ctrl).prev('.help-block').remove();
}

function PrintMessage(ctrl, Message, ValidationPosition) {
    if (ValidationPosition == "after") {
        $('<span class="help-block help-block-error">' + Message + '</span>').insertAfter($(ctrl));
    }
    else {
        $('<span class="help-block help-block-error">' + Message + '</span>').insertBefore($(ctrl));
    }
}