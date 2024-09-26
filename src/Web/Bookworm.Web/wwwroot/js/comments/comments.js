import { updateComments } from './updateComments.js';
import { commentsPagination } from './commentsPagination.js';

function onDeleteCommentBtnClick(e) {
    const modelId = e.getAttribute('data-model-id');
    const commentIdInput = document.querySelector("input[name='deleteCommentId']");
    commentIdInput.value = modelId;
}

function onEditCommentBtnClick(e) {
    const article = e.parentElement.parentElement.parentElement.parentElement;
    tinymce.get("editCommentTextarea").setContent(article.children[1].innerHTML);

    const modelId = e.getAttribute('data-model-id');
    const commentIdInput = document.querySelector("input[name='editCommentId']");
    commentIdInput.value = modelId;
};

function getSortedComments(criteria, bookId, page) {
    fetch(`/ApiComment/GetSortedComments?criteria=${criteria}&bookId=${bookId}&page=${page}`)
        .then(res => res.json())
        .then(res => updateComments(res.comments, res.isUserSignedIn, res.isUserAdmin))
        .catch(err => console.log(err));
}

commentsPagination();

//const spans = document.querySelectorAll('.page-link');
//const paginationNav = document.getElementById('pagination');
//const bookId = paginationNav.getAttribute('bookId');

//spans.forEach(span => {

//    span.addEventListener('click', () => {

//        const parent = span.parentElement;
//        var sortCriteria = document.querySelector('input[name="btnradio"]:checked').id;
//        const isFirstLiEl = parent.previousElementSibling == null;
//        const isLastLiEl = parent.nextElementSibling == null;
//        const isNeitherFirstOrLastLi = !isFirstLiEl && !isLastLiEl;
//        const lastLi = document.querySelector('#pagination li:last-child');
//        const firstLi = document.querySelector('#pagination li:first-child');
//        let page = Number(span.getAttribute('asp-route-page'));
//        if (page == 0) {
//            const activeSpan = document.querySelector('#pagination li.active span');
//            if (isFirstLiEl) {
//                page = Number(activeSpan.getAttribute('asp-route-page')) - 1;
//            }
//            if (isLastLiEl) {
//                page = Number(activeSpan.getAttribute('asp-route-page')) + 1;
//            }
//        }

//        fetch(`/ApiComment/GetSortedComments?criteria=${sortCriteria}&bookId=${bookId}&page=${page}`)
//            .then(res => res.json())
//            .then(res => {

//                updateComments(res.comments, res.isUserSignedIn, res.isUserAdmin);

//                const hasNextPage = res.hasNextPage;
//                const hasPreviousPage = res.hasPreviousPage;

//                const commentsSection = document.querySelector('#comments-section');
//                commentsSection.scrollIntoView({ behavior: 'smooth' });

//                if (isNeitherFirstOrLastLi) {
//                    const activeLi = document.querySelector('#pagination li.active');
//                    activeLi.classList.remove('active');
//                    parent.classList.add('active');

//                    if (!hasPreviousPage) {
//                        if (!firstLi.classList.contains('disabled')) {
//                            firstLi.classList.add('disabled');
//                        }

//                        // remove tooltip
//                        const tooltipInstance = bootstrap.Tooltip.getInstance(firstLi);
//                        if (tooltipInstance) { tooltipInstance.disable(); }
//                    }

//                    if (hasPreviousPage) {

//                        if (firstLi.classList.contains('disabled')) {
//                            firstLi.classList.remove('disabled');
//                        }

//                        // add tooltip
//                        const tooltipInstance = bootstrap.Tooltip.getOrCreateInstance(firstLi);
//                        if (!tooltipInstance._isEnabled) { tooltipInstance.enable(); }
//                    }

//                    if (!hasNextPage) {

//                        if (!lastLi.classList.contains('disabled')) {
//                            lastLi.classList.add('disabled');
//                        }

//                        // remove tooltip
//                        const tooltipInstance = bootstrap.Tooltip.getInstance(lastLi);
//                        if (tooltipInstance) { tooltipInstance.disable(); }
//                    }

//                    if (hasNextPage) {

//                        if (lastLi.classList.contains('disabled')) {
//                            lastLi.classList.remove('disabled');
//                        }

//                        // add tooltip
//                        const tooltipInstance = bootstrap.Tooltip.getOrCreateInstance(lastLi);
//                        if (!tooltipInstance._isEnabled) { tooltipInstance.enable(); }
//                    }
//                }

//                if (isFirstLiEl) {

//                    if (hasNextPage && lastLi.classList.contains('disabled')) {
//                        lastLi.classList.remove('disabled');

//                        //add tooltip
//                        const tooltipInstance = bootstrap.Tooltip.getOrCreateInstance(lastLi);
//                        if (!tooltipInstance._isEnabled) { tooltipInstance.enable(); }
//                    }

//                    if (!hasPreviousPage) {

//                        if (!firstLi.classList.contains('disabled')) {
//                            firstLi.classList.add('disabled');

//                            // remove tooltip
//                            const tooltipInstance = bootstrap.Tooltip.getInstance(firstLi);
//                            if (tooltipInstance) { tooltipInstance.disable(); }
//                        }

//                        if (firstLi.nextElementSibling.classList.contains('active')) {
//                            const spans = document.querySelectorAll('#pagination li:not(:first-child):not(:last-child) span');
//                            spans.forEach(span => {
//                                span.textContent = Number(span.textContent) - 1;
//                                let currentRoutePage = Number(span.getAttribute('asp-route-page'));
//                                span.setAttribute('asp-route-page', currentRoutePage - 1);
//                            });
//                        } else {
//                            const activeLi = document.querySelector('#pagination li.active');
//                            const previousLi = activeLi.previousElementSibling;
//                            activeLi.classList.remove('active');
//                            previousLi.classList.add('active');
//                        }
//                    }

//                    if (hasPreviousPage) {

//                        //add tooltip
//                        const tooltipInstance = bootstrap.Tooltip.getOrCreateInstance(firstLi);
//                        if (!tooltipInstance._isEnabled) { tooltipInstance.enable(); }

//                        if (!firstLi.nextElementSibling.classList.contains('active')) {
//                            const activeLi = document.querySelector('#pagination li.active');
//                            activeLi.classList.remove('active');
//                            activeLi.previousElementSibling.classList.add('active');
//                        } else {
//                            const spans = document.querySelectorAll('#pagination li:not(:first-child):not(:last-child) span');
//                            spans.forEach(span => {
//                                span.textContent = Number(span.textContent) - 1;
//                                let currentRoutePage = Number(span.getAttribute('asp-route-page'));
//                                span.setAttribute('asp-route-page', currentRoutePage - 1);
//                            });
//                        }
//                    }
//                }

//                if (isLastLiEl) {

//                    if (hasPreviousPage && firstLi.classList.contains('disabled')) {
//                        firstLi.classList.remove('disabled');

//                        //add tooltip
//                        const tooltipInstance = bootstrap.Tooltip.getOrCreateInstance(firstLi);
//                        if (!tooltipInstance._isEnabled) { tooltipInstance.enable(); }
//                    }

//                    if (!hasNextPage) {

//                        if (!lastLi.classList.contains('disabled')) {
//                            lastLi.classList.add('disabled');

//                            // remove tooltip
//                            const tooltipInstance = bootstrap.Tooltip.getInstance(lastLi);
//                            if (tooltipInstance) { tooltipInstance.disable(); }
//                        }

//                        if (lastLi.previousElementSibling.classList.contains('active')) {
//                            const spans = document.querySelectorAll('#pagination li:not(:first-child):not(:last-child) span');
//                            spans.forEach(span => {
//                                span.textContent = Number(span.textContent) + 1;
//                                let currentRoutePage = Number(span.getAttribute('asp-route-page'));
//                                span.setAttribute('asp-route-page', currentRoutePage + 1);
//                            });
//                        } else {
//                            const activeLi = document.querySelector('#pagination li.active');
//                            const nextLi = activeLi.nextElementSibling;
//                            activeLi.classList.remove('active');
//                            nextLi.classList.add('active');
//                        }
//                    }

//                    if (hasNextPage) {

//                        //add tooltip
//                        const tooltipInstance = bootstrap.Tooltip.getOrCreateInstance(lastLi);
//                        if (!tooltipInstance._isEnabled) { tooltipInstance.enable(); }

//                        if (!lastLi.previousElementSibling.classList.contains('active')) {
//                            const activeLi = document.querySelector('#pagination li.active');
//                            activeLi.classList.remove('active');
//                            activeLi.nextElementSibling.classList.add('active');
//                        } else {
//                            const spans = document.querySelectorAll('#pagination li:not(:first-child):not(:last-child) span');
//                            spans.forEach(span => {
//                                span.textContent = Number(span.textContent) + 1;
//                                let currentRoutePage = Number(span.getAttribute('asp-route-page'));
//                                span.setAttribute('asp-route-page', currentRoutePage + 1);
//                            });
//                        }
//                    }
//                }

//            }).catch(err => console.log(err));
//    });
//});