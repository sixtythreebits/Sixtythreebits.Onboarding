const model = {    
    tree: null,
    urlUpdate: null,

    onTreeInit: function (e) {
        model.tree = e.component;
        globals.devexpress.setGridFullHeight(e.component);
    }
};

$(function () {
    $(globals.selectors.buttonAddNew).click(function () {
        model.tree.addRow();
    });
});