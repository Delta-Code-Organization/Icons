﻿$(document).ready(function () {
    $('#AddFT').submit(function (event) {
        if ($(this).parsley('validate')) {
            $('#CLoader').css('display', 'block');
            alert("Loader Shown");
            $.ajax({
                url: '/Accounting/AddFT',
                type: 'post',
                data: $(this).serialize(),
                success: function (data) {
                    $.gritter.add({
                        title: '! نجاح العملية',
                        text: ". تم اضافة المعاملة المالية بنجاح برقم (" + data + ")",
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
            $('#CLoader').css('display', 'none');
            return false;
        }
        return false;
    });

    $('#AddEmployement').submit(function (event) {
        if ($(this).parsley('validate')) {
            $('#CLoader').css('display', 'block');
            $.ajax({
                url: '/Accounting/AddEmployementFT',
                type: 'post',
                data: $(this).serialize(),
                success: function (data) {
                    $.gritter.add({
                        title: '! نجاح العملية',
                        text: ". تم اضافة المعاملة المالية بنجاح برقم (" + data + ")",
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
            $('#CLoader').css('display', 'none');
            return false;
        }
        return false;
    });

    $('#EditFT').submit(function (event) {
        if ($(this).parsley('validate')) {
            $('#CLoader').css('display', 'block');
            $.ajax({
                url: '/Accounting/EditFT',
                type: 'post',
                data: $(this).serialize(),
                success: function (data) {
                    $.gritter.add({
                        title: '! نجاح العملية',
                        text: ". تم تعديل المعاملة المالية بنجاح",
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
            $('#CLoader').css('display', 'none');
            return false;
        }
        return false;
    });
});