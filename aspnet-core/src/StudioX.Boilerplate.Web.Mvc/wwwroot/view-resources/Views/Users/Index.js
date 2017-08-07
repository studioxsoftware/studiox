(function () {
    $(function () {

        var userService = studiox.services.app.user;
        var $modal = $('#UserCreateModal');
        var $form = $modal.find('form');

        $form.validate({
            rules: {
                Password: "required",
                ConfirmPassword: {
                    equalTo: "#Password"
                }
            }
        });

        $('#RefreshButton').click(function () {
            refreshUserList();
        });

        $('.delete-user').click(function () {
            var userId = $(this).attr("data-user-id");
            var userName = $(this).attr('data-user-name');

            deleteUser(userId, userName);
        });

        $('.edit-user').click(function (e) {
            var userId = $(this).attr("data-user-id");

            e.preventDefault();
            $.ajax({
                url: studiox.appPath + 'Users/EditUserModal?userId=' + userId,
                type: 'POST',
                contentType: 'application/html',
                success: function (content) {
                    $('#UserEditModal div.modal-content').html(content);
                },
                error: function (e) { }
            });
        });

        $form.find('button[type="submit"]').click(function (e) {
            e.preventDefault();

            if (!$form.valid()) {
                return;
            }

            var user = $form.serializeFormToObject(); //serializeFormToObject is defined in main.js
            user.roleNames = [];
            var $roleCheckboxes = $("input[name='role']:checked");
            if ($roleCheckboxes) {
                for (var roleIndex = 0; roleIndex < $roleCheckboxes.length; roleIndex++) {
                    var $roleCheckbox = $($roleCheckboxes[roleIndex]);
                    user.roleNames.push($roleCheckbox.val());
                }
            }

            studiox.ui.setBusy($modal);
            userService.create(user).done(function () {
                $modal.modal('hide');
                location.reload(true); //reload page to see new user!
            }).always(function () {
                studiox.ui.clearBusy($modal);
            });
        });

        $modal.on('shown.bs.modal', function () {
            $modal.find('input:not([type=hidden]):first').focus();
        });

        function refreshUserList() {
            location.reload(true); //reload page to see new user!
        }

        function deleteUser(userId, userName) {
            studiox.message.confirm(
                "Delete user '" + userName + "'?",
                function (isConfirmed) {
                    if (isConfirmed) {
                        userService.delete({
                            id: userId
                        }).done(function () {
                            refreshUserList();
                        });
                    }
                }
            );
        }
    });
})();