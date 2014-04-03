﻿$(document).ready(function () {
    $('#StatementsForm').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            $('#Loader').css('display', 'block');
            $.ajax({
                type: 'post',
                url: '/Accounting/FilterStatements',
                data: $(this).serialize(),
                success: function (data) {
                    var Total = 0;
                    $('#AreaToAppend').empty();
                    $('#Loader').css('display', 'none');
                    $.each(data, function (index, St) {
                        Total += St.Amount;
                        var millii = St.TransactionDate.replace(/\/Date\((-?\d+)\)\//, '$1');
                        var DateTimee = new Date(parseInt(millii));
                        var Dayy = DateTimee.getDate();
                        var yearr = DateTimee.getFullYear();
                        var mounthh = DateTimee.getMonth() + 1;
                        var FullDatee = mounthh + "/" + Dayy + "/" + yearr;
                        $('#AreaToAppend').append('<tr>'
                                    + '<td style="width: 16.66%; text-align: center;">' + St.AccountingTree.NodeName + '</td>'
                                    + '<td style="width: 16.66%; text-align: center;">' + St.AccountingTree1.NodeName + '</td>'
                                    + '<td style="width: 16.66%; text-align: center;">' + St.Amount + '</td>'
                                    + '<td style="width: 16.66%; text-align: center;">' + St.Statement + '</td>'
                                    + '<td style="width: 16.66%; text-align: center;">' + FullDatee + '</td>'
                                    + '<td style="width: 16.66%; text-align: center;">' + St.Notes + '</td>'
                                + '</tr>');
                    });
                    $('#AreaToAppend').append('<tr>'
                                    + '<td style="width: 16.66%; text-align: center; font-weight:bold;"></td>'
                                    + '<td style="width: 16.66%; text-align: center; font-weight:bold;" colspan="5"></td>'
                                + '</tr>');
                    $('#AreaToAppend').append('<tr>'
                                    + '<td style="width: 16.66%; text-align: center; font-weight:bold;">الصافي</td>'
                                    + '<td style="width: 16.66%; text-align: center; font-weight:bold;" colspan="5">' + Total + '</td>'
                                + '</tr>');
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


function convertDate(inputFormat) {
    var d = new Date(inputFormat);
    return [d.getDate(), d.getMonth() + 1, d.getFullYear()].join('/');
}
