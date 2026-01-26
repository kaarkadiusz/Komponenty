export function scrollToElement(elementId) {
    let element = document.getElementById(elementId);
    if (!element) {
        return;
    }

    element.scrollIntoView({
        behavior: 'smooth',
        block: 'start'
    });
}

export function getTextAreaLineCount(textAreaId, textAreaFakeId) {
    let element = document.getElementById(textAreaId);
    let fakeElement = document.getElementById(textAreaFakeId);

    if (!element || !fakeElement) {
        return -1;
    }

    const style = window.getComputedStyle(element);
    fakeElement.style.width = style.width;
    fakeElement.style.font = style.font;
    fakeElement.style.padding = style.padding;
    fakeElement.style.lineHeight = style.lineHeight;
    fakeElement.style.border = style.border;
    fakeElement.style.boxSizing = style.boxSizing;

    fakeElement.textContent = element.value + '\u200b';

    const lineHeight = parseFloat(style.lineHeight) || parseFloat(style.fontSize) * 1.2;
    const ghostHeight = fakeElement.getBoundingClientRect().height 
        - parseFloat(style.paddingTop)
        - parseFloat(style.paddingBottom);

    return Math.max(1, Math.round(ghostHeight / lineHeight));
}