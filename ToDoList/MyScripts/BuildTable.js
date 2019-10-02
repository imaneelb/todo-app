$(document).ready(
    function () {
        $.ajax({
            url: '/ToDos/BuildToDoTable',
            success: function (result) {
                $('#tableDiv').html(result);
            }
        });
    }
);