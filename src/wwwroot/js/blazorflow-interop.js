// Create global interop namespace (avoid overwriting existing object)
window.blazorFlowInterop = window.blazorFlowInterop || {};

/* ======================================================================
   Focus Trap Utility
   - Ensures keyboard focus stays within a container (e.g., modal, drawer)
   - Restores previous focus when trap is released
   ====================================================================== */
const FocusTrapManager = (() => {
    let lastFocusedElement = null;
    let activeTrapElement = null;

    /**
     * Returns all focusable elements inside a container.
     * Filters out hidden or disabled elements.
     * @param {HTMLElement} container
     * @returns {HTMLElement[]}
     */
    function getFocusableElements(container) {
        return Array.from(container.querySelectorAll(
            'a[href], area[href], input:not([disabled]), select:not([disabled]), ' +
            'textarea:not([disabled]), button:not([disabled]), iframe, object, embed, ' +
            '[tabindex]:not([tabindex="-1"]), [contenteditable]'
        )).filter(el => el.offsetParent !== null);
    }

    /**
     * Traps focus inside a given container.
     * Automatically cycles focus when using Tab or Shift+Tab.
     * Esc key can be used to close modals/drawers (if implemented).
     * @param {HTMLElement} container
     */
    function trapFocus(container) {
        const focusableEls = getFocusableElements(container);
        if (focusableEls.length === 0) return;

        const firstEl = focusableEls[0];
        const lastEl = focusableEls[focusableEls.length - 1];

        function handleKeyDown(e) {
            if (e.key === "Tab") {
                // Handle tab navigation inside trap
                if (e.shiftKey) {
                    if (document.activeElement === firstEl) {
                        e.preventDefault();
                        lastEl.focus();
                    }
                } else {
                    if (document.activeElement === lastEl) {
                        e.preventDefault();
                        firstEl.focus();
                    }
                }
            } else if (e.key === "Escape") {
                // Optional: Hide drawer/modal on Escape
                if (activeTrapElement?.id) {
                    window.blazorFlowInterop.drawer?.hide(activeTrapElement.id);
                    window.blazorFlowInterop.dialog?.hide(activeTrapElement.id);
                }
            }
        }

        activeTrapElement = container;
        lastFocusedElement = document.activeElement;

        document.addEventListener("keydown", handleKeyDown);
        firstEl.focus();

        // Store cleanup handler for later use
        container._focusTrapCleanup = () => {
            document.removeEventListener("keydown", handleKeyDown);
            if (lastFocusedElement) lastFocusedElement.focus();
            activeTrapElement = null;
        };
    }

    /**
     * Releases the focus trap for a given container.
     * Restores the previously focused element.
     * @param {HTMLElement} container
     */
    function releaseFocus(container) {
        container._focusTrapCleanup?.();
    }

    return {
        trap: trapFocus,
        release: releaseFocus
    };
})();

/* ======================================================================
   Drawer Interop
   - Provides methods to control custom drawer components:
     show, hide, toggle, and check visibility
   ====================================================================== */
window.blazorFlowInterop.drawer = (function () {
    const drawers = {};

    class CustomDrawer {
        constructor(element, options = {}) {
            this.el = element;
            this.options = Object.assign({
                placement: 'left',        // Options: left, right, top, bottom
                backdrop: true,
                bodyScrolling: false,
                edge: false,
                edgeOffset: 'bottom-[60px]',
                backdropClasses: 'bg-gray-900/50 dark:bg-gray-900/80 fixed inset-0 z-30',
                onShow: () => { },
                onHide: () => { },
                onToggle: () => { }
            }, options);
            this.visible = false;
            this._init();
        }

        /** Initialize drawer with ARIA attributes and key handlers */
        _init() {
            this.el.setAttribute('aria-hidden', 'true');
            this.el.classList.add('transition-transform');
            this._applyBaseClasses();

            this._handleEscapeKey = (e) => {
                if (e.key === 'Escape' && this.isVisible()) {
                    this.hide();
                }
            };
            document.addEventListener('keydown', this._handleEscapeKey);
        }

        /** Apply placement-based base CSS classes */
        _applyBaseClasses() {
            const classes = this._getPlacementClasses(this.options.placement).base;
            classes.forEach(cls => this.el.classList.add(cls));
        }

        /** Returns CSS classes based on drawer placement */
        _getPlacementClasses(placement) {
            switch (placement) {
                case 'top':
                    return { base: ['top-0', 'left-0', 'right-0'], active: ['transform-none'], inactive: ['-translate-y-full'] };
                case 'right':
                    return { base: ['right-0', 'top-0'], active: ['transform-none'], inactive: ['translate-x-full'] };
                case 'bottom':
                    return { base: ['bottom-0', 'left-0', 'right-0'], active: ['transform-none'], inactive: ['translate-y-full'] };
                case 'left':
                default:
                    return { base: ['left-0', 'top-0'], active: ['transform-none'], inactive: ['-translate-x-full'] };
            }
        }

        /** Show drawer with backdrop and ARIA updates */
        show() {
            const cls = this._getPlacementClasses(this.options.placement);
            cls.active.forEach(c => this.el.classList.add(c));
            cls.inactive.forEach(c => this.el.classList.remove(c));

            this.el.setAttribute('aria-modal', 'true');
            this.el.setAttribute('role', 'dialog');
            this.el.removeAttribute('aria-hidden');

            if (!this.options.bodyScrolling) {
                document.body.classList.add('overflow-hidden');
            }

            if (this.options.backdrop) {
                this._createBackdrop();
            }

            this.visible = true;
            this.options.onShow(this);
        }

        /** Hide drawer and remove backdrop */
        hide() {
            const cls = this._getPlacementClasses(this.options.placement);
            cls.active.forEach(c => this.el.classList.remove(c));
            cls.inactive.forEach(c => this.el.classList.add(c));

            this.el.setAttribute('aria-hidden', 'true');
            this.el.removeAttribute('aria-modal');
            this.el.removeAttribute('role');

            if (!this.options.bodyScrolling) {
                document.body.classList.remove('overflow-hidden');
            }

            this._destroyBackdrop();
            this.visible = false;
            this.options.onHide(this);
        }

        toggle() {
            this.isVisible() ? this.hide() : this.show();
            this.options.onToggle(this);
        }

        /** Create backdrop behind drawer */
        _createBackdrop() {
            if (!document.querySelector('[drawer-backdrop]')) {
                const backdrop = document.createElement('div');
                backdrop.setAttribute('drawer-backdrop', '');
                backdrop.className = this.options.backdropClasses;
                document.body.appendChild(backdrop);

                backdrop.addEventListener('click', () => this.hide());
            }
        }

        /** Remove backdrop element */
        _destroyBackdrop() {
            const backdrop = document.querySelector('[drawer-backdrop]');
            if (backdrop) {
                backdrop.remove();
            }
        }

        isVisible() {
            return this.visible;
        }
    }

    return {
        /** Initialize a drawer by ID */
        init: function (id, options = {}, instanceOptions = {}) {
            const el = document.getElementById(id);
            if (!el) {
                console.warn(`[DrawerInterop] Element with ID '${id}' not found.`);
                return;
            }

            if (!drawers[id] || instanceOptions.override) {
                drawers[id] = new CustomDrawer(el, options);
            }
        },
        show: function (id) { drawers[id]?.show(); },
        hide: function (id) { drawers[id]?.hide(); },
        toggle: function (id) { drawers[id]?.toggle(); },
        isVisible: function (id) { return drawers[id]?.isVisible() ?? false; },
        hideAll: function () {
            Object.keys(drawers).forEach(id => drawers[id]?.hide());
        }
    };
})();

/* ======================================================================
   Modal Interop
   - Provides methods to control modals (show, hide, toggle)
   - Handles dynamic backdrop creation and z-index stacking
   ====================================================================== */
window.blazorFlowInterop.dialog = (function () {
    const modals = {};
    let zIndexBase = 50;

    const defaultOptions = {
        placement: "center",
        backdropClasses: "fixed inset-0 bg-gray-900/50 transition-opacity duration-100 opacity-0",
        backdrop: "dynamic",
        closable: true
    };

    /** Wait for CSS transition to end */
    function waitForTransitionEnd(element, timeout = 300) {
        return new Promise((resolve) => {
            let resolved = false;

            const handler = () => {
                if (resolved) return;
                resolved = true;
                element.removeEventListener("transitionend", handler);
                resolve();
            };

            element.addEventListener("transitionend", handler, { once: true });
            setTimeout(handler, timeout + 50); // Fallback safety
        });
    }

    /** Returns placement-specific flex classes for modal container */
    function getPlacementClasses(placement) {
        switch (placement) {
            case "top-left": return ["justify-start", "items-start"];
            case "top-center": return ["justify-center", "items-start"];
            case "top-right": return ["justify-end", "items-start"];
            case "center-left": return ["justify-start", "items-center"];
            case "center": return ["justify-center", "items-center"];
            case "center-right": return ["justify-end", "items-center"];
            case "bottom-left": return ["justify-start", "items-end"];
            case "bottom-center": return ["justify-center", "items-end"];
            case "bottom-right": return ["justify-end", "items-end"];
            default: return ["justify-center", "items-center"];
        }
    }

    /** Create backdrop element */
    function createBackdrop(modalId, classes, zIndex) {
        const backdrop = document.createElement("div");
        backdrop.className = classes;
        backdrop.style.zIndex = zIndex;
        backdrop.dataset.modalBackdrop = modalId;
        document.body.appendChild(backdrop);

        // Animate fade-in
        requestAnimationFrame(() => backdrop.classList.add("opacity-100"));
        return backdrop;
    }

    /** Remove backdrop element */
    async function removeBackdrop(modalId) {
        const backdrop = document.querySelector(`[data-modal-backdrop="${modalId}"]`);
        if (backdrop) {
            backdrop.classList.remove("opacity-100"); // fade out
            await waitForTransitionEnd(backdrop, 100);
            backdrop.remove();
        }
    }

    return {
        init: function (id, options = {}, dotNetHelper = null, instanceOptions = {}) {
            const el = document.getElementById(id);
            if (!el) {
                console.warn(`[ModalInterop] Element with ID '${id}' not found.`);
                return;
            }

            const modalOptions = { ...defaultOptions, ...options };

            if (!modals[id] || instanceOptions.override) {
                const modal = {
                    el,
                    options: modalOptions,
                    isHidden: true,
                    backdropEl: null,

                    /** Show modal with fade-in */
                    async show() {
                        if (!this.isHidden) return;

                        const currentZ = zIndexBase + Object.keys(modals).length * 10;

                        // Apply placement classes
                        const placementClasses = getPlacementClasses(this.options.placement);
                        placementClasses.forEach(cls => this.el.classList.add(cls));

                        // Initial state
                        this.el.classList.remove("hidden");
                        this.el.classList.add("flex", "opacity-0");
                        this.el.style.zIndex = currentZ + 1;
                        this.el.setAttribute("aria-modal", "true");
                        this.el.setAttribute("role", "dialog");
                        this.el.removeAttribute("aria-hidden");

                        // Backdrop
                        this.backdropEl = createBackdrop(id, this.options.backdropClasses, currentZ);
                        if (this.options.backdrop === "dynamic" && this.options.closable) {
                            this.backdropEl.addEventListener("click", () => this.hide());
                        }

                        // Disable scroll for first modal
                        if (Object.values(modals).filter(m => !m.isHidden).length === 0) {
                            document.body.classList.add("overflow-hidden");
                        }

                        // Animate modal fade-in
                        requestAnimationFrame(() => {
                            this.el.classList.add("opacity-100");
                            this.el.classList.remove("opacity-0");
                        });

                        await waitForTransitionEnd(this.el, 100);
                        this.isHidden = false;

                        if (dotNetHelper) dotNetHelper.invokeMethodAsync("OnShow");
                    },

                    /** Hide modal with fade-out */
                    async hide() {
                        if (this.isHidden) return;

                        this.el.classList.remove("opacity-100");
                        this.el.classList.add("opacity-0");

                        await waitForTransitionEnd(this.el, 100);

                        this.el.classList.add("hidden");
                        this.el.classList.remove("flex");
                        this.el.setAttribute("aria-hidden", "true");
                        this.el.removeAttribute("aria-modal");
                        this.el.removeAttribute("role");

                        await removeBackdrop(id);

                        if (Object.values(modals).filter(m => !m.isHidden).length === 1) {
                            document.body.classList.remove("overflow-hidden");
                        }

                        this.isHidden = true;
                        if (dotNetHelper) dotNetHelper.invokeMethodAsync("OnHide");
                    },

                    toggle() {
                        this.isHidden ? this.show() : this.hide();
                        if (dotNetHelper) dotNetHelper.invokeMethodAsync("OnToggle");
                    },

                    isVisible() { return !this.isHidden; }
                };

                modals[id] = modal;
            }
        },
        show: function (id) { modals[id]?.show(); },
        hide: function (id) { modals[id]?.hide(); },
        toggle: function (id) { modals[id]?.toggle(); },
        isVisible: function (id) { return modals[id]?.isVisible() ?? false; },
        isHidden: function (id) { return modals[id]?.isHidden ?? true; }
    };
})();

/* ======================================================================
   Input Debounce Interop
   - Adds debounced input handling to reduce frequent .NET calls
   ====================================================================== */
window.blazorFlowInterop.input = {
    /**
     * Sets up a debounced 'input' event listener on a given HTML element.
     * @param {DotNetObjectReference} dotNetHelper - .NET object reference for callbacks
     * @param {HTMLElement} inputElement - Input element to monitor
     * @param {string} methodName - .NET method to call
     * @param {number} delay - Debounce delay in milliseconds
     */
    setupInputDebounce: function (dotNetHelper, inputElement, methodName, delay) {
        let timeout;

        if (!inputElement) {
            console.error("[InputDebounceInterop] Input element not found for debounce setup.");
            return;
        }

        inputElement.oninput = null;

        inputElement.oninput = (event) => {
            clearTimeout(timeout);
            timeout = setTimeout(() => {
                dotNetHelper.invokeMethodAsync(methodName, event.target.value);
            }, delay);
        };

        inputElement._debounceCleanup = () => {
            inputElement.oninput = null;
        };
    },

    /** Removes the debounced input event listener */
    cleanupInputDebounce: function (inputElement) {
        if (inputElement && typeof inputElement._debounceCleanup === 'function') {
            inputElement._debounceCleanup();
            delete inputElement._debounceCleanup;
        }
    }
};

/* ======================================================================
   Table Utility
   - Calculates available rows based on container height
   ====================================================================== */
window.blazorFlowInterop.table = {
    /**
     * Calculates how many table rows can fit in a container based on estimated row height.
     * @param {HTMLElement} container - The element containing the table
     * @param {number} rowHeight - Estimated height of a row in pixels (default: 40)
     * @returns {number} - Number of rows that can fit
     */
    getAvailableRowCount: function (container, rowHeight = 40) {
        if (!container) {
            console.warn("[TableInterop] Container not found.");
            return 1;
        }

        const containerHeight = container.clientHeight;
        const availableRows = Math.floor((containerHeight - 88) / rowHeight);

        return availableRows > 1 ? availableRows : 1;
    }
};



