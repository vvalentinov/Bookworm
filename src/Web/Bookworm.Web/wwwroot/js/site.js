import { animateCSS, animateCssRemoveAnimation } from './animate.js';

const toTop = document.querySelector('.topArrow');

toTop.addEventListener('click', () => window.scrollTo(0, 0));

toTop.addEventListener('animationend', (e) => {
    const button = e.currentTarget;

    if (button.classList.contains('animate__zoomOut')) {

        animateCssRemoveAnimation(toTop, 'zoomOut', 'faster');

        button.style.display = 'none';
    }
});

window.addEventListener("scroll", () => {
    if (window.scrollY > 500) {

        if (toTop.classList.contains('animate__zoomOut')) {
            animateCssRemoveAnimation(toTop, 'zoomOut', 'faster');
        }

        animateCSS(toTop, 'zoomIn', 'faster');

        toTop.style.display = 'block';

    } else {

        if (toTop.classList.contains('animate__zoomIn')) {
            animateCssRemoveAnimation(toTop, 'zoomIn', 'faster');
        }

        animateCSS(toTop, 'zoomOut', 'faster');
    }
});