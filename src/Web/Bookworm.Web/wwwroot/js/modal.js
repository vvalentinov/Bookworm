function onModalBtnClick(value, inputClassName) {
    const inputElement = document.querySelector(`.${inputClassName}`);
    inputElement.value = value;
}