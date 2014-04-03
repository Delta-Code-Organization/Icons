var ImageInBase64 = "";

//$(document).ready(function () {

//    $("#Files").change(function (e) {
//        readImagesFromUploader(e);
//    });

//    function readImage(file, event) {
//        ImageInBase64 = event.target.result.replace("data:image/png;base64,", "");
//        ImageInBase64 = ImageInBase64.replace("data:image/jpeg;base64,", "");
//        ImageInBase64 = ImageInBase64.replace("data:image/jpg;base64,", "");
//        ImageInBase64 = ImageInBase64.replace("data:image/gif;base64,", "");
//        ImageInBase64 = ImageInBase64.replace("data:image/doc;base64,", "");
//        ImageInBase64 = ImageInBase64.replace("data:image/docx;base64,", "");
//        ImageInBase64 = ImageInBase64.replace("data:image/txt;base64,", "");
//        ImageInBase64 = ImageInBase64.replace("data:image/XLK;base64,", "");
//        ImageInBase64 = ImageInBase64.replace("data:image/XLS;base64,", "");
//        ImageInBase64 = ImageInBase64.replace("data:image/XLS;base64,", "");
//        ImageInBase64 = ImageInBase64.replace("data:image/rar;base64,", "");
//        ImageInBase64 = ImageInBase64.replace("data:image/zip;base64,", "");
//        alert(ImageInBase64);
//        $('#Attachment').val(ImageInBase64);
//    }

//    function readImages(event) {
//        var files = event.originalEvent.dataTransfer.files;
//        $.each(files, function (index, file) {
//            var fileReader = new FileReader();
//            fileReader.onload = (function (file) {
//                return function (event) {
//                    return readImage(file, event);
//                }
//            })(file);
//            fileReader.readAsDataURL(file);
//        });
//    }

//    function readImagesFromUploader(event) {
//        var files = event.target.files;
//        $.each(files, function (index, file) {
//            var fileReader = new FileReader();
//            fileReader.onload = (function (file) {
//                return function (event) {
//                    return readImage(file, event);
//                }
//            })(file);
//            fileReader.readAsDataURL(file);
//        });
//    }
//});

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