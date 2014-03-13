var ValidatePercentage = 0;
var UnitID = 0;

$(document).ready(function () {
    $('#ContractCreate').submit(function (event) {
        UnitID = $('input:radio[name=rad1]:checked').val();
        $('#TheUnitIDToUpdate').val(UnitID);
        if (ValidatePercentage != 100) {
            alert("مجموع نسبة الملاك يجب ان تكون 100%");
            return;
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
    $('#UnitTableBody').empty();
    if (id == 1000000) {
        return;
    }
    $.ajax({
        url: '/Contract/GetUnits',
        type: 'post',
        data: { 'id': id },
        success: function (data) {
            $.each(data, function (index, u) {
                $('#UnitTableBody').append('<tr>'
                                       + '<td style="width: 25%; text-align:center;">'
                                       + '<label class="radio-inline"> <input type="radio" name="rad1" value="' + u.Id + '" class="icheck"></label> '
                                       + '</td>'
                                       + '<td style="width: 25%; text-align:center;"><strong>' + u.ExpectedPrice + '</strong></td>'
                                       + '<td style="width: 25%; text-align:center;"><strong>' + u.Finishing + '</strong></td>'
                                       + '<td style="width: 25%; text-align:center;"><strong>' + u.UnitName + '</strong></td>'
                                       + '</tr>');
            });
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}

function AddOwner() {
    var Cus = $('#customerid').val();
    var CusName = $("#customerid option:selected").text();
    var Per = $('#percentage').val();
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
                                          + '<button type="button" class="btn btn-danger" onclick="DeleteOwner(' + Cus + ')"><i class="fa fa-times-circle"></i> حذف</button>'
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

function DeleteOwner(id) {
    $.ajax({
        url: '/Contract/DeleteOwner',
        type: 'post',
        data: { 'CusID': id },
        success: function (data) {
            $('#CustomerTableBody').find('#' + id).fadeOut(500);
            $("#resoptgrp option[value='" + id + "']").remove();
            ValidatePercentage -= parseInt(Per);
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}




function SubmitForm() {
    $('#ContractCreate').submit();
}