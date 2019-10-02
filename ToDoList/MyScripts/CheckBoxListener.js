$(document).ready(
    function () {
        $('.IsChecked').change(function () {
            var self = $(this);
            var id = self.attr('id');
            var value = self.prop('checked');

            $.ajax({
                url: '/ToDos/EditToDo',
                data: {
                    id: id,
                    value: value
                },
            type: 'POST',
                success: function (result) {
                    $('#tableDiv').html(result);
                }
            });
        });
    });