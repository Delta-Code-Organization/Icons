$(document).ready(function () {
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
                    var da2en = 0;
                    var maden = 0;
                    $('#AreaToAppend').empty();
                    $('#Loader').css('display', 'none');
                    $.each(data, function (index, St) {
                        if ($('#Acc1').select2('data').text == St.AccountingTree.NodeName) {
                            da2en += St.Amount;
                        } else
                            maden += St.Amount;
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
                    Total = da2en - maden;
                    $('#AreaToAppend').append('<tr>'
                                    + '<td style="width: 16.66%; text-align: center; font-weight:bold;">دائن</td>'
                                    + '<td style="width: 16.66%; text-align: center; font-weight:bold;" colspan="5">' + da2en + '</td>'
                                + '</tr>');
                    $('#AreaToAppend').append('<tr>'
                                    + '<td style="width: 16.66%; text-align: center; font-weight:bold;"> مدين</td>'
                                    + '<td style="width: 16.66%; text-align: center; font-weight:bold;" colspan="5">' + maden + '</td>'
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
