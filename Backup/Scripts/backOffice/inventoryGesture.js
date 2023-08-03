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


function GetView(url) {
     $.ajax({
         type: "Get",
         url: url,
         dataType: 'html',
         /*data: { item: dat,
             tender: tender,
             change: change.toFixed(2)
         },*/
         success: function (resp) {
             $("#panel").html(resp);     
         },
         error: function (data) {
             alert(data);
         }
     });
 }

function PostView(link) {
    $.ajax({
        type: "Post",
        url: link,
        dataType: 'html',
        //   data: { json_str: AdId },
        success: function (resp) {
            $('#panel').html(resp);
        },
        error: function (data) {
        }
    });
}

function SaveView(theForm) {
    var myForm = $('#' + theForm);
    $.ajax({
        url: myForm.attr('action'),
        type: myForm.attr('method'),
        data: myForm.serialize(),
        data: myForm.serialize(),
        success: function (resp) {
            $('#panel').html(resp);
        },
        error: function (data) {
        }
    });
}
function PrintDiv() {
    var divToPrint = document.getElementById('panel');
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
        type: "Get",
        url: 'Pos/Authenticate',
        dataType: 'html',
        data: { json_str: $("#Passq").val() + "-" + action },
        success: function (resp) {
            if (resp == "success") {
                vd = action;
                fancybox.close();
            }
            else {
                $("#error").text(resp);
            }
        },
        error: function (data) {
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
                $('#menu').html(resp);
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
   if ($("#" + id ).length) {
       var dd = parseInt($("#" + id + "t").text())
        prize = parseFloat($("#" + id + "a").text())/ dd
        $("#" + id + "t").text(dd + 1);
       var total = parseFloat($("#total").text()) + prize;
       $("#" + id + "a").text((parseFloat($("#" + id + "a").text()) + prize).toFixed(2))
       $("#total").text(total.toFixed(2));
       product = 'grumble';
       $("#grumbleq").focus();
     }
else {
    $('#product_lines > tbody:first').append('<tr id="' + id + '" onclick="editLine()" style=" cursor: pointer;border-bottom-color: White; border-left-style:none; border-right-style: none"><td class= "desc" style="width: 50%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                             description + '</td><td id="' + id + 't" class="qty" style="width: 20%;font-size: 15px;border-bottom-color: White; border-left-style:none; border-right-style: none"><input id="' +
                                              id + 'q" type="text" class="dt" style="width : 48%;" /></td><td id="' + id + 'a" class="amnt" style="width: 30%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                             parseFloat(prize).toFixed(2) + '</td><td style="display:none">' + tax + '</td></tr>');

    $("#" + id + "q").focus();
    product = id;
    $("#grumbleq").val('');
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
        product = 'grumble';
        $("#grumbleq").focus();
    }
    else {
        $('#product_lines > tbody:first').append('<tr id="' + id + '" onclick="editLine()" style=" cursor: pointer;border-bottom-color: White; border-left-style:none; border-right-style: none"><td class= "desc" style="width: 50%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                             description + '</td><td id="' + id + 't" class="qty" style="width: 20%;font-size: 15px;border-bottom-color: White; border-left-style:none; border-right-style: none"><input id="' +
                                              id + 'q" type="text" class="dt" style="width : 48%;" /></td><td id="' + id + 'a" class="amnt" style="width: 30%;font-size: 15px; border-bottom-color: White; border-left-style:none; border-right-style: none">' +
                                             parseFloat(prize).toFixed(2) + '</td><td style="display:none">' + tax + '</td></tr>');
        $("#" + id + "q").focus();
        product = id;
        $("#grumbleq").val('');
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
    n = $(this).attr('Id');
    //alert(n);
   if(action == "voidline")
   {
    $("#" + n).remove();
   // $("#" + n + "q").focus();
    
     }
}
function onNumber(n) {
    var txt = $("#" + product + "q").val()
    $("#" + product+ "q").val( txt + n);
}

function Back() {
    var n = $("#" + product + "q").val()
    $("#" + product + "q").val(n.substring(0,n.length - 1));
}
 