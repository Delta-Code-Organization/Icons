$(document).ready(function () {
    $('#LoginForm').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/User/Login',
                data: { 'Username': $('#Username').val(), 'Password': $('#Password').val() },
                success: function (data) {
                    if (data == "true") {
                        window.location.href = "/Home/Index";
                    }
                    else {
                        $.gritter.add({
                            title: '! فشل العملية',
                            text: ". الرجاء ادخال اسم مستخدم وكلمة مرور صحيحة",
                            image: '/content/images/user-icon.png',
                            class_name: 'clean',
                            time: '120000'
                        });
                        $('#Username').focus();
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

    $('#UpdateUserData').submit(function (event) {
        if ($(this).parsley('validate')) {
            var Permissions = "";
            var ValidateCheckBoxes = false;
            $('input[type=checkbox]').each(function () {
                if ($(this).is(':checked')) {
                    Permissions += $(this).val() + "#";
                    ValidateCheckBoxes = true;
                }
            });
            if (ValidateCheckBoxes == false) {
                alert("الرجاء اختيار صلاحيات للمستخدم");
                return false;
            }
            var strLen = Permissions.length;
            Permissions = Permissions.slice(0, strLen - 1);
            $.ajax({
                type: 'post',
                url: '/User/UpdateUser',
                data: { 'name': $('#aname').val(), 'pass': $('#apass').val(), 'per': Permissions },
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم تعديل بيانات المستخدم بنجاح",
                            image: '/content/images/user-icon.png',
                            class_name: 'clean',
                            time: '120000'
                        });
                    }
                    else {
                        $.gritter.add({
                            title: '! فشل العملية',
                            text: ". اسم المستخدم موجود بالفعل الرجاء اختيار اسم مستخدم اخر",
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

    $('#CreateAccForm').submit(function (event) {
        if ($(this).parsley('validate')) {
            var Permissions = "";
            var ValidateCheckBoxes = false;
            $('input[type=checkbox]').each(function () {
                if ($(this).is(':checked')) {
                    Permissions += $(this).val() + "#";
                    ValidateCheckBoxes = true;
                }
            });
            if (ValidateCheckBoxes == false) {
                alert("الرجاء اختيار صلاحيات للمستخدم");
                return false;
            }
            var strLen = Permissions.length;
            Permissions = Permissions.slice(0, strLen - 1);
            $.ajax({
                type: 'post',
                url: '/User/AddAccount',
                data: {'name': $('#aname').val(), 'pass': $('#apass').val(), 'per': Permissions },
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم اضافة المستخدم بنجاح",
                            image: '/content/images/user-icon.png',
                            class_name: 'clean',
                            time: '120000'
                        });
                    }
                    else {
                        $.gritter.add({
                            title: '! فشل العملية',
                            text: ". اسم المستخدم موجود بالفعل الرجاء اختيار اسم مستخدم اخر",
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

function Remove(id) {
    $.ajax({
        url: '/User/DeleteUser',
        type: 'post',
        data: { 'id': id },
        success: function (data) {
            if (confirm("Are you sure ?")) {
                $('#' + id).fadeOut(500);
            }
            return false;
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}