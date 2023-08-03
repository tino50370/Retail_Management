var product;
var action;
var vd;
var IsCredit;
var customerName;
var customerBalance;
var customerLimit;
var PaymentType = 'SALE';
var collecttionPoint;
var creditPeriod;
var Transaction;
var CashierSales;
var transactionList ="";
var customerSelected ='N';
var Company;
var Discount;
var tranName;
var currency;
var OrderReceipt;
var BaseBalance = 0;
var Balance = 0;
var Grandtotal = 0; //used for cash collection and end of shift 
var Partpayment = 0;
var partpaymentTotal = 0;
var OriginalTender = 0;
var BaseCurrency;
var Displaycurrency;
var Rate;


/// <reference path="../../Controllers/HomeController.cs" />

function setForm(data) {
   
    if ( product !== 'Credit' && product !== 'Tender' ) {
        CommitProduct();
    }
    product = data; 
}

function setParameter(data) {
    action = data;
}

function DiscountTrans(data,iscredit,tran,discount,Amount)
{
    if  (discount.trim() == "YES")
    {
        verifyDiscountTrans(data, iscredit, tran, discount,Amount);
    } else
    {
        setPayment(data, iscredit, tran)
    }
    
   
}
function YesDiscountTrans(data,iscredit,tran,type,Discount,Amount,discount)
{
    if (type == "PERCENT") {

        Amount = (Amount - (Discount / 100.0000) * Amount);

        setPayment(data, iscredit, tran,discount, Amount)
    }
    if (type == "AMOUNT") {

        Amount = Amount - Discount;
        setPayment(data, iscredit, tran,discount,Amount)

    }

}
function verifyDiscountTrans(data, iscredit, tran, discount,Amount) {
    if (tran != undefined) {
        $.ajax({
            type: "Get",
            url: "/Pos/TransDiscount/",
            dataType: 'html',
            data: {
                tran: tran,             
                discount: discount,
                Amount: Amount

            },
            success: function (resp) {
                $.fancybox(resp, {
                    helpers: {
                        overlay: {
                            closeClick: false
                        }
                    },
                    keys: {
                        close: null
                    },
                    fitToView: true,
                    closeBtn: false,
                    autoSize: true,
                    openEffect: 'none',
                    closeEffect: 'none',
                    topRatio: 0
                });
            },
            error: function (data) {
                $.fancybox.hideLoading();
            }
        });
    }
}
function setPayment(data, iscredit, tran, discount,Amount) {
    if(PaymentType == 'SALE')
    {
        if (discount == "NO")
        {
            
            Savesale(data, iscredit, tran, discount, Amount);
        } else
        {
            Savesale(data, iscredit, tran, discount, Amount);
        }

    }else
    {
        Savepayment(data, iscredit, tran);
    }
}

function Savesale(data, iscredit, tran,discount,Amount)
{
    var tender = "";
    var first = "Y";
    var sp = transactionList.split(',')
    if (sp.length > 1 )
    {
        first = "N";
    }
   // GetTender(data, iscredit, tran);
    if (IsCredit == "N") {
       // if (tran == 'CASH SALES') {
        if (discount == "") {
            tender = $("#Cashq").val();
        }
        else if (discount.trim() == "YES") {
            tender = Amount;
            BaseBalance = Amount;
        }
        else
        {
            tender = Amount;
        }
       // }else
       // {
        //    tender = $("#total").text();
        //}
    }
    else
    {
        if (tran == "External") {
            tender = $("#Amount").val();
        }
        else if (discount == "") {
            tender = $("#Cashq").val();
        }
        else if (discount.trim() == "YES") {
            tender = Amount;
            BaseBalance = Amount;
        }
        else
        {
            tender = Amount;

        }
        
    }
    if (tran == "External") {
        tender = $("#Amount").val();
    }
    if ( Discount == "Y") {

        Discount = $("#Discountq").val();
    }
    else
    {
        Discount = "0";
    }
   // alert(tender);
    if ((first == "Y" && (parseFloat(tender) + parseFloat(partpaymentTotal)) >= parseFloat(BaseBalance))|| (first == "N" && (parseFloat(tender) + parseFloat(partpaymentTotal)) == parseFloat(BaseBalance)))
    {
        var change = (parseFloat(tender) + parseFloat(partpaymentTotal)) - parseFloat(BaseBalance);
        var oneg = tran.split("_");
        Transaction = oneg[0];
        transactionList = transactionList + ',' + Transaction + "-" + parseFloat(tender) + "-" + IsCredit + "-" + currency;
        tender = parseFloat(BaseBalance);
        $("#caption").text("CHANGE");
        $("#total").text(change.toFixed(2));
        var dat = '';
        $('#product_lines > tbody  > tr').each(function () {
            var description = $(this).find(".desc").text();
            var amount = $(this).find(".amnt").text();
            var quantity = $(this).find(".qty").text();
            var code = $(this).attr('id');

            if (dat == '') {
                dat = description + "," + quantity + "," + amount + "," + code
            }
            else {
                dat = dat + "/" + description + "," + quantity + "," + amount + "," + code
            }
        });

        $.ajax({
            type: "Post",
            url: '/Pos/Print',
            dataType: 'html',
            data: {
                item: dat,
                tender: tender,
                change: change.toFixed(2),
                customer: customerName,
                creditPeriod: creditPeriod,
                IsCredit: IsCredit,
                Discount: Discount,
                Currency: currency,
                OrderReceipt: $('#OrderReceipt').val(),
                collectionPoint: collecttionPoint,
                BaseTotal: BaseBalance,
                tr: transactionList
            },
            success: function (resp) {
                if (resp == "Customer Required") {
                    $(".alert-danger").html("Customer data is required");
                    $(".alert-danger").show();
                } else {
                    $("#toprint").html(resp);
                    setTimeout(function () { PrintDiv() }, 2000);
                    $.fancybox.close();
                }
            },
            error: function (data) {
                //alert(data);
            }
        });
    }
    else if ((first == "Y" && (parseFloat(tender) + parseFloat(partpaymentTotal)) >= parseFloat(BaseBalance)) && discount == "YES" || (first == "N" && (parseFloat(tender) + parseFloat(partpaymentTotal)) == parseFloat(BaseBalance)) && discount == "YES") {
        
        var change = (parseFloat(tender) + parseFloat(partpaymentTotal)) - parseFloat(BaseBalance);
        var oneg = tran.split("_");
        Transaction = oneg[0];
        transactionList = transactionList + ',' + Transaction + "-" + parseFloat(tender) + "-" + IsCredit + "-" + currency;
        tender = parseFloat(BaseBalance);
        $("#caption").text("CHANGE");
        $("#total").text(change.toFixed(2));
        var dat = '';
        $('#product_lines > tbody  > tr').each(function () {
            var description = $(this).find(".desc").text();
            var amount = $(this).find(".amnt").text();
            var quantity = $(this).find(".qty").text();
            var code = $(this).attr('id');

            if (dat == '') {
                dat = description + "," + quantity + "," + amount + "," + code
            }
            else {
                dat = dat + "/" + description + "," + quantity + "," + amount + "," + code
            }
        });

        $.ajax({
            type: "Post",
            url: '/Pos/Print',
            dataType: 'html',
            data: {
                item: dat,
                tender: tender,
                change: change.toFixed(2),
                customer: customerName,
                creditPeriod: creditPeriod,
                IsCredit: IsCredit,
                Discount: Discount,
                Currency: currency,
                OrderReceipt: $('#OrderReceipt').val(),
                collectionPoint: collecttionPoint,
                BaseTotal: BaseBalance,
                tr: transactionList
            },
            success: function (resp) {
                if (resp == "Customer Required") {
                    $(".alert-danger").html("Customer data is required");
                    $(".alert-danger").show();
                } else {
                    $("#toprint").html(resp);
                    setTimeout(function () { PrintDiv() }, 2000);
                    $.fancybox.close();
                }
            },
            error: function (data) {
                //alert(data);
            }
        });
    }

    else {
        if ((parseFloat(tender) + parseFloat(partpaymentTotal)) < parseFloat(BaseBalance))//$("#total").text())
        {
            var g = tran.split("_");
            Transaction = g[0];
            Balance = parseFloat(BaseBalance) - (parseFloat(tender) + parseFloat(Partpayment));
            Partpayment = tender;
            partpaymentTotal = parseFloat(partpaymentTotal) + parseFloat(Partpayment);
            transactionList = transactionList + ',' + Transaction + "-" + Partpayment + "-" + IsCredit + "-" + currency;
            
            GetPartialView('/Pos/Tender', 'Cash', 'N', 'Cash Sale');
        }
        else 
        {
            $(".alert-danger").html("split payments can not give out change. Check Your amount and click enter.");
             $(".alert-danger").show();
        }
    }
}

function Savepayment(data, iscredit, tran) {
    var tender = "";
    var first = "Y";
    if (transactionList !== '' || transactionList !== undefined) {
        first = "N";
    }
    // GetTender(data, iscredit, tran);
    if (IsCredit == "N") {
        // if (tran == 'CASH SALES') {
        tender = $("#Cashq").val();
        // }else
        // {
        //    tender = $("#total").text();
        //}
    }
    else {
        tender = $("#Cashq").val();
    }
    if (Discount == "Y") {

        Discount = $("#Discountq").val();
    }
    else {
        Discount = "0";
    }
    // alert(tender);
    if ((first == "Y" && (parseFloat(tender) + parseFloat(partpaymentTotal)) >= parseFloat(OriginalTender)) || (first == "N" && (parseFloat(tender) + parseFloat(partpaymentTotal)) == parseFloat(OriginalTender))) {
        var change = (parseFloat(tender) + parseFloat(partpaymentTotal)) - parseFloat(OriginalTender);
        var oneg = tran.split("_");
        Transaction = oneg[0];
        transactionList = transactionList + ',' + Transaction + "-" + parseFloat(tender) + "-" + IsCredit + "-" + currency;
        tender = parseFloat(OriginalTender);
        //$("#caption").text("CHANGE");
       // $("#total").text(change.toFixed(2));
        var dat = '';
       
        $.ajax({
            type: "Post",
            url: '/Pos/CreditPayment',
            dataType: 'html',
            data: {
                item: dat,
                tender: parseFloat(OriginalTender),
                change: change.toFixed(2),
                customer: customerName,
                paymentType: PaymentType,
                creditPeriod: creditPeriod,
                IsCredit: IsCredit,
                Discount: Discount,
                OrderReceipt: collecttionPoint,
                tr: transactionList
            },
            success: function (resp) {
                $("#toprint").html(resp);
                setTimeout(function () { PrintDiv() }, 2000);
                $.fancybox.close();
            },
            error: function (data) {
                //alert(data);
            }
        });
    }
    else if ((first == "Y" && (parseFloat(tender) + parseFloat(partpaymentTotal)) >= parseFloat(OriginalTender))  || (first == "N" && (parseFloat(tender) + parseFloat(partpaymentTotal)) == parseFloat(OriginalTender)))
    {
        var percent = 10;
        var change = (parseFloat(tender) + parseFloat(partpaymentTotal)) - parseFloat(OriginalTender);
        var oneg = tran.split("_");
        Transaction = oneg[0];
        transactionList = transactionList + ',' + Transaction + "-" + parseFloat(tender) + "-" + IsCredit + "-" + currency;
        tender = parseFloat(OriginalTender);
        //$("#caption").text("CHANGE");
        // $("#total").text(change.toFixed(2));
        var dat = '';

        $.ajax({
            type: "Post",
            url: '/Pos/CreditPayment',
            dataType: 'html',
            data: {
                item: dat,
                tender: parseFloat(OriginalTender),
                change: change.toFixed(2),
                customer: customerName,
                paymentType: PaymentType,
                creditPeriod: creditPeriod,
                IsCredit: IsCredit,
                Discount: Discount,
                OrderReceipt: collecttionPoint,
                tr: transactionList
            },
            success: function (resp) {
                $("#toprint").html(resp);
                setTimeout(function () { PrintDiv() }, 2000);
                $.fancybox.close();
            },
            error: function (data) {
                //alert(data);
            }
        });
    }
    else {
        if ((parseFloat(tender) + parseFloat(partpaymentTotal)) < parseFloat(OriginalTender)) {
            var g = tran.split("_");
            Transaction = g[0];
            Balance = parseFloat(OriginalTender) - (parseFloat(tender) + parseFloat(Partpayment));
            Partpayment = tender;
            partpaymentTotal = parseFloat(partpaymentTotal) + parseFloat(Partpayment);
            transactionList = transactionList + ',' + Transaction + "-" + Partpayment + "-" + IsCredit + "-" + currency;

            GetPartialView('/Pos/Tender?PaymentType=' + PaymentType, 'Other', 'N', 'Cash Sale');
        }
        else {
            $(".alert-danger").html("split payments can not give out change");
            $(".alert-danger").show();
        }
    }
}

function Saveorder(data, iscredit, tran) {
    var cc = 0;
    $('#product_lines > tbody  > tr').each(function () {
        cc=cc+1
    });
    if (cc > 0) {
        var tender = "";
        tender = $("#total").text();
        Discount = "0";
        // alert(tender);
        if (tender >= parseFloat($("#total").text())) {
            var change = parseFloat(tender) - parseFloat($("#total").text());
            $("#caption").text("CHANGE");
            $("#total").text(change.toFixed(2));
            var dat = '';
            $('#product_lines > tbody  > tr').each(function () {
                var description = $(this).find(".desc").text();
                var amount = $(this).find(".amnt").text();
                var quantity = $(this).find(".qty").text();
                var code = $(this).attr('id');

                if (dat == '') {
                    dat = description + "," + quantity + "," + amount + "," + code
                }
                else {
                    dat = dat + "/" + description + "," + quantity + "," + amount + "," + code
                }
            });

            $.ajax({
                type: "Post",
                url: '/Pos/SaveOrder',
                dataType: 'html',
                data: {
                    item: dat,
                    tender: tender,
                    receipt: $("#OrderReceipt").val(),
                    change: change.toFixed(2),
                    customer: customerName,
                    creditPeriod: creditPeriod,
                    IsCredit: IsCredit,
                    Discount: Discount,
                    tr: Transaction
                },
                success: function (resp) {
                    $.fancybox(resp, {
                        helpers: {
                            overlay: {
                                closeClick: false
                            }
                        },
                        keys: {
                            close: null
                        },
                        fitToView: true,
                        closeBtn: false,
                        autoSize: true,
                        openEffect: 'none',
                        closeEffect: 'none',
                        topRatio: 0
                    });
                    setTimeout(function () { location.reload() }, 3000);
                },
                error: function (data) {
                    //alert(data);
                }
            });
        }
        else {
            $(".alert-danger").html("Amount Tendered is too low");
            $(".alert-danger").show();
        }
    }
}

function SaveCreditnote() {
    var cc = 0;
    $('#product_lines > tbody  > tr').each(function () {
        cc = cc + 1
    });
    if (cc > 0) {
        var tender = "";
        tender = $("#total").text();
        Discount = "0";
        // alert(tender);
        if (tender >= parseFloat($("#total").text())) {
            var change = parseFloat(tender) - parseFloat($("#total").text());
            $("#caption").text("CHANGE");
            $("#total").text(change.toFixed(2));
            var dat = '';
            $('#product_lines > tbody  > tr').each(function () {
                var description = $(this).find(".desc").text();
                var amount = $(this).find(".amnt").text();
                var quantity = $(this).find(".qty").text();
                var code = $(this).attr('id');

                if (dat == '') {
                    dat = description + "," + quantity + "," + amount + "," + code
                }
                else {
                    dat = dat + "/" + description + "," + quantity + "," + amount + "," + code
                }
            });

            $.ajax({
                type: "Post",
                url: '/Pos/SaveCreditnote',
                dataType: 'html',
                data: {
                    item: dat,
                    tender: tender,
                    receipt: $("#OrderReceipt").val(),
                    change: change.toFixed(2),
                    customer: customerName,
                    creditPeriod: creditPeriod,
                    IsCredit: IsCredit,
                    Discount: Discount,
                    tr: Transaction
                },
                success: function (resp) {
                    if (resp !== "Error") {
                        $("#toprint").html(resp);
                        setTimeout(function () { location.reload() }, 3000);
                    } else {
                        alert("Return Failed");
                    }
                },
                error: function (data) {
                    //alert(data);
                }
            });
        }
        else {
            $(".alert-danger").html("Amount Tendered is too low");
            $(".alert-danger").show();
        }
    }
}

function SaveCashcollection(supervisor) {
    var cc = 0;
    var dat;
    $('#Collection_lines > tbody  > tr').each(function () {
        cc = cc + 1
    });
    if (cc > 0 && ($("#suppass").val() !== undefined || $("#suppass").val() != "")) {
       
            Balance = 0;
            $('#Collection_lines > tbody  > tr').each(function () {
                var currency = $(this).find(".curr").text();
                var amount = $(this).find(".amnt").text();
                var quantity = $(this).find(".qty").text();
                var denomination = $(this).find(".den").text();
                if (amount !== '') {
                    if (dat == undefined) {
                        dat = currency + "," + quantity + "," + amount + "," + denomination
                    }
                    else {
                        dat = dat + "/" + currency + "," + quantity + "," + amount + "," + denomination
                    }
                    Balance = Balance + parseFloat(amount);
                }
            });

            $.ajax({
                type: "Post",
                url: '/Pos/CashCollection',
                dataType: 'html',
                data: {
                    lines: dat,
                    total:Balance,
                    supervisor: supervisor
                },
                success: function (resp) {
                  
                    var re = resp.split(':');
                    var fi = re[0].split('"');
                    if (fi[0].trim() == "Error") {
                        $(".alert-danger").html(re[1]);
                        $(".alert-danger").show();
                    }
                    else if (fi[0].trim() == "Success") {
                        $(".alert-success").html(re[1]);
                        $(".alert-success").show();
                        
                        
                    }
                    
                },
                error: function (data) {
                    //alert(data);
                }
            });
        }
        else {
            $(".alert-danger").html("Enter Supervisor password");
            $(".alert-danger").show();
    }
    
    $.fancybox.close();
}

function SaveXcollection(supervisor) {
    var cc = 0;
    var dat;
    $('#Collection_lines > tbody  > tr').each(function () {
        cc = cc + 1
    });
    if (cc > 0 && ($("#suppass").val() !== undefined || $("#suppass").val() != "")) {

        Balance = 0;
        $('#Collection_lines > tbody  > tr').each(function () {
            var currency = $(this).find(".curr").text();
            var amount = $(this).find(".amnt").text();
            var quantity = $(this).find(".qty").text();
            var denomination = $(this).find(".den").text();
            if (amount !== '') {
                if (dat == undefined) {
                    dat = currency + "," + quantity + "," + amount + "," + denomination
                }
                else {
                    dat = dat + "/" + currency + "," + quantity + "," + amount + "," + denomination
                }
                Balance = Balance + parseFloat(amount);
            }
        });

        $.ajax({
            type: "Post",
            url: '/Pos/XCollection',
            dataType: 'html',
            data: {
                lines: dat,
                total: Balance,
                supervisor: supervisor
            },
            success: function (resp) {
                var re = resp.split(':');
                var fi = re[0].split('"');
                if (fi[0].trim() == "Error") {
                    $(".alert-danger").html(re[1]);
                    $(".alert-danger").show();
                }
                else if (fi[0].trim() == "Success") {
                    $(".alert-success").html(re[1]);
                    $(".alert-success").show();
                }

            },
            error: function (data) {
                //alert(data);
            }
        });
    }
    else {
        $(".alert-danger").html("Enter Supervisor password");
        $(".alert-danger").show();
    }
    $.fancybox.close();
}

function newCollectionLine() {
    var error = 'No';
    if ($('#currencyq').val() == undefined || $('#currencyq').val() == '')
    {
        $('#currency_error').html("Currency field is required");
        error = "Yes";
    }
    if ($('#denominationq').val() == undefined || $('#denominationq').val() == '') {
        $('#denomination_error').html("Denomination field is required");
        error = "Yes";
    }
    if ($('#quantityq').val() == undefined || $('#quantityq').val() == '') {
        $('#quantity_error').html("Quantity field is required");
        error = "Yes";
    }
    if (error == "No") {
        var line = $('#Collection_lines tr').length;
        if (line == 2)
        { Grandtotal = 0;}
        var amnt = (parseFloat($('#denominationq').val()).toFixed(2) * parseFloat($('#quantityq').val()).toFixed(2))
        Grandtotal = Grandtotal + amnt;
       
       $('#Collection_lines > tbody:first').append('<tr id="' + line + '" style=" cursor: pointer;border-bottom-color: White; border-left-style:none; border-right-style: none">' +
       '<td id="' + line + 'c" class= "curr" style="width: 30%; border-bottom-color: White; border-left-style:none; border-right-style: none">' + $('#currencyq').val() + '</td>' +
       '<td id="' + line + 'd" class="den" style="width: 30%;border-bottom-color: White; border-left-style:none; border-right-style: none">' + $('#denominationq').val() + '</td>' +
       '<td id="' + line + 'q" class="qty" style="width: 30%;border-bottom-color: White; border-left-style:none; border-right-style: none">' + $('#quantityq').val() + '</td>' +
       '<td id="' + line + 'a" class="amnt" style="width: 40%; border-bottom-color: White; border-left-style:none; border-right-style: none">' + amnt + '</td>' +
      '</tr>');
       $('#GrandTotal').html(Grandtotal);
      // $('#currencyq').val('');
       $('#quantityq').val('');
       $('#denominationq').val('');
    }

}

function newXCollectionLine() {
    var error = 'No';
    if ($('#currencyq').val() == undefined || $('#currencyq').val() == '') {
        $('#currency_error').html("Currency field is required");
        error = "Yes";
    }
    if ($('#denominationq').val() == undefined || $('#denominationq').val() == '') {
        $('#denomination_error').html("Denomination field is required");
        error = "Yes";
    }
    if ($('#quantityq').val() == undefined || $('#quantityq').val() == '') {
        $('#quantity_error').html("Quantity field is required");
        error = "Yes";
    }
    if (CashierSales !== undefined) {
        var exp;
        $.each(CashierSales, function (index, value) {
            if (value.AccountName.trim() === $('#currencyq').val())
            {
                exp = value.Sales;
            }
        });
        //var exp = CashierSales.find(x => x.AccountName.trim() === $('#denominationq').val()).Sales;
        var myAmnt = (parseFloat($('#denominationq').val()).toFixed(2) * parseFloat($('#quantityq').val()).toFixed(2))
        if (parseFloat(myAmnt) >= parseFloat(exp))
        {
            error = "No";
        }
        else {
            $('#quantity_error').html("Amount is not Balancing");
            error = "Yes";
        }
        
    }

    if (error == "No") {
        var line = $('#Collection_lines tr').length;
        if (line == 2)
        { Grandtotal = 0; }
        var amnt = (parseFloat($('#denominationq').val()).toFixed(2) * parseFloat($('#quantityq').val()).toFixed(2))
        Grandtotal = Grandtotal + amnt;

        $('#Collection_lines > tbody:first').append('<tr id="' + line + '" style=" cursor: pointer;border-bottom-color: White; border-left-style:none; border-right-style: none">' +
        '<td id="' + line + 'c" class= "curr" style="width: 30%; border-bottom-color: White; border-left-style:none; border-right-style: none">' + $('#currencyq').val() + '</td>' +
        '<td id="' + line + 'd" class="den" style="width: 30%;border-bottom-color: White; border-left-style:none; border-right-style: none">' + $('#denominationq').val() + '</td>' +
        '<td id="' + line + 'q" class="qty" style="width: 30%;border-bottom-color: White; border-left-style:none; border-right-style: none">' + $('#quantityq').val() + '</td>' +
        '<td id="' + line + 'a" class="amnt" style="width: 40%; border-bottom-color: White; border-left-style:none; border-right-style: none">' + amnt + '</td>' +
       '</tr>');
        $('#GrandTotal').html(Grandtotal);
        // $('#currencyq').val('');
        $('#quantityq').val('');
        $('#denominationq').val('');
    }

}

function LoadSale(receipt) {

        $.ajax({
            type: "Get",
            url: '/Pos/getSalesDetails',
            dataType: 'html',
            data: {
                receipt: receipt
            },
            success: function (resp) {
                
                $("#toprint").html(resp);
                setTimeout(function () { PrintDiv() }, 2000);
                $.fancybox.close();
    
            },
            error: function (data) {
                //alert(data);
            }
        });

}

function LoadOrders(receipt) {
    var tender = "";
    $("#OrderReceipt").val(receipt);
    OrderReceipt = receipt;
        $.ajax({
            type: "Post",
            url: '/Pos/getOrderDetails',
            dataType: 'html',
            data: {
                receipt: receipt
            },
            success: function (resp) {
                try {
                    $('#product_lines > tbody').html('');
                    $("#total").text('0.00');
                    var total = 0.00;
                    var list = $.parseJSON(resp);
                    $.each(list, function (index, value) {
                       
                        var description = value.item;
                        var amount = value.priceinc;
                        var quantity = value.quantity;
                        var id = value.ItemCode;
                        var tax = value.tax;
                        $('#product_lines > tbody:first').append('<tr id="' + id + '" style=" cursor: pointer;border-bottom-color: White; border-left-style:none; border-right-style: none"><td class= "desc" style="width: 50%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                                                     description + '</td><td id="' + id + 't" class="qty" style="width: 20%;font-size: 15px;border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                                                     quantity + '</td><td id="' + id + 'a" class="amnt" style="width: 30%;text-align: right;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                                                     parseFloat(amount).toFixed(2) + '</td><td style="display:none">' + tax + '</td><td id="' + id + 'stk" style="display:none">' + quantity + '</td></tr>');

                       // $("#" + $(this).ID + "q").focus();
                        product = id;
                        total = parseFloat(total) + parseFloat(amount);
                        $("#total").text(parseFloat(total).toFixed(2));
                        $("#grumbleq").val('');
                        var height = $('#details')[0].scrollHeight;
                        $('#details').scrollTop(height);
                    });
                  
                    $.fancybox.close();
                }
                catch (e)
                {
                    alert("There are no open orders");
                }
            },
            error: function (data) {
                //alert(data);
            }
        });
   
}

function GetTender(data, iscredit, tran) {

    var cc = 0;
    $('#product_lines > tbody  > tr').each(function () {
        cc = cc + 1
    });
    if (cc > 0 || data == "password") {
        IsCredit = iscredit;
        if (data != "total") {
            Transaction = tran;
        }
        setForm(data)

    } if (data == "password") {
        $("#" + product).focus();
    }
    else {
        $("#" + product + "q").focus();
    }

}

function PrintDiv() {
   //alert("printing");
    
   // divToPrint.show();
    //var output = document.getElementById("ifrOutput").contentWindow;
    //output.document.open();
    //output.document.write('<html><body>' + divToPrint.innerHTML + '</html>');
    //output.document.close();
    //output.focus();
    // output.print();
    var divToPrint = document.getElementById('toprint');
    for(i=0;i<2; i++)
    {
        qz.websocket.connect().then(function () {
            return qz.printers.find("zebra")               // Pass the printer name into the next Promise
        }).then(function (printer) {
            var config = qz.configs.create(printer);       // Create a default config for the found printer
            var data = '<html>' + divToPrint.innerHTML + '</html>';   // Raw html
            printData = [
                {
                    type: 'html',
                    format: 'plain',
                    data: data
                }
            ];
            return qz.print(config, printData);
        }).catch(function (e) { console.error(e); });
    }
   
    setTimeout(function () { location.reload() }, 6000);
}

function Authenticate() {

    $.ajax({
        type: "Post",
        url: '/Pos/Authenticate',
        dataType: 'html',
        data: {
            json_str: action
        },
        success: function (resp) {
            var res = $.parseJSON(resp)
            // $.fancybox.close();
            if ( res == "success") {
                if ( action == 'voidsale') {
                    location.reload();
                }
                else {
                    vd = action;
                }
                $.fancybox.close();
                $("#grumbleq").focus();
            }
            else {
                $("#error").text(resp);
            }
        },
        error: function (data) {
            //$.fancybox.close();
        }
    });  
}

function search() {
    if ($("#grumbleq").val() == '') {
        if (product == '' ||  product == 'grumble') {
            product = 'grumble';
            $("#grumbleq").focus();
        }
    }
    else {
        //alert("search");
        var JsonQuestion = {
            "Id": $("#grumbleq").val()
        };
        $.ajax({
            type: "Get",
            url: 'Pos/getPosItems',
            dataType: 'html',
            data: { json_str: $("#grumbleq").val() },
            success: function (resp) {
                
                try {
                    var G1 = $.parseJSON(data).split(",");
                } catch (e) {
                    G1 = "";
                    // alert(e);
                }
              
                if (G1.length == 4) {

                    var id;
                    var description;
                    var price;
                    var tax;

                    $.each(G1, function (i, item) {
                        if (i == 0) {
                            description = item;
                        }
                        if (i == 1) {
                            id = item;
                        }
                        if (i == 2) {
                            prize = item;
                        }
                        if (i == 3) {
                            tax = item;
                        }
                    });
                    if ($("#" + id).length) {
                        if (action == "voidline") {
                            var amt = $("#" + id + "a").text();
                            var total = parseFloat($("#total").text()) - amt;
                            $("#total").text(parseFloat(total).toFixed(2));
                            if ($("#total").text() == 'NaN') {
                                $("#total").text('0.00')
                            }
                            $("#" + id).remove();
                            action = "";
                            $("#grumbleq").val('');
                            $("#grumbleq").focus();
                        }
                        else {
                            var dd = parseFloat($("#" + id + "t").text())
                            prize = parseFloat($("#" + id + "a").text()) / dd
                            $("#" + id + "t").text(dd + 1);
                            var total = parseFloat($("#total").text()) + prize;
                            $("#" + id + "a").text((parseFloat($("#" + id + "a").text()) + prize).toFixed(2))
                            $("#total").text(total.toFixed(2));
                            if ($("#total").text() == 'NaN') {
                                $("#total").text('0.00')
                            }
                            product = 'grumble';
                            $("#grumbleq").val('');
                            $("#grumbleq").focus();
                        }
                    }
                    else {
                        $('#product_lines > tbody:first').append('<tr id="' + id + '" style=" cursor: pointer;border-bottom-color: White; border-left-style:none; border-right-style: none"><td class= "desc" style="width: 50%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                             description + '</td><td id="' + id + 't" class="qty" style="width: 20%;font-size: 15px;border-bottom-color: White; border-left-style:none; border-right-style: none"><input id="' +
                                              id + 'q" type="number" class="dt" style="width : 48%;" /></td><td id="' + id + 'a" class="amnt" style="width: 30%;text-align: right;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                            parseFloat(prize).toFixed(2)  + '</td><td style="display:none">' + tax + '</td></tr>');

                        $("#" + id + "q").focus();
                        product = id;
                        $("#grumbleq").val('');
                        var height = $('#details')[0].scrollHeight;
                        $('#details').scrollTop(height);
                    }
                }
                else {
                    //'$('#results').html(data);
                 //   alert(resp)
                    $('#menu').html(resp);
                }
            },
            error: function (data) {
            }
        });
    }
}

function getMenus(company) {
    $.ajax({
        type: "Get",
        url: '/Pos/getMenus',
        dataType: 'html',
        data: { json_str: company },
        success: function (resp) {
            $('#menu').html(resp);
        },
        error: function (data) {
        }
    });
} 

function PrintReport(url) { 
    $.ajax({
        type: "Get",
        url: url,
        dataType: 'html',
        //   data: { json_str: AdId },
        success: function (resp) {
            $("#toprint").html(resp);
            setTimeout(function () { PrintDiv() }, 2000);
        },
        error: function (data) {
        }
    });
}

function GetCreate() {
    $.ajax({
        type: "Get",
        url: '/Asset/Daily',
        dataType: 'html',
        //   data: { json_str: AdId },
        success: function (resp) {

            $('#panel').html(resp);
        },
        error: function (data) {

        }
    });
}

// pos events //

function discountYes(id, Name, prize, type, Qty, dcount, tax) {

    if (type == "PERCENT") {

        prize = (prize - (dcount / 100.0000) * prize);

    }
    if (type == "AMOUNT") {

        prize = prize - dcount;

    }
    


    if ($("#" + product + "q").is(':visible') && product != "grumble") {
        CommitProduct();
    }
    if ($("#" + id).length) {
        var dd = parseFloat($("#" + id + "t").text())
        prize = parseFloat($("#" + id + "a").text()) / dd
        $("#" + id + "t").text(dd + 1);
        var total = parseFloat($("#total").text()) + prize;
        $("#" + id + "a").text((parseFloat($("#" + id + "a").text()) + prize).toFixed(2))
        $("#total").text(total.toFixed(2));
        if ($("#total").text() == 'NaN') {
            $("#total").text('0.00')
        }
        product = 'grumble';
        $("#grumbleq").focus();
    }
    else {
        if (type.trim() == "PERCENT") {
            $('#product_lines > tbody:first').append('<tr id="' + id + '" style=" cursor: pointer;border-bottom-color: White; border-left-style:none; border-right-style: none"><td id="' +
               id + 'desc" class= "desc" style="width: 50%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
               Name + '</td><td id="' + id + 't" class="qty" style="width: 20%;font-size: 15px;border-bottom-color: White; border-left-style:none; border-right-style: none"><input id="' +
               id + 'q" type="number" class="dt" style="width : 48%;" /></td><td id="' + id + 'disc" class= "disc" style="width: 50%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
               parseFloat(dcount).toFixed(0) + '% Discount' + '</td><td id="' + id + 'a" class="amnt" style="width: 30%;text-align: right;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                parseFloat(prize).toFixed(2)  + '</td><td style="display:none">' + tax + '</td><td id="' + id + 'stk" style="display:none">' + Qty + '</td></tr>');

            product = id;
            $("#grumbleq").val('');
            $("#" + id + "q").focus();
            var height = $('#details')[0].scrollHeight;
            $('#details').scrollTop(height);
        }


        if (type.trim() == "AMOUNT") {
            $('#product_lines > tbody:first').append('<tr id="' + id + '" style=" cursor: pointer;border-bottom-color: White; border-left-style:none; border-right-style: none"><td id="' +
               id + 'desc" class= "desc" style="width: 50%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
               Name + '</td><td id="' + id + 't" class="qty" style="width: 20%;font-size: 15px;border-bottom-color: White; border-left-style:none; border-right-style: none"><input id="' +
               id + 'q" type="number" class="dt" style="width : 48%;" /></td><td id="' + id + 'disc" class= "disc" style="width: 50%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
               '$ ' + parseFloat(dcount).toFixed(2) + ' Discount/Item' + '</td><td id="' + id + 'a" class="amnt" style="width: 30%;text-align: right;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                parseFloat(prize).toFixed(2)  + '</td><td style="display:none">' + tax + '</td><td id="' + id + 'stk" style="display:none">' + Qty + '</td></tr>');

            product = id;
            $("#grumbleq").val('');
            $("#" + id + "q").focus();
            var height = $('#details')[0].scrollHeight;
            $('#details').scrollTop(height);
        }
    }
    CloseFancy();
    getMenus(Company);

}


function discountNo(id, Name, prize, type, Qty, dcount, tax)
{
    if ($("#" + product + "q").is(':visible') && product != "grumble") {
        CommitProduct();
    }
    if ($("#" + id).length) {
        var dd = parseFloat($("#" + id + "t").text())
        prize = parseFloat($("#" + id + "a").text()) / dd
        $("#" + id + "t").text(dd + 1);
        var total = parseFloat($("#total").text()) + prize;
        $("#" + id + "a").text((parseFloat($("#" + id + "a").text()) + prize).toFixed(2))
        $("#total").text(total.toFixed(2));
        if ($("#total").text() == 'NaN') {
            $("#total").text('0.00')
        }
        product = 'grumble';
        $("#grumbleq").focus();
    }
    else {
        $('#product_lines > tbody:first').append('<tr id="' + id + '"  style=" cursor: pointer;border-bottom-color: White; border-left-style:none; border-right-style: none"><td id="' + id + 'desc" class="desc" style="width: 50%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                                 Name + '</td><td id="' + id + 't" class="qty" style="width: 20%;font-size: 15px;border-bottom-color: White; border-left-style:none; border-right-style: none"><input id="' +
                                                  id + 'q" type="number" class="dt" style="width : 48%;" /></td><td id="' + id + 'disc" class= "space" style="width: 50%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                                     "       " + '</td><td id="' + id + 'a" class="amnt" style="width: 30%;text-align: right;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                                parseFloat(prize).toFixed(2)  + '</td><td style="display:none">' + tax + '</td><td id="' + id + 'stk" style="display:none">' + Qty + '</td></tr>');
        product = id;
        $("#grumbleq").val('');
        $("#" + id + "q").focus();
        var height = $('#details')[0].scrollHeight;
        $('#details').scrollTop(height);
    }
    CloseFancy();
    getMenus(Company);


}





function getItem(id, description, prize, Qty, dcount, type, tax) {

    if (dcount != null && dcount != undefined && dcount != "" && givesDiscounts == true) {

        verifyDiscount(id, description, prize, Qty, dcount, type, tax);

    }
    else
        {
        

        if ($("#" + product + "q").is(':visible') && product != "grumble") {  
            CommitProduct();
        }
    if ($("#" + id).length) {
        if (action == "voidline") {
            var amt = $("#" + id + "a").text();
            var total = parseFloat($("#total").text()) - amt;
            $("#total").text(parseFloat(total).toFixed(2));
            if ($("#total").text() == 'NaN') {
                $("#total").text('0.00')
            }
            $("#" + id).remove();
            action = "";
            $("#grumbleq").val('');
            $("#grumbleq").focus();
        }
        else {
            var dd = parseFloat($("#" + id + "t").text())
            prize = parseFloat($("#" + id + "a").text()) / dd
            $("#" + id + "t").text(dd + 1);
            var total = parseFloat($("#total").text()) + prize;
            $("#" + id + "a").text((parseFloat($("#" + id + "a").text()) + prize).toFixed(2))
            $("#total").text(total.toFixed(2));
            if ($("#total").text() == 'NaN') {
                $("#total").text('0.00')
            }
            product = 'grumble';
            $("#grumbleq").focus();
        }
    }
    else {
        $('#product_lines > tbody:first').append('<tr id="' + id + '" style=" cursor: pointer;border-bottom-color: White; border-left-style:none; border-right-style: none"><td class= "desc" style="width: 50%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                                 description + '</td><td id="' + id + 't" class="qty" style="width: 20%;font-size: 15px;border-bottom-color: White; border-left-style:none; border-right-style: none"><input id="' +
                                                  id + 'q" type="number" class="dt" style="width : 48%;" /></td><td id="' + id + 'a" class="amnt" style="width: 30%; text-align: right;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                                 parseFloat(prize).toFixed(2)  + '</td><td style="display:none">' + tax + '</td><td id="' + id + 'stk" style="display:none">' + Qty + '</td></tr>');
        $("#grumbleq").val('');
        $("#" + id + "q").focus();
        product = id;
   
        var height = $('#details')[0].scrollHeight;
        $('#details').scrollTop(height);
    }
  }
}

function getItemCh(id, description, prize, tax, Qty, dcount, type, givesDiscounts) {
    if (parseInt(Qty) > 0) {
      
        if (dcount != null && dcount != undefined && dcount != "" && dcount != 0 && givesDiscounts == "True") {

            verifyDiscount(id, description, prize, Qty, dcount, type, tax);

        }else
            {

        
        if ($("#" + product + "q").is(':visible') && product != "grumble") {
            CommitProduct();
        }
        if ($("#" + id).length) {
            var dd = parseFloat($("#" + id + "t").text())
            prize = parseFloat($("#" + id + "a").text()) / dd
            $("#" + id + "t").text(dd + 1);
            var total = parseFloat($("#total").text()) + prize;
            $("#" + id + "a").text((parseFloat($("#" + id + "a").text()) + prize).toFixed(2))
            $("#total").text(total.toFixed(2));
            if ($("#total").text() == 'NaN') {
                $("#total").text('0.00')
            }
            product = 'grumble';
            $("#grumbleq").focus();
        }
        else {
            $('#product_lines > tbody:first').append('<tr id="' + id + '"  style=" cursor: pointer;border-bottom-color: White; border-left-style:none; border-right-style: none"><td id="' + id + 'desc" class="desc" style="width: 50%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                                 description + '</td><td id="' + id + 't" class="qty" style="width: 20%;font-size: 15px;border-bottom-color: White; border-left-style:none; border-right-style: none"><input id="' +
                                                  id + 'q" type="number" class="dt" style="width : 48%;" /></td><td id="' + id + 'disc" class= "space" style="width: 50%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                                     "       " + '</td><td id="' + id + 'a" class="amnt" style="width: 30%;text-align: right;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                                 parseFloat(prize).toFixed(2)  + '</td><td style="display:none">' + tax + '</td><td id="' + id + 'stk" style="display:none">' + Qty + '</td></tr>');
            product = id;
            $("#grumbleq").val('');
            $("#" + id + "q").focus();
            var height = $('#details')[0].scrollHeight;
            $('#details').scrollTop(height);
        }
        getMenus(Company);
        }
    }
    else
    {
        OutOfStock(description);
    }
}

function OutOfStock(description) {
    if (description != undefined) {
        $.ajax({
            type: "Get",
            url: "/Pos/OutOfStock/",
            dataType: 'html',
            data: {
                Id: description
            },
            success: function (resp) {
                $.fancybox(resp, {
                    helpers: {
                        overlay: {
                            closeClick: false
                        }
                    },
                    keys: {
                        close: null
                    },
                    fitToView: true,
                    closeBtn: false,
                    autoSize: true,
                    openEffect: 'none',
                    closeEffect: 'none',
                    topRatio: 0
                });
            },
            error: function (data) {
                $.fancybox.hideLoading();
            }
        });
    }
}

function verifyDiscount(id, description, prize, Qty, dcount, type, tax) {
    if (description != undefined) {
        $.ajax({
            type: "Get",
            url: "/Pos/Discount/",
            dataType: 'html',
            data: {
                id: id,
                Name: description,
                prize: prize,
                type: type,
                Qty: Qty,
                dcount: dcount,
                tax: tax

            },
            success: function (resp) {
                $.fancybox(resp, {
                    helpers: {
                        overlay: {
                            closeClick: false
                        }
                    },
                    keys: {
                        close: null
                    },
                    fitToView: true,
                    closeBtn: false,
                    autoSize: true,
                    openEffect: 'none',
                    closeEffect: 'none',
                    topRatio: 0
                });
            },
            error: function (data) {
                $.fancybox.hideLoading();
            }
        });
    }
}



function CommitProduct() {
    var dd;
   // alert(product);
    if (product !== 'grumble')
    {
      
        if ($("#" + product + "q").val() == '' ) {
            dd = "1";
        }
        else {
                dd = $("#" + product + "q").val();
        }
        
        if (parseInt($("#" + product + "stk").html()) >= parseInt(dd)) {
            $("#" + product + "q").hide();
            var amt = $("#" + product + "a").text();
            var amtf = parseFloat(amt) * parseFloat(dd);
            var total = parseFloat($("#total").text()) + amtf;
            $("#total").text(total.toFixed(2));
            $("#" + product + "a").text(amtf.toFixed(2));
            $("#" + product + "t").text(dd);
            product = 'grumble';
            $("#grumbleq").focus();
        } else {
            OutOfStock($("#" + product + "desc").val());
        }
    }

}

function editLine(n) {  
    n = this.id;
   // alert(n);
    if (action == "voidline") {
        $("#" + n).remove();
    }
}

function onNumber(n) {
    var txt = $("#" + product + "q").val()
    $("#" + product + "q").val(txt + n);
    $("#" + product + "q").focus();
    customerSelected = 'N';

    if (product == "Credit")
    {
        var txt = $("#" + product + "q").val()
       // alert(txt);
        if (txt.length > 1) {
            $.ajax({
                type: "Get",
                url: '/Customers/Search',
                dataType: 'html',
                data: { searchtext: $("#Creditq").val() },
                success: function (resp) {
                    $('#customers').html(resp);
                },
                error: function (data) {
                }
            });
        } else {
            $('#customers').html('');
        }
    }
}

function onLNumber(n) {
    //Password key pad
    if (n == "clear") {
        var txt = $("#password").val()
        $("#password").val(txt.substring(0, (txt.length -1)));
    } else {
        var txt = $("#password").val()
        $("#password").val(txt + n);
        $("#password").focus();
        customerSelected = 'N';
    }
}

function onFocused() {
    $("#" + product + "q").focus();
    //alert(product );
}

function Back() {
    var n = $("#" + product + "q").val()
    $("#" + product + "q").val(n.substring(0, n.length - 1));
    $("#" + product + "q").focus();
}

//changes tender to base currency
function setTender(amnt, curr, rate) {
    var sp = transactionList.split(',')
    if (sp.length == 1) {
        if (Rate == undefined) {
            BaseBalance = parseFloat(parseFloat($('#total').text()) * parseFloat(rate)).toFixed(2);
        }
        else
        {
            BaseBalance = parseFloat(parseFloat($('#total').text()));
        }
    }
    Balance = amnt;
    $('#total').text(amnt);
    currency = curr;
    $('#product_lines > tbody  > tr').each(function () {
        if (Rate == undefined) {
            var amt = $(this).find(".amnt").text();
            var namnt = (parseFloat(amt) * parseFloat(rate)).toFixed(2);
            $(this).find(".amnt").text(namnt);
        }
        else
        {
            var amt = $(this).find(".amnt").text();
            var namnt = (parseFloat(amt));
            $(this).find(".amnt").text(namnt);

        }
    });
}

function GetPartialView(link, data, iscredit, tran) {
    $.fancybox.showLoading();
    
    var totl = $("#total").text();
    if (Partpayment != undefined && Partpayment > 0)
    {
        totl = Partpayment;
    }
    var cc = 0;
    $('#product_lines > tbody  > tr').each(function () {
        cc=cc+1
    });
    if (cc > 0 || data == "password" || data == "Other")
    {
        product = "Cash";
        IsCredit = iscredit;
        if(data != "total")
        {
            Transaction = tran;
        }
        setForm(data);
        if (partpaymentTotal > 0 && link != "/Pos/Tender")
        {
            link = link + "_Yes";
        }
        $.ajax({
            type: "Get",
            url: link,
            dataType: 'html',
            data: {
                Baseamount: BaseBalance,
                PartTotal: partpaymentTotal,
                amount: totl,
                Currency: currency,
                Displaycurrency:Displaycurrency
            },
            success: function (resp) {
                $.fancybox(resp, {
                    helpers: {
                        overlay: {
                            closeClick: false,
                            css: { 'overflow': 'hidden' }
                        }
                    },
                    keys: {
                        close: null
                    },
                    scrolling:false,
                    fitToView: false,
                    closeBtn: false,
                    openEffect: 'none',
                    closeEffect: 'none'
                    
                });
            },
            error: function (data) {
            }
        });
      
    }
    if (data == "password") {
        $("#" + product).focus();
    }
    else if (data == "Tender")
    {
        partpaymentTotal = 0;
       // Balance = parseFloat($("#total").text());
    }
    else
    {
        $("#" + product + "q").focus();
       // Balance = parseFloat($('#Balance').val());
    }
    $.fancybox.hideLoading();
}

function GetAuthenticate(data) {
    
    $.ajax({
        type: "Get",
        url: '/Pos/Authenticate',
        dataType: 'html',
        data: { json_str: data },
        success: function (resp) {
            $.fancybox(resp, {
                helpers: {
                    overlay: {
                        closeClick: false
                    }
                },
                keys: {
                    close: null
                },
                minWidth: '400',
                closeBtn: false,
                openEffect: 'none',
                closeEffect: 'none'
            });
        },
        error: function (data) {

        }
    });

    $("#" + product + "q").focus();

}

function GetActions(link, data) {
    // setParameter(data),
    //   scrolling:true,
    var cc = 0;
    var dat;
    $('#product_lines > tbody  > tr').each(function () {
        cc = cc + 1
    });
    if (cc == 0) {

        $.fancybox({
            href: link,
            type: 'ajax',
            ajax: {
                type: "Get"
            },
            maxWidth: 800,
            maxHeight: 600,
            fitToView: true,
            autoSize: true,
            closeClick: false,
            closeBtn: false,
            openEffect: 'none',
            closeEffect: 'none'
        });
        if (data == 'SIGN OUT') {
            location.reload();
        }
    }
}

function SetActions(Actiontype) {
    // setParameter(data),
    //   scrolling:true,
    action = Actiontype;
    $("#actionName").html(Actiontype);
    $("#ActionMenu").show();
    $("#tender").hide();
    CloseFancy();
}

function SaveActions() {
   
    GetAuthenticate(action.toLowerCase());
}

function CancelActions() {
    // setParameter(data),
    //   scrolling:true,
    action = '';
    $("#ActionMenu").hide();
    $("#tender").show();
    location.reload();
}

function GetView(url, Id) {
    $.fancybox.showLoading();
    
    $.ajax({
        type: "Get",
        url: url,
        dataType: 'html',
        data: {
            Company: Company,
            Id: Id
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            $("#panel").html(resp);
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}

function getLogin() {
    $.fancybox({
        href: '/Login/PosLogin',
        type: 'ajax',
        ajax: {
            type: "Get"
        },
        maxWidth: 800,
        maxHeight: 600,
        fitToView: true,
        autoSize: true,
        closeClick: false,
        closeBtn: false,
        openEffect: 'none',
        closeEffect: 'none'
    });
   
    $("#" + product + "q").focus();
}
function GetFancy(url, Id) {
    $.fancybox.showLoading();

    $.ajax({
        type: "Get",
        url: url,
        dataType: 'html',
        data: {
            Company: Company,
            Id: Id
        },
        success: function (resp) {
            $.fancybox(resp, {
                helpers: {
                    overlay: {
                        closeClick: false
                    }
                },
                keys: {
                    close: null
                },
                fitToView: true,
                minWidth: '700',
                closeBtn: true,
                openEffect: 'none',
                closeEffect: 'none',
                topRatio: 0
            });
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}
function CloseFancy() {
    transactionList = '';
     Balance = 0;
     Grandtotal = 0; //used for cash collection and end of shift 
     Partpayment = 0;
    partpaymentTotal = 0;
    $.fancybox.close();
}