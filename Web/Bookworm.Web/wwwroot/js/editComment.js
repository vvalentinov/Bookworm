function editCommentFunc(e) {
    const article = e.parentElement.parentElement.parentElement;
    const commentContentEl = article.children[1].children[0];
    tinymce.activeEditor.setContent(commentContentEl.textContent);
};