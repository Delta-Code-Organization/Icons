$(document).ready(function () {
    $('#ProjCreate').submit(function (event) {
        if ($(this).parsley('validate')) {
            alert("A");
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Project/CreateProject',
                data: $(this).serialize(),
                success: function (data) {
                    alert("A");
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم اضافة المشروع بنجاح",
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


    $('#AddUnit').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Project/CreateUnit',
                data: $(this).serialize(),
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم اضافة الوحدة بنجاح",
                            image: '/content/images/user-icon.png',
                            class_name: 'clean',
                            time: '120000'
                        });
                    }
                    setTimeout(function () {
                        location.reload();
                    }, 4000);
                },
                error: function (data) {
                    alert(data.responseText);
                }
            });
            return false;
        }
        return false;
    });

    $('#EditUnit').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Project/EditUnit',
                data: $(this).serialize(),
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم تعديل الوحدة بنجاح",
                            image: '/content/images/user-icon.png',
                            class_name: 'clean',
                            time: '120000'
                        });
                    }
                    setTimeout(function () {
                        location.reload();
                    }, 4000);
                },
                error: function (data) {
                    alert(data.responseText);
                }
            });
            return false;
        }
        return false;
    });


    $('#ProjEdit').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $.ajax({
                type: 'post',
                url: '/Project/EditProject',
                data: $(this).serialize(),
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: '! نجاح العملية',
                            text: ". تم تعديل بيانات المشروع بنجاح",
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

function SubmitForm() {
    alert("A");
    $('#ProjCreate').submit();
}

function EditFormSubmit() {
    $('#ProjEdit').submit();
}

function Remove(id) {
    $.ajax({
        url: '/Project/Remove',
        type: 'post',
        data: { 'ID': id },
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

function RemoveUnit(id) {
    $.ajax({
        url: '/Project/DeleteUnit',
        type: 'post',
        data: { 'id': id },
        success: function (data) {
            if (confirm("Are you sure ?")) {
                $('#' + id).fadeOut(500);
            }
            $('#DetailRow' + id).fadeOut(300);
            return false;
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}

function FillFields(id) {
    $('#PUnitID').val(id);
    $('#DetailRow' + id).find('td').each(function (index, ele) {
        if (index == 0) {
            $("#Ptype option").each(function () {
                if ($(this).text() == $(ele).text()) {
                    $(this).attr('selected', 'selected');
                }
            });
        }
        if (index == 1) {
            $('#Pspace').val($(ele).text());
        }
        if (index == 2) {
            $('#Pfloors').val($(ele).text());
        }
        if (index == 3) {
            $('#Pprice').val($(ele).text());
        }
        if (index == 4) {
            $("#Pfinish option").each(function () {
                if ($(this).text() == $(ele).text()) {
                    $(this).attr('selected', 'selected');
                }
            });
        }
        if (index == 5) {
            $('#Pnotes').val($(ele).text());
        }
    });
}