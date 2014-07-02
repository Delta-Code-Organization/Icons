var ValidatePercentage = 0;
var UnitID = 0;
var TotOfTot = 0;

$(document).ready(function () {
    GetUnits($('#cprojectid').val());
});

$(document).ready(function () {
    $('#ContractCreate').submit(function (event) {
        if (ValidatePercentage != 100) {
            alert("مجموع نسبة الملاك يجب ان تكون 100%");
            return false;
        }
        if ($(this).parsley('validate')) {
            $.ajax({
                type: 'post',
                url: '/Contract/CreateContract',
                data: $(this).serialize(),
                success: function (data) {
                    $.gritter.add({
                        title: '! نجاح العملية',
                        text: ". تم اضافة العقد بنجاح",
                        image: '/content/images/user-icon.png',
                        class_name: 'clean',
                        time: '120000'
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

function GetUnits(id) {
    $('#OPTUnits').empty();
    $.ajax({
        url: '/Contract/GetUnits',
        type: 'post',
        data: { 'id': id },
        success: function (data) {
            $.each(data, function (index, u) {
                $('#OPTUnits').append('<option value="' + u.Id + '">' + u.DisplayText + '</option>');
            });
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}

function AddOwner() {
    if (ValidatePercentage == 100) {
        alert('لا يمكنك اضافة ملاك اخرين حيث ان النسبة تساوي 100%');
        return;
    }
    var Cus = $('#customerid').val();
    var CusName = $("#customerid option:selected").text();
    var Per = $('#percentage').val();
    if ((ValidatePercentage + parseInt(Per)) > 100) {
        alert("مجموع نسب الملاك لا يمكن ان تتخطي او تقل عن 100%");
        return;
    }
    if ($("#resoptgrp option[value='" + Cus + "']").length > 0) {
        alert("هذا العميل هو احد الملاك بالفعل .... لا يمكن اختيارة مرة اخري !");
        return;
    }
    $.ajax({
        url: '/Contract/AddOwner',
        type: 'post',
        data: { 'CusID': Cus, 'Percentage': Per },
        success: function (data) {
            $('#CustomerTableBody').append('<tr id="' + Cus + '">'
                                          + '<td style="width: 25%; text-align: center;">'
                                          + '<button type="button" class="btn btn-danger" onclick="DeleteOwner(' + Cus + ',' + Per + ')"><i class="fa fa-times-circle"></i> حذف</button>'
                                          + '</td>'
                                          + '<td style="width: 37.5%; text-align: center;">' + Per + '</td>'
                                          + '<td style="width: 37.5%; text-align: center;">' + CusName + '</td>'
                                          + '</tr>');
            $('#resoptgrp').append('<option value="' + Cus + '">' + CusName + '</option>');
            ValidatePercentage += parseInt(Per);
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}

function DeleteOwner(id, per) {
    $.ajax({
        url: '/Contract/DeleteOwner',
        type: 'post',
        data: { 'CusID': id },
        success: function (data) {
            $('#CustomerTableBody').find('#' + id).fadeOut(500);
            $("#resoptgrp option[value='" + id + "']").remove();
            ValidatePercentage -= parseInt(per);
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}

function CalcInstallmentAmount() {
    var InstallNum = parseInt($('#INum').val());
    if (isNaN(InstallNum)) {
        $('#INum').val(0);
    }
    if (InstallNum == 0) {
        return;
    }
    var Remain = parseFloat($('#cremaining').val());
    var SingleInstallAmount = (Remain / InstallNum);
    $('#SingleInstallVal').val(SingleInstallAmount);
}

function CalcRemaining() {
    var price = parseInt($('#cprice').val());
    var paid = parseInt($('#cpaid').val());
    if (isNaN(paid)) {
        paid = 0;
        $('#cpaid').val('0');
    }
    if (price < paid) {
        $.gritter.add({
            title: '! خطأ',
            text: ". لا يمكن ان يكون المدفوع اقل من السعر !",
            image: '/content/images/user-icon.png',
            class_name: 'clean',
            time: '3000'
        });
        $('#cpaid').val('');
        $('#cremaining').val('');
        return false;
    }
    var Remaining = price - paid;
    $('#cremaining').val(Remaining);
    CalcInstallmentAmount();
}

function SubmitForm() {
    $('#ContractCreate').submit();
}

function convertDate(inputFormat) {
    var d = new Date(inputFormat);
    return [d.getDate(), d.getMonth() + 1, d.getFullYear()].join('/');
}

//$(document).ready(function () {
//    $('#SIForm').submit(function (event) {
//        if ($(this).parsley('validate')) {
//            $.ajax({
//                type: 'post',
//                url: '/Contract/SearchInstallments',
//                data: $(this).serialize(),
//                success: function (data) {
//                    $('#accordion3').empty();
//                    $.each(data, function (index, Cus) {
//                        var Tot = 0;
//                        $('#accordion3').append('<div class="panel panel-default">'
//                        + '<div class="panel-heading">'
//                            + '<h4 class="panel-title">'
//                                + '<a data-toggle="collapse" data-parent="#accordion3" href="#TheCounter' + index + '">'
//                                    + '<i class="fa fa-angle-left"></i>&nbsp ' + Cus.Name + ''
//                                + '</a>'
//                            + '</h4>'
//                        + '</div>'
//                        + '<div id="TheCounter' + index + '" class="panel-collapse collapse">'
//                            + '<div class="panel-body">'
//                                + '<div class="block">'
//                                    + '<div class="content no-padding ">'
//                                        + '<ul class="items" id="' + index + '">'
//                                        + '</ul>'
//                                    + '</div>'
//                                    + '<div class="total-data bg-blue">'
//                                        + '<h2>إجمالي الأقساط المدفوعة <span class="pull-left" id="Total' + index + '"></span></h2>'
//                                        + '<h2>إجمالي الأقساط المتبقية <span class="pull-left" id="Rem' + index + '"></span></h2>'
//                                    + '</div>'
//                                + '</div>'
//                            + '</div>'
//                        + '</div>'
//                    + '</div>');
//                        $.each(Cus.Installments, function (inde, Ins) {
//                            var millii = Ins.DueDate.replace(/\/Date\((-?\d+)\)\//, '$1');
//                            var DateTimee = new Date(parseInt(millii));
//                            var Dayy = DateTimee.getDate();
//                            var yearr = DateTimee.getFullYear();
//                            var mounthh = DateTimee.getMonth() + 1;
//                            var FullDatee = (mounthh + "/" + Dayy + "/" + yearr).toString();
//                            if (Ins.PaymentDate == "null" || Ins.PaymentDate == null) {
//                                $('#' + index).append('<li id="Installment' + Ins.Id + '">'
//                                                + '<i class="fa fa-calendar pull-right"></i>' + FullDatee + ' <span class="pull-left value" id="ThisCont' + Ins.Id + '">'
//                                                + '&nbsp' + Ins.Amount + '&nbsp'
//                                                + '<button class="btn btn-success" data-target="#mod-info" data-toggle="modal" type="button" onclick="SetInstallmentData(' + Ins.Id + ',' + Ins.Amount + ')">'
//                                                    + 'دفع'
//                                                + '</button>'
//                                                + '<button class="btn btn-primary" data-target="#mod-Edit" data-toggle="modal" type="button" onclick="SetInstallmentData2Edit(' + Ins.Id + ',' + mounthh + ',' + Dayy + ',' + yearr + ',' + Ins.Amount + ')">'
//                                                    + 'تعديل'
//                                                + '</button>'
//                                                + '</span>'
//                                                + '<small>&nbsp</small>'
//                                            + '</li>');
//                                TotOfTot += parseFloat(Ins.Amount);
//                            }
//                            else {
//                                Tot += parseFloat(Ins.Amount);
//                                TotOfTot = parseFloat(Ins.Amount);
//                                $('#' + index).append('<li>'
//                                                + '<i class="fa fa-calendar pull-right"></i>' + FullDatee + ' <span class="pull-left value">' + '&nbsp' + Ins.Amount + '&nbsp' + 'مدفوع' + '</span>'
//                                                + '<small>&nbsp</small>'
//                                            + '</li>');
//                            }
//                        });
//                        $('#Total' + index).text(Tot);
//                        var Remain = TotOfTot - Tot;
//                        $('#Rem' + index).text(Remain);
//                    });
//                },
//                error: function (data) {
//                    alert(data.responseText);
//                }
//            });
//            return false;
//        }
//        return false;
//    });
//});

function GetUnitType(ID) {
    switch (ID) {
        case 1:
            return "محل";
        case 2:
            return "شقة";
        case 3:
            return "فيلا";
        case 4:
            return "دوبلكس";
        case 5:
            return "ميزانين";
        case 6:
            return "روف";
        case 7:
            return "جراج";
        case 8:
            return "باكية";
        default:
            return "";
    }
}

$(document).ready(function () {
    $('#SIForm').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.ajax({
                type: 'post',
                url: '/Contract/SearchInstallments',
                data: $(this).serialize(),
                success: function (data) {
                    $('#accordion3').empty();
                    $.each(data, function (index, Con) {
                        var Tot = 0;
                        var CusName = "";
                        var UnitType = GetUnitType(Con.ProjectUnit.UnitType);
                        var RespFor;
                        $.each(Con.Installments, function (inde, Ins) {
                            CusName = Ins.Customer.Name;
                            RespFor = Ins.ResponsibleID;
                        });
                        $('#accordion3').append('<div class="panel panel-default">'
                        + '<div class="panel-heading">'
                            + '<h4 class="panel-title" style="margin-bottom:20px;">'
                                + '<a data-toggle="collapse" data-parent="#accordion3" href="#TheCounter' + index + '">'
                                    + '<i class="fa fa-angle-left"></i>&nbsp عقد بيع ' + UnitType + ' للعميل ' + CusName + ' في ' + Con.Project.ProjectName
                                + '</a>'
                                + '<button style="margin-top:10px; float:left;padding-bottom:10px;" class="btn btn-primary" data-target="#mod-Add" data-toggle="modal" type="button" onclick="SetInstallmentData2Add(' + Con.Id + ',' + RespFor + ')">'
                                + 'اضافة'
                                + '</button>'
                            + '</h4>'
                        + '</div>'
                        + '<div id="TheCounter' + index + '" class="panel-collapse collapse">'
                            + '<div class="panel-body">'
                                + '<div class="block">'
                                    + '<div class="content no-padding ">'
                                        + '<ul class="items" id="' + index + '">'
                                        + '</ul>'
                                    + '</div>'
                                    + '<div class="total-data bg-blue">'
                                        + '<h2>إجمالي الأقساط المدفوعة <span class="pull-left" id="Total' + index + '"></span></h2>'
                                        + '<h2>إجمالي الأقساط المتبقية <span class="pull-left" id="Rem' + index + '"></span></h2>'
                                    + '</div>'
                                + '</div>'
                            + '</div>'
                        + '</div>'
                    + '</div>');
                        $.each(Con.Installments, function (inde, Ins) {
                            var millii = Ins.DueDate.replace(/\/Date\((-?\d+)\)\//, '$1');
                            var DateTimee = new Date(parseInt(millii));
                            var Dayy = DateTimee.getDate();
                            var yearr = DateTimee.getFullYear();
                            var mounthh = DateTimee.getMonth() + 1;
                            var FullDatee = (mounthh + "/" + Dayy + "/" + yearr).toString();
                            if (Ins.PaymentDate == "null" || Ins.PaymentDate == null) {
                                $('#' + index).append('<li id="Installment' + Ins.Id + '">'
                                                + '<i class="fa fa-calendar pull-right"></i>' + FullDatee + ' <span class="pull-left value" id="ThisCont' + Ins.Id + '">'
                                                + '&nbsp' + Ins.Amount + '&nbsp'
                                                + '<button class="btn btn-success" data-target="#mod-info" data-toggle="modal" type="button" onclick="SetInstallmentData(' + Ins.Id + ',' + Ins.Amount + ')">'
                                                    + 'دفع'
                                                + '</button>'
                                                + '<button class="btn btn-primary" data-target="#mod-Edit" data-toggle="modal" type="button" onclick="SetInstallmentData2Edit(' + Ins.Id + ',' + mounthh + ',' + Dayy + ',' + yearr + ',' + Ins.Amount + ')">'
                                                    + 'تعديل'
                                                + '</button>'
                                                + '</span>'
                                                + '<small>&nbsp</small>'
                                            + '</li>');
                                TotOfTot += parseFloat(Ins.Amount);
                            }
                            else {
                                Tot += parseFloat(Ins.Amount);
                                TotOfTot = parseFloat(Ins.Amount);
                                $('#' + index).append('<li>'
                                                + '<i class="fa fa-calendar pull-right"></i>' + FullDatee + ' <span class="pull-left value">' + '&nbsp' + Ins.Amount + '&nbsp' + 'مدفوع' + '</span>'
                                                + '<small>&nbsp</small>'
                                            + '</li>');
                            }
                        });
                        $('#Total' + index).text(Tot);
                        var Remain = TotOfTot - Tot;
                        $('#Rem' + index).text(Remain);
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

function SetInstallmentData(id, amount) {
    $('#ISTID').val(id);
    $('#ISTAmount').val(amount);
}

function SetInstallmentData2Edit(id, month2, day2, year2, Amount) {
    $('#IDPop').val(id);
    var fulldate2 = month2 + '/' + day2 + '/' + year2;
    $('#DueDatePop').val(fulldate2);
    $('#AmountPop').val(Amount);
}

function SetInstallmentData2Add(ConID, CusID) {
    $('#ConID').val(ConID);
    $('#CusID').val(CusID);
}

function PayInstallment(id, amount, PaymentDate, RecAcc) {
    $.ajax({
        url: '/Contract/PayInstallment',
        type: 'post',
        data: { 'ID': id, 'PaymentDate': PaymentDate, 'RecAcc': RecAcc },
        success: function (data) {
            $('#ThisCont' + id).empty();
            $('#ThisCont' + id).text(amount);
            $.gritter.add({
                title: '! نجاح العملية',
                text: ". تم دفع القسط بنجاح",
                image: '/content/images/user-icon.png',
                class_name: 'clean',
                time: '4000'
            });
            $('#ClosePopBtn').click();
        },
        error: function (data) {
            alert(data.responseText);
        }
    });
}

$(document).ready(function () {
    $('#PayCurrentInstallment').submit(function (event) {
        if ($(this).parsley('validate')) {
            var PayId = $('#ISTID').val();
            var PayAmount = $('#ISTAmount').val();
            var PaymentDate = $('#PaymentDate').val();
            var RecAcc = $('#RecAcc').val();
            PayInstallment(PayId, PayAmount, PaymentDate, RecAcc);
        }
        return false;
    });
});

$(document).ready(function () {
    $('#EditCurrentInstallment').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.ajax({
                url: '/Contract/EditInstallment',
                type: 'post',
                data: $(this).serialize(),
                success: function (data) {
                    $.gritter.add({
                        title: '! نجاح العملية',
                        text: ". تم تعديل القسط بنجاح",
                        image: '/content/images/user-icon.png',
                        class_name: 'clean',
                        time: '4000'
                    });
                    $('#ClosePopBtn2').click();
                    location.reload();
                },
                error: function (data) {
                    alert(data.responseText);
                }
            });
        }
        return false;
    });
});


$(document).ready(function () {
    $('#AddCurrentInstallment').submit(function (event) {
        if ($(this).parsley('validate')) {
            $.ajax({
                url: '/Contract/AddInstallment',
                type: 'post',
                data: $(this).serialize(),
                success: function (data) {
                    $.gritter.add({
                        title: '! نجاح العملية',
                        text: ". تم اضافة القسط بنجاح",
                        image: '/content/images/user-icon.png',
                        class_name: 'clean',
                        time: '4000'
                    });
                    $('#ClosePopBtn3').click();
                    location.reload();
                },
                error: function (data) {
                    alert(data.responseText);
                }
            });
        }
        return false;
    });
});