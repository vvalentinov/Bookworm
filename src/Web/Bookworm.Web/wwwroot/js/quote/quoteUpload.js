$("#movieQuoteForm").validate({
    rules: {
        MovieTitle: {
            required: true,
            minlength: 5,
            maxlength: 150
        },
        Content: {
            required: true,
            minlength: 10,
            maxlength: 150
        }
    },
    messages: {
        MovieTitle: {
            required: "Movie Title field is required!",
            minlength: "Movie Title must be at least 5 characters long!",
            maxlength: "Movie Title must not exceed 150 characters!"
        },
        Content: {
            required: "Content field is required!",
            minlength: "Content must be at least 10 characters long!",
            maxlength: "Content must not exceed 150 characters!"
        }
    },
    errorElement: "span",
    errorClass: "text-danger",
});

$("#bookQuoteForm").validate({
    rules: {
        BookTitle: {
            required: true,
            minlength: 5,
            maxlength: 150
        },
        Content: {
            required: true,
            minlength: 10,
            maxlength: 150
        },
        AuthorName: {
            required: true,
            minlength: 5,
            maxlength: 50
        }
    },
    messages: {
        BookTitle: {
            required: "Book Title field is required!",
            minlength: "Book Title must be at least 5 characters long!",
            maxlength: "Book Title must not exceed 150 characters!"
        },
        Content: {
            required: "Content field is required!",
            minlength: "Content must be at least 10 characters long!",
            maxlength: "Content must not exceed 150 characters!"
        },
        AuthorName: {
            required: "Author Name field is required!",
            minlength: "Author Name must be at least 5 characters long!",
            maxlength: "Author Name must not exceed 50 characters!"
        },
    },
    errorElement: "span",
    errorClass: "text-danger",
});

$("#generalQuoteForm").validate({
    rules: {
        AuthorName: {
            required: true,
            minlength: 5,
            maxlength: 50
        },
        Content: {
            required: true,
            minlength: 10,
            maxlength: 150
        }
    },
    messages: {
        AuthorName: {
            required: "Author Name field is required!",
            minlength: "Author Name must be at least 5 characters long!",
            maxlength: "Author Name must not exceed 50 characters!"
        },
        Content: {
            required: "Content field is required!",
            minlength: "Content must be at least 10 characters long!",
            maxlength: "Content must not exceed 150 characters!"
        }
    },
    errorElement: "span",
    errorClass: "text-danger",
});