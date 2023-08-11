const authorInputEl = document.getElementById('authorInput');
const bookInputEl = document.getElementById('bookInput');
const movieInputEl = document.getElementById('movieInput');

authorInputEl.addEventListener('focus', function () {
    bookInputEl.setAttribute('disabled', 'disabled');
    bookInputEl.value = '';
    movieInputEl.setAttribute('disabled', 'disabled');
    movieInputEl.value = '';
});

bookInputEl.addEventListener('focus', function () {
    //authorInputEl.setAttribute('disabled', 'disabled');
    //authorInputEl.value = '';
    movieInputEl.setAttribute('disabled', 'disabled');
    movieInputEl.value = '';
});

movieInputEl.addEventListener('focus', function () {
    authorInputEl.setAttribute('disabled', 'disabled');
    authorInputEl.value = '';
    bookInputEl.setAttribute('disabled', 'disabled');
    bookInputEl.value = '';
});