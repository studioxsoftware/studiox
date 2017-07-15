(function () {
    $(function () {

        var tenantService = studiox.services.app.tenant;
        var $modal = $('#TenantCreateModal');
        var $form = $modal.find('form');

        $form.validate();

        $('#RefreshButton').click(function () {
            document.location.reload();
        });

        $('.delete-tenant').click(function () {
            var tenantId = $(this).attr("data-tenant-id");
            var tenancyName = $(this).attr('data-tenancy-name');

            deleteTenant(tenantId, tenancyName);
        });

        $('.edit-tenant').click(function (e) {
            var tenantId = $(this).attr("data-tenant-id");

            e.preventDefault();
            $.ajax({
                url: studiox.appPath + 'Tenants/EditTenantModal?tenantId=' + tenantId,
                type: 'POST',
                contentType: 'application/html',
                success: function (content) {
                    $('#TenantEditModal div.modal-content').html(content);
                },
                error: function (e) { }
            });
        });

        $form.find('button[type="submit"]').click(function (e) {
            e.preventDefault();

            if (!$form.valid()) {
                return;
            }

            var tenant = $form.serializeFormToObject(); //serializeFormToObject is defined in main.js

            studiox.ui.setBusy($modal);
            tenantService.create(tenant).done(function () {
                $modal.modal('hide');
                location.reload(true); //reload page to see new tenant!
            }).always(function () {
                studiox.ui.clearBusy($modal);
            });
        });

        $modal.on('shown.bs.modal', function () {
            $modal.find('input:not([type=hidden]):first').focus();
        });

        function refreshTenantsList() {
            location.reload(true); //reload page to see new tenant!
        }

        function deleteTenant(tenantId, tenancyName) {
            studiox.message.confirm(
                "Delete tenant '" + tenancyName + "'?",
                function (isConfirmed) {
                    if (isConfirmed) {
                        tenantService.delete({
                            id: tenantId
                        }).done(function () {
                            refreshTenantsList();
                        });
                    }
                }
            );
        }
    });
})();