
var cartOn = 'No';
var CurrentPayId = '0';
var Sessionid = "";
function GetOrderCount() {
    
    $.ajax({
        type: "Get",
        url: '/Cart/Orderlist',
        dataType: 'html',
        success: function (resp) {
            var r = resp;
	            
            $("#ChartSize").html('<i class="icons icon-basket-2"></i>' + r + ' items');
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}

function AddToCart(Id, total) {

    $.ajax({
        type: "Get",
        url: '/Cart/AddToCart',
        dataType: 'html',
        data: {
            Id: Id,
            qty: $("#quantity").val(),
            total: parseInt($("#quantity").val()) * parseFloat(total),
            SessionId:localStorage.getItem('Sessionid'),
            spec: $(".chosen-select").val()
        },
        success: function (resp) {
            try {
                var re = $.parseJSON(resp);
            
                if(re == 'Success')
                { 
                    $('#Success').show();
                    $("#Success").html('This item has been added to your shopping cart');
                    GetOrderCount();
                }
                else if(re== "Login")
                {
                    $('#Error').show();
                    $("#Error").html('You need to be logged in to transact');
                }
                else if (re == "Stock") {
                    $("#Qty").html('<span class="green">in stock</span> 0 items')
                    $('#addTC').hide()
                    $('#Error').show();
                    $("#Error").html('This item is now out of stock');
                }
                else {
                   
                    $('#Success').show();
                    $("#Success").html('<span class="green">The last ' + re + 'items where added.These items can only be reserved when you check out</span>');
                    GetOrderCount();
                }
            }
            catch (error) {

            };
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}

function AddToCartQ(Id, total) {

    $.ajax({
        type: "Get",
        url: '/Cart/AddToCartQ',
        dataType: 'html',
        data: {
            Id: Id,
            qty: 1,
            total: parseFloat(total),
            SessionId:localStorage.getItem('Sessionid'),
            spec: ""
        },
        success: function (resp) {
            try {
                var re = $.parseJSON(resp);

                if (re == 'Success') {

                    $('#Success_' + Id).show();
                    $("#Success_" + Id).html('Item added to shopping cart');
                    GetOrderCount();
                    setTimeout(function () { $('#Success_' + Id).fadeOut(500) }, 2000);
                }
                else if (re == "Login") {
                    $('#Error_' + Id).show();
                    $("#Error_" + Id).html('Please log in to transact');
                    setTimeout(function () { $('#Error_' + Id).fadeOut(500) }, 2000);
                }
                else if (re == "Stock") {
                    $("#Qty").html('<span class="green">in stock</span> 0 items')
                    $('#addTC').hide()
                    $('#Error_' + Id).show();
                    $("#Error_" + Id).html('Item is now out of stock');
                    setTimeout(function () { $('#Error_' + Id).fadeOut(500) }, 2000);
                }
                else {

                    $('#Success_' + Id).show();
                    $("#Success_" + Id).html('<span class="green">The last ' + re + 'items where added.These items can only be reserved when you check out</span>');
                    GetOrderCount();
                    setTimeout(function () { $('#Success_' + Id).fadeOut(500) }, 2000);
                }
            }
            catch (error) {

            };
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });

}

function AddToCartSearch(Id, total) {
   
    $.ajax({
        type: "Get",
        url: '/Cart/AddToCart',
        dataType: 'html',
        data: {
            Id: Id,
            qty: $("#quantity-"+Id).val(),
            total: parseInt($("#quantity-" + Id).val()) * parseFloat(total),
            SessionId:localStorage.getItem('Sessionid'),
            spec: $(".chosen-select").text(),
            resptyp: cartOn
        },
        success: function (resp) {
            try {
                var re = $.parseJSON(resp);

                if (re == 'Success') {
                    $("#btns-" + Id).html('<span style="color:#FFFF00; font-weight: bold">This item has been added to your shopping cart</span>');
                    GetOrderCount();
                }
                else if (re == "Login") {  
                    $("#btns-" + Id).html('<span style="color:#E74C3C; font-weight: bold">You need to be logged in to transact</span>');
                }
                else if(re == "Stock")
                {
                    $("#btns-" + Id).html('<span style="color:#E74C3C; font-weight: bold">in stock 0 items</span>')
                }
                else {
                   
                    $('#btns-'+Id).show();
                    $("#btns-" + Id).html('<span style="color:#FFFF00; font-weight: bold">The last ' + re + 'items where added.These items can only be reserved when you check out</span>');
                    GetOrderCount();
                }
            }
            catch (error) {

            };
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}

function PlusOne(id)
{
    var input = $("#quantity-" + id);
    var value = parseInt(input.val());
    input.val(value + 1);
   
}

function minusOne(id)
{
    var input = $("#quantity-" + id);
    var value = parseInt(input.val());
    var newval = value - 1;
    if (newval < 1) {
        input.val(1);
    } else {
        input.val(value - 1);
    }
   
}

function setDelivery(id,Address,time)
{
    $.ajax({
        type: "Post",
        url: '/Cart/Address',
        dataType: 'html',
        data:{
            CollectionId: id 
        },
        success: function (resp) {
            try {
                var e = $.parseJSON(resp)
                if(e == 'Success')
                {
                    var nt = (parseFloat($('#InvoiceTotal').html()) - parseFloat($('#DeliveryPrice').html())).toFixed(2);
                    $('#InvoiceTotal').html(nt);
                    $('#address').html(Address + '<br/> Collection Time: ' + time)
                    $('#CollectionAd').show();
                    $('#delivery').hide();
                    $('#quick-view-modal').fadeOut(300);
                }

            }
            catch(error)
            {

            }
            
        },
        error: function (data) {
            $('#quick-view-modal').fadeOut(300);
        }
    });
   
    
}

function DeliverToAddress()
{
    $.ajax({
        type: "Post",
        url: '/Cart/Address',
        dataType: 'html',
        data: {
            CollectionId: 0
        },
        success: function (resp) {
            try {
                var e = $.parseJSON(resp)
                if (e == 'Success') {
                    var nt = (parseFloat($('#InvoiceTotal').html()) + parseFloat($('#DeliveryPrice').html())).toFixed(2);
                    $('#InvoiceTotal').html(nt);
                    $('#CollectionAd').hide();
                    $('#delivery').show();
                }

            }
            catch (error) {

            }

        },
        error: function (data) {
            $('#quick-view-modal').fadeOut(300);
        }
    });

}

function RemoveOrderline(Id) {
    $.ajax({
        type: "Get",
        url: '/Cart/RemoveCartItem',
        dataType: 'html',
        data: {
            Id: Id
        },
        success: function (resp) {
            $('#panel').html(resp);

        },
        error: function (data) {
            $('#quick-view-modal').fadeOut(300);
        }
    });

}

function MinusOneOrderLine(Id, total, Qty) {
    var quant = parseInt(Qty) - 1;
    if (quant == 0) {
        RemoveOrderline(Id);
    }
    else {
        $.ajax({
            type: "Get",
            url: '/Cart/AdjustCart',
            dataType: 'html',
            data: {
                Id: Id,
                qty: -1,
                total: parseFloat(total),
                spec: $(".chosen-select").text()
            },
            success: function (resp) {
                $('#panel').html(resp);
            },
            error: function (data) {
                $.fancybox.hideLoading();
            }
        });
    }
}

function PlusOneOrderLine(Id, total, Qty) {
    var quant = parseInt(Qty) + 1;
    $.ajax({
        type: "Get",
        url: '/Cart/AdjustCart',
        dataType: 'html',
        data: {
            Id: Id,
            qty: 1,
            total:  total,
            spec: $(".chosen-select").text()
        },
        success: function (resp) {
            $('#panel').html(resp);
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}

function toCollect()
{
    $.ajax({
        type: "Get",
        url: '/Collections/Index',
        dataType: 'html',
        success: function (resp) {
            var r = $.parseJSON(resp);

            $("#ChartSize").html('<i class="icons icon-basket-2"></i>' + r + ' items');
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}

function GetShopList(Id) {

    $.ajax({
        type: "Get",
        url: '/Cart/LoadShoppingList',
        dataType: 'html',
        data: {
            Id: Id
        },
        success: function (resp) {
            $('#panel').html(resp);
            $('#quick-view-modal').fadeOut(300);
        },
        error: function (data) {
            $('#quick-view-modal').fadeOut(300);
        }
    });
}

function ShowPayOption(Id)
{
    if (CurrentPayId != Id)
    {
        $("#details-" + CurrentPayId).hide();
        $("#action-" + CurrentPayId).hide();
        $('#error_' + CurrentPayId).hide()
        CurrentPayId = Id;
    }
    if ($("#details-" + Id).css('display') == 'none' ){
        $("#details-" + Id).show();
        $("#details2-" + Id).show();
        $("#action-" + Id).show();
    }
    else 
    {
        $("#details-" + Id).hide();
        $("#details2-" + Id).hide();
        $("#action-" + Id).hide();
        $('#error_' + Id).hide()
    }
}

function HidePayOption(Id) {
    $("#details-" + Id).hide();
    $("#details2-" + Id).hide();
    $("#action-" + Id).hide();
    $('#error_' + Id).hide()

}

function Pay(Id) {

    $.ajax({
        type: "Post",
        url: '/Cart/WalletPayment',
        dataType: 'html',
        data: {
            Id: $('#v_' + Id).val()
           
        },
        success: function (resp) {
            try
            {
                var r = $.parseJSON(resp);
                if(r == "No")
                {
                    $('#error_td_' + Id).html("Your transaction was not found please try to confirm payment or checking your respense code.");
                    $('#error_' + Id).show();

                }
                else if (r== "Error")
                {
                    $('#error_td_' + Id).html("Sorry the is an error in the payment. Please try again.");
                    $('#error_' + Id).show();
                }
                else if (r == "Order") {
                    $('#error_td_' + Id).html("There are no items in your shopping cart");
                    $('#error_' + Id).show();
                }
                else if (r == "Partial") {
                    $('#error_td_' + Id).html("Sorry the amount paid is less than the invoice value. The amount has been added to your purchase wallet");
                    $('#error_' + Id).show();
                }
                else
                {
                    $('#error_td_' + Id).html(resp);
                    $('#error_' + Id).show();
                }
            }
            catch(error)
            {
                $('#homebody').html(resp);
            }
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}

function Cash(Id) {
    $.fancybox.showLoading();
    $.ajax({
        type: "Post",
        url: '/Account/cashout',
        dataType: 'html',
        data: {
            Id: $('#v_' + Id).val()

        },
        success: function (resp) {
            $.fancybox.hideLoading();
            try {
                var r = $.parseJSON(resp);
                if (r == "No") {
                    $('#error_td_' + Id).html("Your transaction was not found please try to confirm payment or checking your respense code.");
                    $('#error_' + Id).show();

                }
                else if (r == "Error") {
                    $('#error_td_' + Id).html("Sorry the is an error in the payment. Please try again.");
                    $('#error_' + Id).show();
                }
                else if (r == "Order") {
                    $('#error_td_' + Id).html("There are no items in your shopping cart");
                    $('#error_' + Id).show();
                }
                else if (r == "Partial") {
                    $('#error_td_' + Id).html("Sorry the amount paid is less than the invoice value. The amount has been added to your purchase wallet");
                    $('#error_' + Id).show();
                }
                else {
                    $('#error_td_' + Id).html(resp);
                    $('#error_' + Id).show();
                }
            }
            catch (error) {
                $('#homebody').html(resp);
            }
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}

function SaveNumber(Id) {
    if ($('#Pnum').val() == '') {
        $('#PError').html("Please Enter a valid phone number");
        $('#PError').show();
        setTimeout(function () { $('#PError').fadeOut(400) }, 4000);
    }
    else {
        $.ajax({
            type: "Post",
            url: '/Collections/Phone',
            dataType: 'html',
            data: {
                Id: Id,
                phone: $('#Pnum').val()

            },
            success: function (resp) {
                try {
                    var r = $.parseJSON(resp);

                    $('#Phone_' + Id).html(r);
                    $('#quick-view-modal').fadeOut(300);

                }
                catch (error) {
                    $('#homebody').html(resp);
                }
            },
            error: function (data) {
                $.fancybox.hideLoading();
            }
        });
    }
}


function GetAccount(Url) {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: Url,
        dataType: 'html',
        success: function (resp) {
            $.fancybox.hideLoading();
            $('#AccountBody').html(resp);       
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}

function CalculateIncome() {
    var X = parseFloat($("#Income").val());
    var Y = parseFloat($("#Expenditure").val());
    var s1 ;
    var s2 ;
    var s3 ;
    var c1 ;
    var c2 ;
    var c3 ;
    if ( X < 742) {
        
        s1 = parseInt(1.36*X / Y);
        s2 = parseInt(13.65*X / Y);
        s3 = parseInt(68.5*X / Y);
        c1 = parseFloat(s1 * 0.03 * Y).toFixed(2);
        c2 = parseFloat(s2 * 0.02 * Y).toFixed(2);
        c3 = parseFloat(s3 * 0.01 * Y).toFixed(2);
    }
    else if (X >= 743 && X < 1000) {
        
        s1 = parseInt(X / Y);
        s2 = parseInt(12.1*X / Y);
        s3 = parseInt(72.7*X / Y);
        c1 = parseFloat(s1 * 0.03 * Y).toFixed(2);
        c2 = parseFloat(s2 * 0.02 * Y).toFixed(2);
        c3 = parseFloat(s3 * 0.01 * Y).toFixed(2);
    }
    else if (X >= 1000 && X < 2800) {
      
        s1 = parseInt(0.87*X / Y);
        s2 = parseInt(13.9*X / Y);
        s3 = parseInt(69.6*X / Y);
        c1 = parseFloat(s1 * 0.03 * Y).toFixed(2);
        c2 = parseFloat(s2 * 0.02 * Y).toFixed(2);
        c3 = parseFloat(s3 * 0.01 * Y).toFixed(2);
    }
    else if (X >= 2800) {
        
        s1 = parseInt(0.4*X / Y);
        s2 = parseInt(8.25*X / Y);
        s3 = parseInt(82.3*X / Y);
        c1 = parseFloat(s1 * 0.03 * Y).toFixed(2);
        c2 = parseFloat(s2 * 0.02 * Y).toFixed(2);
        c3 = parseFloat(s3 * 0.01 * Y).toFixed(2);
    }
    var s2ea = parseInt(s2 / s1);
    var s3ea = parseInt(s3 / s2);
    $("#Size1").html(s1 + '<br/>NOTE: <em style="color:#21BF64">This means YOU introduce at least <span style="color:#E70031"> ' + s1 + ' people</span>.</em>');
    $("#Size2").html(s2 + '<br/>NOTE: <em style="color:#21BF64">This means each person in LEVEL 1 introduces at least <span style="color:#E70031">' + s2ea + ' people</span>. </em>');
    $("#Size3").html(s3 + '<br/>NOTE: <em style="color:#21BF64">This means each person in LEVEL 2 introduces at least  <span style="color:#E70031"> ' + s3ea + ' people</span>.</em>');

    var tot =  parseFloat(c1)+ parseFloat(c2 ) + parseFloat(c3);
    $("#Commission1").html(c1);
    $("#Commission2").html(c2);
    $("#Commission3").html(c3);
    $("#InvoiceTotal").html(parseFloat(tot).toFixed(2));

}

function SaveView(theForm) {
    var myForm = $('#' + theForm);
   
    $.ajax({
        url: myForm.attr('action'),
        type: myForm.attr('method'),
        data: myForm.serialize(),
        success: function (loginResultHtml) {
       
            if (loginResultHtml == "Success") {
                var target = '/Cart/AddressList'

                $('#quick-view-content .quick-view-container').load(target + ' #product-single', function () {


                    /* Positioning */
                    var q_width = $('#quick-view-content').width();
                    var q_height = $('#quick-view-content').height();
                    var q_margin = ($(window).height() - q_height) / 2;

                    $('#quick-view-content').css('margin-top', q_margin + 'px');


                    /* Cloud Zoom */
                    $("#quick-view-modal .cloud-zoom").imagezoomsl({
                        zoomrange: [3, 3]
                    });

                    $('.quick-view-content').perfectScrollbar('update');
                    $('.quick-view-content').css('overflow', 'hidden');

                    $('.quick-view-content').click(function () {
                        $(this).perfectScrollbar('update');
                    });

                    /* Select Box */
                    var config = {
                        '#quick-view-content .chosen-select': { disable_search_threshold: 10 }
                    }
                    for (var selector in config) {
                        $(selector).chosen(config[selector]);
                    }

                });
            }
            else 
            {
                $('#product-single').html(loginResultHtml);
            }
 
        },
        error: function (data) {
           
        }
    });
}

function SaveListD() {
    var id = $('#Title').val();
    $.ajax({
        type: "POST",
        url: '/Cart/SaveList',
        data: {
            Title: id
        },
        success: function (resp) {
            if (resp == "Error") {
                $("#ErrorList").show();
            }
            else {
                $("#SuccessList").show();
            }
            setTimeout(function () { $('#quick-view-modal').fadeOut(300) });
        }
    });
}