$(document).ready(function () {
    $('#btnSignIn').click(function () {
        
        ShowLoader();
        if (ValidateForm()) {

            
            CheckCredential();
        }
        else {
            HideLoader();
        }
    });
});


$(function () {
    $('input').iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue',
        increaseArea: '20%' // optional
    });
});
var errorSpan = '<span class="help-block help-block-error"> {{Message}}</span>';
function ValidateForm()
{
    
    var IsValid = true;
    $('#txtUserName').parent().removeClass("has-error");
    $('#txtUserName').parent().find(".help-block-error").remove();
    $('#txtPassword').parent().removeClass("has-error");
    $('#txtPassword').parent().find(".help-block-error").remove();
    $('#divError').hide();

        if ($('#txtUserName').val() == "") {
            $('#txtUserName').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter UserName"));
            $('#txtUserName').parent().addClass("has-error");
            IsValid = false;
        }
        else {
            $('#txtUserName').parent().removeClass("has-error");
            $('#txtUserName').parent().find(".help-block-error").remove();
        }
        if ($('#txtPassword').val() == "") {
            $('#txtPassword').parent().append(errorSpan.replace(/{{Message}}/g, "Please Enter Password"));
            $('#txtPassword').parent().addClass("has-error");
            IsValid = false;
        }
        else {
            $('#txtPassword').parent().removeClass("has-error");
            $('#txtPassword').parent().find(".help-block-error").remove();
        }


        return IsValid;

}

function CheckCredential()
{
    
    var URL = "";
    var jsonObject = {
        Uname: $('#txtUserName').val(), pwd: $('#txtPassword').val()
    }
    
    $.ajax({
        cache: false,
        type: "POST",
        url: LoginUrl,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(jsonObject),
        success: function (data) {
            if (data.Message == "Invalid") {
                $('#divError').show();
                //$('#txtPassword').parent().append(errorSpan.replace(/{{Message}}/g, "Invalid UserName or Password"));
                //$('#txtPassword').parent().addClass("has-error");
                HideLoader();
            }
            else {
                
                $('#divError').parent().removeClass("has-error");
                $('#txtPassword').parent().find(".help-block-error").remove();
                $('#divError').hide();
                URL = data.Message;
                redirecttoedit(URL);
            }
        },
        failure: function (errMsg) {
            alert(errMsg.responseText);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(JSON.stringify(jqXHR));
        }
    });
}

function redirecttoedit(url) {
    window.location.href = url;
}




