(function () {
    $(function () {

        var roleService = studiox.services.app.role;
        var $modal = $('#RoleCreateModal');
        var $form = $modal.find('form');

        $form.validate({
        });

        $('#RefreshButton').click(function () {
            refreshRoleList();
        });

        $('.delete-role').click(function () {
            var roleId = $(this).attr("data-role-id");
            var roleName = $(this).attr('data-role-name');

            deleteRole(roleId, roleName);
        });

        $('.edit-role').click(function (e) {
            var roleId = $(this).attr("data-role-id");

            e.preventDefault();
            $.ajax({
                url: studiox.appPath + 'Roles/EditRoleModal?roleId=' + roleId,
                type: 'POST',
                contentType: 'application/html',
                success: function (content) {
                    $('#RoleEditModal div.modal-content').html(content);
                },
                error: function (e) { }
            });
        });

        $form.find('button[type="submit"]').click(function (e) {
            e.preventDefault();

            if (!$form.valid()) {
                return;
            }

            var role = $form.serializeFormToObject(); //serializeFormToObject is defined in main.js
            role.permissions = [];
            var $permissionCheckboxes = $("input[name='permission']:checked");
            if ($permissionCheckboxes) {
                for (var permissionIndex = 0; permissionIndex < $permissionCheckboxes.length; permissionIndex++) {
                    var $permissionCheckbox = $($permissionCheckboxes[permissionIndex]);
                    role.permissions.push($permissionCheckbox.val());
                }
            }

            studiox.ui.setBusy($modal);
            roleService.create(role).done(function () {
                $modal.modal('hide');
                location.reload(true); //reload page to see new role!
            }).always(function () {
                studiox.ui.clearBusy($modal);
            });
        });

        $modal.on('shown.bs.modal', function () {
            $modal.find('input:not([type=hidden]):first').focus();
        });

        function refreshRoleList() {
            location.reload(true); //reload page to see new role!
        }

        function deleteRole(roleId, roleName) {
            studiox.message.confirm(
                "Remove Users from Role and delete Role '" + roleName + "'?",
                function (isConfirmed) {
                    if (isConfirmed) {
                        roleService.delete({
                            id: roleId
                        }).done(function () {
                            refreshRoleList();
                        });
                    }
                }
            );
        }
    });
})();