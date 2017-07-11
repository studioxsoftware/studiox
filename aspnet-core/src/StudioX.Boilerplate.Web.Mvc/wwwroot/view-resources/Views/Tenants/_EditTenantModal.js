(function ($) {

    var tenantService = studiox.services.app.tenant;
    var $modal = $('#TenantEditModal');
    var $form = $('form[name=TenantEditForm]');

    function save() {
        
        if (!$form.valid()) {
            return;
        }

        var tenant = $form.serializeFormToObject(); //serializeFormToObject is defined in main.js

        studiox.ui.setBusy($form);
        tenantService.update(tenant).done(function () {
            $modal.modal('hide');
            location.reload(true); //reload page to see edited tenant!
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

    $('#TenantChangeModal').on('shown.bs.modal', function () {
        $form.find('input[type=text]:first').focus();
    });
})(jQuery);