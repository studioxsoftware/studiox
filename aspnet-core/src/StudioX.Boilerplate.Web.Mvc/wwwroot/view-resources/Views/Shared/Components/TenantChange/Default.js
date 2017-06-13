(function () {
    $('.tenant-change-component a')
        .click(function (e) {
            e.preventDefault();
            $.ajax({
                url: studiox.appPath + 'Account/TenantChangeModal',
                type: 'POST',
                contentType: 'application/html',
                success: function (content) {
                    $('#TenantChangeModal div.modal-content').html(content);
                },
                error: function (e) { }
            });
        });
})();