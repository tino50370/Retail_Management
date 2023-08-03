var cartOn = 'No';
var CurrentPayId = '0';
var sessionid = "";
//----Include-Function----
function include(url){ 
	"use strict";
	document.write('<script src="'+ url + '" type="text/javascript"></script>'); 
}


$(document).ready(function(){
	"use strict";

	var windowWidth = $(window).width();
	var windowHeight = $(window).height();

	/* Quick View */
    quickViewModal($('a.product-hover'));

    //quickViewModal($('a.product-hover'));
    
    /* Functions */
	getMainMenu();// Main nav

	getLogin();// login

	// Search Bar
	GetOrderCount();
	selectBox(); // Select Boxes (Chosen plugin)
	
	backToTop(); // Back to top arrow
	
	backToTopEvent(); // Initialize back to top event
	
	tabsOn(); // Turn Tabs On
	fixNavigation(); // Fix Main Navigation

	accordionsOn(); // Turn Accordions On

	fixGridView(); // Fix Category Grid View
	
	installCarousels(); // Install Owl Carousels
	
	installNoUiSlider(); // Installs noUi Slider
	
	singleProduct(); // Cloud Zoom
	Collections(); // collection points;
	ReceiptCopy();// copy of receipt
	Popup(); // fancybox popup;
	ShoppingList(); // shopping list  points;
	MyLists(); // lists

	//searchBar();//searchbox
	
	setTimeout(function () {
	   
	    dropdownsNavigation(); // DropDowns
	    searchBar();//searchbox
	}, 2000);

	/* Window Scroll */
	$(window).scroll(function(){
		
		backToTop(); // Back to top
				
	});
	
	/* Window Resize */
	$(window).resize(function(){
	
		// Set the new Window Width and Height
		windowWidth = $(window).width();
		windowHeight = $(window).height();
		
		
		fixNavigation(); // Fix Main Navigation
		
		fixGridView(); // Fix Category Grid View
		
		fixRevolutionArrows(); // Fix Revolution Sliders Arrows
		
		
	});
	
	
	
	
	/* Twitter WIdget */
	$('#twitter-widget').tweet({
		modpath: 'twitter/',
		count: 2,
		loading_text: 'loading twitter feed...',
	})
	
	
	
	
	/* Tooltips */
	$('.tooltip-hover').tooltip();
	
	
	
	
	// TinyNav.js 1
	$('#main-navigation>ul').tinyNav({
		active: 'current-item',
		header: 'Navigation',
		indent: '?',
		label: 'Menu'
	});
	
	
	
	
	
	/* Revolutions Slider */
	jQuery('.tp-banner').revolution({
		delay:9000,
		startwidth:1170,
		startheight:500,
		hideThumbs:10,
		navigationType:"none"
	});
	
	
	
	
	/* IOS Slider */
	$('.iosSlider').iosSlider({
		scrollbar: true,
		snapToChildren: true,
		desktopClickDrag: true,
		scrollbarMargin: '5px 40px 0 40px',
		scrollbarBorderRadius: 0,
		scrollbarHeight: '2px',
		navPrevSelector: $('.prevButton'),
		navSelector: $('.nextButton')
	});
	

	
	/* FlexSlider */
	$('.flexslider').flexslider({
		animation: "slide",
		controlNav: false,
		prevText: "",           
		nextText: "",
		start: function(slider){
			$('body').removeClass('loading');
			}
	});
	
	/* Sidebar Flexslider */
	$('.sidebar-flexslider').flexslider({
		animation: "slide",
		controlNav: true,
		directionNav: false,
		prevText: "",           
		nextText: "",
		start: function(slider){
			$('body').removeClass('loading');
			}
	});
	
	
	/* Rating */
	
	// Read Only
	$('.rating.readonly-rating').raty({ 
		readOnly: true,
		path:'js/img',
		score: function() {
			return $(this).attr('data-score');
		}
	 });
	 
	// Rate
	$('.rating.rate').raty({ 
		path:'/js/img',
		score: function() {
			return $(this).attr('data-score');
		}
	});
	
	
	
	
	/* Fix Revolution Slider Arrows */
	function fixRevolutionArrows() {
		
		$('.tp-banner').each(function(){
			
			setTimeout(function(){
			
				var arrow_l = $('.tp-banner').find('.tp-leftarrow');
				var arrow_r = $('.tp-banner').find('.tp-rightarrow');
				
				var tp_height = $('.tp-banner').height();
				var arrow_height = $(arrow_l).height();
				var arrow_top = (tp_height/2)-(arrow_height/2);
				
				
				$(arrow_l).css('top', arrow_top);
				$(arrow_r).css('top', arrow_top);
				
			},1000);
			
		});
	
	}
	
	
	/* Navigation Height Fix */
	function fixNavigation(){
		
		var nav = $('#main-navigation>ul>li');
		$(nav).find('>a').attr('style', '');
		var nav_height = $(nav).height();
		
		$(nav).each(function(){
		
			$(this).find('>a').innerHeight(nav_height);
			
		});
	
	}
	

	/* Product Grid View */
	function fixGridView(){
		
		if($('.grid-view').length>0){
			
			$('.grid-view.product .product-content').attr('style', '');
			
			var product_height = $('.grid-view.product img').height();
			var previous_height = $('.grid-view.product .product-content').innerHeight();
			
			if(previous_height < product_height){
				$('.grid-view.product .product-content').innerHeight(product_height);
			}
		}
		
	}

	
	/* Single Product Page */
	function singleProduct(){
	
		
		/* Product Images Carousel */
		$('#product-carousel').flexslider({
			animation: "slide",
			controlNav: false,
			animationLoop: false,
			directionNav: false,
			slideshow: false,
			itemWidth: 80,
			itemMargin: 0,
			start: function(slider){
			
				setActive($('#product-carousel li:first-child img'));
				slider.find('.right-arrow').click(function(){
					slider.flexAnimate(slider.getTarget("next"));
				});
				
				slider.find('.left-arrow').click(function(){
					slider.flexAnimate(slider.getTarget("prev"));
				});
				
				slider.find('img').click(function(){
					var large = $(this).attr('data-large');
					setActive($(this));
					$('#product-slider img').fadeOut(300, changeImg(large, $('#product-slider img')));
					$('#product-slider a.fullscreen-button').attr('href', large);
				});
				
				function changeImg(large, element){
					var element = element;
					var large = large;
					setTimeout(function(){ startF()},300);
					function startF(){
						element.attr('src', large)
						element.attr('data-large', large)
						element.fadeIn(300);
					}
					
				}
				
				function setActive(el){
					var element = el;
					$('#product-carousel img').removeClass('active-item');
					element.addClass('active-item');
				}
				
			}
			
		});
			
			
		
		/* FullScreen Button */
		$('a.fullscreen-button').click(function(e){
			e.preventDefault();
			var target = $(this).attr('href');
			$('#product-carousel a.fancybox[href="'+target+'"]').trigger('click');
		});
		
		
		/* Cloud Zoom */
		$(".cloud-zoom").imagezoomsl({
			zoomrange: [3, 3]
		});
		
		
		/* FancyBox */
		$(".fancybox").fancybox();
		
		
	}

	/* Set Carousels */
	function installCarousels(){
		
		$('.owl-carousel').each(function(){
		
			/* Max items counting */
			var max_items = $(this).attr('data-max-items');
			var tablet_items = max_items;
			if(max_items > 1){
				tablet_items = max_items - 1;
			}
			var mobile_items = 1;
			
			
			/* Install Owl Carousel */
			$(this).owlCarousel({
				
				items:max_items,
				pagination : false,
				itemsDesktop : [1199,max_items],
				itemsDesktopSmall : [1000,max_items],
				itemsTablet: [920,tablet_items],
				itemsMobile: [560,mobile_items],
			});
		
			
			var owl = $(this).data('owlCarousel');
			
			/* Arrow next */
			$(this).parent().parent().find('.icon-left-dir').click(function(e){
				owl.prev();
			});
			
			/* Arrow previous */
			$(this).parent().parent().find('.icon-right-dir').click(function(e){
				owl.next(); 
			});
			
		});
		
	}

	/* No UI Slider */
	function installNoUiSlider(){
		
		if($('.noUiSlider').length > 0){
		
			var min_val = $('.noUiSlider').attr('data-min-value');
			var max_val = $('.noUiSlider').attr('data-max-value');
			var min_range = $('.noUiSlider').attr('data-min-range');
			var max_range = $('.noUiSlider').attr('data-max-range');
			
			$('.noUiSlider').noUiSlider({
				 range: [min_range,max_range]
				,start: [min_val,max_val]
				,connect: true
				,slide: function(){
					var noui_val = $('.noUiSlider').val();
					$('.price-range-min').text('$'+noui_val[0]);
					$('.price-range-max').text('$'+noui_val[1]);
				}
			});
			
			var noui_val = $('.noUiSlider').val();
			$('.price-range-min').text('$'+noui_val[0]);
			$('.price-range-max').text('$'+noui_val[1]);
		
		}
		
	}	

	/* Product Actions Accordion */
	var productButtons = $('.product-actions').not('.full-width');
	productButtons.find('>span:first-child').addClass('current');
	
	productButtons.find('>span').hover(function(){
		
		$(this).parent().find('>span').removeClass('current');
		$(this).addClass('current');
		
	}, function(){
		
		$(this).removeClass('current');
		
	});
	
	productButtons.hover(function(){
		
	}, function(){
		$(this).find('>span:first-child').addClass('current');
	});
	

    /* NAVIGATION DROPDOWN EFFECTS */ReceiptCopy()
	function dropdownsNavigation(){
		
		var mainNav = $('#main-navigation');
		var mainNavItems = $('#main-navigation>ul>li');
		var sideNavItems = $('.sidebar-box-content>ul>li');
		
		
		mainNav.find('ul.normalAnimation').removeClass('normalAnimation');
		
		
		/* Navigation FadeIn Dropdown Effect */
		mainNavItems.hover(function(){
			
			var dropdown = $(this).find('>ul');
			if(!dropdown.hasClass('animating') && windowWidth>767){
				
				dropdown.css('opacity',0).css('top','150%').show().animate({opacity:1, top:100+'%'},300);
				
			}
			
		}, function(){
			
			var dropdown = $(this).find('>ul');
			if(!dropdown.hasClass('animating')){
				
				dropdown.addClass('animating').animate({opacity:0, top:150+'%'},300, function(){
					$(this).hide().removeClass('animating');;	
				});
				
			}
			
		});
		
		
		
		/* Navigation SlideDown Dropdown Effect */
		sideNavItems.hover(function(){
			
			var dropdown = $(this).find('>ul');
			if(!dropdown.hasClass('animating') && windowWidth>767){
				
				dropdown.hide().fadeIn(200);
			}
			
		}, function(){
			
			var dropdown = $(this).find('>ul');
			if(!dropdown.hasClass('animating')){
				$(this).find('>ul').addClass('animating').show().fadeOut(100, function(){
					$(this).removeClass('animating');	
				});
			}
			
		});
		
		
		
		
		
		
		/* Navigation Fadein Dropdown Effect 2 */
		mainNav.find('ul.normal-dropdown>li').hover(function(){
			
			var dropdown = $(this).find('>ul');
			if(!dropdown.hasClass('animating') && windowWidth>767){
				
				dropdown.hide().fadeIn(200);
			}
			
		}, function(){
			
			var dropdown = $(this).find('>ul');
			if(!dropdown.hasClass('animating')){
				$(this).find('>ul').addClass('animating').show().fadeOut(100, function(){
					$(this).removeClass('animating');	
				});
			}
			
		});	
		
		
	}
	
	function getMainMenu() {
	    $.ajax({
	        type: "GET",
	        url: '/Home/eShopMenu',
            dataType: 'html',
            data: {
                SessionId: localStorage.getItem('SessionId')
            },
	        success: function (resp) {
	            $('#main-navigation').html(resp);
	        },
	        error: function (data) {
	        }
	    });
	}

	function getLogin() {
	    $.ajax({
	        type: "GET",
	        url: '/Account/Login',
	        dataType: 'html',
	        success: function (resp) {
	            try {
	              var d =  $.parseJSON(resp)
	              $('#loginform').html(d);
	            }
	            catch (error) {
	                $('#loginform').html(resp);
	            }
	        },
	        error: function (data) {
	        }
	    });
	}

	/* Search BAR */
	function searchBar(){
		
	
		/* SearchBar Fadein Effect */
		var searchButton, searchBar;
		
		searchButton = $('.nav-search');
		searchBar = $('#search-bar');
		
		searchButton.click(function(){
			
			if(searchBar.hasClass('searchbar-visible')){
				searchButton.removeClass('searchbar-visible');
				searchBar.animate({opacity:0, left:-200, right:200},200, function(){
					$(this).removeClass('searchbar-visible').hide();
				});
			}else{
				searchButton.addClass('searchbar-visible');
				searchBar.css('opacity', 0).css('left', -200).css('right',200).show().animate({opacity:1, left:0, right:1},300, function(){
					$(this).addClass('searchbar-visible');
					var config = {
						'.chosen-select-search' : {disable_search_threshold:10, width:'100%'}
					}
					for (var selector in config) {
					  $(selector).chosen(config[selector]);
					}
				});
			}
			
		});

		$('#searchText').on('change keyup paste mouseup', function (e) {
		
		   var s = $('#searchText').val();
		   // clearTimeout(typingTimer);
		    var typingTimer = setTimeout(function () {
		        if ($('#searchText').val() == s && $('#searchText').val() != "" && s.length > 2) {
		            $.fancybox.showLoading();
		            $.ajax({
		                type: "GET",
		                url: '/pos/getItems',
		                dataType: "html",
		                data: { json_str: $("#searchText").val() },
		                success: function (data) {
		                    $.fancybox.hideLoading();
		                    $('#product_lines').html(data);
		                }
		            });
		        }
		        else {
		            $('#product_lines').html('');
		        }
		    }, 1000);

		});
	}

	/* Back to top button */
	function backToTop(){
		
		var button = $('#back-to-top');
		var w_scroll = $(window).scrollTop();
		
		if(w_scroll > 150  && windowWidth>767){
			button.fadeIn(400);	
		}else{
			button.fadeOut(400);	
		}
		
	}
	
	/* Back to top button event */
	function backToTopEvent(){
		
		var button = $('#back-to-top');
		
		button.click(function(){
			
			$('html,body').animate({scrollTop:0}, 600);
			
		});
		
	}
		
	/* Tabs */
	function tabsOn(){
		
		$('.tabs').each(function(){
			
			var tab = $(this);
			tab.find('.tab-content > div').hide();
			tab.find('.tab-heading>a:first-child').addClass('active');
			tab.find('.tab-content > div:first-child').show();
			
			var tabMenuItems = tab.find('.tab-heading>a');
			tabMenuItems.click(function(e){
				
				e.preventDefault();
				
				var target = $(this).attr('href');
				
				tab.find('.tab-content > div').hide();
				tab.find('.tab-heading>a').removeClass('active');
				
				$(this).addClass('active');
				tab.find(target).show();
				
			});
			
		});
			
	}
	
	/* Accordions */
	function accordionsOn(){
		
		$('.accordion').each(function(){
			
			var accordion = $(this);
			accordion.find('.accordion-content').hide();
			accordion.find('>ul>li:first-child').addClass('accordion-active').find('.accordion-content').show();
			accordion.find('.accordion-header').click(function(){
				
				if($(this).parent().hasClass('accordion-active')){
					$(this).parent().removeClass('accordion-active');
					$(this).parent().find('.accordion-content').slideUp(300);
				}else{
					$(this).parent().parent().find('li.accordion-active').removeClass('accordion-active').find('.accordion-content').slideUp(300);
					$(this).parent().addClass('accordion-active').find('.accordion-content').slideDown(300)
				}
				
			});
			
		});
		
	}
	
	/* Select Box Styles */
	function selectBox(){
	
		var config = {
		  '.chosen-select'           : {disable_search_threshold:10},
		  '.chosen-select-full-width'           : {disable_search_threshold:10, width:'100%'},
		  '.chosen-select-search' : {disable_search_threshold:10, width:'100%'}
		}
		for (var selector in config) {
		  $(selector).chosen(config[selector]);
		}
		
	}
	
	/* quick View */
	function quickViewModal(el){
	   
		$('body').append('<div id="quick-view-modal"><div id="quick-view-content"><div id="quick-view-close"></div><div class="quick-view-content"><div class="quick-view-container col-lg-12 col-md-12 col-sm-12"></div></div></div></div>');
		$('#quick-view-modal').hide();
		
		$('#quick-view-close').click(function(){
			
			$('#quick-view-modal').fadeOut(300);
			
		});
		
		/* Scroll Bar */
		$('.quick-view-content').perfectScrollbar({wheelSpeed: 40, suppressScrollX:true});
		
		var elements = el;
		elements.click(function(e){
		
			e.preventDefault();
			var target = $(this).attr('href');
			
			$('#quick-view-content .quick-view-container').load(target+' #product-single', function(){
				
				
				/* Rating Box */
				$('#quick-view-modal .rating.readonly-rating').raty({ readOnly: true, path:'js/img',score: function() {
					return $(this).attr('data-score');
				}});
				$('#quick-view-modal .rating.rate').raty({ path:'js/img',score: function() {
					return $(this).attr('data-score');
				}});
				
				
				/* Accordions */
				var productButtons = $('#quick-view-content .product-actions').not('.full-width');
				productButtons.find('>span:first-child').addClass('current');
				
				productButtons.find('>span').hover(function(){
					
					$(this).parent().find('>span').removeClass('current');
					$(this).addClass('current');
					
				}, function(){
					
					$(this).removeClass('current');
					
				});
				
				productButtons.hover(function(){
					
				}, function(){
					$(this).find('>span:first-child').addClass('current');
				});
				
				
				/* Tabs */
				tabsOn();
				
				/* Numeric Input */
				$('#quick-view-modal .numeric-input').each(function(){
		
					var el = $(this);
					numericInput(el);
					
				});
				
				/* Char Counter */
				$('#quick-view-modal .char-counter').each(function(){
					
					var el = $(this);
					charCounter(el);
					
				});
				
				
				var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
				po.src = 'https://apis.google.com/js/platform.js';
				var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);
				
				
				/* Product Carousel */
				$('#quick-view-modal #product-carousel').flexslider({
					animation: "slide",
					controlNav: false,
					animationLoop: false,
					directionNav: false,
					slideshow: false,
					itemWidth: 80,
					itemMargin: 0,
					start: function(slider){
							setActive($('#product-carousel li:first-child img'));
							slider.find('.right-arrow').click(function(){
								slider.flexAnimate(slider.getTarget("next"));
							});
							slider.find('.left-arrow').click(function(){
								slider.flexAnimate(slider.getTarget("prev"));
							});
							slider.find('img').click(function(){
								var large = $(this).attr('data-large');
								setActive($(this));
								$('#product-slider img').fadeOut(300, changeImg(large, $('#product-slider img')));
							});
							function changeImg(large, element){
								var element = element;
								var large = large;
								setTimeout(function(){ startF()},300);
								function startF(){
									element.attr('src', large)
									element.attr('data-large', large)
									element.fadeIn(300);
								}
								
							}
							function setActive(el){
								var element = el;
								$('#product-carousel img').removeClass('active-item');
								element.addClass('active-item');
							}
						}
				});
				
				$('#quick-view-modal').fadeIn(300);
				
				
				/* Positioning */
				var q_width = $('#quick-view-content').width();
				var q_height = $('#quick-view-content').height();
				var q_margin = ($(window).height() - q_height)/2;
				
				$('#quick-view-content').css('margin-top',q_margin+'px');
				
				
				/* Cloud Zoom */
				$("#quick-view-modal .cloud-zoom").imagezoomsl({
					 zoomrange: [3, 3]
				  });
				
				$('.quick-view-content').perfectScrollbar('update');
				$('.quick-view-content').css('overflow','hidden');
				
				$('.quick-view-content').click(function(){
					$(this).perfectScrollbar('update');
				});
				
				/* Select Box */
				var config = {
					'#quick-view-content .chosen-select' : {disable_search_threshold:10}
				}
				for (var selector in config) {
				  $(selector).chosen(config[selector]);
				}
				
			});
			
		});
		
	}
	
	function Collections() {

	    var elements = $('a#Collection');
	    elements.click(function (e) {

	        e.preventDefault();
	        var target = $(this).attr('href');

	        $('#quick-view-content .quick-view-container').load(target + ' #product-single', function () {


	            /* Rating Box */
	            $('#quick-view-modal .rating.readonly-rating').raty({
	                readOnly: true, path: 'js/img', score: function () {
	                    return $(this).attr('data-score');
	                }
	            });
	            $('#quick-view-modal .rating.rate').raty({
	                path: 'js/img', score: function () {
	                    return $(this).attr('data-score');
	                }
	            });


	            /* Accordions */
	            var productButtons = $('#quick-view-content .product-actions').not('.full-width');
	            productButtons.find('>span:first-child').addClass('current');

	            productButtons.find('>span').hover(function () {

	                $(this).parent().find('>span').removeClass('current');
	                $(this).addClass('current');

	            }, function () {

	                $(this).removeClass('current');

	            });

	            productButtons.hover(function () {

	            }, function () {
	                $(this).find('>span:first-child').addClass('current');
	            });


	            /* Tabs */
	            tabsOn();

	            /* Numeric Input */
	            $('#quick-view-modal .numeric-input').each(function () {

	                var el = $(this);
	                numericInput(el);

	            });

	            /* Char Counter */
	            $('#quick-view-modal .char-counter').each(function () {

	                var el = $(this);
	                charCounter(el);

	            });


	            var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
	            po.src = 'https://apis.google.com/js/platform.js';
	            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);


	            /* Product Carousel */
	            $('#quick-view-modal #product-carousel').flexslider({
	                animation: "slide",
	                controlNav: false,
	                animationLoop: false,
	                directionNav: false,
	                slideshow: false,
	                itemWidth: 80,
	                itemMargin: 0,
	                start: function (slider) {
	                    setActive($('#product-carousel li:first-child img'));
	                    slider.find('.right-arrow').click(function () {
	                        slider.flexAnimate(slider.getTarget("next"));
	                    });
	                    slider.find('.left-arrow').click(function () {
	                        slider.flexAnimate(slider.getTarget("prev"));
	                    });
	                    slider.find('img').click(function () {
	                        var large = $(this).attr('data-large');
	                        setActive($(this));
	                        $('#product-slider img').fadeOut(300, changeImg(large, $('#product-slider img')));
	                    });
	                    function changeImg(large, element) {
	                        var element = element;
	                        var large = large;
	                        setTimeout(function () { startF() }, 300);
	                        function startF() {
	                            element.attr('src', large)
	                            element.attr('data-large', large)
	                            element.fadeIn(300);
	                        }

	                    }
	                    function setActive(el) {
	                        var element = el;
	                        $('#product-carousel img').removeClass('active-item');
	                        element.addClass('active-item');
	                    }
	                }
	            });

	            $('#quick-view-modal').fadeIn(300);


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

	    });

	}

	function ReceiptCopy() {
	   
	    var elements = $('a.PayDetails');
	    elements.click(function (e) {
	       
	        e.preventDefault();
	        var target = $(this).attr('href');

	        $('#quick-view-content .quick-view-container').load(target + ' #product-single', function () {


	            /* Rating Box */
	            $('#quick-view-modal .rating.readonly-rating').raty({
	                readOnly: true, path: 'js/img', score: function () {
	                    return $(this).attr('data-score');
	                }
	            });
	            $('#quick-view-modal .rating.rate').raty({
	                path: 'js/img', score: function () {
	                    return $(this).attr('data-score');
	                }
	            });


	            /* Accordions */
	            var productButtons = $('#quick-view-content .product-actions').not('.full-width');
	            productButtons.find('>span:first-child').addClass('current');

	            productButtons.find('>span').hover(function () {

	                $(this).parent().find('>span').removeClass('current');
	                $(this).addClass('current');

	            }, function () {

	                $(this).removeClass('current');

	            });

	            productButtons.hover(function () {

	            }, function () {
	                $(this).find('>span:first-child').addClass('current');
	            });

	            /* Numeric Input */
	            $('#quick-view-modal .numeric-input').each(function () {

	                var el = $(this);
	                numericInput(el);

	            });

	            /* Char Counter */
	            $('#quick-view-modal .char-counter').each(function () {

	                var el = $(this);
	                charCounter(el);

	            });


	            var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
	            po.src = 'https://apis.google.com/js/platform.js';
	            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);


	            $('#quick-view-modal').fadeIn(300);

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

	    });

	}
	
	function Popup() {

	    var elements = $('.FancyPop');
	    elements.click(function (e) {

	        e.preventDefault();
	        var target = $(this).attr('href');

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

	    });

	}

	function ShoppingList() {

	    var elements = $('a#ShoppingList');
	    elements.click(function (e) {

	        e.preventDefault();
	        var target = $(this).attr('href');

	        $('#quick-view-content .quick-view-container').load(target + ' #product-single', function () {

	            /* Rating Box */
	            $('#quick-view-modal .rating.readonly-rating').raty({
	                readOnly: true, path: 'js/img', score: function () {
	                    return $(this).attr('data-score');
	                }
	            });
	            $('#quick-view-modal .rating.rate').raty({
	                path: 'js/img', score: function () {
	                    return $(this).attr('data-score');
	                }
	            });


	            /* Accordions */
	            var productButtons = $('#quick-view-content .product-actions').not('.full-width');
	            productButtons.find('>span:first-child').addClass('current');

	            productButtons.find('>span').hover(function () {

	                $(this).parent().find('>span').removeClass('current');
	                $(this).addClass('current');

	            }, function () {

	                $(this).removeClass('current');

	            });

	            productButtons.hover(function () {

	            }, function () {
	                $(this).find('>span:first-child').addClass('current');
	            });


	            /* Tabs */
	            tabsOn();

	            /* Numeric Input */
	            $('#quick-view-modal .numeric-input').each(function () {

	                var el = $(this);
	                numericInput(el);

	            });

	            /* Char Counter */
	            $('#quick-view-modal .char-counter').each(function () {

	                var el = $(this);
	                charCounter(el);

	            });


	            var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
	            po.src = 'https://apis.google.com/js/platform.js';
	            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);

	            $('#quick-view-modal').fadeIn(300);


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

	    });

	}

	function MyLists() {

	    var elements = $('a#MyLists');
	    elements.click(function (e) {

	        e.preventDefault();
	        var target = $(this).attr('href');

	        $('#quick-view-content .quick-view-container').load(target + ' #product-single', function () {


	            /* Rating Box */
	            $('#quick-view-modal .rating.readonly-rating').raty({
	                readOnly: true, path: 'js/img', score: function () {
	                    return $(this).attr('data-score');
	                }
	            });
	            $('#quick-view-modal .rating.rate').raty({
	                path: 'js/img', score: function () {
	                    return $(this).attr('data-score');
	                }
	            });


	            /* Accordions */
	            var productButtons = $('#quick-view-content .product-actions').not('.full-width');
	            productButtons.find('>span:first-child').addClass('current');

	            productButtons.find('>span').hover(function () {

	                $(this).parent().find('>span').removeClass('current');
	                $(this).addClass('current');

	            }, function () {

	                $(this).removeClass('current');

	            });

	            productButtons.hover(function () {

	            }, function () {
	                $(this).find('>span:first-child').addClass('current');
	            });


	            /* Tabs */
	            tabsOn();

	            /* Numeric Input */
	            $('#quick-view-modal .numeric-input').each(function () {

	                var el = $(this);
	                numericInput(el);

	            });

	            /* Char Counter */
	            $('#quick-view-modal .char-counter').each(function () {

	                var el = $(this);
	                charCounter(el);

	            });


	            var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
	            po.src = 'https://apis.google.com/js/platform.js';
	            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);


	            $('#quick-view-modal').fadeIn(300);


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

	    });

	}
	
	/* AJAX FORMS */
	
	/* Newsletter */
	
	$('#newsletter').submit(function(e){
		
		e.preventDefault();
		
		var url = $(this).attr('action');
		
		if($(this).find('input[type="text"]').val() == ''){
			
			if($(this).find('span.error').length==0){
				$(this).append('<span class="error">Please, fill in your email address</span>');
			}else{
				$(this).find('span.error').text('Please, fill in your email address');
			}
			
		}else{
			
			$(this).find('span.error').hide();
			
			$.ajax({
				type: "POST",
				url: url,
				data: $('#newsletter').serialize(), 
				success: function(data){
				   $('#newsletter').find('>*').slideUp(300);
				   $('#newsletter').append('<span>'+data+'</span>');
				}
			});
		}
		
	});
		
	/* Contact Form */
	
	$('#contact-form').submit(function(e){
		
		e.preventDefault();
		
		var url = $(this).attr('action');
		var error = false;
		
		if($(this).find('input[name="contact-name"]').val()==''){
			error = true;
		}
		
		if($(this).find('input[name="contact-email"]').val()==''){
			error = true;
		}
		
		if($(this).find('textarea[name="contact-message"]').val()==''){
			error = true;
		}
		
		if(error == true){
			
			if($(this).find('span.error').length==0){
				$(this).append('<span class="error">Please, fill in all the fields</span>');
			}else{
				$(this).find('span.error').text('Please, fill in all the fields');
			}
			
		}
		
		if(error == false){	
			$(this).find('span.error').hide();
			
			$.ajax({
				type: "POST",
				url: url,
				data: $('#contact-form').serialize(), 
				success: function(data){
				   $('#contact-form').find('>*').slideUp(300);
				   $('#contact-form').append('<span>'+data+'</span>');
				}
			});
		}
		
	});

	$('#loginAccountr').submit(function (e) {
	    //alert("tripo");
	    e.preventDefault();

	    var url = $(this).attr('action');
	    var error = false;

	    if ($(this).find('input[name="User"]').val() == '') {
	        error = true;
	    }

	    if ($(this).find('input[name="Password"]').val() == '') {
	        error = true;
	    }

	    if (error == true) {

	        if ($(this).find('span.error').length == 0) {
	            $(this).append('<span class="error">Please, fill in all the fields</span>');
	        } else {
	            $(this).find('span.error').text('Please, fill in all the fields');
	        }

	    }

	    if (error == false) {
	        $(this).find('span.error').hide();

	        $.ajax({
	            type: "POST",
	            url: url,
	            data: $('#loginAccount').serialize(),
	            success: function (data) {
	                try {
	                    var jsonResp = $.parseJSON(resp);
	                    $("#product-single").html(jsonResp);
	                }
	                catch (error) {
	                    $("#product-single").html(data);
	                }
	            }
	        });
	    }

	});
	
	/* Numeric Input */
	$('.numeric-input').each(function(){
	   
	    var el = $(this);
	   
		numericInput(el);
		
	});
	
	/* Numeric Input */
	function numericInput(el){
		
		var element = el;
		var input = $(element).find('input');
		
		$(element).find('.arrow-up').click(function(){
			var value = parseInt(input.val());
			input.val(value+1);
		});
		
		$(element).find('.arrow-down').click(function(){
			var value = parseInt(input.val());
			input.val(value-1);
		});
		
		input.keypress(function(e){
			
			var value = parseInt(String.fromCharCode(e.which));
			if(isNaN(value)){
				e.preventDefault();
			}
			
		});
		
	}
	
	/* Char Counter */
	$('.char-counter').each(function() {	
		var el = $(this);
		charCounter(el);	
	});
	
    /*Registration*/
	$('#NetworkType').change(function (e) {
	    var id = $('#NetworkType').val();
	    $.ajax({
	        type: "GET",
	        url: '/Account/GetLevels',
	        data: {
                Id:id
	        },
	        success: function (resp) {	                     
	                $("#Fee").html(resp);
	                $("#RegFee").show();           
	        }
	    });

	});


	function charCounter(el){
		
		var element = el;
		var counter = $(element).find('input');
		var target = $(counter).attr('data-target');
		
		$(target).bind("change paste keyup",function(){
			
			var value = $(this).val();
			var length = value.length;
			
			counter.val(length);
			
		});
		
	}
});

function GetOrderCount() {

	    $.ajax({
	        type: "Get",
	        url: '/Cart/Orderlist',
	        dataType: 'html',
	        data: {
	            SessionId: localStorage.getItem('SessionId')
	        },
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
            SessionId: localStorage.getItem('SessionId'),
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

function AddToCartList(Id, total) {

    var target = "/Cart/ItemQuantity?Id=" + Id;

    $('#quick-view-content .quick-view-container').load(target + ' #product-single', function () {

        /* Rating Box */
        $('#quick-view-modal .rating.readonly-rating').raty({
            readOnly: true, path: 'js/img', score: function () {
                return $(this).attr('data-score');
            }
        });
        $('#quick-view-modal .rating.rate').raty({
            path: 'js/img', score: function () {
                return $(this).attr('data-score');
            }
        });


        /* Accordions */
        var productButtons = $('#quick-view-content .product-actions').not('.full-width');
        productButtons.find('>span:first-child').addClass('current');

        productButtons.find('>span').hover(function () {

            $(this).parent().find('>span').removeClass('current');
            $(this).addClass('current');

        }, function () {

            $(this).removeClass('current');

        });

        productButtons.hover(function () {

        }, function () {
            $(this).find('>span:first-child').addClass('current');
        });

        var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
        po.src = 'https://apis.google.com/js/platform.js';
        var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);

        $('#quick-view-modal').fadeIn(300);

        /* Positioning */
        var q_width = $('#quick-view-content').width();
        var q_height = $('#quick-view-content').height();
        var q_margin = ($(window).height() - q_height) / 2;

        $('#quick-view-content').css('margin-top', q_margin + 'px');

        $('.quick-view-content').perfectScrollbar('update');
        $('.quick-view-content').css('overflow', 'hidden');

        $('.quick-view-content').click(function () {
            $(this).perfectScrollbar('update');
        });

    });

}

function AddToCartFinal(Id, total,PId) {
    $.ajax({
        type: "Get",
        url: '/Cart/AddToCartQ',
        dataType: 'html',
        data: {
            Id: Id,
            qty: $("#quantity_" + PId).val(),
            total: parseFloat(total) + parseFloat($("#quantity_" + PId).val()),
            SessionId: localStorage.getItem('SessionId'),
            spec: $("#Cpoints_" + PId).val()
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
            SessionId: localStorage.getItem('SessionId'),
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

function NextMonth(itemId,currentDate)
    {
        $.ajax({
            type: "Get",
            url: '/Cart/ItemQuantity',
            dataType: 'html',
            data: {
                id: itemId,
                sEcho: 'NextMonth',
                sStart: currentDate
            },
            success: function (resp) {
                $('#product-single').html(resp);
            },
            error: function (data) {
            }
        });
    //$('#quick-view-modal').fadeOut(100);
   
    }

function NextQuarter(itemId, currentDate)
{
    $.ajax({
        type: "Get",
        url: '/Cart/ItemQuantity',
        dataType: 'html',
        data: {
            id: itemId,
            sEcho: 'NextQuarter',
            sStart: currentDate
        },
        success: function (resp) {
            $('#product-single').html(resp);
        },
        error: function (data) {
        }
    });
  
}

function PrevMonth(itemId,currentDate)
    {
        $.ajax({
            type: "Get",
            url: '/Cart/ItemQuantity',
            dataType: 'html',
            data: {
                id: itemId,
                sEcho: 'PrevMonth',
                sStart: currentDate
            },
            success: function (resp) {
                $('#monthdata').html(resp);
            },
            error: function (data) {
            }
        });
    }

function PrevQuarter(itemId,currentDate)
    {
        $.ajax({
            type: "Get",
            url: '/Cart/ItemQuantity',
            dataType: 'html',
            data: {
                id: itemId,
                sEcho: 'PrevQuarter',
                sStart: currentDate
            },
            success: function (resp) {
                $('#monthdata').html(resp);
            },
            error: function (data) {
            }
        });

    }

function GetChangeAdd() {
    $.ajax({
        type: "Get",
        url: '/Cart/ChangeAddress',
        dataType: 'html',
        data: {
            SessionId: localStorage.getItem('SessionId')
        },
        success: function (resp) {
            //alert("IN");
            $('#CreateAd').html(resp);
            $("#adform").show();
            $("#Adshow").hide();
        },
        error: function (data) {
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
            CollectionId: id,
            OrderId: $("#CartId").html(),
            SessionId: localStorage.getItem('SessionId')
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
            CollectionId: 0,
            SessionId: localStorage.getItem('SessionId')
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
            Id: Id,
            SessionId: localStorage.getItem('SessionId')

        },
        success: function (resp) {
            $('#panel').html(resp);

        },
        error: function (data) {
            $('#quick-view-modal').fadeOut(300);
        }
    });

}

function Payment(OrderId) {
    $.ajax({
        type: "Get",
        url: '/Cart/Payment',
        dataType: 'html',
        data: {
            SessionId: localStorage.getItem('SessionId'),
            OrderId: OrderId
        },
        success: function (resp) {
            $('#panel').html(resp);

        },

    });

}

function Cartitems() {
    $.ajax({
        type: "Get",
        url: '/Cart/Cartitems',
        dataType: 'html',
        data: {            
            SessionId: localStorage.getItem('SessionId')

        },
        success: function (resp) {
            $('#panel').html(resp);

        },
      
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
                SessionId: localStorage.getItem('SessionId'),
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
            total: total,
            SessionId: localStorage.getItem('SessionId'),
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
function EmptyCart() {
    $.ajax({
        type: "Get",
        url: '/Cart/EmptyCart',
        dataType: 'html',
        data: {            
            SessionId: localStorage.getItem('SessionId')
        },
        success: function (resp) {
            $('#panel').html(resp);

        },
        error: function (data) {
            $('#quick-view-modal').fadeOut(300);
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
        url: '/Cart/YomoneyPayment',
        dataType: 'html',
        data: {
            TransactionRef: $("#OdID").html(),
            CustomerMSISDN: $('#v_' + Id).val(),
            ServiceId: Id,
            SessionId: sessionid
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
            Title: id,
            SessionId:localStorage.getItem('SessionId')
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
function GetFancy(url, Id) {
    $.fancybox.showLoading();

    $.ajax({
        type: "Get",
        url: url,
        dataType: 'html',
        data: {
           SessionId: localStorage.getItem('SessionId'),
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

function GetView(url, Id) {
    $.fancybox.showLoading();
    /*if (tinymce.editors.length != 0 && tinymce.activeEditor != undefined) {
         tinymce.remove();
        for (i = 0; i < tinymce.editors.length; i++) {
            tinymce.editors[i].remove();
        }
    }*/
    $.ajax({
        type: "Get",
        url: url,
        dataType: 'html',
        data: {
            SessionId: localStorage.getItem('SessionId'),
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