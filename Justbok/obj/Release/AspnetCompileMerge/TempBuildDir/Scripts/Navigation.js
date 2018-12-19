var readyStateCheckInterval = self.setInterval(function () {
    // Check if DOM ready.
    if (document.readyState === "complete") {
        // When DOM ready, destroy timer and execute setupLinks() function.
        clearInterval(readyStateCheckInterval);

        // Attach event listeners for browser history and hash changes.
        popStateHandler();
    }
}, 10);

function popStateHandler() {
    // FF, Chrome, Safari, IE9.
    if (history.pushState) {
        // Event listener to capture when user pressing the back and forward buttons within the browser.
        window.addEventListener("popstate", function (e) {
            // Get the URL from the address bar and fetch the page.
            ShowLoader();
            location.reload();
        });
    }

        // IE8.
    else {
        // Event listener to cature address bar updates with hashes.
        window.attachEvent("onhashchange", function (e) {

            // Extract the hash
            location.reload();
        });
    }
}