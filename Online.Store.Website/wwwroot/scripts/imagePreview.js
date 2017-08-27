var imagePreview = {
    attachImagePreviewToImages: function () {
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

                window.open((type === 'image') ? base_url + 'showmedia' : url, '_newtab');
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
    }    
};