var product;
var action;
var vd;
var Company;
var Reciept;
var supplierSelected;
var Supplier;
var Qty;
var ItemCode;
var Allocated = 0;
var unAllocated = 0;
var AllocationBalance = 0;
var Partpayment = 0;
var partpaymentTotal = 0;
var BaseBalance = 0;
var Balance = 0;
var currency;
var transactionList = "";
var PaymentType = 'SALE';
var Discount;
var customerName;
var creditPeriod;
var IsCredit;
var collecttionPoint;
var Quotationidd;

function setForm(data) {
   // alert("ioioj");
    //CommitProduct();
    product = data;
   
}
function onFocused() {
    $("#" + product + "q").focus();
    //alert(product );
}
function setParameter(data) {
    action = data;
}


function PrintDiv() {
    //alert("printing");
    var divToPrint = document.getElementById('toprint');
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
    setTimeout(function () { location.reload() }, 6000);
}


function DeleteItem(ID, currentPage) {
    if (currentPage == undefined || currentPage == null) {
        currentPage = 0;
    }
        $.ajax({
            type: "Post",
            url: "/Items/DeleteItem",
            dataType: 'html',
            data: {
                ID: ID,
                currentPage: currentPage
            },
            success: function (resp) {
                $.fancybox.hideLoading();
                $("#panel").html(resp);
                $.fancybox.close();
            },
            error: function (data) {
                $.fancybox.hideLoading();
            }
        });
    
}


function GetView(url, Id, currentPage) {
    if (currentPage == undefined || currentPage == null) {
        currentPage = 0;
    }
    $.fancybox.showLoading();
    if (tinymce.editors.length != 0 && tinymce.activeEditor != undefined) {
        // tinymce.remove();
        for (i = 0; i < tinymce.editors.length; i++) {
            tinymce.editors[i].remove();
        }
    }
        $.ajax({
            type: "Get",
            url: url,
            dataType: 'html',
            data: {
                Company: Company,
                Id: Id,
                currentPage: currentPage
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


function GetSideView(url, Id) {
    $.fancybox.showLoading();
    if (tinymce.editors.length != 0 && tinymce.activeEditor != undefined) {
        // tinymce.remove();
        for (i = 0; i < tinymce.editors.length; i++) {
            tinymce.editors[i].remove();
        }
    }
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
            $("#RightPanel").html(resp);
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}

function GetFancy(url, Id, currentPage) {
    if (currentPage == undefined || currentPage == null) {
        currentPage = 0;
    }
    $.fancybox.showLoading();
  
    $.ajax({
        type: "Get",
        url: url,
        dataType: 'html',
        data: {
            Company: Company,
            Id: Id,
            currentPage: currentPage
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

function GetGraph(url, Id) {
    $.fancybox.showLoading();
    if (tinymce.editors.length != 0 && tinymce.activeEditor != undefined) {
        // tinymce.remove();
        for (i = 0; i < tinymce.editors.length; i++) {
            tinymce.editors[i].remove();
        }
    }
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
            $("#graphs").html(resp);
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}

function GetDetails(url, Id) {
    $.fancybox.showLoading();
    //alert($("#Date").val())
    $.ajax({
        type: "Get",
        url: url,
        dataType: 'html',
        data: {
            date: $("#Month").val(),
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

function GetSearch(theForm) {
    $.fancybox.showLoading();
    var myForm = $('#' + theForm);
    $.ajax({
        iframe: true,
        url: myForm.attr('action'),
        type: myForm.attr('method'),
        data: myForm.serialize(),
        success: function (resp) {
            $.fancybox.hideLoading();
            if (theForm == 'createPurchase') {
                if (resp == "Supplier") {
                    alert("Please enter a valid supplier")
                }
                else {
                    $.fancybox.close();
                    Reciept = resp;
                }
            }
            else {
                $('#searchData').html(resp);
            }
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}

function PostView(link) {
    $.fancybox.showLoading();
    $.ajax({
        type: "Post",
        url: link,
        dataType: 'html',
        //   data: { json_str: AdId },
        success: function (resp) {
            $.fancybox.hideLoading();
            $('#panel').html(resp);

        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}
//********* Mailing service ****************** 
function Getlogs(link,service)
{
     //for the listing of the mail logs
        $.fancybox.showLoading();
        $.ajax({
            type: "Get",
            url: link,
            dataType: 'html',
            data: {
                type: $("#type").val(),
                date:$("#date").val(),
                service: service
            },
            success: function (resp) {
                $.fancybox.hideLoading();
                $('#panel').html(resp);

            },
            error: function (data) {
                $.fancybox.hideLoading();
            }
        });
}

function Resend(msg, service) {
    //for the listing of the mail logs
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/MailSettings/ResendMail',
        dataType: 'html',
        data: {
            message: msg,
            service: service
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            alert(resp);

        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}

function SaveView(theForm) {
    $.fancybox.showLoading();
    var myForm = $('#' + theForm);  
    $.ajax({
        iframe: true,
        url: myForm.attr('action'),
        type: myForm.attr('method'),
        data: myForm.serialize(),
        success: function (resp) {
            $.fancybox.hideLoading();
            if (theForm == 'createPurchase')
           
            {
                if (resp == "Supplier")
                {
                    alert("Please enter a valid supplier")
                }
                else
                {        
                    $.fancybox.close();
                    $('#panel').html(resp);

                }
            }
            else
            {
                $('#panel').html(resp);
            }
        },
        error: function (data) {
            $.fancybox.hideLoading();
        }
    });
}


//*************** Pos  ********************
function Authenticate() {
    $.ajax({
        type: "Get",
        url: '/Pos/Authenticate',
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
    $.fancybox.showLoading();

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
            url: '/Pos/getItems',
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

function PrintReport() {
    var divToPrint = document.getElementById('panelReport');
   // divToPrint.show();
    var output = document.getElementById("ifrOutput").contentWindow;
    output.document.open();
    output.document.write('<html><body>' + divToPrint.innerHTML + '</html>');
    output.document.close();
    output.focus();
    output.print();
}

function GetCreate() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Asset/Daily',
        dataType: 'html',
        //   data: { json_str: AdId },
        success: function (resp) {
            $.fancybox.hideLoading();
            $('#panel').html(resp);
        },
        error: function (data) {

        }
    });
}
// pos events //

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
 
function GetSalesProduct() {
    $.fancybox.showLoading();
        $.ajax({
            type: "Get",
            url: '/Sales/Index',
            dataType: 'html',
            data: {
                company: $("#activeco").text(),
                category: $("#category").val(),
                ItemCode: "",
                iDisplayLength: $("#SizeR").val(),
                sStart: $("#DateCreated").val(),
                sEnd: $("#Date").val(),
                sEcho: 1
            },
            success: function (resp) {
                $.fancybox.hideLoading();
                if(resp == "Dates"){
                    alert('End Date should always be greater than start date');
                }
                else{
                    $("#panel").html(resp);
                }
            },
            error: function (data) {
               // alert(data);
            }
        });
}
function GetTransfers() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Transfers/Index',
        dataType: 'html',
        data: {
            company: $("#activeco").text(),
            category: $("#category").val(),
            ItemCode: "",
            iDisplayLength: $("#SizeR").val(),
            sStart: $("#DateCreated").val(),
            sEnd: $("#Date").val(),
            sEcho: 1
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            if (resp == "Dates") {
                alert('End Date should always be greater than start date');
            }
            else {
                $("#panel").html(resp);
            }
        },
        error: function (data) {
            // alert(data);
        }
    });
}
function GetLosses() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Loss/Index',
        dataType: 'html',
        data: {
            company: $("#activeco").text(),
            category: $("#category").val(),
            ItemCode: "",
            iDisplayLength: $("#SizeR").val(),
            sStart: $("#DateCreated").val(),
            sEnd: $("#Date").val(),
            sEcho: 1
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            if (resp == "Dates") {
                alert('End Date should always be greater than start date');
            }
            else {
                $("#panel").html(resp);
            }
        },
        error: function (data) {
            // alert(data);
        }
    });
}
function GetPaymentMethodReport() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Sales/paymentMethodReport',
        dataType: 'html',
        data: {
            company: $("#activeco").text(),
            category: $("#category").val(),
            ItemCode: "",
            iDisplayLength: $("#SizeR").val(),
            sStart: $("#DateCreated").val(),
            sEnd: $("#Date").val(),
            sEcho: 1
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            if (resp == "Dates") {
                alert('End Date should always be greater than start date');
            }
            else {
                $("#panel").html(resp);
            }
        },
        error: function (data) {
            // alert(data);
        }
    });
}

function GetGrvList() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Items/GrvList/',
        dataType: 'html',
        data: {
            company: $("#activeco").text(),
            category: $("#category").val(),
            ItemCode: "",
            iDisplayLength: $("#SizeR").val(),
            sStart: $("#DateCreated").val(),
            sEnd: $("#Date").val(),
            sEcho: 1
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            if (resp == "Dates") {
                alert('End Date should always be greater than start date');
            }
            else {
                $("#panel").html(resp);
            }
        },
        error: function (data) {
            // alert(data);
        }
    });
}

function GetDeliveryList() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Deliveries/Index',
        dataType: 'html',
        data: {
            company: $("#activeco").text(),
            category: $("#category").val(),
            ItemCode: "",
            iDisplayLength: $("#SizeR").val(),
            sStart: $("#DateCreated").val(),
            sEnd: $("#Date").val(),
            sEcho: 1
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            if (resp == "Dates") {
                alert('End Date should always be greater than start date');
            }
            else {
                $("#panel").html(resp);
            }
        },
        error: function (data) {
            // alert(data);
        }
    });
}

function GetDeliveryPickingList() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Deliveries/DeliveryPickingList',
        dataType: 'html',
        data: {
            company: $("#activeco").text(),
            category: $("#category").val(),
            ItemCode: "",
            iDisplayLength: $("#SizeR").val(),
            sStart: $("#DateCreated").val(),
            sEnd: $("#Date").val(),
            sEcho: 1
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            if (resp == "Dates") {
                alert('End Date should always be greater than start date');
            }
            else {
                $("#panel").html(resp);
            }
        },
        error: function (data) {
            // alert(data);
        }
    });
}

function GetDeliverySammary() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Deliveries/DeliverySammery',
        dataType: 'html',
        data: {
            company: $("#activeco").text(),
            category: $("#category").val(),
            ItemCode: "",
            iDisplayLength: $("#SizeR").val(),
            sStart: $("#DateCreated").val(),
            sEnd: $("#Date").val(),
            sEcho: 1
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            if (resp == "Dates") {
                alert('End Date should always be greater than start date');
            }
            else {
                $("#panel").html(resp);
            }
        },
        error: function (data) {
            // alert(data);
        }
    });
}

function GetCollictionList() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Deliveries/Collections',
        dataType: 'html',
        data: {
            company: $("#activeco").text(),
            category: $("#category").val(),
            ItemCode: "",
            iDisplayLength: $("#SizeR").val(),
            sStart: $("#DateCreated").val(),
            sEnd: $("#Date").val(),
            sEcho: 1
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            if (resp == "Dates") {
                alert('End Date should always be greater than start date');
            }
            else {
                $("#panel").html(resp);
            }
        },
        error: function (data) {
            // alert(data);
        }
    });
}

function GetCollectionPickingList() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Deliveries/CollectionPickingList',
        dataType: 'html',
        data: {
            company: $("#activeco").text(),
            category: $("#category").val(),
            ItemCode: "",
            iDisplayLength: $("#SizeR").val(),
            sStart: $("#DateCreated").val(),
            sEnd: $("#Date").val(),
            sEcho: 1
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            if (resp == "Dates") {
                alert('End Date should always be greater than start date');
            }
            else {
                $("#panel").html(resp);
            }
        },
        error: function (data) {
            // alert(data);
        }
    });
}

function SaveOrderDetails(sale) {
    var dat = '';

    $('#' + sale + ' > tbody  > tr').each(function () {
        var description = $(this).find(".Description").val();
        var quantity = $(this).find(".Itemcode").val();
        // alert(quantity);
        if (dat == '' && quantity) {
            dat = description + "," + quantity
        }
        else if (dat != '') {
            dat = dat + "/" + description + "," + quantity
        }
    });
    $.fancybox.showLoading();
    $.ajax({
        type: "Post",
        url: '/Quotations/Notes',
        dataType: 'html',
        data: {
            item: dat,
            company: $("#activeco").text(),
            Id: sale
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            $("#toprint").html(resp);
            setTimeout(function () { PrintDispatch() }, 4000);
        },
        error: function (data) {
            // alert(data);
        }
    });
    // setTimeout(function () { PrintDiv() }, 6000);
}

function GetOdersProduct() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Orders/Index',
        dataType: 'html',
        data: {
            company: $("#activeco").text(),
            category: $("#category").val(),
            ItemCode: "",
            iDisplayLength: $("#SizeR").val(),
            sStart: $("#DateCreated").val(),
            sEnd: $("#Date").val(),
            sEcho: 1
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            if (resp == "Dates") {
                alert('End Date should always be greater than start date');
            }
            else {
                $("#panel").html(resp);
            }
        },
        error: function (data) {
           // alert(data);
        }
    });
}

function GetOdersDetail(id) {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Orders/Details',
        dataType: 'html',
        data: {
            id: id
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            $("#" + id).html(resp);
            $("#" + id).show()
            $("#" + id + "_b").show()
            $("#" + id + "_x").hide()

        },
        error: function (data) {
           // alert(data);
        }
    });

}

function SaveDispatch(sale) {
    var dat = '';

    $('#' + sale + ' > tbody  > tr').each(function () {
        var description = $(this).find(".Description").val();
        var quantity = $(this).find(".Itemcode").val();
       // alert(quantity);
        if (dat == '' && quantity) {
            dat = description + "," + quantity 
        }
        else if (dat != '') {
            dat = dat + "/" + description + "," + quantity 
        }
    });
    $.fancybox.showLoading();
    $.ajax({
        type: "Post",
        url: '/Orders/Dispatch',
        dataType: 'html',
        data: {
            item: dat,
            company: $("#activeco").text(),
            Id: sale
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            $("#toprint").html(resp);
            setTimeout(function () { PrintDispatch() }, 4000);
        },
        error: function (data) {
           // alert(data);
        }
    });
    // setTimeout(function () { PrintDiv() }, 6000);
}

function PrintDispatch() {
    // alert("printing");
    var divToPrint = document.getElementById('toprint');
    var output = document.getElementById("ifrOutput").contentWindow;
    output.document.open();
    output.document.write('<html><body>' + divToPrint.innerHTML + '</html>');
    output.document.close();
    output.focus();
    output.print();
    setTimeout(function () { GetOdersProduct() }, 6000);
}

function GetPurchaseOrderDetail(id, newId) {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/PurchaseOrders/Details',
        dataType: 'html',
        data: {
            id: id
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            $("#" + newId).html(resp);
            $("#" + newId).show()
            $("#" + newId + "_b").show()
            $("#" + newId + "_x").hide()

        },
        error: function (data) {
            // alert(data);
        }
    });

}

function GetQuotationDetail(id, newId) {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Quotations/Details',
        dataType: 'html',
        data: {
            id: id
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            $("#" + newId).html(resp);
            $("#" + newId).show()
            $("#" + newId + "_b").show()
            $("#" + newId + "_x").hide()

        },
        error: function (data) {
            // alert(data);
        }
    });

}

function GetOrderDetail(id, newId) {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Orders/Details',
        dataType: 'html',
        data: {
            id: id
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            $("#" + newId).html(resp);
            $("#" + newId).show()
            $("#" + newId + "_b").show()
            $("#" + newId + "_x").hide()

        },
        error: function (data) {
            // alert(data);
        }
    });

}

function GetSalesDetail(id,newId)
{
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Sales/Details',
        dataType: 'html',
        data: {
            id: id
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            $("#" + newId).html(resp);
            $("#" + newId).show();
            $("#break_" + newId).show();
            $("#" + newId + "_b").show();
            $("#" + newId + "_x").hide();
        },
        error: function (data) {
           // alert(data);
        }
    });

}

function GetGrvDetail(id, newId) {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Items/GrvLines',
        dataType: 'html',
        data: {
            id: id
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            $("#" + newId).html(resp);
            $("#" + newId).show();
            $("#" + newId + "_b").show();
            $("#" + newId + "_x").hide();
        },
        error: function (data) {
            // alert(data);
        }
    });

}

function GetSalesHamper(id, newId) {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Sales/Hamper',
        dataType: 'html',
        data: {
            id: id
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            $("#" + newId).html(resp);
            $("#" + newId).show()
            $("#" + newId + "_b").show()
            $("#" + newId + "_x").hide()

        },
        error: function (data) {
             alert(data);
        }
    });

}

function PickHamper(id,size) {
    $.fancybox.showLoading();
    var yy = $("#Picked_" + id).val();
    cc = 'False';
    if (yy == 'checked') {
        cc = 'True';
    }

    $.ajax({
        type: "Get",
        url: '/Deliveries/PickHamper',
        dataType: 'html',
        data: {
            id: id,
            chec: cc,
            size: size
        },
        success: function (resp) {
            $.fancybox.hideLoading();
           $("#Picked").val(resp);

        },
        error: function (data) {
            //alert(data);
        }
    });
}

function PickDelivery(id) {
    $.fancybox.showLoading();
    //var yy = $("#" + id).find('input:checkbox:first').attr('checked');
    var yy =  $("#" + id + " input:checkbox")[0].checked;
   cc = yy;
      
    $.ajax({
        type: "Get",
        url: '/Deliveries/PickDelivery',
        dataType: 'html',
        data: {
            id: id,
            chec:cc
        },
        success: function (resp) {
            $.fancybox.hideLoading();
             $("#Picked").val(resp);

        },
        error: function (data) {
            //alert(data);
        }
    });
}

function PickSupplier(id) {
    $.fancybox.showLoading();
    var yy = $("#" + id).find('input:checkbox:first').attr('checked');
    cc = 'False';
    if (yy == 'checked') {
        cc = 'True';
    }

    $.ajax({
        type: "Get",
        url: '/Deliveries/PickSupplier',
        dataType: 'html',
        data: {
            id: id,
            chec: cc
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            $("#Picked").val(resp);

        },
        error: function (data) {
            //alert(data);
        }
    });
}

function MinSalesDetail(old,id)
{
    $("#break_" + newId).hide();
    $("#" + id).hide()
    $("#" + id + "_b").hide()
    $("#" + id + "_x").show()
}

function GetSalesCategory() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Sales/Index',
        dataType: 'html',
        data: {
            company: $("#activeco").text(),
            category: $("#category").val(),
            ItemCode: "",
            iDisplayLength: $("#SizeR").val(),
            sStart: $("#DateCreated").val(),
            sEnd: $("#Date").val(),
            sEcho: 1

        },
        success: function (resp) {
            $.fancybox.hideLoading();
            $("#panel").html(resp);
        },
        error: function (data) {
           // alert(data);
        }
    });
}

function GetSalesSummary() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Sales/Index',
        dataType: 'html',
        data: {
            company: $("#activeco").text(),
            category: $("#category").val(),
            ItemCode: "",
            iDisplayLength: $("#SizeR").val(),
            sStart: $("#DateCreated").val(),
            sEnd: $("#Date").val(),
            sEcho: 1
        },
        success: function (resp) {
            $.fancybox.hideLoading();
            $("#panel").html(resp);
        },
        error: function (data) {
           // alert(data);
        }
    });
}

function GetFloatView(link) {
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

//*****************Intialisation***********************
function GetTopmenu() {

    $.ajax({
        type: "Get",
        url: '/Home/TopMenu',
        dataType: 'html',
        success: function (resp) {
            $("#side_menu").append(resp);
        },
        error: function (data) {
           // alert(data);
        }
    });
}

function GetSidemenu() {
    $.ajax({
        type: "Get",
        url: '/Home/SideMenu',
        dataType: 'html',
        success: function (resp) {
            $("#smenu").append(resp);
        },
        error: function (data) {
           // alert(data);
        }
    });
}

function GetActiveCo() {
    $.ajax({
        type: "Get",
        url: '/Home/ActiveCo',
        dataType: 'html',
        data: {
            Company: Company
        },
        success: function (resp) {
            Company = $.parseJSON(resp)
            $("#activeco").html(Company);
        },
        error: function (data) {
           // alert(data);
        }
    });
}

function GetPOSupplier(name) {

    $.ajax({
        type: "Get",
        url: '/PurchaseOrders/Purchases',
        dataType: 'html',
        data: {
            Company: Company,
            Supplier: name
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
                minWidth: '400',
                closeBtn: false,
                openEffect: 'none',
                closeEffect: 'none'
            });
        },
        error: function (data) {
        }
    });
}

function GetSupplier(name) { 
   
    $.ajax({
        type: "Get",
        url: '/Items/Purchases',
        dataType: 'html',
        data: {
            Company: Company,
            Supplier: name
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
                minWidth:'400',
                closeBtn: false,
                openEffect: 'none',
                closeEffect: 'none'
            });
        },
        error: function (data) {
        }
    });
}
function GetTransfer(name,code) {

    $.ajax({
        type: "Get",
        url: '/GRVTransfer/Purchases',
        dataType: 'html',
        data: {
            Company: Company,
            Supplier: name,
            Recieptno: code
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
                minWidth: '400',
                closeBtn: false,
                openEffect: 'none',
                closeEffect: 'none'
            });
        },
        error: function (data) {
        }
    });
}
function GetCustomer(name) {
    // Quode to add customer to quote or invoice
    $.ajax({
        type: "Get",
        url: '/Quotations/Purchases',
        dataType: 'html',
        data: {
            Company: Company,
            Supplier: name
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
                minWidth: '400',
                closeBtn: false,
                openEffect: 'none',
                closeEffect: 'none'
            });
        },
        error: function (data) {
        }
    });
}

function NewCustomer() {
    // Quode to add customer to quote or invoice
    $.ajax({
        type: "Get",
        url: '/Customers/Create',
        dataType: 'html',
        data: {
            Company: Company
            
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
                minWidth: '400',
                closeBtn: false,
                openEffect: 'none',
                closeEffect: 'none'
            });
        },
        error: function (data) {
        }
    });
}

function BrowseSupplier() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Suppliers/Browse',
        dataType: 'html',
        data: {
            Company: Company,
        },
        success: function (resp) {
            $.fancybox.hideLoading();
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
}

function BrowseTransfers() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/GRVTransfer/Browse',
        dataType: 'html',
        data: {
            Company: Company,
        },
        success: function (resp) {
            $.fancybox.hideLoading();
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
}
function BrowseCustomer() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: '/Customers/Browse',
        dataType: 'html',
        data: {
            Company: Company,
        },
        success: function (resp) {
            $.fancybox.hideLoading();
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
}

function AppendToCustomer(code, name) {
    GetCustomer(name + "_" + code);
}

function AppendToSuplier(code,name) {

    GetSupplier(name + "_" + code);
}
function AppendToTransfer(code,name) {

    GetTransfer(name+ "_" + code);
}
function AddToQuote(nn, mm) {
    if ($('#Amount').val() == '') {
        $('#Error').html('Please enter the cost price for each ' + nn)
        $('#Error').show();
        $('#Amount').focus();
    }
    else {
        var amt = parseFloat($('#Amount').val()) * Qty

        if (parseFloat($('#Amount').val()).toFixed(2) == 0.00) {
            $('#Error').html('Buying Price can not be 0.00')
            $('#Error').show();
        }
        else {
            $.ajax({
                type: "Post",
                url: '/Quotations/newStock',
                dataType: 'html',
                data: {
                    company: Company,
                    Qty: Qty,
                    Reciept: Reciept,
                    price: $('#Amount').val(),
                    ItemCode: ItemCode,
                    Supplier: Supplier
                },
                success: function (resp) {
                    //try {
                    //    var jsonItem = $.parseJSON(resp);
                    //}
                    //catch (e) {
                    //}

                    $('#panel').html(resp);
                    $.fancybox.close();
                    // $('.' + ItemCode).html(jsonItem.NewStock);
                    // $('#balance_' + ItemCode).html(jsonItem.Balance);
                    // $('#' + ItemCode).val('');
                },
                error: function (data) {
                }
            });
        }
    }
}
function AddToQuoteLoss(nn, qty) {
    // nn as description
    if ($('#Amount').val() == '') {
        $('#Error').html('Please enter the cost price for each ' + nn)
        $('#Error').show();
        $('#Amount').focus();
    }
    else {
        var amt = parseFloat($('#Amount').val()) * Qty
        var itm = nn + "," + Qty + "," + "0" + "," + ItemCode
            $.ajax({
                type: "Post",
                url: '/Loss/newStock',
                dataType: 'html',
                data: {
                    company: Company,
                    Qty: Qty,
                    Reciept: Reciept,
                    price: 0,
                    ItemCode: ItemCode,
                    item: itm,
                    Supplier: Supplier
                },
                success: function (resp) {
                    //try {
                    //    var jsonItem = $.parseJSON(resp);
                    //}
                    //catch (e) {
                    //}

                    $('#panel').html(resp);
                    $.fancybox.close();
                    // $('.' + ItemCode).html(jsonItem.NewStock);
                    // $('#balance_' + ItemCode).html(jsonItem.Balance);
                    // $('#' + ItemCode).val('');
                },
                error: function (data) {
                }
            });
        
    }
}
function AddToTransfer(nn, qty) {
    // nn as description
    if ($('#Amount').val() == '') {
        $('#Error').html('Please enter the cost price for each ' + nn)
        $('#Error').show();
        $('#Amount').focus();
    }
    else {
        var amt = parseFloat($('#Amount').val()) * Qty
        var itm = nn + "," + Qty + "," + "0" + "," + ItemCode
        $.ajax({
            type: "Post",
            url: '/Transfers/newStock',
            dataType: 'html',
            data: {
                company: Company,
                Qty: Qty,
                Reciept: Reciept,
                price: 0,
                ItemCode: ItemCode,
                item: itm,
                Supplier: Supplier
            },
            success: function (resp) {
                //try {
                //    var jsonItem = $.parseJSON(resp);
                //}
                //catch (e) {
                //}

                $('#panel').html(resp);
                $.fancybox.close();
                // $('.' + ItemCode).html(jsonItem.NewStock);
                // $('#balance_' + ItemCode).html(jsonItem.Balance);
                // $('#' + ItemCode).val('');
            },
            error: function (data) {
            }
        });

    }
}
function AddStock(nn,mm) {
    if ($('#Amount').val() == '' ) {
        $('#Error').html('Please enter the cost price for each ' + nn )
        $('#Error').show();
        $('#Amount').focus();
    }
    else {
        var amt = parseFloat($('#Amount').val()) * Qty
        
        if (parseFloat($('#Amount').val()).toFixed(2) == 0.00)
        {
            $('#Error').html('Buying Price can not be 0.00' )
            $('#Error').show();
        }
        if(parseFloat(mm) < parseFloat(amt))
        {
            $('#Error').html('You have surpased your suppliers invoice' )
            $('#Error').show();
        }
      else{
        $.ajax({
            type: "Post",
            url: '/Items/newStock',
            dataType: 'html',
            data: {
                company: Company,
                Qty: Qty,
                Reciept: Reciept,
                price: $('#Amount').val(),
                ItemCode: ItemCode,
                Supplier: Supplier
            },
            success: function (resp) {
                //try {
                //    var jsonItem = $.parseJSON(resp);
                //}
                //catch (e) {
                //}

                $('#panel').html(resp);
                $.fancybox.close();
               // $('.' + ItemCode).html(jsonItem.NewStock);
               // $('#balance_' + ItemCode).html(jsonItem.Balance);
               // $('#' + ItemCode).val('');
            },
            error: function (data) {
            }
        });
       }
    }
}

function CompleteAutho() {
   
            $.ajax({
                type: "Post",
                url: '/Loss/CompleteAuthorizeloss',
                dataType: 'html',
                data: {
                    
                    Reciept: Reciept,
                   
                    },
                success: function (resp) {
                    //try {
                    //    var jsonItem = $.parseJSON(resp);
                    //}
                    //catch (e) {
                    //}

                    $('#panel').html(resp);
                    $.fancybox.close();
                    // $('.' + ItemCode).html(jsonItem.NewStock);
                    // $('#balance_' + ItemCode).html(jsonItem.Balance);
                    // $('#' + ItemCode).val('');
                },
                error: function (data) {
                }
            });
        }
    
function CompleteShrinkage() {

    $.ajax({
        type: "Post",
        url: '/Loss/CompleteShrinkage',
        dataType: 'html',
        data: {

            Reciept: Reciept,

        },
        success: function (resp) {
            //try {
            //    var jsonItem = $.parseJSON(resp);
            //}
            //catch (e) {
            //}

            $('#panel').html(resp);
            $.fancybox.close();
            // $('.' + ItemCode).html(jsonItem.NewStock);
            // $('#balance_' + ItemCode).html(jsonItem.Balance);
            // $('#' + ItemCode).val('');
        },
        error: function (data) {
        }
    });
}
function CompleteTransfer() {

    $.ajax({
        type: "Post",
        url: '/Transfers/CompleteTransfer',
        dataType: 'html',
        data: {

            Reciept: Reciept,

        },
        success: function (resp) {
            //try {
            //    var jsonItem = $.parseJSON(resp);
            //}
            //catch (e) {
            //}

            $('#panel').html(resp);
            $.fancybox.close();
            // $('.' + ItemCode).html(jsonItem.NewStock);
            // $('#balance_' + ItemCode).html(jsonItem.Balance);
            // $('#' + ItemCode).val('');
        },
        error: function (data) {
        }
    });
}

function AddIngridient(nn, mm) {
    if ($('#Amount').val() == '') {
        $('#Error').html('Please enter the unit of measure eg ea for each ' + nn)
        $('#Error').show();
        $('#Amount').focus();
    }
    else {
   
            $.ajax({
                type: "Post",
                url: '/Items/AddRecipeMeasure',
                dataType: 'html',
                data: {
                    company: Company,
                    Qty: Qty,
                    ProductCode: nn,
                    measure: $('#Amount').val(),
                    ItemCode: ItemCode
                },
                success: function (resp) {
                    //try {
                    //    var jsonItem = $.parseJSON(resp);
                    //}
                    //catch (e) {
                    //}

                    $('#panel').html(resp);
                    $.fancybox.close();
                    // $('.' + ItemCode).html(jsonItem.NewStock);
                    // $('#balance_' + ItemCode).html(jsonItem.Balance);
                    // $('#' + ItemCode).val('');
                },
                error: function (data) {
                }
            });
        }
    }

function Zerorise(Id, ItemCode) {
            $.ajax({
                type: "Post",
                url: '/Items/StockOut',
                dataType: 'html',
                data: {
                    company: Company,
                    Id: Id
                },
                success: function (resp) {
                    try {
                        var jsonItem = $.parseJSON(resp);
                        $('.' + ItemCode).html(jsonItem.NewStock);
                        $('#balance_' + ItemCode).html(jsonItem.Balance);
                    }
                    catch (e) {
                        jsonItem = 0;
                    }
                    
                   
                },
                error: function (data) {
                }
            });
} // for zerorising stock 

function ClearOut() {
    $.ajax({
        type: "Post",
        url: '/Items/ClearOut',
        dataType: 'html',
        data: {
            company: Company,
           
        },
        success: function (resp) {
            try {
                var jsonItem = $.parseJSON(resp);
                alert("Database cleared")
            }
            catch (e) {
                jsonItem = 0;
            }
           

        },
        error: function (data) {
        }
    });
} // for zerorising stock 

function GetInvoices(type) {
    $.fancybox.showLoading();
    if ($("#Payee").val() == '' || $('#Amount').val() == '' || $("#type").val() == '') {
        
        $("#fielserror").html('Make sure all top field are filled in first');
    } else {
        $("#fielserror").html('');
        $.ajax({
            type: "Get",
            url: '/Accounts/GetInvoices',
            dataType: 'html',
            data: {
                Company: Company,
                Transaction: $("#type").val(),
                Payee: $("#Payee").val(),
                Amount: $('#Amount').val()
            },
            success: function (resp) {
                $.fancybox.hideLoading();
                $.fancybox(resp, {
                    openEffect: 'none',
                    closeEffect: 'none'
                });
            },
            error: function (data) {
            }
        });
    }
}
//changes tender to base currency
function setTender(amnt, curr, rate) {
    var sp = transactionList.split(',')
    if (sp.length == 1) {
        BaseBalance = parseFloat(parseFloat($('#total').text()) * parseFloat(rate)).toFixed(2);
    }
    Balance = amnt;
    $('#total').text(amnt);
    currency = curr;
    $('#product_lines > tbody  > tr').each(function () {
        var amt = $(this).find(".amnt").text();
        var namnt = (parseFloat(amt) * parseFloat(rate)).toFixed(2);
        $(this).find(".amnt").text(namnt);
    });
}

function GetPartialView(link, data, iscredit, tran) {
    $.fancybox.showLoading();

    var totl = $("#total").text();
    if (Partpayment != undefined && Partpayment > 0) {
        totl = Partpayment;
    }
    var cc = 0;
    $('#product_lines > tbody  > tr').each(function () {
        cc = cc + 1
    });
    if (cc > 0 || data == "password" || data == "Other") {
        product = "Cash";
        IsCredit = iscredit;
        if (data != "total") {
            Transaction = tran;
        }
        setForm(data);
        if (partpaymentTotal > 0 && link != "/Quotations/Tender") {
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
                Currency: currency
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
                    scrolling: false,
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
    else if (data == "Tender") {
        partpaymentTotal = 0;
        // Balance = parseFloat($("#total").text());
    }
    else {
        $("#" + product + "q").focus();
        // Balance = parseFloat($('#Balance').val());
    }
    $.fancybox.hideLoading();
}

function Savesale(data, iscredit, tran, Qid) {
    var tender = "";
    var first = "Y";
    var sp = transactionList.split(',')
    if (sp.length > 1) {
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
    if (BaseBalance == "NaN" || BaseBalance == null || BaseBalance == "undefined") {

        BaseBalance = parseFloat(tender);

    

    if ((first == "Y" && (parseFloat(tender) + parseFloat(partpaymentTotal)) >= parseFloat(BaseBalance)) || (first == "N" && (parseFloat(tender) + parseFloat(partpaymentTotal)) == parseFloat(BaseBalance))) {
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
            url: '/Quotations/Print',
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
                tr: transactionList,
                Qid: Qid
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
                $.fancybox.close();
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

            GetPartialView('/Quotations/Tender', 'Cash', 'N', 'Cash Sale');
        }
        else {
            $(".alert-danger").html("split payments can not give out change. Check Your amount and click enter.");
            $(".alert-danger").show();
        }
    }
    }
}

function Savepayment(data, iscredit, tran, Qid) {
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
function setPayment(data, iscredit, tran, Qid) {
    if (PaymentType == 'SALE') {
        Savesale(data, iscredit, tran, Qid);
    } else {
        Savepayment(data, iscredit, tran, Qid);
    }
}
function saveCurrency() {

    var dat = '';
    $('#currency_lines > tbody  > tr').each(function () {
        var Id = $(this).find(".Id").text();
        var idd = Id.trim();
        //var IsBase = $(this).find("").checked;
        var IsBase = $(this).find("#isbase_" + idd)[0].checked;
        //var IsBase = $("#isbase_" + idd).val();
        var ExchangeRate = $("#exchangerate_" + idd).val();
        //var code = $(this).attr('id');

        //var isbase = text(IsBase);
        //var exRate = text(ExchangeRate);

        if (dat == '') {
            dat = idd + "," + IsBase + "," + ExchangeRate
        }
        else {
            dat = dat + "/" + idd + "," + IsBase + "," + ExchangeRate
        }
    });

    $.ajax({
        type: "Post",
        url: '/Accounts/changeBaseCurrency',
        dataType: 'html',
        data: {
            itemz: dat,
            //tender: tender,
            //receipt: $("#OrderReceipt").val(),
            //change: change.toFixed(2),
            //customer: customerName,
            //creditPeriod: creditPeriod,
            //IsCredit: IsCredit,
            //Discount: Discount,
            //tr: Transaction
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


function CloseCancel() {
    $.fancybox.close();
  
}

function CloseCancelItem(ItemCode) {
    $.fancybox.close();
    $('#' + ItemCode).val('');
}

