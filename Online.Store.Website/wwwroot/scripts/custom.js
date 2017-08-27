var myFile = '';
var base_url = app.getBaseURL();
// AJAX loader image object
var ajaxLoaderImage = $('<img/>').attr({ src: base_url + '/assets/images/ajax-loader.gif', id: 'ajax-loader' });
var ajaxLoaderBarImage = $('<img/>').attr({ src: base_url + '/assets/images/ajax-loader-bar.gif', id: 'ajax-loader-bar', style:'margin-top:10px;margin-bottom:10px;' });

var maxBlockSize = 256 * 1024;//Each file will be split in 256 KB.
var numberOfBlocks = 1;
var selectedFile = null;
var fileType = '';
var isImage = '';
var haveMedia = false;
var currentFilePointer = 0;
var totalBytesRemaining = 0;
var blockIds = [];
var blockIdPrefix = "block-";
var submitUri = null;
var bytesUploaded = 0;
var reader = '';

var assetId = '';
var assetFileName = '';

if (!window.File && !window.FileReader && !window.FileList && !window.Blob) {
    alert('The File APIs are not fully supported in this browser.');
}
if(window.FileReader) {
    reader = new FileReader();
} else {
    alert('The File APIs are not fully supported in this browser. So Please use latest browser.');
}

// jQuery with the document ready function on page load
$(function () {
    imagePreview();

    signUpAndLogin();

    showTab(); // show tabs in users login and signup pages

    validateForm(); // Validate user control forms

    showMoreData(); // Show OR Hide data on click in Product page      

    filterChange();   // Community and Response discussion Filter change
    paginationChange(); // Community and Response discussion pagination view

    $(".emaillogin").click(function () {
        $(".emaillogin > span").show();
    });

    // When non login user click on this link,
    // Find weather .navbar toggle is block or none 
    // if block then expand the toggle by self click 
    $("#login-to-responses, #login-to-post-response, #login-to-post").click(function () {
        if ($(".navbar-toggle").css("display") != "none") {
            $(".navbar-toggle").trigger('click');
        }

        $("#login-link").trigger('click');
        $('#article-container').css('opacity', '0.2');
        $("html, body").scrollTop(0);
        $("#loginHint").show("slow");
        window.setTimeout(function () { $("#loginHint").slideUp('slow'); }, 8000);
        return false;
    });

    $("body").click(function () {
        $('#article-container').css('opacity', '1');
    });
    // Load top 5 users in right hand side panel
    $("#top-users").html(ajaxLoaderBarImage);
    $.ajaxSetup({
        // Disable caching of AJAX responses 
        cache: false
    });
    $.get(base_url + "users/topFiveUsers/5", function (data) {
        $("#top-users").html(data);
    });
});

// BEGIN Show more content
this.showMoreData = function () {
    $(".show-more a").on("click", function () {
        var $link = $(this);
        var $content = $link.parent().prev("div.text-content");
        var linkText = $link.text();

        switchClasses($content);

        $link.text((linkText.toUpperCase() === "SHOW MORE") ? "Show less" : "Show more");
        return false;
    });
};

this.switchClasses = function ($content) {
    if ($content.hasClass("short-text")) {
        //$content.switchClass("short-text", "full-text", 165);
        $content.toggleClass("full-text");
    } else {
        //$content.switchClass("full-text", "short-text", 165);
        $content.toggleClass("short-text");
    }
}
// END Show more content

this.ajaxCall = function (currentUrl) {

    window.history.pushState(null, null, currentUrl);

    var divId ='';

    if(methodName === 'index'){
        var url = currentUrl.replace('/index/', "/ajax_discussion_list/");
        divId = 'disc-content';
    }else  if(methodName === 'view'){
        var url = currentUrl.replace('/view/', "/ajax_resp_list/");
        divId = 'resp-content';
    }

    // Load Ajax bar loader image
    $('#' + divId).empty().css('text-align', 'center').append(ajaxLoaderBarImage);

    $.ajax({
        url: url,
        dataType: "html",
        success: function (data) {
            $('#ajax-loader-bar').remove(); // Remove Ajax bar loader image
            $('#' + divId).css('text-align', 'left');
            $('#' + divId).html(data);
            paginationChange();
        }
    });
};

this.filterChange = function () {
    var href = '';
    $("#filter").change(function () {
        href = $(this).val();
        ajaxCall(href);
    });
};

this.paginationChange = function () {
    var href = '';
    // Redirect user to selected page, when user select filters in discussion page and responses page
    $('#disc-list-view ul.pagination li a').click(function (e) {    
        e.preventDefault();
        href = $(this).attr('href');
        if (href != '#') {
            ajaxCall(href);
        }
    });
    $('#resp-list-view ul.pagination li a').click(function (e) {
        e.preventDefault();
        href = $(this).attr('href');
        if (href != '#') {
            ajaxCall(href);
        }
    });

};
// END Filter And Pagination Change view AJAX

// COOKIE SET
this.setCookie = function(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();
    document.cookie = cname + "=" + cvalue + "; " + expires + '; path=/';
}

/**
 *
 * This is method showImage 
 * Show the big image in popup box
 * Show video in poup on mouseover of the video image
 *
 */
this.imagePreview = function () {
    $('.media_popup_parent').on({

        // Redirect to show media page when user click on any image or video from website except user profile
        click: function (e) {
            var type = $(this).attr('data-media-type');
            var url = $(this).attr('data-media-path');
            var description = $(this).attr('data-media-description');

            if ($('#imgVideoPopup').length > 0) {                
                /* Remove video when user click on video image to fix video play on mobiles */                
                if ($('#imgVideoPopup')[0].tagName !== 'IMG') {
                    var ivOopUp = document.getElementById('imgVideoPopup');                    
                    ivOopUp.pause();
                }
                $('#imgVideoPopup').remove();
            }            
            if (type === 'image') {
                setCookie("mediaDescription", description, 1);
                setCookie("mediaPath", url, 1);
            }

            window.open( (type === 'image') ? base_url + 'showmedia' : url, '_newtab');
        },
        mouseenter: function (e) {
            var width = videoWidth = parseInt(400);

            if ($(window).width() <= width) {
                width = parseInt($(window).width());
                videoWidth = width - 50;
            }

            $(window).resize(function () {
                if ($(window).width() <= width) {
                    width = parseInt($(window).width());
                    videoWidth = width - 50;
                }
            });

            var mediaPath = $(this).attr('data-media-path');
            var mediaWidth = parseInt($(this).attr('data-media-width'));
            var mediaHeight = $(this).attr('data-media-height');
            var mediaType = $(this).attr('data-media-type');
            var mediaDescription = $(this).attr('data-media-description');
            var newHTML = '';
            var temp = '';

            if (mediaType == 'image') {
                width = (mediaWidth < parseInt(width)) ? mediaWidth : width;
                temp += '<img id="imgVideoPopup" class="overlay_image" src="' + mediaPath + '" style="width:' + width + 'px" />';
            }

            if (mediaType == 'video') {
                width = parseInt(videoWidth);
                temp += '<video id="imgVideoPopup" style="width:' + videoWidth + 'px;" loop="true" autoplay="autoplay" class="media-object"><source src="' + mediaPath + '" type="video/mp4">Your browser does not support HTML5 video.</video>';
            }

            newHTML += temp;

            if ($.trim(mediaDescription) != '') {
                newHTML += '<div id="mediaDescPopup" class="media_description">' + mediaDescription + '</div>';
            }

            nhpup.popup(newHTML, { 'width': width }, mediaType);
        },
        mouseleave: function (e) {
            if ($('#imgVideoPopup').length > 0) {
                var mediaType = $(this).attr('data-media-type');

                // stop video if media type is video when user leave mouse from video object
                if (mediaType === 'video') {
                    var vid = document.getElementById("imgVideoPopup");
                    vid.pause();
                }

                $('#imgVideoPopup').animate({ "opacity": "hide" }, "slow").remove();
                $('#mediaDescPopup').animate({ "opacity": "hide" }, "slow").remove();
            }
        }
    });
};

this.showTab = function () {
    /**
     * Show home page right side panel TABS
     */
    $('#t_widget a').click(function (e) {
        e.preventDefault();
        $(this).tab('show');
    });
};

this.signUpAndLogin = function () {
    $('#forgotpassword-link').click(function () {
        $('#login-form-div').hide();
        $('#forgotpassword-div').show();
    });
};

/**
 *
 * Validate user input form
 * login, sing up, forgot password etc...
 *
 */
this.validateForm = function () {
    // jQuery.validator.setDefaults({debug: true,success: "valid" });

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

    /**
     * Validate Forgot Password Form
     */
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

    /**
     * Validate Login Form Password
     */
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
        },
        submitHandler: function (form) {
            $('#ajax-loader').remove(); // self remove
            var loginBtn = $('#login-form .btn'); // load login button

            $(loginBtn).hide(); // hide after user clik on login button
            $(loginBtn).after(ajaxLoaderImage); // load ajax loder image

            // Using AJAX send form data to
            var xhr = new XMLHttpRequest();

            // open XHR connection and send 3 parm 'action method', action url, boolean to set Asynchronous or not
            xhr.open('POST', form.action, true);

            // Send form data
            var formData = new FormData(form);
            xhr.send(formData);

            // Load xhr response
            xhr.onload = function () {
                //console.log(this.responseText);               
                var response = '';
                // Try to parse json string if not then directly show the message
                try {
                    response = JSON.parse(this.responseText);
                } catch (e) {
                    response = { 'status': false, 'message': this.responseText };
                }

                $('#ajax-loader').remove(); // slef remove
                var span = '';
                if (!response.status) {
                    span = $('<label>').html(response.message).attr('class', 'error');
                    $(loginBtn).show(); // show login button
                    $(loginBtn).after(span); // append response after button
                } else if (response.status) {
                    span = $('<label>').html('<br />Successfully login.').attr('style', 'color:green');
                    $(loginBtn).after(span); // append response after button
                    location.reload(); // Reload/Refresh same page
                }
            };
        }
    });

    // BEGIN VALIDATE SINGUP FORM
    /**
     * Validate Signup Password
     */
    $('#signup-form').validate({
        rules: {
            fullname: { required: true, alphabet: true },
            avatar: { accept: "image/*" },
            city: { required: true, maxlength: 50, alphabet: true },
            state: { required: true, maxlength: 50, alphabet: true },
            twitter_handle: { maxlength: 100 },
            email: { required: true, email: true, maxlength: 200 },
            password: { required: true, rangelength: [8, 14], ContainsAtLeastOneDigit: true, ContainsAtLeastOneCapitalLetter: true },
            confirm_password: { required: true, equalTo: "#signup_password" },
            //twitter_handle: {required: },
            terms_service: "required"
        },
        highlight: function (element) {
            $(element).closest('.control-group').removeClass('success').addClass('error');
        },
        success: function (element) {
            //element.text('OK!').addClass('valid').closest('.control-group').removeClass('error').addClass('success');
            element.closest('label.error').remove();
        }
    });

    // When user click on tweet notification checkbox and if that is checked then set validation required true for twitter handle
    $('#tweet_notification').on('change', function (e) {
        //console.log(this.name + ' ' + this.value + ' ' + this.checked);
        if (this.checked && $('#isTwitterUser').val()) {
            if ($('#twitter_handle').length > 0) {
                $('#twitter_handle').rules('add', { required: true });
            }
        } else {
            if ($('#twitter_handle').length > 0) {
                $('#twitter_handle').rules('add', { required: false });
            }
        }
    });

    if ($('#twitter_handle').length > 0) {
        // While change the value of twitter hanle
        $('#twitter_handle').on('change', function (e) {
            addTwHadleRule('#twitter_handle');
        });

        // When form loaded and there is an empty value of twitter handle
        addTwHadleRule('#twitter_handle');
    }

    this.addTwHadleRule = function (twhdle) {
        if ($.trim(twhdle.value) === '' && $("#tweet_notification").is(":checked")) {
            $(twhdle).rules('add', { required: true });
        } else {
            $(twhdle).rules('add', { required: false });
        }
    }

    // END VALIDATE SINGUP FORM


    // BEGIN VALIDATE CHANGE PASSWORD FORM
    /**
     * Validate Change Password Form
     */
    $('#changepassword-form').validate({
        rules: {
            password: { required: true, rangelength: [8, 14], ContainsAtLeastOneDigit: true, ContainsAtLeastOneCapitalLetter: true },
            confirm_password: { required: true, equalTo: "#change_password" }
        },
        highlight: function (element) {
            $(element).closest('.control-group').removeClass('success').addClass('error');
        },
        success: function (element) {
            element.closest('label.error').remove();
        }
    });
    // END VALIDATE CHANGE PASSWORD FORM

    /**
     * Validate Community Form
     */
    $('#chooseMediaFile').click(function () {
        //console.log('Click chooseMediaFile');
        $("#mediaFile").trigger("click");
    });

    $("#mediaFile").change(function () {
        //console.log('Click mediaFile');
        var fileval = $(this).val();
        $("#uploadFileInfo").html(fileval);

        if ($.trim(fileval) !== '') { $("#media-description-form-group").show(); } else { $("#media-description-form-group").hide(); }
    });

    // START FILE UPLOAD TO WAMS
    $('#new-discussion-form-upload').validate({
        rules: {
            title: { required: true, maxlength: 60 },
            content: { required: true },
            mediaFile: { accept: "image/*|video/*" },
            mediaDescription: { required: getBooleanMediaEmpty, maxlength: 140 }
        },
        highlight: function (element) {
            $(element).closest('.control-group').removeClass('success').addClass('error');
        },
        success: function (element) {
            element.closest('label.error').remove();
        },
        submitHandler: function (form) {
            $('#status-msg').show().addClass('alert-success').html("Uploading..." ).append(ajaxLoaderBarImage);
            var $bar = $('#progress-bar');
            $(form).hide();
            var pbValue = '';
            var formData = new FormData();
            formData.append('title', $('#title').val());
            formData.append('content', $('#content').val());
            formData.append('mediaFileName', $('#mediaFile').val());
            formData.append('mediaDescription', $('#mediaDescription').val());

            if (fileType !== '') {                
                isImage = (fileType.split('/')[0] === 'image') ? 1 : 0;
                if (isImage) {
                    formData.append('mediaFile', myFile);
                }
                formData.append('fileType', fileType);
                //console.log('isimage '+isImage);
                formData.append('isImage', isImage);
                formData.append('haveMedia', true);                 
            } else {
                formData.append('fileType', '');
                formData.append('isImage', 0);
                formData.append('haveMedia', 0);
            }            

            // Using AJAX send form data to
            var xhr = new XMLHttpRequest();

            // open XHR connection and send 3 parm 'action method', action url, boolean to set Asyncronous or not
            xhr.open('POST', form.action, true);

           // console.log('form data '+formData);
            xhr.send(formData);  // multipart/form-data
            
            // Load
            xhr.onload = function () {
                $('#progress').removeClass('active');
                $bar.width('100%');
                var response = '';
                try {
                    response = JSON.parse(this.responseText);
                } catch (e) {
                    response = { 'status': false, 'message': this.responseText };
                }

                if (!response.status) {
                    $(form).show();
                    $('#status-msg').addClass('alert-danger').html(response.message).css({ 'background-color': '#fff', 'padding': '10px;' });
                } else if (response.status) {
                    if (fileType !== '') {
                        if (isImage) {
                            $('#status-msg').addClass('alert-success');
                            $('#status-msg').html('Details updated successfully');
                            location.href = $('#success-redirect-url').val();
                        } else {
                          //  console.log('SAS Write URL ', response.sasWriteUrl);
                            submitUri = response.sasWriteUrl;
                            //submitUri = submitUri.replace('https://','');                            
                            //console.log('submit url  ', submitUri);
                            assetId = response.assetId;
                            assetFileName = response.assetFileName;
                            uploadFileUsingSasUrl();
                        }
                    } else {
                        $('#status-msg').addClass('alert-success');
                        $('#status-msg').html('Details updated successfully');
                        location.href = $('#success-redirect-url').val();
                    }
                }
            };
        }
    });
    // END FILE UPLOAD TO WAMS

    /**
     * Community Response Form
     */
    $('#discussion-response-form-upload').validate({
        rules: {
            content: { required: true },
            mediaFile: { accept: "image/*|video/*" },
            mediaDescription: { required: getBooleanMediaEmpty, maxlength: 140 }
        },
        highlight: function (element) {
            $(element).closest('.control-group').removeClass('success').addClass('error');
        },
        success: function (element) {
            element.closest('label.error').remove();
        },
        submitHandler: function (form) {
            $('#status-msg').show().addClass('alert-success').html("Uploading...").append(ajaxLoaderBarImage);
            var $bar = $('#progress-bar');
            $(form).hide();
            var pbValue = '';
            
            var formData = new FormData();           
            formData.append('content', $('#content').val());
            formData.append('postId', $('#postId').val());
            formData.append('mediaFile', $('#mediaFile').val());
            formData.append('mediaDescription', $('#mediaDescription').val());

            if (fileType !== '') {                
                isImage = (fileType.split('/')[0] === 'image') ? 1 : 0;
                if(isImage){                    
                    formData.append('mediaFile', myFile);
                }
                formData.append('fileType', fileType);
                //console.log('isimage '+isImage);
                formData.append('isImage', isImage);
                formData.append('haveMedia', true);                 
            } else {
                formData.append('fileType', '');
                formData.append('isImage', 0);
                formData.append('haveMedia', 0);
            }
            
            // Using AJAX send form data to
            var xhr = new XMLHttpRequest();

            // open XHR connection and send 3 parm 'action method', action url, boolean to set Asyncronous or not
            xhr.open('POST', form.action, true);

            // open XHR connection and send 3 parm 'action method', action url, boolean to set Asyncronous or not
            xhr.open('POST', form.action, true);
          
            xhr.send(formData);  // multipart/form-data

            // Load 
            xhr.onload = function () {
                //console.log(this.responseText);
                $('#progress').removeClass('active');
                $bar.width('100%');
                //console.log('RS TEXT-->', JSON.parse(this.responseText));
                var response = '';
                try {
                    response = JSON.parse(this.responseText);
                } catch (e) {
                    response = { 'status': false, 'message': this.responseText };
                }

                if (!response.status) {
                    $(form).show();
                    $('#status-msg').addClass('alert-danger').html(response.message).css({ 'background-color': '#fff', 'padding': '10px;' });
                } else if (response.status) {
                    if (fileType !== '') {
                        if (isImage) {
                            $('#status-msg').addClass('alert-success');
                            $('#status-msg').html('Details updated successfully');
                            location.reload();
                        } else {
                          //  console.log('SAS Write URL ', response.sasWriteUrl);
                            submitUri = response.sasWriteUrl;
                            //submitUri = submitUri.replace('https://','');                            
                            //console.log('submit url  ', submitUri);
                            assetId = response.assetId;
                            assetFileName = response.assetFileName;
                            uploadFileUsingSasUrl();
                        }
                    } else {
                        $('#status-msg').addClass('alert-success');
                        $('#status-msg').html('Details updated successfully');
                        location.reload();
                    }
                }
            };
        }
    });
};

this.getBooleanMediaEmpty = function () {
    var flag = ($.trim($("#mediaFile").val()) === '') ? false : true;
    return flag;
};

// Add event listener when file selector change
if ($('#mediaFile').length > 0) {
    document.getElementById('mediaFile').addEventListener('change', function (e) { console.log('FILE content', myFile); handleFileSelect(e); }, false);
}

this.handleFileSelect = function (e) {    
    var files = e.target.files || e.dataTransfer.files;;
    selectedFile = files[0];
    myFile = selectedFile;    
    fileType = selectedFile.type;
    //console.log('fileName->', selectedFile.name, 'fileSize', selectedFile.size, 'fileType', selectedFile.type)
    var fileSize = selectedFile.size;
    if (fileSize < maxBlockSize) {
        maxBlockSize = fileSize;
        //console.log("max block size = " + maxBlockSize);
    }
    totalBytesRemaining = fileSize;
    if (fileSize % maxBlockSize == 0) {
        numberOfBlocks = fileSize / maxBlockSize;
    } else {
        numberOfBlocks = parseInt(fileSize / maxBlockSize, 10) + 1;
    }
    //console.log("total blocks = " + numberOfBlocks);
}

this.uploadFileUsingSasUrl = function () {
    if (totalBytesRemaining > 0) {
       // console.log("current file pointer = " + currentFilePointer + " bytes read = " + maxBlockSize);
        var fileContent = selectedFile.slice(currentFilePointer, currentFilePointer + maxBlockSize);
        //console.log('File Content', fileContent);
        var blockId = blockIdPrefix + pad(blockIds.length, 6);
       // console.log("block id = " + blockId);
        blockIds.push(btoa(blockId));
        reader.readAsArrayBuffer(fileContent);
        currentFilePointer += maxBlockSize;
        totalBytesRemaining -= maxBlockSize;
        if (totalBytesRemaining < maxBlockSize) {
            maxBlockSize = totalBytesRemaining;
        }
    } else {
      // console.log("totalBytesRemaining = " + totalBytesRemaining);
        commitBlockList();
    }
}

reader.onloadend = function (evt) {
    if (evt.target.readyState == FileReader.DONE) { // DONE == 2
        var uri = submitUri + '&comp=block&blockid=' + blockIds[blockIds.length - 1];
        var requestData = new Uint8Array(evt.target.result);
        $.ajax({
            url: uri,
            type: "PUT",
            data: requestData,
            processData: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader('x-ms-blob-type', 'BlockBlob');
                xhr.setRequestHeader('Content-Length', requestData.length);
                xhr.setRequestHeader('x-ms-blob-content-type', selectedFile.type);
            },
            success: function (data, status) {
              // console.log('data'+ data + ' status ' + status);
               bytesUploaded += requestData.length;
               var percentComplete = ((parseFloat(bytesUploaded) / parseFloat(selectedFile.size)) * 100).toFixed(2);
               // $("#fileUploadProgress").text(percentComplete + " %");
              // console.log('percentComplete '+percentComplete);
               uploadFileUsingSasUrl();
            },
            error: function (xhr, desc, err) {
               // console.log(desc);
               // console.log(err);
            }
        });
    }
};

this.commitBlockList = function () {
    var uri = submitUri + '&comp=blocklist';
    var requestBody = '<?xml version="1.0" encoding="utf-8"?><BlockList>';
    for (var i = 0; i < blockIds.length; i++) {
        requestBody += '<Latest>' + blockIds[i] + '</Latest>';
    }
    requestBody += '</BlockList>';

    $.ajax({
        url: uri,
        type: "PUT",
        data: requestBody,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('x-ms-blob-content-type', selectedFile.type);
            xhr.setRequestHeader('Content-Length', requestBody.length);
        },
        success: function (data, status) {
            //console.log('Data '+ data + 'status '+ status);
            //$('#status-msg').html('It will take some time to show ......... ');
            createJob();
        },
        error: function (xhr, desc, err) {
            $('#status-msg').addClass('alert-danger');
            $('#status-msg').html('Error: '.err);
           // console.log('Desc->'+ desc + 'Error->'+ err);
        }
    });
};

this.pad = function (number, length) {
    var str = '' + number;
    while (str.length < length) {
        str = '0' + str;
    }
    return str;
};

this.createJob = function () {
    assetId, assetFileName
    if (assetId !== '') {
        var url = base_url + 'community/createJob/' + assetId + '/' + assetFileName;
        $.ajax({
            url: url,
            type: "POST",           
            success: function (data) {
                var dataobj = $.parseJSON(data);                
                if (dataobj.status) {                    
                    location.href = $('#success-redirect-url').val();
                } else {
                    $('#status-msg').addClass('alert-danger');
                    $('#status-msg').html('Error: Please Try again');
                }
            },
            error: function (xhr, desc, err) {
                $('#status-msg').addClass('alert-danger');
                $('#status-msg').html('Error: '.err);
            }
        });
    } else {
        location.reload(true);
    }
}

if ($('#avatar').length > 0) { document.getElementById('avatar').addEventListener('change', function (e) { readURL(this, 'avatar'); }, false); }

this.readURL = function (input, id) {
    if ($('#showavatar').length > 0) { $('#showavatar').remove(); }
    if (input.files && input.files[0]) {
        //console.log(input.files);        
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
}

if ($('#flashdata').length) {
    window.setTimeout(function () { $("#flashdata").slideUp('slow'); }, 3000);
}