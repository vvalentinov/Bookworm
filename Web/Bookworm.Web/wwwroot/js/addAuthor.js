var authorIndex = 0;
function AddAuthors() {
    $("<div id='authorName' class='form-group justify-content-center'><label asp-for='Author'><strong>Author's Name</strong></label><input asp-for='Author' type='text' class='form-control'><span asp-validation-for='Author' class='small text-danger'></span></div>")
        .hide()
        .appendTo("#AuthorsContainer")
        .show('fast');

    authorIndex++;
}