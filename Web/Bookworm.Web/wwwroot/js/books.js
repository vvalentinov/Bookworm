import { updatePagination } from './pagination.js';

const bookSearchInput = document.querySelector('.bookSearchInput');
const bookSearchButton = document.getElementById('bookSearchButton');

bookSearchButton.addEventListener('click', () => fetchForBooks());
bookSearchInput.addEventListener('keypress', function (event) {
    if (event.key === 'Enter') {
        fetchForBooks();
    }
});

const fetchForBooks = (page) => {
    const searchParams = new URLSearchParams(window.location.search);
    let category = searchParams.get('category');
    category = encodeURIComponent(category);

    if (!page) {
        page = 1;
    }

    let input = bookSearchInput.value.trim();
    if (!input) {
        input = '';
    }

    fetch(`/ApiBook/SearchBook?input=${input}&page=${page}&category=${category}`)
        .then(res => res.json())
        .then(res => {
            updateBooks(res.books);
            updatePagination(res, fetchForBooks);
        })
        .catch(err => console.log(err.message));
};

const updateBooks = (books) => {
    const bookSection = document.querySelector('.booksSection');
    bookSection.innerHTML = '';

    if (books?.length > 0) {
        books.forEach(book => {
            const divElement = document.createElement('div');

            const anchorElement = document.createElement('a');
            anchorElement.href = `/Book/Details/${book.id}`;

            const imageElement = document.createElement('img');
            imageElement.title = book.title;
            imageElement.src = book.imageUrl;
            imageElement.alt = book.title;
            imageElement.className = 'imageZoom';
            imageElement.height = 300;

            anchorElement.appendChild(imageElement);

            divElement.appendChild(anchorElement);

            bookSection.appendChild(divElement);
        });
    } else {
        const h2Element = document.createElement('h2');
        h2Element.textContent = 'No books found based on search!';
        h2Element.style = 'color: white';
        bookSection.appendChild(h2Element);
    }
};