window.flowbiteBlazorInterop = window.flowbiteBlazorInterop || {};

// FLOWBITE INITIALIZER
window.flowbiteBlazorInterop.core = {
    initialize: function () {
        return initFlowbite();
    }
};

// FOCUS TRAP UTILITY
const FocusTrapManager = (() => {
    let lastFocusedElement = null;
    let activeTrapElement = null;

    function getFocusableElements(container) {
        return Array.from(container.querySelectorAll(
            'a[href], area[href], input:not([disabled]), select:not([disabled]), ' +
            'textarea:not([disabled]), button:not([disabled]), iframe, object, embed, ' +
            '[tabindex]:not([tabindex="-1"]), [contenteditable]'
        )).filter(el => el.offsetParent !== null);
    }

    function trapFocus(container) {
        const focusableEls = getFocusableElements(container);
        if (focusableEls.length === 0) return;

        const firstEl = focusableEls[0];
        const lastEl = focusableEls[focusableEls.length - 1];

        function handleKeyDown(e) {
            if (e.key === "Tab") {
                if (e.shiftKey) {
                    // Shift + Tab
                    if (document.activeElement === firstEl) {
                        e.preventDefault();
                        lastEl.focus();
                    }
                } else {
                    // Tab
                    if (document.activeElement === lastEl) {
                        e.preventDefault();
                        firstEl.focus();
                    }
                }
            } else if (e.key === "Escape") {
                // Optional: Hide drawer/modal on Escape
                if (activeTrapElement?.id) {
                    window.flowbiteBlazorInterop.drawer?.hide(activeTrapElement.id);
                    window.flowbiteBlazorInterop.dialog?.hide(activeTrapElement.id);
                }
            }
        }

        activeTrapElement = container;
        lastFocusedElement = document.activeElement;

        document.addEventListener("keydown", handleKeyDown);
        firstEl.focus();

        // Store handler for removal
        container._focusTrapCleanup = () => {
            document.removeEventListener("keydown", handleKeyDown);
            if (lastFocusedElement) lastFocusedElement.focus();
            activeTrapElement = null;
        };
    }

    function releaseFocus(container) {
        container._focusTrapCleanup?.();
    }

    return {
        trap: trapFocus,
        release: releaseFocus
    };
})();


// DRAWER INTEROP
window.flowbiteBlazorInterop.drawer = (function () {
    const drawers = {};

    return {
        init: function (id, options = {}, instanceOptions = {}) {
            const el = document.getElementById(id);
            if (!el) {
                console.warn(`[DrawerInterop] Element with ID '${id}' not found.`);
                return;
            }

            if (!drawers[id] || instanceOptions.override) {
                drawers[id] = new Drawer(el, options, instanceOptions);
            }
        },
        show: function (id) {
            const drawer = drawers[id];
            const el = document.getElementById(id);
            if (drawer && el) {
                drawers[id]?.show();
                FocusTrapManager.trap(el);
            }
        },
        hide: function (id) {
            const drawer = drawers[id];
            const el = document.getElementById(id);
            if (drawer && el) {
                drawers[id]?.hide();
                FocusTrapManager.release(el);
            }
        },
        toggle: function (id) {
            drawers[id]?.toggle();
        },
        isVisible: function (id) {
            return drawers[id]?.isVisible() ?? false;
        }
    };
})();


// MODAL INTEROP
window.flowbiteBlazorInterop.dialog = (function () {
    const modals = {};

    return {
        init: function (id, options = {}, dotNetHelper = null, instanceOptions = {}) {
            const el = document.getElementById(id);
            if (!el) {
                console.warn(`[ModalInterop] Element with ID '${id}' not found.`);
                return;
            }

            if (!modals[id] || instanceOptions.override) {
                const modal = new Modal(el, {
                    ...options,
                    onShow: () => {
                        if (dotNetHelper) dotNetHelper.invokeMethodAsync("OnShow");
                    },
                    onHide: () => {
                        if (dotNetHelper) dotNetHelper.invokeMethodAsync("OnHide");
                    },
                    onToggle: () => {
                        if (dotNetHelper) dotNetHelper.invokeMethodAsync("OnToggle");
                    }
                });

                modals[id] = modal;
            }
        },
        show: function (id) {
            const modal = modals[id];
            const el = document.getElementById(id);
            if (modal && el) {
                modal.show();
                FocusTrapManager.trap(el);
            }
        },
        hide: function (id) {
            const modal = modals[id];
            const el = document.getElementById(id);
            if (modal && el) {
                modals[id]?.hide();
                FocusTrapManager.release(el);
            }
        },
        toggle: function (id) {
            modals[id]?.toggle();
        },
        isVisible: function (id) {
            return modals[id]?.isVisible() ?? false;
        },
        isHidden: function (id) {
            return modals[id]?.isHidden() ?? true;
        }
    };
})();

// INPUT DEBOUNCE INTEROP (New section)
window.flowbiteBlazorInterop.input = {
    /**
     * Sets up a debounced 'input' event listener on a given HTML element.
     * @param {DotNetObjectReference} dotNetHelper - The .NET object reference to invoke methods on.
     * @param {HTMLElement} inputElement - The HTML input element to attach the listener to.
     * @param {string} methodName - The name of the .NET method to invoke.
     * @param {number} delay - The debounce delay in milliseconds.
     */
    setupInputDebounce: function (dotNetHelper, inputElement, methodName, delay) {
        let timeout;

        if (!inputElement) {
            console.error("[InputDebounceInterop] Input element not found for debounce setup.");
            return;
        }

        // Remove any existing oninput listener to prevent duplicates if called multiple times
        // This is important if component is re-rendered without full disposal.
        inputElement.oninput = null;

        inputElement.oninput = (event) => {
            clearTimeout(timeout);
            timeout = setTimeout(() => {
                dotNetHelper.invokeMethodAsync(methodName, event.target.value);
            }, delay);
        };

        inputElement._debounceCleanup = () => {
            inputElement.oninput = null;
            // No need to dispose dotNetHelper here, as Blazor's Dispose() handles that.
        };
    },

    /**
     * Cleans up the debounced input event listener from an element.
     * @param {HTMLElement} inputElement - The HTML input element to clean up.
     */
    cleanupInputDebounce: function (inputElement) {
        if (inputElement && typeof inputElement._debounceCleanup === 'function') {
            inputElement._debounceCleanup();
            delete inputElement._debounceCleanup; // Remove the property
        }
    }
};

