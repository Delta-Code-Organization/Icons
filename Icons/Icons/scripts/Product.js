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
});