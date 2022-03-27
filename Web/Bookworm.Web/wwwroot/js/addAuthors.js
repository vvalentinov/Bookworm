var authorIndex = 0;
function AddAuthors() {
    $("<div id='authorName' class='form-group justify-content-center'><div class='input-group'><div class='input-group-prepend'><span class='input-group-text'>Author's name</span></div><input asp-for='Name' type='text' name='AuthorsNames[" + authorIndex + "].Name' class='form-control'></div><span asp-validation-for='Name' class='small text-danger'></span></div>")
        .hide()
        .appendTo("#AuthorsContainer")
        .show('fast');

    authorIndex++;
}