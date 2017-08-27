; var signup = {
    initialize: function () {
        $('#avatar').change(function (e) {
            app.readURL(this, 'avatar');
        });
    },
};