const model = {    
    grid: null,
    tree: null,
    urlPermissionsGetByRole: null,    
    roleIDRocused: null,    
    isTreeContentReady: false,

    initModelProperties: function () {
        model.urlPermissionsGetByRole = $('.js-urlPermissionsGetByRole').val();
    },

    onGridInit: function (e) {
        model.grid = e.component;
        globals.devexpress.setGridFullHeight(e.component);
    },
    onGridFocusedRowChanged: function (e) {

        if (!model.isTreeContentReady) {
            setTimeout(function () {
                model.onGridFocusedRowChanged(e);
            }, 1000);

            return;
        }

        const roleID = model.roleIDRocused = e.row.key;
        $.ajax({
            type: 'GET',
            url: model.urlPermissionsGetByRole,
            data: { RoleID: roleID },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    model.tree.selectRows(res.Data);
                }
            },
            complete: function () {
                preloader.hide();
            }
        });
    },
    onTreeInit: function (e) {
        model.tree = e.component;
        globals.devexpress.setGridFullHeight(e.component);
    },
    onTreeContentReady: function (e) {
        model.isTreeContentReady = true;
    }
};

$(function () {

    model.initModelProperties();

    $('.js-save-button').click(function () {
        const permissionIDs = model.tree.getSelectedRowKeys();
        const url = $(this).attr('data-url');
        
        $.ajax({
            type: 'POST',
            url: url,
            data: { RoleID: model.roleIDRocused, PermissionIDs: permissionIDs },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (e) {
                if (e.IsSuccess) {
                    successErrorToast63Bits.showSuccessMessage();
                }
            },
            complete: function () {
                preloader.hide();
            }
        });
    });
});