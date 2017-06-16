(function ($) {
    var accountService = studiox.services.app.account;

    var $form = $('form[name=TenantChangeForm]');

    function switchToSelectedTenant () {

        var tenancyName = $form.find('input[name=TenancyName]').val();

        if (!tenancyName) {
            studiox.multiTenancy.setTenantIdCookie(null);
            location.reload();
            return;
        }

        accountService.isTenantAvailable({
            tenancyName: tenancyName
        }).done(function (result) {
            switch (result.state) {
            case 1: //Available
                studiox.multiTenancy.setTenantIdCookie(result.tenantId);
                //_modalManager.close();
                location.reload();
                return;
            case 2: //InActive
                studiox.message.warn(studiox.utils.formatString(studiox.localization
                    .localize("TenantIsNotActive", "Boilerplate"), tenancyName));
                break;
            case 3: //NotFound
                studiox.message.warn(studiox.utils.formatString(studiox.localization
                    .localize("ThereIsNoTenantDefinedWithName{0}", "Boilerplate"), tenancyName));
                break;
            }
        });
    }

    //Handle save button click
    $form.closest('div.modal-content').find(".save-button").click(function(e) {
        e.preventDefault();
        switchToSelectedTenant();
    });

    //Handle enter key
    $form.find('input').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            switchToSelectedTenant();
        }
    });

    $.AdminBSB.input.activate($form);

    $('#TenantChangeModal').on('shown.bs.modal', function () {
        $form.find('input[type=text]:first').focus();
    });
})(jQuery);