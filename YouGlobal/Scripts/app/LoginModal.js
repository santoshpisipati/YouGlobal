$(document).ready(function () {
    var loginLink = $("a[id='loginLink']");
    loginLink.attr({ "href": "#", "data-toggle": "modal", "data-target": "#ModalLogin" });

    $("#loginForm").submit(function (event) {
        if ($("#loginForm").valid()) {
            var username = document.getElementById('LoginModel_UserName').value;
            var password = document.getElementById('LoginModel_LoginPassword').value;
            var rememberme = document.getElementById('LoginModel_RememberMe').checked;
            var IsJobSeeker = document.getElementById('IsSeek').checked;
            var IsEmployer = document.getElementById('IsEmp').checked;
            var IsConsultant = document.getElementById('IsCon').checked;
            var antiForgeryToken = Sample.Web.ModalLogin.Views.Common.getAntiForgeryValue();

            Sample.Web.ModalLogin.Identity.LoginIntoStd(username, password, rememberme, IsJobSeeker, IsEmployer, IsConsultant, antiForgeryToken, Sample.Web.ModalLogin.Views.LoginModal.loginSuccess, Sample.Web.ModalLogin.Views.LoginModal.loginFailure);
        }
        return false;
    });

    $("#ModalLogin").on("hidden.bs.modal", function (e) {
        Sample.Web.ModalLogin.Views.LoginModal.resetloginForm();
    });

    //TODO alle Referenzen auf Form controls bezogen auf form, um Doppeldeutigkeiten zu vermeiden.
    $("#ModalLogin").on("shown.bs.modal", function (e) {
        $("#Email").focus();
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

Sample.Web.ModalLogin.Views.LoginModal = {
    resetLoginForm: function () {
        $("#loginForm").get(0).reset();
        $("#alertBox").css("display", "none");
    },

    loginFailure: function (message) {
        var alertBox = $("#alertBox");
        alertBox.html(message);
        alertBox.css("display", "block");
    },

    loginSuccess: function () {
        window.location.href = window.location.href;
    }
}

Sample.Web.ModalLogin.Identity = {
    LoginIntoStd: function (username, password, rememberme, IsJobSeeker, IsEmployer, IsConsultant, antiForgeryToken, successCallback, failureCallback) {
        var data = { "__RequestVerificationToken": antiForgeryToken, "username": username, "password": password, "rememberme": rememberme, "IsJobSeeker": IsJobSeeker, "IsEmployer": IsEmployer, "IsConsultant": IsConsultant };
        $.ajax({
            url: "/Account/Login",
            type: "POST",
            data: data
        })
        .done(function (member) {
            if (member.MemberId > 0) {
                if (member.IsJobSeeker) {
                    window.location.href = "/Jobs/JobSeeker";
                    return false;
                }
                else {
                    window.location.href = "/Jobs/UnSignedEmployerRecruiter";
                    return false;
                }
            }
            else {
                failureCallback("Invalid login attempt.");
            }
        })
        .fail(function (jqxhr, textStatus, errorThrown) {
            failureCallback(errorThrown);
        });
    }
}