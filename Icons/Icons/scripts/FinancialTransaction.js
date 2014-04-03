$(document).ready(function () {
    $('#AddFT').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.ajax({
                url: '/Accounting/AddFT',
                type: 'post',
                data: $(this).serialize(),
                success: function (data) {
                    $.gritter.add({
                        title: '! نجاح العملية',
                        text: ". تم اضافة المعاملة المالية بنجاح",
                        image: '/content/images/user-icon.png',
                        class_name: 'clean',
                        time: '120000'
                    });
                    $('#State').val("");
                    $('#Amount').val("");
                    $('#Date').val("");
                    $('#Notes').val("");
                    $('#Notes').text("");
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