var resetPassword = {
    initialize: function () {
        resetPassword.initializeValidation();
        resetPassword.attachEvents();
    },

    initializeValidation:function(){
        $('#resetPasswordForm').validate({
            rules: {

                Password: { required: true, rangelength: [8, 14], ContainsAtLeastOneDigit: true, ContainsAtLeastOneCapitalLetter: true },
                ConfirmPassword: { required: true, equalTo: "#Password" }
            },
            messages: {

                Password: {
                    required: "Please enter password"

                },

            },
            errorContainer: $('#errorContainer'),
            errorLabelContainer: $('#errorContainer ul'),
            wrapper: 'li'
        });
    },

    attachEvents: function () {
        $("body").off("click", '.rest_psw')
        $("body").on('click', '.rest_psw', function () {
            $("#resetPasswordForm").validate();
            if ($("#resetPasswordForm").valid()) {
                $("#resetPasswordForm").submit();
            }
        });
    }
};