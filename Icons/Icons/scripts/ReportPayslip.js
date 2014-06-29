function convertDate(inputFormat) {
    var d = new Date(inputFormat);
    return [d.getDate(), d.getMonth() + 1, d.getFullYear()].join('/');
}

function RefreshReportTitle() {
    var From = $('#From').val();
    var To = $('#To').val();
    var EmpName = $("#Employee option:selected").text();
    $('#FromLabel').text(From);
    $('#ToLabel').text(To);
    $('#EmployeeNameLabel').text(EmpName);
}

function GetTypeName(T) {
    switch (T) {
        case 1:
            return "مكافأة";
        case 2:
            return "جزاء";
        case 3:
            return "دفع صافي الراتب للموظف";
    }
}

function GetSalaryType(T)
{
    switch (T) {
        case 1:
            return "يومي";
        case 2:
            return "نصف شهري";
        case 3:
            return "شهري";
        case 4:
            return "اسبوعي";
    }
}

$(document).ready(function () {
    RefreshReportTitle();

    $('#ReportPayslip').submit(function (event) {
        if ($(this).parsley('validate')) {
            $('#CLoader').css('display', 'block');
            $.ajax({
                type: 'post',
                url: '/Accounting/PayslipFilter',
                data: $(this).serialize(),
                success: function (data) {
                    RefreshReportTitle();
                    $('#TblToAppend').empty();
                    $.each(data, function (index, F) {
                        var milli = F.Date.replace(/\/Date\((-?\d+)\)\//, '$1');
                        var FtDate = new Date(parseInt(milli));
                        FtDate = FtDate.toDateString();
                        var TTT = GetTypeName(F.Type);
                        var SalaType = GetSalaryType(F.Employee.SalaryType);
                        $('#SalaryTypeLabel').text(SalaType);
                        $('#BasicSalaryLabel').text(F.Employee.BasicSalary);
                        $('#TblToAppend').append('<tr>'
										+ '<td style="border: 1px solid #000;width:33%;text-align:center;">' + FtDate + '</td>'
                                        + '<td style="border: 1px solid #000;width:33%;text-align:center;">' + TTT + '</td>'
                                        + '<td style="border: 1px solid #000;width:33%;text-align:center;">' + F.Amount + '</td>'
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
    //mywindow.document.write('<link rel="stylesheet" href="http://www.test.com/style.css" type="text/css" />');
    mywindow.document.write('<style type="text/css">.test { color:red; } </style></head><body>');
    mywindow.document.write(data); mywindow.document.write('</body></html>');
    mywindow.document.close(); mywindow.print();
}

function PrintReport() {
    PrintElem($('#DivToPrint'));
}