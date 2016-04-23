$(document).ready(function () {
    var registerLink = $("a[id='registerLink']");
    registerLink.attr({ "href": "#", "data-toggle": "modal", "data-target": "#ModalRegister" });

    $("#registerform").submit(function (event) {
        if ($("#registerform").valid()) {
            var form = $("#registerform").serialize();
            var FirstName = document.getElementById('RegisterModel_FirstName').value;
            var LastName = document.getElementById('RegisterModel_LastName').value;
            var Email = document.getElementById('RegisterModel_Email').value;
            var RegisterPassword = document.getElementById('RegisterModel_RegisterPassword').value;
            var ConfirmPassword = document.getElementById('RegisterModel_ConfirmPassword').value;
            var PhoneCode = document.getElementById('RegisterModel_PhoneCode').value;
            var AreaCode = document.getElementById('RegisterModel_AreaCode').value;
            var PhoneNumber = document.getElementById('RegisterModel_PhoneNumber').value;
            var RegisterAs = document.getElementById('RegisterModel_RegisterAs').value;
            var CaptchaValue = document.getElementById('g-recaptcha-response').value;
            //var CaptchaValue = document.getElementById('recaptcha_response_field').value;

            var antiForgeryToken = Sample.Web.ModalLogin.Views.Common.getAntiForgeryValue();
            registerAuser(form);
            //registerAuser(FirstName, LastName, Email, RegisterPassword, ConfirmPassword, PhoneCode, AreaCode, PhoneNumber, RegisterAs, CaptchaValue, antiForgeryToken, Sample.Web.ModalLogin.Views.RegisterModal.loginSuccess, Sample.Web.ModalLogin.Views.RegisterModal.loginFailure);
        }
        return false;
    });

    $("#ModalRegister").on("hidden.bs.modal", function (e) {
        Sample.Web.ModalLogin.Views.RegisterModal.resetRegisterForm();
    });

    //TODO alle Referenzen auf Form controls bezogen auf form, um Doppeldeutigkeiten zu vermeiden.
    $("#ModalRegister").on("shown.bs.modal", function (e) {
        $("#FirstName").focus();
    });
});

var Sample = Sample || {};
Sample.Web = Sample.Web || {};
Sample.Web.ModalLogin = Sample.Web.ModalLogin || {};
Sample.Web.ModalLogin.Views = Sample.Web.ModalLogin.Views || {}

Sample.Web.ModalLogin.Views.Common = {
    getAntiForgeryValue: function () {
        return $('input[name="__RequestVerificationToken"]').val();
    }
}

Sample.Web.ModalLogin.Views.RegisterModal = {
    resetRegisterForm: function () {
        $("#registerform").get(0).reset();
        $("#regAlertBox").css("display", "none");
    },

    loginFailure: function (message) {
        var regAlertBox = $("#regAlertBox");
        regAlertBox.html(message);
        regAlertBox.css("display", "block");
    },

    loginSuccess: function () {
        window.location.href = window.location.href;
    }
}

//function registerAuser(FirstName, LastName, Email, RegisterPassword, ConfirmPassword, PhoneCode, AreaCode, PhoneNumber, RegisterAs, CaptchaValue, antiForgeryToken, successCallback, failureCallback) {
//    var data = { "__RequestVerificationToken": antiForgeryToken, "FirstName": FirstName, "FirstName": FirstName, "Email": Email, "RegisterPassword": RegisterPassword, "ConfirmPassword": ConfirmPassword, "PhoneCode": PhoneCode, "AreaCode": AreaCode, "PhoneNumber": PhoneNumber, "RegisterAs": RegisterAs, "CaptchaValue": CaptchaValue };
function registerAuser(form) {
        $.ajax({
            url: "/Account/RegisterJs",
            type: "POST",
            data: form
        })
        .done(function (data) {
            if (data.Success === "True") {
                //alert("Registration Successful");
                Sample.Web.ModalLogin.Views.RegisterModal.loginSuccess();
            }
            else {
                Sample.Web.ModalLogin.Views.RegisterModal.loginFailure(data.Message);
            }
        })
        .fail(function (jqxhr, textStatus, errorThrown) {
            Sample.Web.ModalLogin.Views.RegisterModal.loginFailure(errorThrown);
        });
    }