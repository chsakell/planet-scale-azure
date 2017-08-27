var signupFormValidate = {
    
    signupFormValidate: function () {

        $('#signup-form').validate({
            rules: {
                fullname: { required: true, alphabet: true },
                avatar: { accept: "image/*" },
                city: { required: true, maxlength: 50, alphabet: true },
                state: { required: true, maxlength: 50, alphabet: true },
                twitter_handle: { maxlength: 100 },
                email: { required: true, email: true, maxlength: 200 },
                password: { required: true, rangelength: [8, 14], ContainsAtLeastOneDigit: true, ContainsAtLeastOneCapitalLetter: true },
                confirmPassword: { required: true, equalTo: "#signup_password" },
                //twitter_handle: {required: },
                termsService: "required",

            },
            highlight: function (element) {
                $(element).closest('.control-group').removeClass('success').addClass('error');
            },
            success: function (element) {
                //element.text('OK!').addClass('valid').closest('.control-group').removeClass('error').addClass('success');
                element.closest('label.error').remove();
            }

        })

    }
};
