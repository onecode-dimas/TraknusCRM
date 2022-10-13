function ExecuteAction(actionName, question) {
    var _return = window.confirm(question);

    if (_return) {
        var result = null

        // Creating the Odata Endpoint
        var oDataPath = Xrm.Page.context.getClientUrl() + "/api/data/v8.0/";
        var req = new XMLHttpRequest();
        req.open("POST", oDataPath + actionName, false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState == 4) {
                req.onreadystatechange = null;
                if (this.status == 200 || this.status == 204) {
                    Xrm.Utility.alertDialog('Successfully executed.');
                    location.reload();
                }
                else {
                    var error = "";
                    if (this.response != null) {
                        error = JSON.parse(this.response).error.message;
                    }
                    Xrm.Utility.alertDialog('Fail to Execute.\r\nResponse Code : (' + req.status + ')' + "\r\nPlease contact your IT support.\r\nTechnical Details : " + error);
                }
            }
        };
        req.send();
    }
}

function Process_Generate() {
    debugger;
    try {
        var actionName = 'tss_AGITSummaryGenerateSummaryMarketSizeByPartType';
        ExecuteAction(actionName, "Do you want to Generate Summary Market Size By Part Type ?");
    } catch (e) {
        Xrm.Utility.alertDialog('Fail to Execute : ' + e.message);
    }
}

function Show_Generate() {
    debugger;
    var _result = true;

    return _result;
}