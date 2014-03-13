var ValidatePercentage = 0;
var UnitID = 0;

$(document).ready(function () {
    GetUnits($('#cprojectid').val());
});

$(document).ready(function () {
    $('#ContractCreate').submit(function (event) {
        if (ValidatePercentage != 100) {
            alert("مجموع نسبة الملاك يجب ان تكون 100%");
            return false;
        }
        if ($(this).parsley('validate')) {
            $.ajax({
                type: 'post',
                url: '/Contract/CreateContract',
                data: $(this).serialize(),
                success: function (data) {
                    $.gritter.add({
                        title: '! نجاح العملية',
                        text: ". تم اضافة العقد بنجاح",
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

function GetUnits(id) {
    $('#OPTUnits').empty();
    $.ajax({
        url: '/Contract/GetUnits',
        type: 'post',
        data: { 'id': id },
        success: function (data) {
            $.each(data, function (index, u) {
                $('#OPTUnits').append('<option value="' + u.Id + '">' + u.Id + '</option>');
            });
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}

function AddOwner() {
    if (ValidatePercentage == 100) {
        alert('لا يمكنك اضافة ملاك اخرين حيث ان النسبة تساوي 100%');
        return;
    }
    var Cus = $('#customerid').val();
    var CusName = $("#customerid option:selected").text();
    var Per = $('#percentage').val();
    if ((ValidatePercentage + parseInt(Per)) > 100) {
        alert("مجموع نسب الملاك لا يمكن ان تتخطي او تقل عن 100%");
        return;
    }
    if ($("#resoptgrp option[value='" + Cus + "']").length > 0) {
        alert("هذا العميل هو احد الملاك بالفعل .... لا يمكن اختيارة مرة اخري !");
        return;
    }
    $.ajax({
        url: '/Contract/AddOwner',
        type: 'post',
        data: { 'CusID': Cus, 'Percentage': Per },
        success: function (data) {
            $('#CustomerTableBody').append('<tr id="' + Cus + '">'
                                          + '<td style="width: 25%; text-align: center;">'
                                          + '<button type="button" class="btn btn-danger" onclick="DeleteOwner(' + Cus + ',' + Per + ')"><i class="fa fa-times-circle"></i> حذف</button>'
                                          + '</td>'
                                          + '<td style="width: 37.5%; text-align: center;">' + Per + '</td>'
                                          + '<td style="width: 37.5%; text-align: center;">' + CusName + '</td>'
                                          + '</tr>');
            $('#resoptgrp').append('<option value="' + Cus + '">' + CusName + '</option>');
            ValidatePercentage += parseInt(Per);
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}

function DeleteOwner(id, per) {
    $.ajax({
        url: '/Contract/DeleteOwner',
        type: 'post',
        data: { 'CusID': id },
        success: function (data) {
            $('#CustomerTableBody').find('#' + id).fadeOut(500);
            $("#resoptgrp option[value='" + id + "']").remove();
            ValidatePercentage -= parseInt(per);
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}

function CalcInstallmentAmount()
{
    var InstallNum = parseInt($('#INum').val());
    if (isNaN(InstallNum)) {
        $('#INum').val(0);
    }
    if (InstallNum == 0) {
        return;
    }
    var Remain = parseFloat($('#cremaining').val());
    var SingleInstallAmount = (Remain / InstallNum);
    $('#SingleInstallVal').val(SingleInstallAmount);
}

function CalcRemaining() {
    var price = parseInt($('#cprice').val());
    var paid = parseInt($('#cpaid').val());
    if (isNaN(paid)) {
        paid = 0;
        $('#cpaid').val('0');
    }
    if (price < paid) {
        $.gritter.add({
            title: '! خطأ',
            text: ". لا يمكن ان يكون المدفوع اقل من السعر !",
            image: '/content/images/user-icon.png',
            class_name: 'clean',
            time: '3000'
        });
        $('#cpaid').val('');
        $('#cremaining').val('');
        return false;
    }
    var Remaining = price - paid;
    $('#cremaining').val(Remaining);
    CalcInstallmentAmount();
}

function SubmitForm() {
    $('#ContractCreate').submit();
}