const quoteBtns = document.querySelectorAll('.quoteBtn');
const quoteIdInputs = document.querySelectorAll('.quoteIdInput');

quoteBtns.forEach(button => {
    button.addEventListener('click', function () {
        const quoteId = this.getAttribute('data-quote-id');
        quoteIdInputs.forEach(input => input.value = quoteId);
    });
});