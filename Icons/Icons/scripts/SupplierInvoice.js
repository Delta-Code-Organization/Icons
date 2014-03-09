var Total = 0;
var Discount = 0;

function AddInvoiceLine() {
    var Prod = $('#prod').val();
    var Qty = $('#qty').val();
    var Price = $('#price').val();
    var ttotal = $('#total').val();
    var data = { 'Prod': Prod, 'Qty': Qty, 'Price': Price, 'Total': ttotal };
    $.ajax({
        url: '/Supplier/AddInvoiceLine',
        type: 'post',
        data: data,
        success: function (data) {
            $('#ProdEle').append('<tr id="' + data.Id + '">'
                            + '<td style="width: 30%;">' + data.Product.ProductName + '</td>'
                            + '<td>' + data.Qty + '</td>'
                            + '<td class="text-right">' + data.Price + '</td>'
                            + ' <td class="text-right" id="total' + data.Id + '">' + data.Total + '</td>'
                            + '<td>'
                            + '<button class="btn btn-danger Arabic" onclick="RemoveInvoiceLine(' + data.Id + ')">حذف</button></td>'
                        + '</tr>');
            Total += parseFloat(data.Total);
            UpdateTotal();
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}

function UpdateTotal()
{
    if (Total == 0) {
        Discount = 0;
        $('#DiscountOfDiscount').text("0");
        $('#Dis').val(Discount);
    }
    $('#TotalOfTotal').text(Total);
    UpdateNet();
}

function SetDiscount()
{
    if ($('#Dis').val() == "" || $('#Dis').val() == '0') {
        Discount = 0;
        UpdateDiscount();
    }
    else if ($('#Dis').val() > Total) {
        $.gritter.add({
            title: '! خطأ',
            text: ". لا يمكن ان يكون الخصم اكبر من او يساوي اجمالي قيمة الفاتورة",
            image: '/content/images/user-icon.png',
            class_name: 'clean',
            time: '5000'
        });
        $('#Dis').val(Discount);
    }
    else {
        Discount = parseFloat($('#Dis').val());
        UpdateDiscount();
    }
}

function UpdateDiscount()
{
    $('#DiscountOfDiscount').text(Discount);
    UpdateNet();
}
 
function UpdateNet()
{
    var Net = Total - Discount;
    $('#NetOfNet').text(Net);
}

function RemoveInvoiceLine(id) {
    $.ajax({
        url: '/Supplier/RemoveInvoiceLine',
        type: 'post',
        data: { 'id': id },
        success: function (data) {
            $('#' + id).slideToggle(500);
            setTimeout(function () {
                $('#' + id).remove();
            }, 500);
            $('#qty').val('');
            $('#price').val('');
            $('#total').val('');
            var totalToRemove = parseFloat($('#' + id).find('#total' + id).text());
            Total -= totalToRemove;
            UpdateTotal();
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}

function AddFullInvoice()
{
    var ISup = $('#sup').val();
    var IDate = $('#invoicedate').val();
    var IRef = $('#invoiceref').val();
    var IDis = $('#Dis').val();
    var ITotal = Total;
    var INet = $('#NetOfNet').text();
    var LineIds = "";
    $('#EleTbl > tbody  > tr').each(function (index, ele) {
        LineIds += $(ele).attr('id') + ",";
    });
    var data = { 'ISup': ISup, 'IDate': IDate, 'IRef': IRef, 'IDis': IDis, 'ITotal': ITotal, 'INet': INet, 'LineIds': LineIds };
    $.ajax({
        url: '/Supplier/AddFullInvoice',
        type: 'post',
        data: data,
        success: function (data) {
            $.gritter.add({
                title: '! نجاح العملية',
                text: ". تم تسجل الفاتورة بنجاح",
                image: '/content/images/user-icon.png',
                class_name: 'clean',
                time: '5000'
            });
            setTimeout(function () {
                location.reload();
            }, 2000);
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}

$(document).ready(function () {
    $('#SIForm').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            AddFullInvoice();
            return false;
        }
        return false;
    });
});