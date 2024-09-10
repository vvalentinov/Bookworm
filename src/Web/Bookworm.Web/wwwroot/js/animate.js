export function animateCSS(
    element,
    animation,
    speed,
    shouldBeRemoved = false,
    prefix = 'animate__') {

    new Promise((resolve, reject) => {

        const animationName = `${prefix}${animation}`;

        const animationSpeed = `${prefix}${speed}`;

        element.classList.add(
            `${prefix}animated`,
            animationName,
            animationSpeed);

        if (shouldBeRemoved) {
            function handleAnimationEnd(event) {
                event.stopPropagation();
                element.classList.remove(`${prefix}animated`, animationName, animationSpeed);
                resolve('Animation ended');
            }

            element.addEventListener(
                'animationend',
                handleAnimationEnd,
                { once: true });
        }
    });
}

export function animateCssRemoveAnimation(
    element,
    animationName,
    animationSpeed) {

    const prefix = 'animate__';

    const animation = `${prefix}${animationName}`;
    const speed = `${prefix}${animationSpeed}`;

    element.classList.remove(
        `${prefix}animated`,
        animation,
        speed);
}