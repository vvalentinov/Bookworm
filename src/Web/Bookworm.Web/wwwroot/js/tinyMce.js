tinymce.init({
    selector: "textarea.bookComment",
    placeholder: 'Type your comment here...',
    plugins: "code wordcount lists preview searchreplace",
    content_style: 'body { font-size: 14pt;}',
});

tinymce.init({
    selector: "textarea.bookDescription",
    placeholder: 'Type book description here...',
    plugins: "code wordcount lists preview searchreplace",
    content_style: 'body { font-size: 14pt;}',
});