$(document).ready(function () {
    $('#CreateCatForm').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Product/CreateCategory',
                data: $(this).serialize(),
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم اضافة التصنيف بنجاح",
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


    $('#prodCreate').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Product/CreateProduct',
                data: $(this).serialize(),
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم اضافة المنتج بنجاح",
                            image: '/content/images/user-icon.png',
                            class_name: 'clean',
                            time: '120000'
                        });
                    }
                    else {
                        $.gritter.add({
                            title: '! فشل العملية',
                            text: ". المنتج موجود بالفعل",
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

    $('#EditCatForm').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Product/EditCategory',
                data: $(this).serialize(),
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم تعديل التصنيف بنجاح",
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

    $('#prodEdit').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Product/EditProduct',
                data: $(this).serialize(),
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم تعديل المنتج بنجاح",
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
    if (confirm("هل أنت متأكد انك تريد حذف هذا التصنيف ؟")) {
        $.ajax({
            url: '/Product/RemoveCategory',
            type: 'post',
            data: { 'id': id },
            success: function (data) {
                if (data == "false") {
                    $.gritter.add({
                        title: '! فشل العملية',
                        text: ". لا يمكن حذف تصنيف يحتوي علي منتجات",
                        image: '/content/images/user-icon.png',
                        class_name: 'clean',
                        time: '120000'
                    });
                }
                else {
                    $('#' + id).fadeOut(500);
                }
            },
            error: function (data) {
                alert(data.responseText);
            }
        });
    }
}

function RemoveProduct(id)
{
    if (confirm("هل أنت متأكد انك تريد حذف هذا المنتج ؟")) {
        $.ajax({
            url: '/Product/RemoveProduct',
            type: 'post',
            data: { 'id': id },
            success: function (data) {
                if (data == "false") {
                    $.gritter.add({
                        title: '! فشل العملية',
                        text: ". لا يمكن حذف هذا المنتج",
                        image: '/content/images/user-icon.png',
                        class_name: 'clean',
                        time: '120000'
                    });
                }
                else {
                    $('#' + id).fadeOut(500);
                }
            },
            error: function (data) {
                alert(data.responseText);
            }
        });
    }
}