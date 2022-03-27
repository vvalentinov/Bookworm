$('#customFile').on('change', function () {
    if ($(this).val().length > 55) {
        var fileName = $(this).val()
            .substring(0, 55)
            .concat("...")
            .replace('C:\\fakepath\\', " ");

        $(this).next('.custom-file-label').html(fileName);
    } else {
        var fileName = $(this).val().replace('C:\\fakepath\\', " ");
        $(this).next('.custom-file-label').html(fileName);
    }
})