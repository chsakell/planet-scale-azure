// the class to work with all the ajax request and
var ajaxRequest =
{
    makeRequest: function (requestUrl, type, contextData, successCallback, errorCallback, dataType) {

        requestUrl = app.getBaseURL() + requestUrl;

        if (dataType == null) {
            dataType = "json";
        }
        
        switch (type) {

            case "GET":
                $.ajax({
                    type: "GET",
                    cache: false,
                    contentType: "application/json; charset=utf-8",
                    url: requestUrl,
                    data: contextData,
                    dataType: dataType,
                    success: function (response) {
                        if (successCallback) {
                            successCallback(response);
                        }
                    },
                    error: function (response) { },
                    complete: function (jqXHR, textStatus) { ajaxRequest.hideBusyIndicator(); }
                });
                break;
            case "POST":
                // Make POST http ajax request
                $.ajax({
                    type: "POST",
                    cache: false,
                    contentType: "application/json; charset=utf-8",
                    url: requestUrl,
                    data: JSON.stringify(contextData),
                    dataType: dataType,
                    success: function (response) {
                        if (successCallback) {
                            successCallback(response);
                        }
                    },
                    error: function (response) { },
                    statusCode:
                    {
                        400: function (data) {
                            var validationResult = $.parseJSON(data.responseText);
                            $.publish("ShowValidationError", [validationResult]);
                        }
                    },
                    complete: function (jqXHR, textStatus) { ajaxRequest.hideBusyIndicator(); }
                });
                break;

        }
    },

    showBusyIndicator: function () {
        //        $.blockUI({
        //            message: '<img style="" src="/asset/busy.gif" />',
        //            css: { backgroundColor: "transparent", border: 'none' }
        //        }); ;
    },

    hideBusyIndicator: function () {
        //$.unblockUI();
    }
};