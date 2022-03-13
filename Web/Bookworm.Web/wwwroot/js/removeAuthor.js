function RemoveAuthors() {
    $("#authorName:last-child")
        .hide('fast', function () {
        $(this).remove();
    });
}