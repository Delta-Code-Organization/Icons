$(document).ready(function () {
    $('#AddImprest').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Employee/AddImprest',
                data: $(this).serialize(),
                success: function (data) {
                    $.gritter.add({
                        title: '! نجاح العملية',
                        text: ". تم اضافة العهد بنجاح",
                        image: '/content/images/user-icon.png',
                        class_name: 'clean',
                        time: '3000'
                    });
                    location.reload();
                },
                error: function (data) {
                    alert(data.responseText);
                }
            });
            return false;
        }
        return false;
    });
});

function RemoveImprest(id) {
    if (confirm("هل أنت متأكد انك تريد حذف هذا العهد ؟")) {
        $.ajax({
            url: '/Employee/RemoveImprest',
            type: 'post',
            data: { 'id': id },
            success: function (data) {
                $('#' + id).fadeOut(500);
                $('#DetailRow' + id).fadeOut(300);
            },
            error: function (data) {
                alert(data.responseText);
            }
        });
    }
}