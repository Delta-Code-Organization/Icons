$(document).ready(function () {
    $('#SaveForm').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            try{
                App.nestableLists();
            }
            catch(err){
            }
            var Tree = $('#out1').text();
            $.ajax({
                type: 'post',
                url: '/Accounting/SaveTree',
                data: { 'TreeJson': Tree },
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: 'تم الحفظ',
                            text: ". تم  تحديث الشجره المحاسبيه",
                            image: '/content/images/user-icon.png',
                            class_name: 'clean',
                            time: '120000'
                        });
                        $('#aaname').focus();
                    }
                    else {
                        $.gritter.add({
                            title: '! فشل العملية',
                            text: ". تعزر حفظ الشجره",
                            image: '/content/images/user-icon.png',
                            class_name: 'clean',
                            time: '120000'
                        });
                        $('#aaname').focus();
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
    $('#UATS').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            try {
                App.nestableLists();
            }
            catch (err) {
            }
            $.ajax({
                type: 'post',
                url: '/Accounting/SaveSettings',
                data: {
                    'Suppliers': $("#cmbSuppliers").val(),
                    'Customers': $("#cmbCustomers").val(),
                    'Employee': $("#cmbEmployee").val(),
                    'Banks': $("#cmbBanks").val(),
                    'Projects': $("#cmbProjects").val(),
                    'Imprest': $("#cmbImprest").val(),
                    'Safes': $("#cmbSafes").val(),
                    'Sales': $("#cmbSales").val(),
                    'Stock': $("#cmbStock").val(),
                },
                success: function (data) {
                    if (data == "true") {
                        $.gritter.add({
                            title: 'تم الحفظ',
                            text: ". تم  تحديث اعدادات الحسابات",
                            image: '/content/images/user-icon.png',
                            class_name: 'clean',
                            time: '120000'
                        });
                        $('#aaname').focus();
                    }
                    else {
                        $.gritter.add({
                            title: '! فشل العملية',
                            text: ". تعزر حفظ الاعدادات",
                            image: '/content/images/user-icon.png',
                            class_name: 'clean',
                            time: '120000'
                        });
                        $('#aaname').focus();
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
function DeleteNode() {
    $.ajax({
        type: 'post',
        url: '/Accounting/DeleteNode',
        data: { 'id': $("#accnum").val() },
        success: function (data) {
            if (data == "true") {
                $.gritter.add({
                    title: 'تم الحفظ',
                    text: ". تم  حذف الحساب",
                    image: '/content/images/user-icon.png',
                    class_name: 'clean',
                    time: '120000'
                });
                $('#aaname').focus();
                $("li[data-id='" + $("#accnum").val() + "']").remove();
            }
            else {
                $.gritter.add({
                    title: '! فشل العملية',
                    text: ". تعزر حذف الحساب",
                    image: '/content/images/user-icon.png',
                    class_name: 'clean',
                    time: '120000'
                });
                $('#aaname').focus();
            }
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}


   