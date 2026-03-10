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

export function addFocusTrap(elementId) {
    const element = document.getElementById(elementId);
    if (!element) {
        return;
    }

    const focusableChildren = element.querySelectorAll(focusableSelectors.join(","));
    if (focusableChildren.length === 0) {
        return;
    }

    let first = focusableChildren[0];
    let last = focusableChildren[focusableChildren.length - 1];

    const focusTrapHandler = function (e) {
        if (e.key !== "Tab") {
            return;
        }

        if (e.shiftKey) {
            if (document.activeElement === first) {
                e.preventDefault();
                last.focus();
            }
        } else {
            if (document.activeElement === last) {
                e.preventDefault();
                first.focus();
            }
        }
    };

    element.addEventListener("keydown", focusTrapHandler);

    onElementRemovedFromDOM(element, () => {
        element.removeEventListener("keydown", focusTrapHandler);
        addFocusTrap(elementId);
    });
}

const focusableSelectors = [
    "a[href]",
    "button:not([disabled])",
    "textarea:not([disabled])",
    "input:not([disabled])",
    "select:not([disabled])",
    '[tabindex]:not([tabindex="-1"])'
];

function onElementRemovedFromDOM(element, action) {
    if (!element) {
        return;
    }

    let observer = new MutationObserver(() => {
        if (!document.body.contains(element)) {
            action?.();
            observer.disconnect();
        }
    });

    observer.observe(document.body, { childList: true, subtree: true });
}