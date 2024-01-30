//To Top Button JS
const toTop = document.querySelector('.topArrow');
toTop.addEventListener('click', () => window.scrollTo(0, 0));
window.addEventListener("scroll", () => toTop.style.display = window.scrollY > 500 ? 'block' : 'none');