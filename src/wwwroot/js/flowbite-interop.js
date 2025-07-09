window.flowbiteInterop = {
    initializeFlowbite: function () {
        return initFlowbite();
    }
};

window.drawerInterop = (function () {
    const drawers = {};

    return {
        init: function (id, options = {}, instanceOptions = {}) {
            const el = document.getElementById(id);
            if (!el) return;

            if (!drawers[id] || instanceOptions.override) {
                drawers[id] = new Drawer(el, options, instanceOptions);
            }
        },
        showDrawer: function (id) {
            drawers[id]?.show();
        },
        hideDrawer: function (id) {
            drawers[id]?.hide();
        },
        toggleDrawer: function (id) {
            drawers[id]?.toggle();
        },
        isDrawerVisible: function (id) {
            return drawers[id]?.isVisible() ?? false;
        }
    };
})();
