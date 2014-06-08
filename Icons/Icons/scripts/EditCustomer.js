var ImageInBase64 = "";

function getExt(filename) {
    var ext = filename.split('.').pop();
    if (ext == filename) return "";
    return ext;
}

$(document).ready(function () {

    $("#Attachs").change(function (e) {
        readImagesFromUploader(e);
        $('#Ext').val(getExt($('#Attachs').val()));
        $('#FileName').val($('#Attachs').val());
    });

    function readImage(file, event) {
        ImageInBase64 = event.target.result.replace("data:image/png;base64,", "");
        ImageInBase64 = ImageInBase64.replace("data:image/jpeg;base64,", "");
        ImageInBase64 = ImageInBase64.replace("data:image/jpg;base64,", "");
        ImageInBase64 = ImageInBase64.replace("data:image/gif;base64,", "");
        ImageInBase64 = ImageInBase64.replace("data:application/vnd.openxmlformats-officedocument.wordprocessingml.document;base64,", "");
        ImageInBase64 = ImageInBase64.replace("data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64", "");
        ImageInBase64 = ImageInBase64.replace("data:application/pdf;base64,", "");
        ImageInBase64 = ImageInBase64.replace("data:application/octet-stream;base64,", "");
        ImageInBase64 = ImageInBase64.replace("data:application/zip;base64,", "");
        ImageInBase64 = ImageInBase64.replace("data:text/plain;base64,", "");
        alert(ImageInBase64);
        $('#Attachment').val(ImageInBase64);
    }

    function readImages(event) {
        var files = event.originalEvent.dataTransfer.files;
        $.each(files, function (index, file) {
            var fileReader = new FileReader();
            fileReader.onload = (function (file) {
                return function (event) {
                    return readImage(file, event);
                }
            })(file);
            fileReader.readAsDataURL(file);
        });
    }

    function readImagesFromUploader(event) {
        var files = event.target.files;
        $.each(files, function (index, file) {
            var fileReader = new FileReader();
            fileReader.onload = (function (file) {
                return function (event) {
                    return readImage(file, event);
                }
            })(file);
            fileReader.readAsDataURL(file);
        });
    }
});

$(document).ready(function () {
    $('#SubCreate').submit(function (event) {
        if ($(this).parsley('validate')) {
            var Name = $('#custname').val();
            var Address = $('#custaddress').val();
            var Phone = $('#PhoneMask').val();
            var BirthDate = $('#custbirthdate').val();
            var Notes = $('#custnotes').val();
            var Ext = $('#Ext').val();
            var Attachment = $('#Attachment').val();
            var FileName = $('#FileName').val();
            var res = { '_Name': Name, '_Address': Address, '_Phone': Phone, '_BirthDate': BirthDate, '_Notes': Notes, 'Ext': Ext, 'FileName': FileName, 'Attachment': Attachment }
            $.ajax({
                url: '/Customer/EditCutomerData',
                type: 'post',
                data: res,
                success: function (data) {
                    $.gritter.add({
                        title: '! نجاح العملية',
                        text: ". تم تعديل بيانات العميل بنجاح",
                        image: '/content/images/user-icon.png',
                        class_name: 'clean',
                        time: '120000'
                    });
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



function Cancel(id) {
    window.location.href = "/Customer/EditCustomer?_ID=" + id;
}


function Remove(id) {
    if (confirm("Are you sure ?")) {
        $.ajax({
            url: '/Customer/RemoveCustomer',
            type: 'post',
            data: { '_ID': id },
            success: function (data) {
                $('#' + id).fadeOut(500);
                return false;
            },
            error: function (data) {
                alert(data.responseText);
            }
        });
    }
}