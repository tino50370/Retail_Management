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
        $.ajax({
            type: "Get",
            url: url,
            dataType: 'html',
            data: {
                Company: Company,
                Id: Id
            },
            success: function (resp) {
               
                $("#panel").html(resp);
                $.fancybox.hideLoading();
            },
            error: function (data) {
               // alert(data);
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
            $.fancybox.hideLoading();
        },
        error: function (data) {
        }
    });
}

function SaveView(theForm) {
    var myForm = $('#' + theForm);  
    $.ajax({
        iframe: true,
        url: myForm.attr('action'),
        type: myForm.attr('method'),
        data: myForm.serialize(),
        success: function (resp) {
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
    $.ajax({
        type: "Get",
        url: '/Orders/Details',
        dataType: 'html',
        data: {
            id: id
        },
        success: function (resp) {

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

function PickHamper(id) {
    $.fancybox.showLoading();
    var yy = $("#" + id).find('input:checkbox:first').attr('checked');
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
            chec: cc
        },
        success: function (resp) {
            $.fancybox.hideLoading();
           // $("#Picked").val(resp);

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
            // $("#Picked").val(resp);

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
            // $("#Picked").val(resp);

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
            $("#panel").html(resp);
        },
        error: function (data) {
           // alert(data);
        }
    });
}

function GetSalesSummary() {
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
            $("#Top_menu").html(resp);
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
            $("#side_menu").html(resp);
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

function GetSupplier() { 
   
    $.ajax({
        type: "Get",
        url: '/Items/Purchases',
        dataType: 'html',
        data: {
            Company: Company,
        },
        success: function (resp) {
            $.fancybox(resp, {
                helpers: {
                    overlay: {
                        closeClick: false
                    }
                },
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
    $.ajax({
        type: "Get",
        url: '/Suppliers/Browse',
        dataType: 'html',
        data: {
            Company: Company,
        },
        success: function (resp) {
            $.fancybox(resp, {
                helpers: {
                    overlay: {
                        closeClick: false
                    }
                },
                closeBtn: false,
                openEffect: 'none',
                closeEffect: 'none'
            });
        },
        error: function (data) {
        }
    });
}


function AddStock() {
    if ($('#Amount').val() == '') {
        alert("Please enter the cost price for each " + '@ViewData["ItemName"]');
        $('#Amount').focus();
    }
    else {
        $.ajax({
            type: "Post",
            url: '/Items/newStock',
            dataType: 'html',
            data: {
                company: Company,
                Qty: Qty,
                Reciept: Reciept,
                price: $('#Amount').val(),
                ItemCode:ItemCode
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

function GetInvoices(type) {

    if ($("#Payee").val() == '' || $('#Amount').val() == '' || $("#type").val()=='') {
        $("#fielserror").html('Make sure all top field are filled in first');
    } else {
        $("#fielserror").html('');
        $.ajax({
            type: "Get",
            url: 'Accounts/GetInvoices',
            dataType: 'html',
            data: {
                Company: Company,
                Transaction: $("#type").val(),
                Payee: $("#Payee").val(),
                Amount: $('#Amount').val()
            },
            success: function (resp) {
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