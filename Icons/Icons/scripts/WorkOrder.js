var Total = 0;
var Discount = 0;
var counter = 500149;

$(document).ready(function () {
    var value = $('#prod').val();
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
});

function CalcCurrentLineTotal() {
    var Qty = parseFloat($('#qty').val());
    var LastPrice = parseFloat($('#price').val());
    var Tota = Qty * LastPrice;
    $('#total').val(Tota);
    if ($('#total').val() == "NaN" || $('#total').val() == NaN) {
        $('#total').val(0);
    }
}

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
    $('#' + id).slideToggle(500);
    setTimeout(function () {
        $('#' + id).remove();
    }, 500);
    var totalToRemove = parseFloat($('#' + id).find('#total' + id).text());
    Total -= totalToRemove;
    UpdateTotal();
}

$(document).ready(function () {
    $('#WorkOrderForm').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.gritter.removeAll();
            var InvoiceLines = "";
            var IDate = $('#Date').val();
            var IAcc = $('#Acc').val();
            var ITotal = Total;
            var IProjId = $('#ProjId').val();
            var Notes = $('#Notes').val();
            $('#EleTbl > tbody  > tr').each(function (index, ele) {
                if (index != 0) {
                    var EleID = $(ele).attr('id');
                    InvoiceLines += $(ele).find('#lineprodname').attr('prodidattr') + ",";
                    InvoiceLines += $(ele).find('#lineqty').text() + ",";
                    InvoiceLines += $(ele).find('#lineprice').text() + ",";
                    InvoiceLines += $(ele).find('#total' + EleID).text() + ",";
                }
            });
            var data = { 'Date': IDate, 'ProjId': IProjId, 'Acc': IAcc, 'Total': ITotal, 'Notes': Notes, 'LineIds': InvoiceLines };
            $.ajax({
                url: '/Accounting/SaveWorkOrder',
                type: 'post',
                data: data,
                success: function (data) {
                    $.gritter.add({
                        title: '! نجاح العملية',
                        text: ". تم تسجل الأمر بنجاح",
                        image: '/content/images/user-icon.png',
                        class_name: 'clean',
                        time: '5000'
                    });
                    $('#EleTbl > tbody  > tr').each(function (index, ele) {
                        if (index != 0) {
                            $(ele).remove();
                        }
                    });
                    $('#Date').val('');
                    $('#Date').val('');
                    $('#Notes').val('');
                    $('#Notes').text('');
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