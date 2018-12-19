
    $(document).ready(function () {
        $('input[type=datetime]').datepicker({
            dateFormat: "dd/m/yy",
            changeMonth: true,
            changeYear: true,
            yearRange: "-60:+100"
        });

        $('#ddlStaffBranch').multiselect({

            enableFiltering: true,
            includeSelectAllOption: true,
            enableCaseInsensitiveFiltering: true,
            maxHeight: 400,
            buttonWidth: '310px',
            dropUp: true

        });

        $('#ddlShiftType').multiselect({

            enableFiltering: true,
            includeSelectAllOption: true,
            enableCaseInsensitiveFiltering: true,
            maxHeight: 400,
            buttonWidth: '310px',
            dropUp: true

        });

        $('#ddlRole').change(function () {var opt = $(this).val();
            if (opt == 'Admin') { $('#btnCreateMember').show();}
            else if (opt == 'Manager') { $('#btnCreateMember').show(); }
            else if (opt == "Trainer") { $('#TrainerForm').show();  }
            else { $('#btnCreateMember').hide(); $('#userform').hide(); $('#TrainerForm').hide(); }
        });

        $(function () {$('#btnCreateMember').click(function () {$('#userform').show();});});

        GetBranches();
        GetShiftList();

        $('#btnSaveStaff').click(function () { if (ValidateStaff()) { SaveStaff(); } });
        $("#Imagefile").change(function () { readURL(this); });
        
        //$('.img1 img').attr('src');
        HideLoader();
    });

    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#uploadedimage').attr('src', e.target.result);

            }
            reader.readAsDataURL(input.files[0]);
        }
    }

   
    function GetBranches()
    {
        $.ajax({
            cache: false,
            type: "GET",
            url: GetBranchListUrl,
            dataType: "json",
        contentType: "application/json; charset=utf-8",
        data:"",
        success: function (data) {
            var options = [];
            $.each(data, function (i, item) {
                options.push({ label: item.BranchName, title: item.BranchName, value: item.BranchId });
            })
            $('#ddlStaffBranch').multiselect('dataprovider', options);
        },
        error: function () {
            alert("Failed! Please try again.");
        }
    });

    }

    function SaveStaff()
    {
        ShowLoader();
        var multipleBranch = $('#ddlStaffBranch').val();
        var multipleShift = $('#ddlShiftType').val();
        var jsonObject = {
            FirstName: $('#txtFirstName').val(), LastName: $('#txtLastName').val(), DOB: $('#txtDOB').val(), PhoneNumber: $('#txtPhoneNo').val(),
            Email: $('#txtEmail').val(), StaffAddress: $('#txtAddress').val(), StaffRole: $('#ddlRole').val(),
            MultiselectBranch: multipleBranch.toString(), MultiselectShiftType: multipleShift.toString(), DailySalary: $('#txtDailySalary').val(),
            Isactive: $('#chkIsActive').val(), UserName: $('#txtUname').val(), Password: $('#txtpwd').val(), IsLoginactive: $('#chkIsLoginActive').val(),
            Experience: $('#txtExperience').val(), Qulifiaction: $('#txtQualification').val(), TrainerDescription: $('#txtDescription').val()
        }
        $.ajax({
            cache: false,
            type: "POST",
            url: AddStaffUrl,
            dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            UploadTrainerImage();
            HideLoader();
            toastr.success("Data Saved Successfully.");
           // alert("Data Saved Successfully");
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        }
    });
    }

    function UploadTrainerImage()
    {
        var formData = new FormData();
        var totalFiles = document.getElementById("Imagefile").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("Imagefile").files[i];
            formData.append("imageUploadForm", file);
        }
        $.ajax({
            type: "POST",
            url: UpdateUploadImage,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                //if (response == "success") {
                //    alert('Image saved successfully!!');
                //}
                //else {
                //    alert("Error:Image not saved");
                //}

            },
            error: function (error) {
                alert("error");
            }
        });
    }

    function GetShiftList()
    {
        $.getJSON(GetShiftListUrl, function (data) {
            var options = [];
            $.each(data, function (i, item) {
                options.push({ label: item.ShiftName, title: item.ShiftName, value: item.ShiftId });
            })
            $('#ddlShiftType').multiselect('dataprovider', options);
        })

    }

    var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';
    function ValidateStaff() {
        var IsValid = true;
        var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
        $('#txtFirstName').parent().removeClass("has-error");
        $('#txtFirstName').parent().find(".help-block-error").remove();
        $('#txtLastName').parent().removeClass("has-error");
        $('#txtLastName').parent().find(".help-block-error").remove();
        $('#ddlRole').parent().removeClass("has-error");
        $('#ddlRole').parent().find(".help-block-error").remove();
        $('#ddlStaffBranch').parent().removeClass("has-error");
        $('#ddlStaffBranch').parent().find(".help-block-error").remove();
        $('#ddlShiftType').parent().removeClass("has-error");
        $('#ddlShiftType').parent().find(".help-block-error").remove();
        $('#txtEmail').parent().removeClass("has-error");
        $('#txtEmail').parent().find(".help-block-error").remove();

        if ($('#txtFirstName').val() == "") {

            $('#txtFirstName').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter FirstName"));
            $('#txtFirstName').parent().addClass("has-error");
            IsValid = false;
        }
        else {
            $('#txtFirstName').parent().removeClass("has-error");
            $('#txtFirstName').parent().find(".help-block-error").remove();
        }
        if ($('#txtLastName').val() == "") {

            $('#txtLastName').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter LastName"));
            $('#txtLastName').parent().addClass("has-error");
            IsValid = false;
        }
        else {
            $('#txtLastName').parent().removeClass("has-error");
            $('#txtLastName').parent().find(".help-block-error").remove();
        }

        if ($('#ddlRole  option:selected').text() == "- Please select -") {

            $('#ddlRole').parent().append(errorSpan.replace(/{{Message}}/g, "Please select Role"));
            $('#ddlRole').parent().addClass("has-error");
            IsValid = false;
        }
        else {
            $('#ddlRole').parent().removeClass("has-error");
            $('#ddlRole').parent().find(".help-block-error").remove();
        }

        if ($('#ddlStaffBranch').val() ==null) {

            $('#ddlStaffBranch').parent().append(errorSpan.replace(/{{Message}}/g, "Please Select Branch"));
            $('#ddlStaffBranch').parent().addClass("has-error");
            IsValid = false;
        }
        else {
            $('#ddlStaffBranch').parent().removeClass("has-error");
            $('#ddlStaffBranch').parent().find(".help-block-error").remove();
        }
        if ($('#ddlShiftType').val() == null) {

            $('#ddlShiftType').parent().append(errorSpan.replace(/{{Message}}/g, "Please Select Shift"));
            $('#ddlShiftType').parent().addClass("has-error");
            IsValid = false;
        }
        else {
            $('#ddlShiftType').parent().removeClass("has-error");
            $('#ddlShiftType').parent().find(".help-block-error").remove();
        }
        if ($('#txtEmail').val() != "") {
            if (filter.test($('#txtEmail').val())) {

            }
            else {
                $('#txtEmail').parent().append(errorSpan.replace(/{{Message}}/g, "Not a valid format"));
                $('#txtEmail').parent().addClass("has-error");
                IsValid = false;
            }
        }
        return IsValid;
    }

    function AllowOnlyText(elementid) {
        $('#' + elementid).keydown(function (e) {
            if (e.shiftKey || e.ctrlKey || e.altKey) {
                e.preventDefault();
            } else {
                var key = e.keyCode;
                if (!((key == 8) || (key == 9) || (key == 32) || (key == 46) || (key >= 35 && key <= 40) || (key >= 65 && key <= 90))) {
                    e.preventDefault();
                }
            }
        });
    }

    function AllowOnlyNumbers(elementid) {
        $("#" + elementid).keydown(function (event) {
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
    }

    AllowOnlyText("txtFirstName");
    AllowOnlyText("txtLastName");

    AllowOnlyNumbers("txtPhoneNo");
    AllowOnlyNumbers("txtDailySalary");

    