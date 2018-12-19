var pageurl = "", pageTitle = "";

var enroll = false;

$(document).ready(function () {
    HideSideBar();
    LoadSideBar();
    GetBranchId();
    $("#ddlBranch").change(function () { GetBranchId(); });

    //Navigation bar search text box Auto Complete
    var availableAttributes = [];
    availableAttributes = GetAutoCompleteOptions();

        $('#txtNavSearch').autocomplete({
            source: function () {availableAttributes }
            
        });
});

function HideSideBar()
{
    ShowLoader();
    $('#gym').hide();
    $('#dashboard').hide();
    $('#quickprint').hide();
    $('#attendance').hide();
    $('#enquiry').hide();
    $('#membership').hide();
    $('#setting').hide();
    $('#memberlist').hide();
    $('#reports').hide();
    $('#inventory').hide();
    $('#sms').hide();
    $('#workout').hide();
    $('#diet').hide();
    $('#expense').hide();
    $('#pos').hide();
    $('#scheduler').hide();
    HideLoader();
}

function LoadSideBar()
{
    ShowLoader();
    $.ajax({
        url: LoadSidebarUrl,
        data: "",
    type: "GET",
    dataType: "json",
    success: function (data) {
        $.each(data, function (i, item) {
            if (item.ServiceName == "Membership") {
                $('#membership').show();
                $('#memberlist').show();
            }
            if (item.ServiceName == "Enquiry") {
                $('#enquiry').show();
            }
            if (item.ServiceName == "Setting") {
                $('#setting').show();
            }
            if (item.ServiceName == "Expense") {
                $('#expense').show();
            }
            if (item.ServiceName == "Attendance") {
                $('#attendance').show();
            }
            if (item.ServiceName == "Reports") {
                $('#reports').show();
            }
            if (item.ServiceName == "Inventory") {
                $('#inventory').show();
            }
            if (item.ServiceName == "SMS") {
                $('#sms').show();
            }
            if (item.ServiceName == "Workout") {
                $('#workout').show();
            }
            if (item.ServiceName == "Diet") {
                $('#diet').show();
            }
            if (item.ServiceName == "Gym") {
                $('#gym').show();
            }
            if (item.ServiceName == "POS") {
                $('#pos').show();
            }
            if (item.ServiceName == "DashBoard") {
                $('#dashboard').show();
            }
            if (item.ServiceName == "QuickPrint") {
                $('#quickprint').show();
            }
            if (item.ServiceName == "Calender") {
                $('#scheduler').show();
            }
            if (item.ServiceName == "MemberInfo") {
                $('#memberlist').show();
            }
            HideLoader();
        
        });
    },
    failure: function(errMsg) {
        HideLoader();
        alert(errMsg.responseText);
    }
});


}

function ConvertDate(convertdate) {
   
    var date = convertdate;
    var parsedDate = new Date(parseInt(date.substr(6)));
    var newDate = new Date(parsedDate);

    //var getMonth = newDate.getMonth();
    var getDay = newDate.getDay();
    var getYear = newDate.getYear();
    var getMonth = "";
    var twoDigitDate = newDate.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
    var getMonth = "";
    var mm =(newDate.getMonth() + 1).toString();
    if (mm.length == 1) {
        getMonth = '0' + (newDate.getMonth() + 1);
    }
    else {
        getMonth = (newDate.getMonth() + 1)
    }

    //var getMonth = ((newDate.getMonth()+1).length === 1) ? '0' + (newDate.getMonth() + 1) : (newDate.getMonth() + 1);

    var startdate = twoDigitDate + '/' + getMonth + '/' + newDate.getFullYear();

    return startdate;
}

//$('#btnSignOut').click(function () {
//   // alert("hi");
//  //  window.location.href = '/Home/Index/';
//    $.ajax({
//        url: LogoutUrl,
//        data: "",
//    type: "GET",
//    dataType: "json",
//    success: function (data) {
//    },
//    failure: function (errMsg) {
//        alert(errMsg.responseText);
//    }
//});
//});

function LogOut()
{
    $.ajax({
                url: LogoutUrl,
                data: "",
            type: "GET",
            dataType: "json",
            success: function (data) {
            },
            failure: function (errMsg) {
                alert(errMsg.responseText);
            }
        });
}

function GetBranchId() {
    ShowLoader();
    $.ajax({
        url: BranchIdUrl,
        data: { BranchId: $('#ddlBranch option:selected').val() },
        type: "GET",
        dataType: "json",
        success: function (data) {
            HideLoader();
        },
        failure: function (errMsg) {
            HideLoader();
            alert(errMsg.responseText);
        }
    });


}

function GetAutoCompleteOptions() {
    var availableAttributes = [];
    $.ajax({
        cache: false,
        type: "POST",
        contentType: "application/json",
        url: NavSearchAutoCompleteUrl,
        dataType: "json",
        data: "",
        success: function (data) {
            for (var i in data)
            {
                            availableAttributes.push(data[i].FirstName);
            }
           
        },
        error: function (result) {
            console.log(JSON.stringify(result));
        }
    });
    

    return availableAttributes;
}
