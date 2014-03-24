$(document).ready(function () {
    $('#CreateEmployee').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Employee/CreateEmployee',
                data: $(this).serialize(),
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم اضافة الموظف بنجاح",
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

    $('#EditEmployee').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Employee/EditEmployee',
                data: $(this).serialize(),
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم تعديل بيانات الموظف بنجاح",
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

    $('#PaySalary').submit(function (event) {
        if ($(this).parsley('validate')) {
            var EmpID = $('#id').val();
            var Total = $('#Total').val();
            var ToAccID = $('#ToAccID').val();
            var PaymentDate = $('#PaymentDate').val();
            var data = { 'id': EmpID, 'Total': Total, 'PaymentDate': PaymentDate, 'ToAccID': ToAccID };
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Employee/Pay',
                data: data,
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم دفع راتب الموظف بنجاح",
                            image: '/content/images/user-icon.png',
                            class_name: 'clean',
                            time: '3000'
                        });
                    }
                    else {
                        $.gritter.add({
                            title: '! فشل العملية',
                            text: ". لا يمكن دفع راتب الموظف قبل معادة",
                            image: '/content/images/user-icon.png',
                            class_name: 'clean',
                            time: '3000'
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
    if (confirm("هل انت متأكد انك تريد حذف هذا الموظف ؟")) {
        $.ajax({
            url: '/Employee/RemoveEmployee',
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

function SetHiddenFields(id,Total)
{
    $('#id').val(id);
    $('#Total').val(Total);
}