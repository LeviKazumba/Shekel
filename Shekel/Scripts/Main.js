//const { debug } = require("util");

function Login() {

    $("#LoginForm").validate({
        rules: {

        },
        messages: {
            Email: {
                required: "Please enter your email",
                email: "Please enter a valid email address"
            },
            Password: "Please enter your password",
        },
        errorElement: "em",
        errorPlacement: function (error, element) {

            error.addClass("invalid-feedback");

            if (element.prop("type") === "checkbox") {
                error.insertAfter(element.next("label"));
            } else {
                error.insertAfter(element);
            }
        },
        success: function (label) {
            label.addClass("valid").text("Looks good!");
        },

        highlight: function (element, errorClass, validClass) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).addClass("is-valid").removeClass("is-invalid");
        }

    });

    if ($("#LoginForm").valid()) {

        Notiflix.Loading.Pulse('Please wait...');

        var o = {};
        o.Email = $("#Email").val();
        o.Password = $("#Password").val();

        try {
            $.ajax({
                type: "POST",
                url: "../Api/Login",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(o),
                dataType: "json",
                success: function (response) {

                    Notiflix.Loading.Remove();

                    if (response.Status === "Success") {
                        window.location = "../Portal/Index";
                    }
                    else {
                        Notiflix.Report.Failure('Oops!', response.Data, 'Okay');
                    }
                },
                failure: function (e) {
                    Notiflix.Loading.Remove();
                }
            })
        } catch (ex) {
            Notiflix.Loading.Remove();
            Notiflix.Report.Failure('Oops!', ex.message, 'Okay');
        }
    }
    else {
        return false
    }

}

function SignUp() {

    $("#SignUpForm").validate({
        rules: {
            Telephone: {
                minlength: 7
            }
        },
        messages: {
            Email: {
                required: "Please enter your email",
                email: "Please enter a valid email address"
            },
            Password: "Please create a password",
            Name: "Please enter your name",
            Surname: "Please enter your surname",
            Telephone: {
                required: "Please enter your telephone number",
                minlength: "Please enter a valid telephone number",
            }
        },
        errorElement: "em",
        errorPlacement: function (error, element) {

            error.addClass("invalid-feedback");

            if (element.prop("type") === "checkbox") {
                error.insertAfter(element.next("label"));
            } else {
                error.insertAfter(element);
            }
        },
        success: function (label) {
            label.addClass("valid").text("Looks good!");
        },

        highlight: function (element, errorClass, validClass) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).addClass("is-valid").removeClass("is-invalid");
        }

    });

    if ($("#SignUpForm").valid()) {

        Notiflix.Loading.Pulse('Please wait...');

        var o = {};
        o.Name = $("#Name").val();
        o.Surname = $("#Surname").val();
        o.Email = $("#Email").val();
        o.Password = $("#Password").val();
        o.Telephone = $("#Telephone").val();
        o.Country = $("#Country").val();
        //o.ReferralCode = $("#ReferralCode").val();

        try {
            $.ajax({
                type: "POST",
                url: "../Api/SignUp",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(o),
                dataType: "json",
                success: function (response) {

                    Notiflix.Loading.Remove();

                    if (response.Status === "Success") {
                        Notiflix.Report.Success('Awesome!', response.Data, 'Sign In', function () {
                            window.location = "../Portal/Index";
                        });
                    }
                    else {
                        Notiflix.Report.Failure('Oops!', response.Data, 'Okay');
                    }
                },
                failure: function (e) {
                    Notiflix.Loading.Remove();
                }
            })
        } catch (ex) {
            Notiflix.Loading.Remove();
            Notiflix.Report.Failure('Oops!', ex.message, 'Okay');
        }
    }
    else {
        return false
    }

}

function Logout() {

    Notiflix.Loading.Pulse('Please wait...');

    try {

        $.ajax({
            type: "GET",
            url: "../Api/Logout",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {

                Notiflix.Loading.Remove();

                if (response.Status === "Success") {

                    window.location = "../Home/Index";

                } else {
                    Notiflix.Report.Failure('Error', response.Data, 'Okay');
                }

            },
            failure: function (e) {
                Notiflix.Loading.Remove();

            }
        })
    } catch (ex) {
        Notiflix.Loading.Remove();
        Notiflix.Report.Failure('Error', ex.message, 'Okay');
    }
}

function Countries() {

    try {
        $.ajax({
            type: "GET",
            url: "../Api/Countries",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.Status != "Failed") {
                    $.each(JSON.parse(response.Data), function (index, item) {
                        $("#Country").append($('<option>').text(item.Name).attr('value', item.PhoneCode));
                    });
                }
            },
            failure: function (e) {
                Notiflix.Loading.Remove();
            }
        })
    } catch (ex) {
    }
}

function EditProfile() {

    try {
        $("#Name").attr("readonly", false);
        $("#Surname").attr("readonly", false);
    } catch (ex) {
    }
}

function UpdateProfile() {

    $("#ProfileForm").validate({
        rules: {
        },
        messages: {
            Name: "Please enter your name",
            Surname: "Please enter your surname",
        },
        errorElement: "em",
        errorPlacement: function (error, element) {

            error.addClass("invalid-feedback");

            if (element.prop("type") === "checkbox") {
                error.insertAfter(element.next("label"));
            } else {
                error.insertAfter(element);
            }
        },
        success: function (label) {
            label.addClass("valid").text("Looks good!");
        },

        highlight: function (element, errorClass, validClass) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).addClass("is-valid").removeClass("is-invalid");
        }

    });

    if ($("#ProfileForm").valid()) {

        Notiflix.Loading.Pulse('Please wait...');

        var o = {};
        o.Name = $("#Name").val();
        o.Surname = $("#Surname").val();

        try {
            $.ajax({
                type: "POST",
                url: "../Api/UpdateProfile",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(o),
                dataType: "json",
                success: function (response) {

                    Notiflix.Loading.Remove();

                    if (response.Status === "Success") {
                        Notiflix.Report.Success('Awesome!', response.Data, 'Okay', function () {
                            location.reload();
                        });
                    }
                    else {
                        Notiflix.Report.Failure('Oops!', response.Data, 'Okay');
                    }
                },
                failure: function (e) {
                    Notiflix.Loading.Remove();
                }
            })
        } catch (ex) {
            Notiflix.Loading.Remove();
            Notiflix.Report.Failure('Oops!', ex.message, 'Okay');
        }
    }
    else {
        return false
    }

}

function UpdatePassword() {

    $("#AccountSettingsForm").validate({
        rules: {
        },
        messages: {
            OPassword: "Please enter your current password",
            NPassword: "Please enter a new password",
        },
        errorElement: "em",
        errorPlacement: function (error, element) {

            error.addClass("invalid-feedback");

            if (element.prop("type") === "checkbox") {
                error.insertAfter(element.next("label"));
            } else {
                error.insertAfter(element);
            }
        },
        success: function (label) {
            label.addClass("valid").text("Looks good!");
        },

        highlight: function (element, errorClass, validClass) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).addClass("is-valid").removeClass("is-invalid");
        }

    });

    if ($("#AccountSettingsForm").valid()) {

        Notiflix.Loading.Pulse('Please wait...');

        var o = {};
        o.OPassword = $("#OPassword").val();
        o.NPassword = $("#NPassword").val();

        try {
            $.ajax({
                type: "POST",
                url: "../Api/UpdatePassword",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(o),
                dataType: "json",
                success: function (response) {

                    Notiflix.Loading.Remove();

                    if (response.Status === "Success") {
                        Notiflix.Report.Success('Awesome!', response.Data, 'Okay', function () {
                            window.location = "../Portal/Index";
                        });
                    }
                    else {
                        Notiflix.Report.Failure('Oops!', response.Data, 'Okay');
                    }
                },
                failure: function (e) {
                    Notiflix.Loading.Remove();
                }
            })
        } catch (ex) {
            Notiflix.Loading.Remove();
            Notiflix.Report.Failure('Oops!', ex.message, 'Okay');
        }
    }
    else {
        return false
    }

}

function KYC() {

    $("#KYCForm").validate({
        rules: {
        },
        messages: {
            DocumentNumber: "Please enter your document number",
        },
        errorElement: "em",
        errorPlacement: function (error, element) {

            error.addClass("invalid-feedback");

            if (element.prop("type") === "checkbox") {
                error.insertAfter(element.next("label"));
            } else {
                error.insertAfter(element);
            }
        },
        success: function (label) {
            label.addClass("valid").text("Looks good!");
        },

        highlight: function (element, errorClass, validClass) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).addClass("is-valid").removeClass("is-invalid");
        }

    });

    if ($("#KYCForm").valid()) {
        debugger;
        var Selfie = $("#Selfie").get(0).files;
        var Document = $("#Document").get(0).files;
        if (!Document.length > 0) {
            Notiflix.Report.Failure('Oops!', 'Please uplaod a copy of your indentity document', 'Okay');
            return;
        }
        if (!Selfie.length > 0) {
            Notiflix.Report.Failure('Oops!', 'Please upload a picture of yourself', 'Okay');
            return;
        }
        Notiflix.Loading.Pulse('Please wait...');

        try {
            var o = new FormData();
            o.append('Document', Document[0]);
            o.append('Selfie', Selfie[0]);
            o.append('KYCOTP', $('#KYCOTP').text());
            o.append('DocumentNumber', $('#DocumentNumber').val());
            o.append('DocumentType', $('#DocumentType').val());

            $.ajax({
                url: "../Api/KYC",
                data: o,
                processData: false,
                contentType: false,
                type: 'POST',
                success: function (response) {

                    Notiflix.Loading.Remove();

                    if (response.Status === "Success") {
                        Notiflix.Report.Success('Awesome!', response.Data, 'Okay', function () {
                            window.location = "../Portal/Index";
                        });
                    }
                    else {
                        Notiflix.Report.Failure('Oops!', response.Data, 'Okay');
                    } alert(data);
                }
            });
        } catch (ex) {
            Notiflix.Loading.Remove();
            Notiflix.Report.Failure('Oops!', ex.message, 'Okay');
        }
    }
    else {
        return false
    }

}

function GetKYC() {

    try {

        Notiflix.Loading.Pulse('Please wait...');

        $.ajax({
            type: "GET",
            url: "../Api/GetKYC",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {

                if (response.Status != "Failed") {

                    $('#ProcessKYCTable').DataTable({
                        "bDestroy": true,
                        "scrollX": true,
                        "scrollY": false,
                        "buttons": ['copy', 'csv', 'excel', 'pdf', 'print'],
                        "order": [[7, "desc"]],
                        "lengthMenu": [5, 10],
                        "columnDefs": [
                            {
                                targets: 0, render: function (data) {
                                    return moment(data).format('DD MMM YYYY H:mm');
                                }
                            }
                        ],
                        "aoColumns": [
                            {
                                "mData": "DateCreated"
                            },
                            {
                                "mData": "Name"
                            },
                            {
                                "mData": "Surname"
                            },
                            {
                                "mData": "Email"
                            },
                            {
                                "mData": "DocumentType"
                            },
                            {
                                "mData": "DocumentNumber"
                            },

                            {
                                "mData": "KYCOTP"
                            },
                            {
                                "mData": "ID",
                                "render": function (data, type, row) {
                                    return '<button onclick="KYCDocs(' + data + ')" class = " btn btn-success btn-sm rounded shadow">View</button>';
                                }
                            },
                        ]
                    });

                    $('#ProcessKYCTable').DataTable().clear().rows.add(JSON.parse(response.Data)).draw();

                    Notiflix.Loading.Remove();

                }
                else {
                    Notiflix.Report.Failure('Oops!', response.Data, 'Okay');
                }
            },
            failure: function (e) {
                Notiflix.Loading.Remove();
            }
        })
    } catch (ex) {
    }
}

function KYCDocs(ID) {


    Notiflix.Loading.Pulse('Please wait...');

    var o = {};
    o.ID = ID;

    try {
        $.ajax({
            type: "POST",
            url: "../Api/KYCDocs",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(o),
            dataType: "json",
            success: function (response) {

                Notiflix.Loading.Remove();

                if (response.Status === "Success") {
                    $("#Selfie").attr("src", response.Selfie);
                    $("#IDDoc").attr("src", response.Doc);
                    $("#KYCID").val(ID);

                    $("#KYCDocsModal").modal();
                }
                else {
                    Notiflix.Report.Failure('Oops!', response.Data, 'Okay');
                }
            },
            failure: function (e) {
                Notiflix.Loading.Remove();
            }
        })
    } catch (ex) {
        Notiflix.Loading.Remove();
        Notiflix.Report.Failure('Oops!', ex.message, 'Okay');
    }
}

function ProcessKYC(KYCStatus) {


    Notiflix.Loading.Pulse('Please wait...');

    var o = {};
    o.ID = $("#KYCID").val();
    o.KYCStatus = KYCStatus;

    try {
        $.ajax({
            type: "POST",
            url: "../Api/ProcessKYC",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(o),
            dataType: "json",
            success: function (response) {

                Notiflix.Loading.Remove();

                if (response.Status === "Success") {
                    $("#KYCDocsModal").modal('hide');
                    Notiflix.Report.Success('Awesome!', response.Data, 'Okay', function () {
                        window.location = "../Portal/ProcessKYC";
                    });
                }
                else {
                    Notiflix.Report.Failure('Oops!', response.Data, 'Okay');
                }
            },
            failure: function (e) {
                Notiflix.Loading.Remove();
            }
        })
    } catch (ex) {
        Notiflix.Loading.Remove();
        Notiflix.Report.Failure('Oops!', ex.message, 'Okay');
    }
}

function ForgotPassword() {

    $("#ForgotPasswordForm").validate({
        rules: {

        },
        messages: {
            Email: {
                required: "Please enter your email",
                email: "Please enter a valid email address"
            }
        },
        errorElement: "em",
        errorPlacement: function (error, element) {

            error.addClass("invalid-feedback");

            if (element.prop("type") === "checkbox") {
                error.insertAfter(element.next("label"));
            } else {
                error.insertAfter(element);
            }
        },
        success: function (label) {
            label.addClass("valid").text("Looks good!");
        },

        highlight: function (element, errorClass, validClass) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).addClass("is-valid").removeClass("is-invalid");
        }

    });

    if ($("#ForgotPasswordForm").valid()) {

        Notiflix.Loading.Pulse('Please wait...');

        var o = {};
        o.Email = $("#Email").val();

        try {
            $.ajax({
                type: "POST",
                url: "../Api/ForgotPassword",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(o),
                dataType: "json",
                success: function (response) {

                    Notiflix.Loading.Remove();

                    if (response.Status === "Success") {
                        Notiflix.Report.Success('Awesome!', response.Data, 'Okay', function () {
                            window.location = "../Home/Index";
                        });
                    }
                    else {
                        Notiflix.Report.Failure('Oops!', response.Data, 'Okay');
                    }
                },
                failure: function (e) {
                    Notiflix.Loading.Remove();
                }
            })
        } catch (ex) {
            Notiflix.Loading.Remove();
            Notiflix.Report.Failure('Oops!', ex.message, 'Okay');
        }
    }
    else {
        return false
    }


}

function Users() {
    $("#SearchUserForm").validate({
        rules: {
        },
        messages: {
            Term: "Please enter a search term",
        },
        errorElement: "em",
        errorPlacement: function (error, element) {

            error.addClass("invalid-feedback");
            if (element.prop("type") === "checkbox") {
                error.insertAfter(element.next("label"));
            } else {
                error.insertAfter(element);
            }
        },
        success: function (label) {
            label.addClass("valid").text("Looks good!");
        },

        highlight: function (element, errorClass, validClass) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).addClass("is-valid").removeClass("is-invalid");
        }

    });

    if ($("#SearchUserForm").valid()) {
        var o = {};
        o.Term = $("#Term").val();
        o.Filter = $("#Filter").val();

        Notiflix.Loading.Pulse('Please wait...');
        try {
            $.ajax({
                type: "POST",
                url: "../Api/Users",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(o),
                dataType: "json",
                success: function (response) {

                    if (response.Status != "Failed") {

                        $('#UsersTable').DataTable({
                            "bDestroy": true,
                            "scrollX": true,
                            "scrollY": false,
                            "lengthMenu": [5, 10],
                            "order": [[3, "desc"]],
                            "aoColumns": [
                                {
                                    "mData": "DisplayName"
                                },
                                {
                                    "mData": "Email"
                                },
                                {
                                    "mData": "Telephone"
                                },
                                {
                                    "mData": "ID",
                                    "render": function (data, type, row) {
                                        return '<button onclick="User(' + data + ')" class = " btn btn-success btn-sm rounded shadow">Select</button>';
                                    }
                                },
                            ]
                        });
                     
                        $('#UsersTable').DataTable().clear().rows.add(JSON.parse(response.Data)).draw();
                      

                        Notiflix.Loading.Remove();

                    }
                    else {
                        Notiflix.Report.Failure('Oops!', response.Data, 'Okay');
                    }
                },
                failure: function (e) {
                    Notiflix.Loading.Remove();
                }
            })
        } catch (ex) {
        }
    }
    else {
        return false
    }
}

function User(ID) {

    Notiflix.Loading.Pulse('Please wait...');

    var o = {};
    o.ID = ID;

    try {
        $.ajax({
            type: "POST",
            url: "../Api/User",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(o),
            dataType: "json",
            success: function (response) {

                Notiflix.Loading.Remove();

                if (response.Status === "Success") {

                    var d = JSON.parse(response.Data);

                    $(".UsersTable").hide();
                    $(".SearchUserForm").hide();
                  
                    $("#Name").val(d.Name);
                    $("#Surname").val(d.Surname);
                    $("#Email").val(d.Email);
                    $("#Telephone").val(d.Telephone);
                    $("#DocumentType").val(d.DocumentType);
                    $("#DocumentNumber").val(d.DocumentNumber);
                    $("#LastUpdated").val(d.DateModified);
                    $("#UpdatedBy").val(d.ModifiedBy);
                    $("#UserID").val(d.UserID);
                    $("#Balance").val('R ' + d.Balance);

                    $("#ReloadName").text(d.Name + ' ' + d.Surname);
                    $("#CurrentBalance").text('R ' + d.Balance);
                    $("#ReloadName").val(d.Name + ' ' + d.Surname);

                    $(".UserDetails").show();
                }
                else {
                    Notiflix.Report.Failure('Oops!', response.Data, 'Okay');
                }
            },
            failure: function (e) {
                Notiflix.Loading.Remove();
            }
        })
    } catch (ex) {
        Notiflix.Loading.Remove();
        Notiflix.Report.Failure('Oops!', ex.message, 'Okay');
    }
}

function AccountReload() {

    $("#AccountReloadForm").validate({
        rules: {

        },
        messages: {
            Amount: "Please enter amount to reload",
        },
        errorElement: "em",
        errorPlacement: function (error, element) {

            error.addClass("invalid-feedback");

            if (element.prop("type") === "checkbox") {
                error.insertAfter(element.next("label"));
            } else {
                error.insertAfter(element);
            }
        },
        success: function (label) {
            label.addClass("valid").text("Looks good!");
        },

        highlight: function (element, errorClass, validClass) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).addClass("is-valid").removeClass("is-invalid");
        }

    });

    if ($("#AccountReloadForm").valid()) {

        Notiflix.Loading.Pulse('Please wait...');

        var o = {};
        o.Amount = parseFloat($("#Amount").val());
        o.UserID = $("#UserID").val();

        try {
            $.ajax({
                type: "POST",
                url: "../Api/AccountReload",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(o),
                dataType: "json",
                success: function (response) {

                    Notiflix.Loading.Remove();

                    if (response.Status === "Success") {
                        Notiflix.Report.Success('Awesome!', response.Data, 'Okay', function () {
                            window.location = "../Portal/Users";
                        });
                    }
                    else {
                        Notiflix.Report.Failure('Oops!', response.Data, 'Okay');
                    }
                },
                failure: function (e) {
                    Notiflix.Loading.Remove();
                }
            })
        } catch (ex) {
            Notiflix.Loading.Remove();
            Notiflix.Report.Failure('Oops!', ex.message, 'Okay');
        }
    }
    else {
        return false
    }


}
