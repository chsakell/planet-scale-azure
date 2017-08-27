; var viewCommunity = {
    initialize: function () {
        viewCommunity.attachevents();
    },

    attachevents: function () {

        $("body").off("change", '#filter')
        $("body").on('change', '#filter', function (e) {
            e.preventDefault();
            var baseUrl = app.getBaseURL();
            var id = $("#communityId").val();
            var filterId = $('#filter').val();
            $('#disc-response').html("");
            $('#status_msg').show().addClass('alert-success').html("Loading...").append(app.getLoadingBar(true));
            //$('#disc-response').append(app.getLoadingBar(true));
            //window.location.href = baseUrl + "/Community/View/" + id + "/" + filterId + "/0";
            ajaxRequest.makeRequest("/Community/ViewComunityResponse", "GET", {
                "id": id,
                "filterId":filterId,
                "pageId": 0
            }, function (data) {
                $('#status_msg').hide().removeClass('alert-success');
                $('#status_msg').html("");
                $('#disc-response').html("");
                $('#disc-response').html(data);
                imagePreview.attachImagePreviewToImages();
            }, null, 'html');
        });

        $("body").off("click", '.pageID')
        $("body").on('click', '.pageID', function (e) {
            e.preventDefault();
            var baseUrl = app.getBaseURL();
            var id = $("#communityId").val();
            var filterId = $('#filter').val();
            var pageid = $(this).attr("pageId");
            $('#disc-response').html("");
            $('#status_msg').show().addClass('alert-success').html("Loading...").append(app.getLoadingBar(true));
            //$('#disc-response').append(app.getLoadingBar(true));
            //window.location.href = baseUrl + "/Community/View/" + id + "/" + filterId + "/" + pageid;
            ajaxRequest.makeRequest("/Community/ViewComunityResponse", "GET", {
                "id": id,
                "filterId": filterId,
                "pageId": pageid
            }, function (data) {
                $('#status_msg').hide().removeClass('alert-success');
                $('#status_msg').html("");
                $('#disc-response').html("");
                $('#disc-response').html(data);
                imagePreview.attachImagePreviewToImages();

            }, null, 'html');
        });

        $("#chooseMediaFile").unbind('click');
        $("#chooseMediaFile").click(function () {
            $("#mediaFile").trigger("click");
        });

        $('#discussion-response-form-upload').validate({
            rules: {
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

        var options = {
            //target: '#dicussion-reply',   // target element(s) to be updated with server response
            // pre-submit callback
            timeout: 0,
            beforeSubmit: function (formData, jqForm, options) {
                $('#dicussion-reply').hide();
                $('#status-msg').show().addClass('alert-success').html("Uploading...").append(app.getLoadingBar(true));
            },
            // post-submit callback
            success: function (responseText, statusText, xhr, $form) {
                //$('#resp-content').append(responseText);
                $('#status-msg').html('');
                $('#status-msg').hide();
                $('#dicussion-reply').show();
                $("#uploadFileInfo").html('');
                $("#media-description-form-group").hide();
                $('#disc-response').html("");

                var baseUrl = app.getBaseURL();
                var id = $("#communityId").val();
                var filterId = $('#filter').val();
                var pageid = $(this).attr("pageId");
                $('#disc-response').html("");
                $('#status_msg').show().addClass('alert-success').html("Loading...").append(app.getLoadingBar(true));
                //$('#disc-response').append(app.getLoadingBar(true));
                ajaxRequest.makeRequest("/Community/ViewComunityResponse", "GET", {
                    "id": id,
                    "filterId": filterId,
                    "pageId": pageid
                }, function (data) {
                    $('#status_msg').hide().removeClass('alert-success');
                    $('#status_msg').html("");
                    $('#disc-response').html("");                    
                    $('#disc-response').html(data);
                    imagePreview.attachImagePreviewToImages();
                }, null, 'html');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                // probably because of a timeout (502.3 Bad Gateway)
                document.location = app.getBaseURL() + '/Community';
            },
            clearForm: true,        // clear all form fields after successful submit
            resetForm: true        // reset the form after successful submit
        };
        // bind 'myForm' and provide a simple callback function
        $('#discussion-response-form-upload').ajaxForm(options);

        $("#mediaFile").change(function () {
            //console.log('Click mediaFile');
            var fileval = $(this).val();
            $("#uploadFileInfo").html(fileval);

            if ($.trim(fileval) !== '') { $("#media-description-form-group").show(); } else { $("#media-description-form-group").hide(); }
        });
    }

};