(function() {
    $(function() {

        var service = studiox.services.app.tenant;
        var $modal = $('#TenantCreateModal');
        var $form = $modal.find('form');

        $form.validate();

        $form.find('button[type="submit"]').click(function (e) {
            e.preventDefault();

            if (!$form.valid()) {
                return;
            }

            var tenant = $form.serializeFormToObject(); //serializeFormToObject is defined in main.js

            studiox.ui.setBusy($modal);
            service.createTenant(tenant).done(function () {
                $modal.modal('hide');
                location.reload(true); //reload page to see new tenant!
            }).always(function() {
                studiox.ui.clearBusy($modal);
            });
        });
        
        $modal.on('shown.bs.modal', function () {
            $modal.find('input:not([type=hidden]):first').focus();
        });
    });
})();