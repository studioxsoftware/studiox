(function ($) {

    var userService = studiox.services.app.user;
    var $modal = $('#UserEditModal');
    var $form = $('form[name=UserEditForm]');

    function save() {

        if (!$form.valid()) {
            return;
        }

        var user = $form.serializeFormToObject(); //serializeFormToObject is defined in main.js
        user.roles = [];
        var $roleCheckboxes = $("input[name='role']:checked:visible");

        if ($roleCheckboxes) {
            for (var roleIndex = 0; roleIndex < $roleCheckboxes.length; roleIndex++) {
                var $roleCheckbox = $($roleCheckboxes[roleIndex]);
                user.roles.push($roleCheckbox.val());
            }
        }

        studiox.ui.setBusy($form);
        userService.update(user).done(function () {
            $modal.modal('hide');
            location.reload(true); //reload page to see edited user!
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