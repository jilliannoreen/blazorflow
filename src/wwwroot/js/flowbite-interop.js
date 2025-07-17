window.flowbiteBlazorInterop = window.flowbiteBlazorInterop || {};

// FLOWBITE INITIALIZER
window.flowbiteBlazorInterop.core = {
    initialize: function () {
        return initFlowbite();
    }
};

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
            drawers[id]?.show();
        },
        hide: function (id) {
            drawers[id]?.hide();
        },
        toggle: function (id) {
            drawers[id]?.toggle();
        },
        isVisible: function (id) {
            return drawers[id]?.isVisible() ?? false;
        }
    };
})();



// MODAL INTEROP WITH CALLBACK SUPPORT
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
                        console.log("On show.");
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
            modals[id]?.show();
        },
        hide: function (id) {
            modals[id]?.hide();
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

