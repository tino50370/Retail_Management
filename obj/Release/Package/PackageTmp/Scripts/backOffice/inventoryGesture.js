/// <reference path="C:\Users\Walter\Documents\Visual Studio 2013\Projects\RetailKing - EF\Content/Template/js/main-script.js" />
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
/// <reference path="../../Controllers/HomeController.cs" />

function setForm(data) {
   // alert("ioioj");
    CommitProduct();
    product = data;
   
}

function setParameter(data) {
    action = data;
}

function GetView(url, Id) {
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
                    Reciept = resp;
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

function PrintDiv() {
    // alert("printing");
    var divToPrint = document.getElementById('toprint');
    var output = document.getElementById("ifrOutput").contentWindow;
    output.document.open();
    output.document.write('<html><body>' + divToPrint.innerHTML + '</html>');
    output.document.close();
    output.focus();
    output.print();
    setTimeout(function () { location.reload() }, 6000);
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
   
    $.ajax({
        type: "Get",
        url: '/Asset/Daily',
        dataType: 'html',
        //   data: { json_str: AdId },
        success: function (resp) {
            $('#panel').html(resp);
            $.fancybox.hideLoading();
        },
        error: function (data) {
        }
    });
}

function GetCreate() {
    $.fancybox.showLoading();
    $.ajax({
        type: "Get",
        url: 'Asset/Daily',
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
            $("#" + newId).show()
            $("#" + newId + "_b").show()
            $("#" + newId + "_x").hide()
           
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
    var yy = $("#" + id).find('input:checkbox:first').attr('checked');
   cc = 'False';
    if (yy == 'checked')
    {
        cc = 'True';
    }
    
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

function AppendToSuplier(code,name) {

    GetSupplier(name + "_" + code);
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
                try {
                    var jsonItem = $.parseJSON(resp);
                }
                catch (e) {

                }
                $.fancybox.close();
                $('.' + ItemCode).html(jsonItem.NewStock);
                $('#balance_' + ItemCode).html(jsonItem.Balance);
                $('#' + ItemCode).val('');
            },
            error: function (data) {
            }
        });
       }
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
                    }
                    catch (e) {
                        jsonItem = 0;
                    }
                    $('.' + ItemCode).html(jsonItem.NewStock);
                    $('#balance_' + ItemCode).html(jsonItem.Balance);
                   
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

function CloseCancel() {
    $.fancybox.close();
}