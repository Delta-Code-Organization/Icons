﻿function DeleteInvoice(id) {
    if (confirm("هل انت متأكد انك تريد حذف هذة الفاتورة")) {
        $.ajax({
            url: '/Customer/DeleteInvoice',
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

function Depart(id) {
    $.ajax({
        url: '/Customer/Depart',
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