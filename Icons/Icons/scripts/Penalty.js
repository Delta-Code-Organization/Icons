$(document).ready(function () {
    $('#AddPenalty').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Employee/AddPenalty',
                data: $(this).serialize(),
                success: function (data) {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم اضافة الجزاء بنجاح",
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

function RemovePenalty(id)
{
    if (confirm("هل أنت متأكد انك تريد حذف هذا الجزاء ؟")) {
        $.ajax({
            url: '/Employee/RemovePenalty',
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