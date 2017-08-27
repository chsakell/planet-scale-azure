; var products = {
    initialize: function () {
        products.attachEvents();
    },

    attachEvents: function () {


        $(".show-more a").on("click", function () {
            var $link = $(this);
            var $content = $link.parent().prev("div.text-content");
            var linkText = $link.text();
            if ($content.hasClass("short-text")) {
                //$content.switchClass("short-text", "full-text", 165);
                $content.toggleClass("full-text");
            } else {
                //$content.switchClass("full-text", "short-text", 165);
                $content.toggleClass("short-text");
            }
            $link.text((linkText.toUpperCase() === "SHOW MORE") ? "Show less" : "Show more");
            return false;
        });

        $("body").off("click", '.addToCart')
        $("body").on('click', '.addToCart', function () {
            var productId = $(this).data('productid');
            ajaxRequest.makeRequest("/Product/AddToCart/" + productId, 'POST', null,
                function (data) {
                    if (data) {
                        $.growl({
                            icon: 'glyphicon glyphicon-ok',
                            title: ' ',
                            message: 'Product added to cart.',
                        }, {
                            element: 'body',
                            type: "success",
                            template: '<div data-growl="container" class="alert" role="alert"> <button type="button" class="close" data-growl="dismiss"> <span aria-hidden="true">×</span><span class="sr-only">Close</span></button><span data-growl="icon"></span><span data-growl="title"></span><span data-growl="message"></span><a href="#" data-growl="url"></a></div>'
                        });
                    }
                    else {
                        $.growl({
                            icon: 'glyphicon glyphicon-warning-sign',
                            title: ' ',
                            message: 'Error while adding product to cart.',
                        }, {
                            element: 'body',
                            type: "danger",
                            template: '<div data-growl="container" class="alert" role="alert"> <button type="button" class="close" data-growl="dismiss"> <span aria-hidden="true">×</span><span class="sr-only">Close</span></button><span data-growl="icon"></span><span data-growl="title"></span><span data-growl="message"></span><a href="#" data-growl="url"></a></div>'
                        });
                    }
                });
        });
    }
};