var Total = 0;
var Discount = 0;
var counter = 958935;

$(document).ready(function () {
    Total = parseFloat($('#TotalOfTotal').text());
    Discount = parseFloat($('#DiscountOfDiscount').text());
});

function GetProdPrice(ele) {
    var value = ele.options[ele.selectedIndex].value;
    $.ajax({
        url: '/Accounting/GetProdPrice',
        type: 'post',
        data: { 'ProdID': value },
        success: function (data) {
            $('#price').val(data);
            CalcCurrentLineTotal();
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}

function CalcCurrentLineTotal() {
    var Qty = parseFloat($('#qty').val());
    var LastPrice = parseFloat($('#price').val());
    var Tota = Qty * LastPrice;
    $('#total').val(Tota);
    if ($('#total').val() == "NaN" || $('#total').val() == NaN) {
        $('#total').val(0);
    }
}

function AddInvoiceLine() {
    var Prod = $('#prod').val();
    var Prodname = $("#prod option:selected").text();
    var Qty = $('#qty').val();
    var Price = $('#price').val();
    var ttotal = $('#total').val();
    $('#ProdEle').append('<tr id="' + counter + '">'
                    + '<td style="width: 30%;" id="lineprodname" prodidattr="' + Prod + '">' + Prodname + '</td>'
                    + '<td id="lineqty">' + Qty + '</td>'
                    + '<td class="text-right" id="lineprice">' + Price + '</td>'
                    + ' <td class="text-right" id="total' + counter + '">' + ttotal + '</td>'
                    + '<td>'
                    + '<button class="btn btn-danger Arabic" onclick="RemoveInvoiceLine(' + counter + ')">حذف</button></td>'
                + '</tr>');
    Total += parseFloat(ttotal);
    UpdateTotal();
    counter++;
}

function UpdateTotal() {
    if (Total == 0) {
        Discount = 0;
        $('#DiscountOfDiscount').text("0");
        $('#Dis').val(Discount);
    }
    $('#TotalOfTotal').text(Total);
    UpdateNet();
}

function SetDiscount() {
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

function UpdateDiscount() {
    $('#DiscountOfDiscount').text(Discount);
    UpdateNet();
}

function UpdateNet() {
    var Net = Total - Discount;
    $('#NetOfNet').text(Net);
}

function RemoveInvoiceLine(id) {
    if (confirm("هل انت متأكد انك تريد حذف هذا المنتج من الفاتورة ؟")) {
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
        $.ajax({
            url: '/Supplier/DeleteInvoiceLine',
            type: 'post',
            data: { 'ID': id },
            success: function (data) { },
            error: function (data) { alert(data.responseText); }
        });
    }
}

function AddFullInvoice() {
    var InvoiceLines = "";
    var ISup = $('#sup').val();
    var IDate = $('#invoicedate').val();
    var IRef = $('#invoiceref').val();
    var IDis = $('#Dis').val();
    var ITotal = Total;
    var INet = $('#NetOfNet').text();
    var proj = $('#acc').val();
    var ToAcc = $('#toacc').val();
    var LineIds = "";
    $('#EleTbl > tbody  > tr').each(function (index, ele) {
        if (index != 0) {
            var EleID = $(ele).attr('id');
            InvoiceLines += EleID + ",";
            InvoiceLines += $(ele).find('#lineprodname').attr('prodidattr') + ",";
            InvoiceLines += $(ele).find('#lineqty').text() + ",";
            InvoiceLines += $(ele).find('#lineprice').text() + ",";
            InvoiceLines += $(ele).find('#total' + EleID).text() + ",";
        }
    });
    var data = { 'ISup': ISup, 'IDate': IDate, 'projId': proj, 'IRef': IRef, 'IDis': IDis, 'ITotal': ITotal, 'INet': INet, 'LineIds': InvoiceLines, 'ToAcc': ToAcc };
    $.ajax({
        url: '/Supplier/EditFullInvoice',
        type: 'post',
        data: data,
        success: function (data) {
            $.gritter.add({
                title: '! نجاح العملية',
                text: ". تم تعديل الفاتورة بنجاح",
                image: '/content/images/user-icon.png',
                class_name: 'clean',
                time: '5000'
            });
            location.reload();
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