var posts = {
    initialize: function () {
        posts.attachEvents();
        // START FILE UPLOAD TO WAMS
        $('#new-discussion-form-upload').validate({
            rules: {
                title: { required: true, maxlength: 60 },
                content: { required: true },
                mediaFile: { accept: "image/*|video/*" },
                mediaDescription: {
                    required: function () {
                        var flag = ($.trim($("#mediaFile").val()) === '') ? false : true;
                        return flag;
                    }, maxlength: 140
                }
            },
            highlight: function (element) {
                $(element).closest('.control-group').removeClass('success').addClass('error');
            },
            success: function (element) {
                element.closest('label.error').remove();
            }
        });
    },

    attachEvents: function () {
        $("#chooseMediaFile").click(function () {
            $("#mediaFile").trigger("click")
        });

        $("#mediaFile").change(function () {
            //console.log('Click mediaFile');
            var fileval = $(this).val();
            $("#uploadFileInfo").html(fileval);

            if ($.trim(fileval) !== '') { $("#media-description-form-group").show(); } else { $("#media-description-form-group").hide(); }
        });

        var options = {
            //target: '#dicussion-reply',   // target element(s) to be updated with server response
            // pre-submit callback
            timeout: 0,
            beforeSubmit: function (formData, jqForm, options) {
                var isValid = $('#new-discussion-form-upload').valid();
                if (isValid) {
                    $('#status-msg').show().addClass('alert-success').html("Uploading...").append(app.getLoadingBar(true));
                    var $bar = $('#progress-bar');
                    $('#new-discussion-form-upload').hide();
                }
                return isValid;
            },
            // post-submit callback
            success: function (response, statusText, xhr, $form) {
                $('#progress').removeClass('active');
                if (!response.Status) {
                    $form.show();
                    $('#status-msg').addClass('alert-danger').html(response.message).css({ 'background-color': '#fff', 'padding': '10px;' });
                } else if (response.Status) {
                    document.location = app.getBaseURL() + '/Community';
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                // probably because of a timeout (502.3 Bad Gateway)
                document.location = app.getBaseURL() + '/Community';
            },
            clearForm: true,        // clear all form fields after successful submit
            resetForm: true        // reset the form after successful submit
        };
        // bind 'myForm' and provide a simple callback function
        $('#new-discussion-form-upload').ajaxForm(options);
    }
};