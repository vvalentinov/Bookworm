import { updateComments } from './updateComments.js';

export function commentsPagination() {

    const spans = document.querySelectorAll('.page-link');

    const bookId = document
        .getElementById('pagination')
        .getAttribute('bookId');

    spans.forEach(span => {

        span.addEventListener('click', () => {

            const liElement = span.parentElement;

            var sortCriteria = document.querySelector('input[name="btnradio"]:checked').id;

            const isFirstLiEl = liElement.previousElementSibling == null;
            const isLastLiEl = liElement.nextElementSibling == null;
            const isNeitherFirstOrLastLi = !isFirstLiEl && !isLastLiEl;
            const lastLi = document.querySelector('#pagination li:last-child');
            const firstLi = document.querySelector('#pagination li:first-child');

            let page = Number(span.getAttribute('asp-route-page'));
            if (page == 0) {

                const activeSpan = document.querySelector('#pagination li.active span');

                if (isFirstLiEl) {
                    page = Number(activeSpan.getAttribute('asp-route-page')) - 1;
                }

                if (isLastLiEl) {
                    page = Number(activeSpan.getAttribute('asp-route-page')) + 1;
                }
            }

            fetch(`/ApiComment/GetSortedComments?criteria=${sortCriteria}&bookId=${bookId}&page=${page}`)
                .then(res => res.json())
                .then(res => {

                    updateComments(
                        res.comments,
                        res.isUserSignedIn,
                        res.isUserAdmin);

                    const hasNextPage = res.hasNextPage;
                    const hasPreviousPage = res.hasPreviousPage;

                    const commentsSection = document.querySelector('#comments-section');
                    commentsSection.scrollIntoView({ behavior: 'smooth' });

                    if (isNeitherFirstOrLastLi) {
                        const activeLi = document.querySelector('#pagination li.active');
                        activeLi.classList.remove('active');
                        liElement.classList.add('active');

                        if (!hasPreviousPage) {
                            if (!firstLi.classList.contains('disabled')) {
                                firstLi.classList.add('disabled');
                            }

                            removeTooltip(firstLi);
                        }

                        if (hasPreviousPage) {

                            if (firstLi.classList.contains('disabled')) {
                                firstLi.classList.remove('disabled');
                            }

                            addTooltip(firstLi);
                        }

                        if (!hasNextPage) {

                            if (!lastLi.classList.contains('disabled')) {
                                lastLi.classList.add('disabled');
                            }

                            removeTooltip(lastLi);
                        }

                        if (hasNextPage) {

                            if (lastLi.classList.contains('disabled')) {
                                lastLi.classList.remove('disabled');
                            }

                            addTooltip(lastLi);
                        }
                    }

                    if (isFirstLiEl) {

                        if (hasNextPage && lastLi.classList.contains('disabled')) {
                            lastLi.classList.remove('disabled');
                            addTooltip(lastLi);
                        }

                        if (!hasPreviousPage) {

                            if (!firstLi.classList.contains('disabled')) {
                                firstLi.classList.add('disabled');
                                removeTooltip(firstLi);
                            }

                            if (firstLi.nextElementSibling.classList.contains('active')) {
                                const spans = document.querySelectorAll('#pagination li:not(:first-child):not(:last-child) span');
                                spans.forEach(span => {
                                    span.textContent = Number(span.textContent) - 1;
                                    let currentRoutePage = Number(span.getAttribute('asp-route-page'));
                                    span.setAttribute('asp-route-page', currentRoutePage - 1);
                                });
                            } else {
                                const activeLi = document.querySelector('#pagination li.active');
                                const previousLi = activeLi.previousElementSibling;
                                activeLi.classList.remove('active');
                                previousLi.classList.add('active');
                            }
                        }

                        if (hasPreviousPage) {

                            addTooltip(firstLi);

                            if (!firstLi.nextElementSibling.classList.contains('active')) {
                                const activeLi = document.querySelector('#pagination li.active');
                                activeLi.classList.remove('active');
                                activeLi.previousElementSibling.classList.add('active');
                            } else {
                                const spans = document.querySelectorAll('#pagination li:not(:first-child):not(:last-child) span');
                                spans.forEach(span => {
                                    span.textContent = Number(span.textContent) - 1;
                                    let currentRoutePage = Number(span.getAttribute('asp-route-page'));
                                    span.setAttribute('asp-route-page', currentRoutePage - 1);
                                });
                            }
                        }
                    }

                    if (isLastLiEl) {

                        if (hasPreviousPage && firstLi.classList.contains('disabled')) {
                            firstLi.classList.remove('disabled');
                            addTooltip(firstLi);
                        }

                        if (!hasNextPage) {

                            if (!lastLi.classList.contains('disabled')) {
                                lastLi.classList.add('disabled');
                                removeTooltip(lastLi);
                            }

                            if (lastLi.previousElementSibling.classList.contains('active')) {
                                const spans = document.querySelectorAll('#pagination li:not(:first-child):not(:last-child) span');
                                spans.forEach(span => {
                                    span.textContent = Number(span.textContent) + 1;
                                    let currentRoutePage = Number(span.getAttribute('asp-route-page'));
                                    span.setAttribute('asp-route-page', currentRoutePage + 1);
                                });
                            } else {
                                const activeLi = document.querySelector('#pagination li.active');
                                const nextLi = activeLi.nextElementSibling;
                                activeLi.classList.remove('active');
                                nextLi.classList.add('active');
                            }
                        }

                        if (hasNextPage) {

                            addTooltip(lastLi);

                            if (!lastLi.previousElementSibling.classList.contains('active')) {
                                const activeLi = document.querySelector('#pagination li.active');
                                activeLi.classList.remove('active');
                                activeLi.nextElementSibling.classList.add('active');
                            } else {
                                const spans = document.querySelectorAll('#pagination li:not(:first-child):not(:last-child) span');
                                spans.forEach(span => {
                                    span.textContent = Number(span.textContent) + 1;
                                    let currentRoutePage = Number(span.getAttribute('asp-route-page'));
                                    span.setAttribute('asp-route-page', currentRoutePage + 1);
                                });
                            }
                        }
                    }

                }).catch(err => console.log(err));
        });
    });
}

function removeTooltip(element) {
    const tooltipInstance = bootstrap.Tooltip.getInstance(element);
    if (tooltipInstance) { tooltipInstance.disable(); }
}

function addTooltip(element) {
    const tooltipInstance = bootstrap.Tooltip.getOrCreateInstance(element);
    if (!tooltipInstance._isEnabled) { tooltipInstance.enable(); }
}