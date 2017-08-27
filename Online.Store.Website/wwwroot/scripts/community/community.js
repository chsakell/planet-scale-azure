var community = {
    initialize: function () {
        community.attachEvents();
    },

    attachEvents: function () {
        $("body").off("change", '#filter')
        $("body").on('change', '#filter', function (e) {
            e.preventDefault();
            var baseUrl = app.getBaseURL();
            var id = $('#filter').val();
            $('#disc-content').html("");
            $('#status_msg').show().addClass('alert-success').html("Loading...").append(app.getLoadingBar(true));
            //$('#disc-content').append(app.getLoadingBar(true));
            ajaxRequest.makeRequest("/Community/GetCommunity", "GET", {
                "id": id,
                "pageId":0
            }, function (data) {
                $('#status_msg').hide().removeClass('alert-success');
                $('#status_msg').html("");
                $('#disc-content').html("");
                $('#disc-content').html(data);
            },null,'html');
        });

        $("body").off("click", '.pageID')
        $("body").on('click', '.pageID', function (e) {
            e.preventDefault();
            var baseUrl = app.getBaseURL();
            var id = $('#filter').val();
            var pageid = $(this).attr("pageId");
            $('#disc-content').html("");
            $('#status_msg').show().addClass('alert-success').html("Loading...").append(app.getLoadingBar(true));
            //$('#disc-content').append(app.getLoadingBar(true));
            ajaxRequest.makeRequest("/Community/GetCommunity", "GET", {
                "id": id,
                "pageId":pageid
            }, function (data) {
                $('#status_msg').hide().removeClass('alert-success');
                $('#status_msg').html("");
                $('#disc-content').html("");
                $('#disc-content').html(data);
            },null,'html');
        });
    }
};