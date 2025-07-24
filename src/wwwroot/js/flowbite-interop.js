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