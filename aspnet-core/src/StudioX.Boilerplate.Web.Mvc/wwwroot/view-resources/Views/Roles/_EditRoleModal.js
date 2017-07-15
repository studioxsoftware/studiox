(function ($) {

    var roleService = studiox.services.app.role;
    var $modal = $('#RoleEditModal');
    var $form = $('form[name=RoleEditForm]');

    function save() {

        if (!$form.valid()) {
            return;
        }

        var role = $form.serializeFormToObject(); //serializeFormToObject is defined in main.js
        role.permissions = [];
        var $permissionCheckboxes = $("input[name='permission']:checked:visible");
        if ($permissionCheckboxes) {
            for (var permissionIndex = 0; permissionIndex < $permissionCheckboxes.length; permissionIndex++) {
                var $permissionCheckbox = $($permissionCheckboxes[permissionIndex]);
                role.permissions.push($permissionCheckbox.val());
            }
        }

        studiox.ui.setBusy($form);
        roleService.update(role).done(function () {
            $modal.modal('hide');
            location.reload(true); //reload page to see edited role!
        }).always(function () {
            studiox.ui.clearBusy($modal);
        });
    }

    //Handle save button click
    $form.closest('div.modal-content').find(".save-button").click(function (e) {
        e.preventDefault();
        save();
    });

    //Handle enter key
    $form.find('input').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            save();
        }
    });

    $.AdminBSB.input.activate($form);

    $modal.on('shown.bs.modal', function () {
        $form.find('input[type=text]:first').focus();
    });
})(jQuery);