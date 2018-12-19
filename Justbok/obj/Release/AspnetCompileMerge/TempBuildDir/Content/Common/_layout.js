var currenturl = "", currenttitle = ""
function LoadPage(url, title) {
    ShowLoader();
    $('#RenderBody').load(url);
    currenturl = url;
    currenttitle = title;

    // Change URL with browser address bar using the HTML5 History API.
    if (history.pushState) {
        // Parameters: data, page title, URL
        history.pushState(null, title, url);
    }
        // Fallback for non-supported browsers.
    else {
        document.location.hash = url;
    }
    document.title = title;
}

function branchChange() {
    LoadPage(currenturl, currenttitle);
    return false;
}
