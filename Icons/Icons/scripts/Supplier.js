$(document).ready(function () {
    $('#SubCreate').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Supplier/CreateSupplier',
                data: $(this).serialize(),
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم اضافة المورد بنجاح",
                            image: '/content/images/user-icon.png',
                            class_name: 'clean',
                            time: '120000'
                        });
                    }
                },
                error: function (data) {
                    alert(data.responseText);
                }
            });
            return false;
        }
        return false;
    });

    $('#SubEdit').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Supplier/EditSupplier',
                data: $(this).serialize(),
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم تعديل بيانات المورد بنجاح",
                            image: '/content/images/user-icon.png',
                            class_name: 'clean',
                            time: '120000'
                        });
                    }
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

function Remove(id)
{
    if (confirm("هل انت متأكد انك تريد حذف هذا المورد ؟")) {
        $.ajax({
            url: '/Supplier/Remove',
            type: 'post',
            data: { 'id': id },
            success: function (data) {
                $('#' + id).fadeOut(500);
            },
            error: function (data) {
                alert(data.responseText);
            }
        });
    }
}