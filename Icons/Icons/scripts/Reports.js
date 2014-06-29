var DebitLabel;
var CreditLabel;
var BalanceLabel;

function convertDate(inputFormat) {
    var d = new Date(inputFormat);
    return [d.getDate(), d.getMonth() + 1, d.getFullYear()].join('/');
}

function RefreshReportTitle() {
    var AccName = $("#Acc option:selected").text();
    $('#RTitle').text('كشف حساب ' + AccName);
    var From = $('#From').val();
    var To = $('#To').val();
    $('#FromLabel').text(From);
    $('#ToLabel').text(To);
}

$(document).ready(function () {
    RefreshReportTitle();

    $('#SearchReport').submit(function (event) {
        if ($(this).parsley('validate')) {
            $('#CLoader').css('display', 'block');
            $.ajax({
                type: 'post',
                url: '/Accounting/SearchReportData',
                data: $(this).serialize(),
                success: function (data) {
                    RefreshReportTitle();
                    $('#TblToAppend').empty();
                    $.each(data, function (index, F) {
                        var milli = F.TransactionDate.replace(/\/Date\((-?\d+)\)\//, '$1');
                        var FtDate = new Date(parseInt(milli));
                        FtDate = FtDate.toDateString();
                        DebitLabel = F.DebitSum;
                        CreditLabel = F.CreditSum;
                        BalanceLabel = parseFloat(CreditLabel) - parseFloat(DebitLabel);
                        $('#TblToAppend').append('<tr>'
										+ '<td style="border: 1px solid #000;width:25%;text-align:center;">' + F.Debit + '</td>'
										+ '<td style="border: 1px solid #000;width:25%;text-align:center;">' + F.Credit + '</td>'
                                        + '<td style="border: 1px solid #000;width:25%;text-align:center;">' + F.Statement + '</td>'
                                        + '<td style="border: 1px solid #000;width:25%;text-align:center;">' + FtDate + '</td>'
									+ '</tr>');
                    });
                    $('#CreditLabel').text(CreditLabel);
                    $('#DebitLabel').text(DebitLabel);
                    $('#BalanceLabel').text(BalanceLabel);
                    $('#CLoader').css('display', 'none');
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

function PrintElem(elem) {
    Popup(jQuery(elem).html());
}

function Popup(data) {
    var mywindow = window.open('', 'my div', 'height=400,width=600');
    mywindow.document.write('<html><head><title></title>');
    mywindow.document.write('<link rel="stylesheet" href="http://www.test.com/style.css" type="text/css" />');
    mywindow.document.write('<style type="text/css">.test { color:red; } </style></head><body>');
    mywindow.document.write(data); mywindow.document.write('</body></html>');
    mywindow.document.close(); mywindow.print();
}

function PrintReport() {
    PrintElem($('#DivToPrint'));
}