; var viewCart = {
    initialize: function () {
        viewCart.attachevents();
    },
    attachevents: function () {

        $("body").off("click", '.removeItem')
        $("body").on('click', '.removeItem', function () {
            var cartid = $(this).data('cartid');
            ajaxRequest.makeRequest("/Product/RemoveItem/" + cartid + "/" + $(this).data('productid'), "POST", null,
                function (data) {
                    if (data) {
                        window.location.reload();
                    }
                });
        });

        $("body").off("change", '.itemQty')
        $("body").on('change', '.itemQty', function () {
            var cartid = $(this).data('cartid');
            ajaxRequest.makeRequest("/Product/UpdateQty/" + cartid + "/" + $(this).data('productid') + "/" + $(this).val(), "POST", null,
                function (data) {
                    if (data) {
                        $.growl({
                            icon: 'glyphicon glyphicon-ok',
                            title: ' ',
                            message: 'Product Quantity updated.',
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
                            message: 'Error while updating Quantity.',
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