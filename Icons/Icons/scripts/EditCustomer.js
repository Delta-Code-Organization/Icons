$(document).ready(function () {
    $('#SubCreate').submit(function (event) {
        alert('sasasa');
        if ($(this).parsley('validate')) {
            var Name = $('#custname').val();
            var Address = $('#custaddress').val();
            var Phone = $('#PhoneMask').val();
            var BirthDate = $('#custbirthdate').val();
            var Notes = $('#custnotes').val();
            var res = { '_Name': Name, '_Address': Address, '_Phone': Phone, '_BirthDate': BirthDate, '_Notes': Notes }
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