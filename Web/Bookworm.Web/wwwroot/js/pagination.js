//import { fetchForQuotes} from './quotes/fetchForQuotes.js';

export function updatePagination(model, fetcher) {
    const navigation = document.getElementById('pagination');
    navigation.children[0]?.remove();

    if (model.pagesCount > 1) {
        const ulElement = document.createElement('ul');
        ulElement.className = 'pagination pagination-lg justify-content-center';

        const previousLiEl = document.createElement('li');
        let previousLiElClass;
        if (!model.hasPreviousPage) {
            previousLiElClass = 'page-item disabled';
        } else {
            previousLiElClass = 'page-item';
            previousLiEl.style.cursor = 'pointer';
        }
        previousLiEl.className = previousLiElClass;
        const previousLiAnchorEl = document.createElement('a');
        previousLiAnchorEl.className = 'page-link';
        const previousIconEl = document.createElement('i');
        previousIconEl.className = 'fa fa-angles-left';
        previousLiAnchorEl.appendChild(previousIconEl);
        if (model.hasPreviousPage) {
            previousLiAnchorEl.addEventListener('click', () => fetcher(model.previousPageNumber));
        }
        previousLiEl.appendChild(previousLiAnchorEl);
        ulElement.appendChild(previousLiEl);

        for (var i = model.pageNumber - 4; i < model.pageNumber; i++) {
            if (i > 0) {
                let page = i;
                const liElement = document.createElement('li');
                liElement.className = 'page-item';
                liElement.style.cursor = 'pointer';

                const anchorEl = document.createElement('a');
                anchorEl.className = 'page-link';
                anchorEl.textContent = `${i}`;
                anchorEl.addEventListener('click', function () {
                    fetcher(page);
                });

                liElement.appendChild(anchorEl);
                ulElement.appendChild(liElement);
            }
        }

        const currPageLiEl = document.createElement('li');
        currPageLiEl.className = 'page-item active';
        currPageLiEl.setAttribute('aria-current', 'page');

        const currPageSpanEl = document.createElement('span');
        currPageSpanEl.className = 'page-link';
        currPageSpanEl.textContent = `${model.pageNumber}`;

        const currPageInnerSpanEl = document.createElement('span');
        currPageInnerSpanEl.className = 'sr-only';
        currPageInnerSpanEl.textContent = '(current)';

        currPageSpanEl.appendChild(currPageInnerSpanEl);
        currPageLiEl.appendChild(currPageSpanEl);
        ulElement.appendChild(currPageLiEl);

        for (var i = model.pageNumber + 1; i <= model.pageNumber + 4; i++) {
            if (i <= model.pagesCount) {
                let page = i;
                const liElement = document.createElement('li');
                liElement.className = 'page-item';
                liElement.style.cursor = 'pointer';

                const anchorElement = document.createElement('a');
                anchorElement.className = 'page-link';
                anchorElement.textContent = `${i}`;
                anchorElement.addEventListener('click', () => fetcher(page));

                liElement.appendChild(anchorElement);
                ulElement.appendChild(liElement);
            }
        }

        const nextPageLiEl = document.createElement('li');
        let nextPageLiClassName;
        if (model.hasNextPage == false) {
            nextPageLiClassName = 'page-item disabled';
        } else {
            nextPageLiClassName = 'page-item';
            nextPageLiEl.style.cursor = 'pointer';
        }
        nextPageLiEl.className = nextPageLiClassName;

        const nextPageAnchorEl = document.createElement('a');
        nextPageAnchorEl.className = 'page-link';
        const nextIconEl = document.createElement('i');
        nextIconEl.className = 'fa fa-angles-right';
        nextPageAnchorEl.appendChild(nextIconEl);
        if (model.hasNextPage) {
            nextPageAnchorEl.addEventListener('click', () => fetcher(model.nextPageNumber))
        }
        nextPageLiEl.appendChild(nextPageAnchorEl);
        ulElement.appendChild(nextPageLiEl);
        navigation.appendChild(ulElement);
    }
}