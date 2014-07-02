var DebitLabel;
var CreditLabel;
var BalanceLabel;

function convertDate(inputFormat) {
    var d = new Date(inputFormat);
    return [d.getDate(), d.getMonth() + 1, d.getFullYear()].join('/');
}

function RefreshReportTitle() {
    var Cus = $("#Cus option:selected").text();
    var Status = $("#Status option:selected").text();
    var Proj = $("#Proj option:selected").text();
    $('#CusLabel').text(Cus);
    $('#ProjLabel').text(Proj);
    $('#StatusLabel').text(Status);
    var From = $('#From').val();
    var To = $('#To').val();
    $('#FromLabel').text(From);
    $('#ToLabel').text(To);
}

$(document).ready(function () {
    RefreshReportTitle();

    $('#SearchInstallmentsReport').submit(function (event) {
        if ($(this).parsley('validate')) {
            $('#CLoader').css('display', 'block');
            $.ajax({
                type: 'post',
                url: '/Accounting/GetInstallmentsReportData',
                data: $(this).serialize(),
                success: function (data) {
                    RefreshReportTitle();
                    $('#TblToAppend').empty();
                    $.each(data, function (index, F) {
                        var milli = F.DueDate.replace(/\/Date\((-?\d+)\)\//, '$1');
                        var FtDate = new Date(parseInt(milli));
                        FtDate = FtDate.toDateString();
                        var Paydate = "غير مدفوع";
                        if (F.PaymentDate != null) {
                            var millii = F.PaymentDate.replace(/\/Date\((-?\d+)\)\//, '$1');
                            Paydate = new Date(parseInt(millii));
                            Paydate = Paydate.toDateString();
                        }
                        var ConText = "عقد بيع " + F.ProjectUnit.Name + " في " + F.Project.ProjectName;
                        $('#TblToAppend').append('<tr>'
										+ '<td style="border: 1px solid #000;width:30%;text-align:center;">' + ConText + '</td>'
										+ '<td style="border: 1px solid #000;width:17.5%;text-align:center;">' + F.Customer.Name + '</td>'
                                        + '<td style="border: 1px solid #000;width:17.5%;text-align:center;">' + FtDate + '</td>'
                                        + '<td style="border: 1px solid #000;width:17.5%;text-align:center;">' + Paydate + '</td>'
                                        + '<td style="border: 1px solid #000;width:17.5%;text-align:center;">' + F.Amount + '</td>'
									+ '</tr>');
                    });
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