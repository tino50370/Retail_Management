<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

	
<!DOCTYPE HTML>
<html>
	<head>
		<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"/>
		<title> Account  </title>
        <link href="<%=Url.Content("~/Content/themes/ui-lightness/jquery-ui-1.8.6.custom.css") %>" rel="stylesheet"
            type="text/css" />
           <link rel="stylesheet" href="<%=Url.Content("~/Scripts/slickgrid/slick.grid.css ") %>" type="text/css" media="screen" charset="utf-8" />
        <link href="<%=Url.Content("~/Content/themes/base/jquery.ui.datepicker.css ") %>" rel="stylesheet"
            type="text/css" />
		<link rel="stylesheet" href="<%=Url.Content("~/Scripts/slickgrid/examples/examples.css") %>" type="text/css" media="screen" charset="utf-8" />
        <link rel="stylesheet" href="<%=Url.Content("~/Content/tablestyle.css") %>" />


<script type="text/javascript">
    $(document).ready(function () {

     $("#DateCreated").datepicker({ dateFormat: "d M, y" });


     $("#DateLastAccess").datepicker({ dateFormat: "d M, y" }); 

    })
        
        </script>

 <script type="text/javascript">

     function isNumber(n) {
         return !isNaN(parseFloat(n)) && isFinite(n);
     }
      
 </script>

<script type="text/javascript">
    // javasript validation...
    function ValidateAccount() {

        if (($('#AccountNumber').val() == null) || ($('#AccountNumber').val() == '')) {
            alert("Please Enter AccountNumber ");
            $('#AccountNumber').focus();
            return false;
        }
        alert($('AccountType option:selected').val());
        return true;
    }
</script>

<script type="text/javascript">
    //  to save the object
    function CommitAccount() {
        //building the json object
        var JsonAccount = {
            "AccountNumber": $('#AccountNumber').val(),
            "AccountName": $('#AccountName').val(),
            "FirstName": $('#FirstName').val(),
            "Surname": $('#Surname').val(),
            "DateCreated": $('#DateCreated').val(),
            "DateLastAccess": $('#DateLastAccess').val(),
            "LastAccessFrom": $('#LastAccessFrom').val(),
            "Email": $('#Email').val(),
            "ActualBalance": $('#ActualBalance').val(),
            "SuspenseBalance": $('#SuspenseBalance').val(),
            "AvailableBalance": $('#AvailableBalance').val()
        };


        if (ValidateAccount() == true) {
            $.post("Account/Save", { json_str: $.toJSON(JsonAccount) }, onAccountSaveSuceeded);
        }
    }
    function onAccountSaveSuceeded(e) {
        alert(e);
    }

    function DeleteAccount() {
        //building the json object
        var JsonAccount = {
            "AccountNumber": $('#AccountNumber').val(),
            "AccountName": $('#AccountName').val(),
            "Email": $('#Email').val(),
            "ActualBalance": $('#ActualBalance').val(),
            "SuspenseBalance": $('#SuspenseBalance').val(),
            "AvailableBalance": $('#AvailableBalance').val(),
            "FirstName": $('#FirstName').val(),
            "Surname": $('#Surname').val(),
            "DateCreated": $('#DateCreated').val(),
            "DateLastAccess": $('#DateLastAccess').val(),
            "LastAccessFrom": $('#LastAccessFrom').val()
        };


        if (ValidateAccount() == true) {
            $.post("Account/Delete", { json_str: $.toJSON(JsonAccount) }, onAccountDeletedSuceeded);
        }
    }
    function onAccountDeletedSuceeded(e) {
        alert(e);
    }
      
     
        
  </script>  

<script type="text/javascript">

            function loadAccounTypeCombobox() {
                
                    $.post("AccountType/GetCombobox", {}, onGetAccountTypeCombobox);
                
            }
            function onGetAccountTypeCombobox(e) {

                $('#divAccountTypeCombobox').html(e);


            }
        </script>

<script type="text/javascript">
    //  to get the from web service/ controller

    function loadAccount() {
        $.post("Account/GetAccount", {}, onAccountGetSuceeded);
    }

    function onAccountGetSuceeded(e) {
        var jsonAccount = $.parseJSON(e);
        $('#AccountNumber').val(jsonAccount.AccountNumber);
        $('#AccountName').val(jsonAccount.AccountName);
        $('#Email').val(jsonAccount.Email);
        $('#ActualBalance').val(jsonAccount.ActualBalance);
        $('#SuspenseBalance').val(jsonAccount.SuspenseBalance);
        $('#AvailableBalance').val(jsonAccount.AvailableBalance);
        $('#FirstName').val(jsonAccount.FirstName);
        $('#Surname').val(jsonAccount.Surname);
        $('#DateCreated').val(jsonAccount.DateCreated);
        $('#DateLastAccess').val(jsonAccount.DateLastAccess);
        $('#LastAccessFrom').val(jsonAccount.LastAccessFrom);
      
    }

        
  </script>  


        <script type="text/javascript">
            $(document).ready(function () {

                loadAccounTypeCombobox();

            })
        
        </script>

  <script type="text/javascript">
      function SaveAccountDetails() {
          $(function () {
     
              // a workaround for a flaw in the demo system (http://dev.jqueryui.com/ticket/4375), ignore!
              $("#dialog:ui-dialog").dialog("destroy");

              $("#dialog-confirm").dialog({
                  resizable: false,
                  height: 250,
                  modal: true,
                  buttons: {
                      "Okay": function () {

                          CommitAccount();
                          $(this).dialog("close");
                      },
                      Cancel: function () {
                          $(this).dialog("close");
                      }
                  }
              });
          });
      };
	</script>
            
    <script type="text/javascript">
           function DeleteAccountDetails() {
                 $(function () {
                   // a workaround for a flaw in the demo system (http://dev.jqueryui.com/ticket/4375), ignore!
                         $("#dialog:ui-dialog").dialog("destroy");
                         $("#dialog-confirmdelete").dialog({
                             resizable: false,
                             height: 250,
                             modal: true,
                             buttons: {
                                 "Okay": function () {

                                     DeleteAccount();
                                     $(this).dialog("close");
                                 },
                                 Cancel: function () {
                                     $(this).dialog("close");
                                 }
                             }
                         });
                     });
                 };
	</script>

	</head>
	<body>
    <div style="width:100%; height:20px ; background-color:#b9c9fe">
				<label style=" margin-left:45%; font-size: x-large; color:#039;">Yo!Money</label>&nbsp;	
			</div>
		<div style="width:60%;float:left;">
        
			<div class="grid-header" style="width:100%; margin-top:40px;; background-color:#b9c9fe">
				<label style="color:#039">Account Details</label>
				<span style="float:right;display:inline-block;">
					Search:
					<input type="text" id="txtSearch" />
				</span>
			</div>
			<div id="myGrid" style="width:100%;height:600px;"></div>
			<div id="pager" style="width:100%;height:20px;"></div>
		</div>
        
        <div style="margin-left:65%;margin-top:40px;">
   
        <table id="gradient-style">
        <th>Account Details:</th>
        <tbody>
        <tr>
        <td>
            Account Type</td>
          <td>
          <div id="divAccountTypeCombobox">Test Data</div>
            </td>
        </tr>
        <tr>
        <td>
        <label>AccountNumber</label>
        </td>
          <td>
        <input type="text" class="input_text" name="AccountNumber" id="AccountNumber"/>
        </td>
        </tr>
        <tr>
        <td>
        <label>AccountName</label>
        </td>
          <td>
        <input type="text" class="input_text" name="AccountName" id="AccountName"/>
        </td>
        </tr>
        <tr>
        <td>
        <label>FirstName</label>
        </td>
          <td>
        <input type="text" class="input_text" name="FirstName" id="FirstName"/>
        </td>
        </tr>
        <tr>
        <td>
        <label>Surname</label>
        </td>
          <td>
        <input type="text" class="input_text" name="Surname" id="Surname"/>
        </td>
        </tr>
        <tr>
        <td>
        <label>DateCreated</label>
        </td>
          <td>
        <input type="text" class="input_text" name="DateCreated" id="DateCreated"/>
        </td>
        </tr>
        <tr>
        <td>
        <label>DateLastAccess</label>
        </td>
          <td>
        <input type="text" class="input_text" name="DateLastAccess" id="DateLastAccess"/>
        </td>
        </tr>
        <tr>
        <td>
        <label>LastAccessFrom</label>
        </td>
          <td>
        <input type="text" class="input_text" name="LastAccessFrom" id="LastAccessFrom"/>
        </td>
        </tr>
        <tr>
        <td>
        <label>Email</label>
        </td>
          <td>
        <input type="text" class="input_text" name="Email" id="Email"/>
        </td>
        </tr>
        <tr>
        <td>
        <label>ActualBalance</label>
        </td>
          <td>
        <input type="text" class="input_text" name="ActualBalance" id="ActualBalance"/>
        </td>
        </tr>
        <tr>
        <td>
        <label>SuspenseBalance</label>
        </td>
          <td>
        <input type="text" class="input_text" name="SuspenseBalance" id="SuspenseBalance"/>
        </td>
        </tr>
        <tr>
        <td>
        <label>AvailableBalance</label>
        </td>
          <td>
        <input type="text" class="input_text" name="AvailableBalance" id="AvailableBalance"/>
        </td>
        </tr>
        </tbody>
        <th>
        <input type= "button" value="Save" onclick="SaveAccountDetails()" />
        <input type= "button" value="Delete" onclick="DeleteAccountDetails()" /></th>
        </table>
        
		</div> 
       

		<script type="text/javascript">
		    var grid;
		    var data = [];
		    var loader = new Slick.Data.RemoteModel('../../Account/GetAll');

		    //please edit formater here ...
		    var storyTitleFormatter = function (row, cell, value, columnDef, dataContext) {
		        return "<b><a href='" + dataContext["BusTel"] + "' target=_blank>" + dataContext["Surname"] + "</a></b><br/>" + dataContext["FNames"];
		    };

		    var dateFormatter = function (row, cell, value, columnDef, dataContext) {

		        var vdate = new Date(parseInt(value.substr(6)));

		        return vdate;
		    };

		    var columns = [

     { id: "AccountNumber", name: "AccountNumber", field: "AccountNumber", width: 100, sortable: true },
     { id: "AccountName", name: "AccountName", field: "AccountName", width: 140, sortable: true },
       { id: "Email", name: "Email", field: "Email", width: 80, sortable: true },
     { id: "ActualBalance", name: "ActualBalance", field: "ActualBalance", width: 80, sortable: true },
     { id: "SuspenseBalance", name: "SuspenseBalance", field: "SuspenseBalance", width: 80, sortable: true },
     { id: "AvailableBalance", name: "AvailableBalance", field: "AvailableBalance", width: 80, sortable: true },
     { id: "FirstName", name: "FirstName", field: "FirstName", width: 140, sortable: true },
     { id: "Surname", name: "Surname", field: "Surname", width: 140, sortable: true },
     { id: "DateCreated", name: "DateCreated", field: "DateCreated", width: 80, sortable: true },
     { id: "DateLastAccess", name: "DateLastAccess", field: "DateLastAccess", width: 80, sortable: true },
     { id: "LastAccessFrom", name: "LastAccessFrom", field: "LastAccessFrom", width: 140, sortable: true }
   

		];

		    var options = {
		        //rowHeight: 64,
		        editable: false,
		        enableAddRow: false,
		        enableCellNavigation: false
		    };

		    var loadingIndicator = null;



		    $(function () {
		        grid = new Slick.Grid("#myGrid", loader.data, columns, options);

		        grid.onViewportChanged.subscribe(function (e, args) {
		            var vp = grid.getViewport();
		            loader.ensureData(vp.top, vp.bottom);
		        });

		        grid.onSort.subscribe(function (e, args) {
		            loader.setSort(args.sortCol.field, args.sortAsc ? 1 : -1);
		            var vp = grid.getViewport();
		            loader.ensureData(vp.top, vp.bottom);
		        });

		        grid.onClick.subscribe(function (e) {
		            var cell = grid.getCellFromEvent(e);
		            var i = cell.row;
		            $('#AccountNumber').val(loader.data[i].AccountNumber);
		            $('#AccountName').val(loader.data[i].AccountName);
		            $('#FirstName').val(loader.data[i].FirstName);
		            $('#Surname').val(loader.data[i].Surname);
		            var vDateCreated = new Date(parseInt(loader.data[i].DateCreated.substr(6)));
		            $('#DateCreated').val(vDateCreated);

		            var vDateLastAccess = new Date(parseInt(loader.data[i].DateLastAccess.substr(6)));
		            $('#DateLastAccess').val(vDateLastAccess);
		            $('#LastAccessFrom').val(loader.data[i].LastAccessFrom);
		            $('#Email').val(loader.data[i].Email);
		            $('#ActualBalance').val(loader.data[i].ActualBalance);
		            $('#SuspenseBalance').val(loader.data[i].SuspenseBalance);
		            $('#AvailableBalance').val(loader.data[i].AvailableBalance);
		        });

		        loader.onDataLoading.subscribe(function () {
		            if (!loadingIndicator) {
		                loadingIndicator = $("<span class='loading-indicator'><label>Buffering...</label></span>").appendTo(document.body);
		                var $g = $("#myGrid");

		                loadingIndicator
						.css("position", "absolute")
						.css("top", $g.position().top + $g.height() / 2 - loadingIndicator.height() / 2)
						.css("left", $g.position().left + $g.width() / 2 - loadingIndicator.width() / 2)
		            }

		            loadingIndicator.show();
		        });

		        loader.onDataLoaded.subscribe(function (e, args) {
		            for (var i = args.from; i <= args.to; i++) {
		                grid.invalidateRow(i);
		            }

		            grid.updateRowCount();
		            grid.render();

		            loadingIndicator.fadeOut();
		        });

		        $("#txtSearch").keyup(function (e) {
		            if (e.which == 13) {

		                loader.setSearch($(this).val());
		                loader.setUrl('Account/GetByName');
		                var vp = grid.getViewport();
		                loader.ensureData(vp.top, vp.bottom);
		            }
		        });



		        // load the first page
		        grid.onViewportChanged.notify();
		    })
		
        </script>    
     <div id="dialog-confirm" title="Save  Account  ?" >
        <p><span class="ui-icon ui-icon-alert" style="float:left; margin:0 7px 20px 0;"></span>You are about to save  Account -. Are you sure?</p>
        </div>
        
         <div id="dialog-confirmdelete" title="Delete   Account ?" >
        <p><span class="ui-icon ui-icon-alert" style="float:left; margin:0 7px 20px 0;"></span>You are about to delete  Account -. Are you sure?</p>
        </div>
	</body>

</html>
