$(document).ready(function () {
    // Handle all clicks on links with ajax-link class
    $(document).on("click", ".ajax-link", function (e) {
        e.preventDefault();

        let url = $(this).data("url"); // Get URL from data-url attribute
        let target = $("#main-content"); // Where to load content

        if (!url) {
            console.error("No URL found for this link");
            return;
        }

        // Load content dynamically via AJAX
        $.ajax({
            url: url,
            type: "GET",
            success: function (data) {
                target.html(data);
            },
            error: function (xhr) {
                console.error("Error loading page", xhr);
                target.html("<p class='text-danger'>Error loading content.</p>");
            }
        });
    });
});
