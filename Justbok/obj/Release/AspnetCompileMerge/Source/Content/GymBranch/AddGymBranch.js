$(document).ready(function () {
    ShowLoader();
    GymList();
    BindBranchList();
    $('#btSaveBranch').click(function () { SaveBranch(); });

    $(document).ready(function () { }).on('click', '#btnYes', function () { DeleteConfirmation(); });
    HideLoader();

});

function GymList() {
    $.ajax({
        url: GetGymListUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $.each(data, function (i, item) {
                $("#ddlGymList").append($("<option></option>").val(item.Gymid).html(item.GymName));
            })
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function BindBranchList() {
    $.ajax({
        url: BranchesListUrl,
        data: "",
        type: "GET",
        dataType: "json",
        success: function (data) {
            paymentData = data;
            $('#tblBranches tbody').empty();
            $.each(data, function (i, item) {
                var rows = "<tr>"
                      + "<td style='display:none'>" + '<input type="hidden" name="branchId" value=' + item.BranchId + '>' + "</td>"
                        + "<td style='display:none'>" + '<input type="hidden" name="gymId" value=' + item.GymId + '>' + "</td>"
    + "<td>" + item.Branchcode + "</td>"
    + "<td>" + item.BranchName + "</td>"
    + "<td>" + item.PhoneNo + "</td>"
    + "<td>" + item.City + "</td>"
    + "<td>" + item.BranchState + "</td>"
    + "<td> <i class='fa fa-fw fa-edit btnEditBranch' onclick='return EditBranch(this)' data-toggle='modal' data-target='#modal-branch'></i></td>"
    + "</tr>";
                $('#tblBranches tbody').append(rows);
            });
            HideLoader();
        },
        error: function () {
            HideLoader();
            alert("Failed! Please try again.");
        }
    });

}

function EditBranch(id) {
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var branchid = $('#tblBranches tr').eq(rowIndex + 2).find('td').eq(0).find('input[type="hidden"]').val();
    $.ajax({
        cache: false,
        type: "GET",
        url: EditBranchUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { BranchId: branchid },
        success: function (data) {
            $.each(data, function (i, obj) {
                $('#txtBranchID').val(obj.BranchId);
                $('#txtGymId').val(obj.GymId);
                $('#txtBranchCode').val(obj.Branchcode);
                $('#txtBranchName ').val(obj.BranchName);
                $('#txtBranchAddress').val(obj.BranchAdress);
                $('#txtBranchPhoneNumber').val(obj.PhoneNo);
                $('#txtCity').val(obj.City);
                $('#txtState').val(obj.BranchState);
            });
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });
}

function SaveBranch() {
    var jsonObject = {
        Branchcode: $('#txtBranchCode').val(), BranchName: $('#txtBranchName').val(), BranchAdress: $('#txtBranchAddress').val(),
        PhoneNo: $('#txtBranchPhoneNumber').val(),
        City: $('#txtCity').val(), BranchState: $('#txtState').val(), GymId: $('#txtGymId').val(), BranchId: $('#txtBranchID').val()
    }
    $.ajax({
        cache: false,
        type: "POST",
        url: AddGymBranch,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            $('#modal-branch').modal('hide');
            BindBranchList();
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
}

function DeleteBranch(id) {
    var rowIndex = $(id).closest('td').parent()[0].sectionRowIndex;
    var branchid = $('#tblBranches tr').eq(rowIndex + 2).find('td').eq(0).find('input[type="hidden"]').val();
    $('#txtBranchId').val(branchid);
    $('#modal_Conformation').modal('show');
}

function DeleteConfirmation() {
    $('#modal_Conformation').modal('hide');
    var branchid = $('#txtBranchId').val();
    $('#txtBranchId').val("");
    $.ajax({
        cache: false,
        type: "GET",
        url: DeleteBranchUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: { BranchId: branchid },
        success: function (data) {
            BindBranchList();
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });
}