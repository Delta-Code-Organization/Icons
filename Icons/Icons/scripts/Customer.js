$(document).ready(function () {
    $('#SubCreate').submit(function (event) {
        if ($(this).parsley('validate')) {
            var Name = $('#custname').val();
            var Address = $('#custaddress').val();
            var Phone = $('#PhoneMask').val();
            var BirthDate = $('#custbirthdate').val();
            var Notes = $('#custnotes').val();
            var res = { '_Name': Name, '_Address': Address, '_Phone': Phone, '_BirthDate': BirthDate, '_Notes': Notes }
            $.ajax({

                url: '/Customer/CreateCustom',
                type: 'post',
                data: res,
                success: function (data) {
                    $.gritter.add({
                        title: '! نجاح العملية',
                        text: ". تم اضافة العميل بنجاح",
                        image: '/content/images/user-icon.png',
                        class_name: 'clean',
                        time: '120000'
                    });
                    setInterval(function () {
                        window.location.href = '/Customer/CreateCustomer';
                    }, 5000);
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

function Cancel() {
    window.location.href = "/Customer/CreateCustomer";
}


$(document).ready(function () {
    $('#AddCard').submit(function (event) {
        if ($(this).parsley('validate')) {
            var Code = $('#custcode').val();
            $.ajax({
                url: '/Customer/AddNewCard',
                type: 'post',
                data: { '_Code': Code },
                success: function (data) {
                    $('#result').text(data);
                },
                error: function (data) {
                    alert(data.responseText);
                }
            });
        }
        return false;
    });
});



//$(document).ready(function () {
//    $('#SubCreate').submit(function (event) {
//        if ($(this).parsley('validate')) {


//        }
//        return false;
//    });
//});

