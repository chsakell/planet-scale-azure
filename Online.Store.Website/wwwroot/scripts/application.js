var forgotPassword = {
    initialize: function () {
         /**
     * Validate Forgot Password Form
     *///debugger;
        $('#forgotpassword-form').validate({
        rules: { forgotEmail: { required: true, email: true, maxlength: 200 } },
        highlight: function (element) {
            $(element).closest('.control-group').removeClass('success').addClass('error');
        },
        success: function (element) {
            //element.text('OK!').addClass('valid').closest('.control-group').removeClass('error').addClass('success');            
            element.closest('label.error').remove();
        }
    });
    }
}
var app = {

    initialize: function () {
        app.addValidationExtensions();
        if ($('#flashdata').length) {
            window.setTimeout(function () { $("#flashdata").slideUp('slow'); }, 3000);
        }

    },

    addValidationExtensions: function () {
        // Add custom validation rules
        jQuery.validator.addMethod('alphanumericspecial', function (value, element, param) {
            return this.optional(element) || value == value.match(/^[-a-zA-Z0-9_ ]+$/);
        }, 'Only letters, Numbers & Space/underscore Allowed.');

        jQuery.validator.addMethod('alphanumeric', function (value, element, param) {
            return this.optional(element) || value == value.match(/^[a-z0-9A-Z#]+$/);
        }, 'Only Characters, Numbers & Hash Allowed.');

        jQuery.validator.addMethod('alphabet', function (value, element, param) {
            return this.optional(element) || value == value.match(/^[a-zA-Z ]+$/);
        }, 'Only Characters Allowed.');

        jQuery.validator.addMethod('ContainsAtLeastOneDigit', function (value, element, param) {
            var patt = new RegExp("[0-9]");
            return patt.test(value);
        }, 'Your password must contain at least one digit.');

        jQuery.validator.addMethod('ContainsAtLeastOneCapitalLetter', function (value, element, param) {
            var patt = new RegExp("[A-Z]");
            return patt.test(value);
        }, 'Your password must contain at least one capital letter.');

        var validImageTypes = '.png, .gif, .JPEG, .jpg';
        var validVideoTypes = '.asf, .avi, .m2ts, .m2v, .mp4, .mpeg, .mpg, .mts, .ts, .wmv, .3gp, .3g2, .3gp2, .mod, .dv, .vob, .ismv, .m4a';
        var validMimeType = 'Choose a file with one of the following extensions ';

        // Accept a value from a file input based on a required mimetype
        jQuery.validator.addMethod("accept",
            function (value, element, param) {
                // Split mime on commas in case we have multiple types we can accept
                var typeParam = typeof param === "string" ? param.replace(/\s/g, '').replace(/,/g, '|') : "image/*",
                optionalValue = this.optional(element),
                i, file;
                // Element is optional
                if (optionalValue) {
                    return optionalValue;
                }

                if ($(element).attr("type") === "file") {
                    // If we are using a wildcard, make it regex friendly
                    typeParam = typeParam.replace(/\*/g, ".*");

                    // Check if the element has a FileList before checking each file
                    if (element.files && element.files.length) {
                        for (i = 0; i < element.files.length; i++) {
                            file = element.files[i];

                            // Grab the mimetype from the loaded file, verify it matches
                            if (!file.type.match(new RegExp(".?(" + typeParam + ")$", "i"))) {
                                return false;
                            }
                        }
                    }
                }

                // Either return true because we've validated each file, or because the
                // browser does not support element.files and the FileList feature
                return true;
            },
            function (value, element, param) {
                return (param === 'image/*') ? validMimeType + validImageTypes : validMimeType + validVideoTypes + ', ' + validImageTypes;
            }
        );
    },

    //Function used to get the base url
    getBaseURL: function () {
        // The domain path.
        var domainPath = config.contextPath;
        var pattern = '';
        var url = '';

        if (domainPath === "/") {
            // The regex to match and return the url for localhost.
            url = location.href;
            pattern = "https?:\/\/[a-zA-Z0-9.:]+";
        }
        else {
            // To get the root path till hostname.
            var rootPath = location.protocol + "//" + location.hostname + ":" + location.port;
            url = rootPath + domainPath.substring(0, domainPath.length - 1);

            // The pattern which matches the base url.
            // Will match http://example.com , http://example/domain , https://example/domain, http://example:9000/domain etc
            pattern = /(^|\s)((https?:\/\/)?[\w-]+(:\d+)?((?:\.|\/)[\w-]*)*\.?(:\d+)?(\/\S*)?)/gi;
        }

        var baseUrl = (url.match(pattern));

        // Returns the base url
        return baseUrl;
    },

    getLoadingBar: function (bar) {
        var base_url = app.getBaseURL();
        // AJAX loader image object
        if (bar)
            return $('<img/>').attr({ src: base_url + '/content/images/ajax-loader-bar.gif', id: 'ajax-loader-bar', style: 'margin-top:10px;margin-bottom:10px;' });
        return $('<img/>').attr({ src: base_url + '/content/images/ajax-loader.gif', id: 'ajax-loader' });

    },

    initializeLogin: function () {

        $('#login-form').validate({
            rules: {
                email: { required: true, email: true, maxlength: 200 },
                password: { required: true }
            },
            highlight: function (element) {
                $(element).closest('.control-group').removeClass('success').addClass('error');
                //document.getElementById(element.id).style.border = '1px solid red'; 
            },
            success: function (element) {
                //element.text('OK!').addClass('valid').closest('.control-group').removeClass('error').addClass('success');
                element.closest('label.error').remove();
            }
        });

        var options = {
            // pre-submit callback
            beforeSubmit: function (formData, jqForm, options) {
                $('#ajax-loader').remove(); // self remove
                var loginBtn = $('#login-form .btn'); // load login button
                $(loginBtn).hide(); // hide after user clik on login button
                $(loginBtn).after(app.getLoadingBar()); // load ajax loder image
            },
            // post-submit callback
            success: function (responseText, statusText, xhr, $form) {
                response = responseText;
                $('#ajax-loader').remove(); // slef remove
                var span = '';
                var loginBtn = $('#login-form .btn');
                if (!response.Status) {
                    span = $('<label>').html(response.Message).attr('class', 'error');
                    $(loginBtn).show(); // show login button
                    $(loginBtn).after(span); // append response after button
                } else if (response.Status) {
                    span = $('<label>').html('<br />Successfully login.').attr('style', 'color:green');
                    $(loginBtn).after(span); // append response after button
                    location.reload(); // Reload/Refresh same page
                }
            },
            clearForm: true,        // clear all form fields after successful submit
            resetForm: true         // reset the form after successful submit
        };

        $('#login-form').ajaxForm(options);
    },

    readURL: function (input, id) {
        if ($('#showavatar').length > 0) { $('#showavatar').remove(); }
        if (input.files && input.files[0]) {
            //console.log(input.files); 
            var reader = new FileReader();
            reader.readAsDataURL(input.files[0]);
            reader.onload = function (e) {
                //console.log(e);
                var dataURL = e.target.result;
                var mimeType = input.files[0].type;
                if (mimeType.split('/')[0] == 'image') {
                    $('#edit-profile-avatar').hide();
                    $('<img>').attr('title', input.files.name).attr('src', e.target.result).attr('id', 'showavatar').css('width', '64px').appendTo($('#' + id).parent());
                }
            };
        }
    },

}