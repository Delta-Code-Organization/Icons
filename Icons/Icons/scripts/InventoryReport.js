function convertDate(inputFormat) {
    var d = new Date(inputFormat);
    return [d.getDate(), d.getMonth() + 1, d.getFullYear()].join('/');
}

function RefreshReportTitle() {
    var Cat = $("#Cat option:selected").text();
    var Prod = $("#Prod option:selected").text();
    var Proj = $("#Proj option:selected").text();
    $('#CatLabel').text(Cat);
    $('#ProdLabel').text(Prod);
    $('#ProjLabel').text(Proj);
}

$(document).ready(function () {
    RefreshReportTitle();

    $('#InventoryReport').submit(function (event) {
        if ($(this).parsley('validate')) {
            $('#CLoader').css('display', 'block');
            $.ajax({
                type: 'post',
                url: '/Accounting/InventoryData',
                data: $(this).serialize(),
                success: function (data) {
                    RefreshReportTitle();
                    $('#TblToAppend').empty();
                    $.each(data, function (index, F) {
                        $('#TblToAppend').append('<tr>'
										+ '<td style="border: 1px solid #000;width:20%;text-align:center;">' + F.Product.ProductName + '</td>'
                                        + '<td style="border: 1px solid #000;width:20%;text-align:center;">' + F.Quantity + '</td>'
                                        + '<td style="border: 1px solid #000;width:20%;text-align:center;">' + F.Project.ProjectName + '</td>'
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