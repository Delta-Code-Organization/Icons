function convertDate(inputFormat) {
    var d = new Date(inputFormat);
    return [d.getDate(), d.getMonth() + 1, d.getFullYear()].join('/');
}

function RefreshReportTitle() {
    var Cat = $("#Cat option:selected").text();
    var Prod = $("#Prod option:selected").text();
    var Proj = $("#Proj option:selected").text();
    var From = $('#From').val();
    var To = $('#To').val();
    $('#FromLabel').text(From);
    $('#ToLabel').text(To);
    $('#CatLabel').text(Cat);
    $('#ProdLabel').text(Prod);
    $('#ProjLabel').text(Proj);
}

function CheckType(T)
{
    switch (T) {
        case 1:
            return "عملية شراء";
        case 2:
            return "عملية بيع";
        case 3:
            return "امر عمل";
        case 4:
            return "ضبط مخزن";
        case 5:
            return "تعديل فاتورة شراء";
        case 6:
            return "تعديل فاتورة بيع";
        case 7:
            return "حذف فاتورة بيع";
        case 8:
            return "حذف فاتورة شراء";

    }
}

$(document).ready(function () {
    RefreshReportTitle();

    $('#ItemCardReport').submit(function (event) {
        if ($(this).parsley('validate')) {
            $('#CLoader').css('display', 'block');
            $.ajax({
                type: 'post',
                url: '/Accounting/ItemCardData',
                data: $(this).serialize(),
                success: function (data) {
                    RefreshReportTitle();
                    $('#TblToAppend').empty();
                    $.each(data, function (index, F) {
                        var Type = CheckType(F.Type);
                        var milli = F.Date.replace(/\/Date\((-?\d+)\)\//, '$1');
                        var FtDate = new Date(parseInt(milli));
                        FtDate = FtDate.toDateString();
                        $('#TblToAppend').append('<tr>'
										+ '<td style="border: 1px solid #000;width:20%;text-align:center;">' + F.Id + '</td>'
										+ '<td style="border: 1px solid #000;width:20%;text-align:center;">' + F.Product.ProductName + '</td>'
                                        + '<td style="border: 1px solid #000;width:20%;text-align:center;">' + F.Quantity + '</td>'
                                        + '<td style="border: 1px solid #000;width:20%;text-align:center;">' + FtDate + '</td>'
                                        + '<td style="border: 1px solid #000;width:20%;text-align:center;">' + Type + '</td>'
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
    var mywindow = window.open('', 'my div', 'height=700,width=1000');
    mywindow.document.write('<html><head><title></title>');
    //mywindow.document.write('<link rel="stylesheet" href="http://www.test.com/style.css" type="text/css" />');
    mywindow.document.write('<style type="text/css">.test { color:red; } </style></head><body>');
    mywindow.document.write(data); mywindow.document.write('</body></html>');
    mywindow.document.close(); mywindow.print();
}

function PrintReport() {
    PrintElem($('#DivToPrint'));
}

function LoadProdsByCats() {
    $('#Prod').empty();
    $.ajax({
        url: '/Accounting/GetProdsByCats',
        type: 'post',
        data: { 'Cat': $('#Cat').val() },
        success: function (data) {
            if (data != null) {
                $('#Prod').append('<option selected="selected" value="0">الكل</option>');
            }
            $.each(data, function (index, p) {
                $('#Prod').append('<option value="' + p.Id + '">' + p.ProductName + '</option>');
            });
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
    RefreshReportTitle();
}