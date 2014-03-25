$(document).ready(function () {
    $('#WorkOrderReportForm').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.ajax({
                type: 'post',
                url: '/Accounting/FilterWorkOrderReport',
                data: $(this).serialize(),
                success: function (data) {
                    $.each(data, function (index, Stock) {
                        var millii = Stock.Date.replace(/\/Date\((-?\d+)\)\//, '$1');
                        var DateTimee = new Date(parseInt(millii));
                        var Dayy = DateTimee.getDate();
                        var yearr = DateTimee.getFullYear();
                        var mounthh = DateTimee.getMonth() + 1;
                        var FullDatee = mounthh + "/" + Dayy + "/" + yearr;
                        $('#ProdEle').append('<tr id="' + Stock.Id + '">'
                            + '<td>' + FullDatee
                            + '</td>'
                            + '<td>' + Stock.Quantity
                            + '</td>'
                        + '</tr>');
                    });
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