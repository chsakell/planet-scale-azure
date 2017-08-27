var starNewDiscussion = {
    starNewDiscussionLoginCheck: function () {

        $("#login-to-responses, #login-to-post-response, #login-to-post").click(function () {
            if ($(".navbar-toggle").css("display") != "none") {
                $(".navbar-toggle").trigger('click');
            }

            $("#login-link").trigger('click');
            $('#article-container').css('opacity', '0.2');
            $("html, body").scrollTop(0);
            $("#loginHint").show("slow");
            window.setTimeout(function () { $("#loginHint").slideUp('slow'); }, 3000);
            return false;
        });

        $("#login-to-addcart").click(function () {
            if ($(".navbar-toggle").css("display") != "none") {
                $(".navbar-toggle").trigger('click');
            }

            $("#login-link").trigger('click');
            $('#article-container').css('opacity', '0.2');
            $("html, body").scrollTop(0);
            $("#loginCartHint").show("slow");
            window.setTimeout(function () { $("#loginCartHint").slideUp('slow'); }, 3000);
            return false;
        });

        $(".viewCart").click(function () {
            if ($(".navbar-toggle").css("display") != "none") {
                $(".navbar-toggle").trigger('click');
            }

            $("#login-link").trigger('click');
            $('#article-container').css('opacity', '0.2');
            $("html, body").scrollTop(0);
            $("#loginViewCartHint").show("slow");
            window.setTimeout(function () { $("#loginViewCartHint").slideUp('slow'); }, 3000);
            return false;
        });

        $("body").click(function () {
            $('#article-container').css('opacity', '1');
        });

    }
};