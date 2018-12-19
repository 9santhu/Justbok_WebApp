
$(document).ready(function () {
    $('#btnGymBranch').click(function () { SaveBranch(); });
    HideLoader();
});

function AutoCompleteGymList()
{
    $.ajax(
{
    url: BindGymListUrl,
    data: { prefix: request.term },
    dataType: "json",
    type: "GET",
    contentType: "application/json; charset=utf-8",
    success: function (data) {
        response($.map(data, function (item, id) {
            return {

                label: item.GymName,
                val: item.GymId
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
}

function Validate() {
    var IsValid = true;
    if ($('#txtSelectGym').val() == "") {
        $('#txtSelectGym').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Expense Amount"));
        $('#txtSelectGym').parent().addClass("has-error");
        IsValid = false;
    }
    else {
        $('#txtSelectGym').parent().removeClass("has-error");
        $('#txtSelectGym').parent().find(".help-block-error").remove();
    }
}

function SaveBranch()
{
    if (Validate()) {
        var jsonObject = {
            Branchcode: $('#txtBranchCode').val(), BranchName: $('#txtBranchName').val(), BranchAdress: $('#txtBranchAddress').val(),
            PhoneNo: $('#txtBranchPhoneNumber').val(),
            City: $('#txtCity').val(), BranchState: $('#txtState').val(), GymId: $('#txtGymId').val()
        }
        $.ajax({
            cache: false,
            type: "POST",
            url: AddGymBranchUrl,
            dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            $('.nav-tabs a[href="#Membership"]').tab('show');
            toastr.success("Data Saved Successfully.");
          //  alert("Data Saved Successfully");
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}
}