var product;
var action;
var vd;
/// <reference path="../../Controllers/HomeController.cs" />

function setForm(data) {
   // alert("ioioj");
    CommitProduct();
    product = data;
   
}

function setParameter(data) {
    action = data;
}

function Savesale() {
    var tender = $("#totalq").val();
    
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
         url: '/Pos/Print',
         dataType: 'html',
         data: { item: dat,
             tender: tender,
             change: change.toFixed(2)
         },
         success: function (resp) {
             $("#toprint").html(resp);
             $.fancybox.close();     
         },
         error: function (data) {
             alert(data);
         }
     });
     setTimeout(function () { PrintDiv() }, 6000);
}

function PrintDiv() {
    var divToPrint = document.getElementById('toprint');
    var output = document.getElementById("ifrOutput").contentWindow;
    output.document.open();
    output.document.write('<html><body>' + divToPrint.innerHTML + '</html>');
    output.document.close();
    output.focus();
    output.print();
    setTimeout(function () { location.reload(); }, 4000);
}

function Authenticate() {

    $.ajax({
        type: "Post",
        url: 'Pos/Authenticate',
        dataType: 'html',
        data: { json_str: $("#Passq").val() + "-" + action },
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
        var JsonQuestion = {
            "Id": $("#grumbleq").val()
        };
        $.ajax({
            type: "Get",
            url: 'Pos/getItems',
            dataType: 'html',
            data: { json_str: $("#grumbleq").val() },
            success: function (resp) {
                var G1 = $.parseJSON(data).split(",");
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
                            var dd = parseInt($("#" + id + "t").text())
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
                                              id + 'q" type="text" class="dt" style="width : 48%;" /></td><td id="' + id + 'a" class="amnt" style="width: 30%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                             parseFloat(prize).toFixed(2) + '</td><td style="display:none">' + tax + '</td></tr>');

                        $("#" + id + "q").focus();
                        product = id;
                        $("#grumbleq").val('');
                        var height = $('#details')[0].scrollHeight;
                        $('#details').scrollTop(height);
                    }
                }
                else {
                    //'$('#results').html(data);

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
        url: 'Pos/getMenus',
        dataType: 'html',
        data: { json_str: company },
        success: function (resp) {
            $('#menu').html(resp);
        },
        error: function (data) {
        }
    });
} 

function PrintReport() {
   
    $.ajax({
        type: "Get",
        url: 'Asset/Daily',
        dataType: 'html',
        //   data: { json_str: AdId },
        success: function (resp) {
            $('#panel').html(resp);
        },
        error: function (data) {
        }
    });
}

function GetCreate() {
    
    $.ajax({
        type: "Get",
        url: 'Asset/Daily',
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

function getItem(id, description, prize,tax) {

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
            var dd = parseInt($("#" + id + "t").text())
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
                                              id + 'q" type="text" class="dt" style="width : 48%;" /></td><td id="' + id + 'a" class="amnt" style="width: 30%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                             parseFloat(prize).toFixed(2) + '</td><td style="display:none">' + tax + '</td></tr>');

    $("#" + id + "q").focus();
    product = id;
    $("#grumbleq").val('');
    var height = $('#details')[0].scrollHeight;
    $('#details').scrollTop(height);
  }

}

function getItemCh(id, description, prize, tax) {

    if ($("#" + product + "q").is(':visible') && product != "grumble") {
        CommitProduct();
    }
    if ($("#" + id).length) {
        var dd = parseInt($("#" + id + "t").text())
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
        $('#product_lines > tbody:first').append('<tr id="' + id + '"  style=" cursor: pointer;border-bottom-color: White; border-left-style:none; border-right-style: none"><td class= "desc" style="width: 50%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                             description + '</td><td id="' + id + 't" class="qty" style="width: 20%;font-size: 15px;border-bottom-color: White; border-left-style:none; border-right-style: none"><input id="' +
                                              id + 'q" type="text" class="dt" style="width : 48%;" /></td><td id="' + id + 'a" class="amnt" style="width: 30%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                             parseFloat(prize).toFixed(2) + '</td><td style="display:none">' + tax + '</td></tr>');
        $("#" + id + "q").focus();
        product = id;
        $("#grumbleq").val('');
        var height = $('#details')[0].scrollHeight;
        $('#details').scrollTop(height);
    }
    getMenus('PRECIOUS SUPERMARKET');
}

function CommitProduct() {
    var dd;

    if (product !== 'grumble') {
        if ($("#" + product + "q").val() == '' ) {
            dd = "1";
        }
        else {
                dd = $("#" + product + "q").val();
        }
        $("#" + product + "q").hide();
        var amt = $("#" + product + "a").text();

        var amtf = parseFloat(amt) * parseInt(dd);
        var total = parseFloat($("#total").text()) + amtf;
        $("#total").text(total.toFixed(2));
        $("#" + product + "a").text(amtf.toFixed(2));
        $("#" + product + "t").text(dd);
        product = 'grumble';
        $("#grumbleq").focus();
    }

}

function editLine(n) {
    
    n = this.id;
    alert(n);
    if (action == "voidline") {
        $("#" + n).remove();
    }
}

function onNumber(n) {
    var txt = $("#" + product + "q").val()
    $("#" + product + "q").val(txt + n);
    $("#" + product + "q").focus();
}

function Back() {
    var n = $("#" + product + "q").val()
    $("#" + product + "q").val(n.substring(0, n.length - 1));
    $("#" + product + "q").focus();
}

function GetPartialView(link, data) {
    setForm(data),
    
    $.fancybox({
        href: link,
        type: 'ajax',
        ajax: {
            type: "Get"
        },
        openEffect: 'none',
        closeEffect: 'none'
    });
    
}

function GetAuthenticate(link, data) {
    setParameter(data),
     
    $.fancybox({
        href: link,
        type: 'ajax',
        ajax: {
            type: "Get"
        },
        openEffect: 'none',
        closeEffect: 'none'
    });
    $("#" + product + "q").focus();

}
 