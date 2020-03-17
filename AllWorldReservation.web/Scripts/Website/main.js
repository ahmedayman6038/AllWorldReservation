 AOS.init({
 	duration: 800,
 	easing: 'slide'
 });

(function($) {

	"use strict";

	var isMobile = {
		Android: function() {
			return navigator.userAgent.match(/Android/i);
		},
			BlackBerry: function() {
			return navigator.userAgent.match(/BlackBerry/i);
		},
			iOS: function() {
			return navigator.userAgent.match(/iPhone|iPad|iPod/i);
		},
			Opera: function() {
			return navigator.userAgent.match(/Opera Mini/i);
		},
			Windows: function() {
			return navigator.userAgent.match(/IEMobile/i);
		},
			any: function() {
			return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows());
		}
	};


	$(window).stellar({
    responsive: true,
    parallaxBackgrounds: true,
    parallaxElements: true,
    horizontalScrolling: false,
    hideDistantElements: false,
    scrollProperty: 'scroll'
  });


	var fullHeight = function() {

		$('.js-fullheight').css('height', $(window).height());
		$(window).resize(function(){
			$('.js-fullheight').css('height', $(window).height());
		});

	};
	fullHeight();

	// loader
	var loader = function() {
		setTimeout(function() { 
			if($('#ftco-loader').length > 0) {
				$('#ftco-loader').removeClass('show');
			}
		}, 1);
	};
	loader();

	// Scrollax
   $.Scrollax();

	var carousel = function() {
		$('.carousel-testimony').owlCarousel({
			center: true,
			loop: true,
			items:1,
			margin: 30,
			stagePadding: 0,
			nav: true,
			navText: ['<span class="ion-ios-arrow-back">', '<span class="ion-ios-arrow-forward">'],
			responsive:{
				0:{
					items: 1
				},
				600:{
					items: 3
				},
				1000:{
					items: 3
				}
			}
		});

		$('.single-slider').owlCarousel({
			animateOut: 'fadeOut',
	    animateIn: 'fadeIn',
			autoplay: true,
			loop: true,
			items:1,
			margin: 0,
			stagePadding: 0,
			nav: true,
			dots: true,
			navText: ['<span class="ion-ios-arrow-back">', '<span class="ion-ios-arrow-forward">'],
			responsive:{
				0:{
					items: 1
				},
				600:{
					items: 1
				},
				1000:{
					items: 1
				}
			}
		});

	};
	carousel();

	$('nav .dropdown').hover(function(){
		var $this = $(this);
		// 	 timer;
		// clearTimeout(timer);
		$this.addClass('show');
		$this.find('> a').attr('aria-expanded', true);
		// $this.find('.dropdown-menu').addClass('animated-fast fadeInUp show');
		$this.find('.dropdown-menu').addClass('show');
	}, function(){
		var $this = $(this);
			// timer;
		// timer = setTimeout(function(){
			$this.removeClass('show');
			$this.find('> a').attr('aria-expanded', false);
			// $this.find('.dropdown-menu').removeClass('animated-fast fadeInUp show');
			$this.find('.dropdown-menu').removeClass('show');
		// }, 100);
	});


	$('#dropdown04').on('show.bs.dropdown', function () {
	  console.log('show');
	});

	// scroll
	var scrollWindow = function() {
		$(window).scroll(function(){
			var $w = $(this),
					st = $w.scrollTop(),
					navbar = $('.ftco_navbar'),
					sd = $('.js-scroll-wrap');

			if (st > 150) {
				if ( !navbar.hasClass('scrolled') ) {
					navbar.addClass('scrolled');	
				}
			} 
			if (st < 150) {
				if ( navbar.hasClass('scrolled') ) {
					navbar.removeClass('scrolled sleep');
				}
			} 
			if ( st > 350 ) {
				if ( !navbar.hasClass('awake') ) {
					navbar.addClass('awake');	
				}
				
				if(sd.length > 0) {
					sd.addClass('sleep');
				}
			}
			if ( st < 350 ) {
				if ( navbar.hasClass('awake') ) {
					navbar.removeClass('awake');
					navbar.addClass('sleep');
				}
				if(sd.length > 0) {
					sd.removeClass('sleep');
				}
			}
		});
	};
	scrollWindow();

	var isMobile = {
		Android: function() {
			return navigator.userAgent.match(/Android/i);
		},
			BlackBerry: function() {
			return navigator.userAgent.match(/BlackBerry/i);
		},
			iOS: function() {
			return navigator.userAgent.match(/iPhone|iPad|iPod/i);
		},
			Opera: function() {
			return navigator.userAgent.match(/Opera Mini/i);
		},
			Windows: function() {
			return navigator.userAgent.match(/IEMobile/i);
		},
			any: function() {
			return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows());
		}
	};

	
	var counter = function() {
		
		$('#section-counter').waypoint( function( direction ) {

			if( direction === 'down' && !$(this.element).hasClass('ftco-animated') ) {

				var comma_separator_number_step = $.animateNumber.numberStepFactories.separator(',')
				$('.number').each(function(){
					var $this = $(this),
						num = $this.data('number');
						console.log(num);
					$this.animateNumber(
					  {
					    number: num,
					    numberStep: comma_separator_number_step
					  }, 7000
					);
				});
				
			}

		} , { offset: '95%' } );

	}
	counter();

	var contentWayPoint = function() {
		var i = 0;
		$('.ftco-animate').waypoint( function( direction ) {

			if( direction === 'down' && !$(this.element).hasClass('ftco-animated') ) {
				
				i++;

				$(this.element).addClass('item-animate');
				setTimeout(function(){

					$('body .ftco-animate.item-animate').each(function(k){
						var el = $(this);
						setTimeout( function () {
							var effect = el.data('animate-effect');
							if ( effect === 'fadeIn') {
								el.addClass('fadeIn ftco-animated');
							} else if ( effect === 'fadeInLeft') {
								el.addClass('fadeInLeft ftco-animated');
							} else if ( effect === 'fadeInRight') {
								el.addClass('fadeInRight ftco-animated');
							} else {
								el.addClass('fadeInUp ftco-animated');
							}
							el.removeClass('item-animate');
						},  k * 50, 'easeInOutExpo' );
					});
					
				}, 100);
				
			}

		} , { offset: '95%' } );
	};
	contentWayPoint();


	// navigation
	var OnePageNav = function() {
		$(".smoothscroll[href^='#'], #ftco-nav ul li a[href^='#']").on('click', function(e) {
		 	e.preventDefault();

		 	var hash = this.hash,
		 			navToggler = $('.navbar-toggler');
		 	$('html, body').animate({
		    scrollTop: $(hash).offset().top
		  }, 700, 'easeInOutExpo', function(){
		    window.location.hash = hash;
		  });


		  if ( navToggler.is(':visible') ) {
		  	navToggler.click();
		  }
		});
		$('body').on('activate.bs.scrollspy', function () {
		  console.log('nice');
		})
	};
	OnePageNav();


	// magnific popup
	$('.image-popup').magnificPopup({
    type: 'image',
    closeOnContentClick: true,
    closeBtnInside: false,
    fixedContentPos: true,
    mainClass: 'mfp-no-margins mfp-with-zoom', // class to remove default margin from left and right side
     gallery: {
      enabled: true,
      navigateByImgClick: true,
      preload: [0,1] // Will preload 0 - before current, and 1 after the current image
    },
    image: {
      verticalFit: true
    },
    zoom: {
      enabled: true,
      duration: 300 // don't foget to change the duration also in CSS
    }
  });

  $('.popup-youtube, .popup-vimeo, .popup-gmaps').magnificPopup({
    disableOn: 700,
    type: 'iframe',
    mainClass: 'mfp-fade',
    removalDelay: 160,
    preloader: false,

    fixedContentPos: false
  });


  $('.checkin_date, .checkout_date, .DOB').datepicker({
	  'format': 'm/d/yyyy',
	  'autoclose': true
	});


    // rooms popup

    // Part 1  // Adults : 16+ years
    var y1 = 2;
    $("#sinolo2 #Input2").attr('value', y1);
    $("#inc2").click(function () {
        $("#Input2").attr('value', ++y1);
        if (x == 1) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
        }
        if (x == 2) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

        }
        if (x == 3) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

        }
    });
    $("#dec2").click(function () {
        if ($("#Input2").val() != 1) {
            $("#Input2").attr('value', --y1);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }
        }
    });


    // Part 1  // Teenagers : 12-15 y1ears
    var z1 = 0;
    $("#sinolo3 #Input3").attr('value', z1);
    $("#inc3").click(function () {
        $("#Input3").attr('value', ++z1);
        if (x == 1) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
        }
        if (x == 2) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

        }
        if (x == 3) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

        }
    });
    $("#dec3").click(function () {
        if ($("#Input3").val() != 0) {
            $("#Input3").attr('value', --z1);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }
        }
    });


    // Part 1 // Children : 2-11 y1ears
    var w1 = 0;
    $("#sinolo4 #Input4").attr('value', w1);
    $("#inc4").click(function () {
        if (w1 != 6) {
            $("#Input4").attr('value', ++w1);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }

            if (w1 == 1) {
                $('#ChildContainer1').append('<div class="row" id="C1"></div><br>');
                $('#C1').append('<div class="col-lg-6 font-weight-bold">Child <span id="a1"> ' + w1 + '</span> Age </div>');
                $('#C1').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');
            }
            if (w1 == 2) {
                $('#ChildContainer1').append('<div class="row" id="C2"></div><br>');
                $('#C2').append('<div class="col-lg-6 font-weight-bold">Child <span id="a2"> ' + w1 + ' </span> Age </div>');
                $('#C2').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }
            if (w1 == 3) {
                $('#ChildContainer1').append('<div class="row" id="C3"></div><br>');
                $('#C3').append('<div class="col-lg-6 font-weight-bold">Child <span id="a3"> ' + w1 + '</span> Age </div>');
                $('#C3').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }
            if (w1 == 4) {
                $('#ChildContainer1').append('<div class="row" id="C4"></div><br>');
                $('#C4').append('<div class="col-lg-6 font-weight-bold">Child <span id="a4"> ' + w1 + ' </span> Age </div>');
                $('#C4').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }
            if (w1 == 5) {
                $('#ChildContainer1').append('<div class="row" id="C5"></div><br>');
                $('#C5').append('<div class="col-lg-6 font-weight-bold">Child <span id="a5">' + w1 + '</span> Age </div>');
                $('#C5').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }
            if (w1 == 6) {
                $('#ChildContainer1').append('<div class="row" id="C6"></div><br>');
                $('#C6').append('<div class="col-lg-6 font-weight-bold">Child <span id="a6">' + w1 + '</span> Age </div>');
                $('#C6').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }

        }
    });
    $("#dec4").click(function () {
        if ($("#Input4").val() != 0) {
            $("#Input4").attr('value', --w1);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }
            if (w1 == 0) {
                $('#C1').remove();
                $('#ChildContainer1').empty();
            }
            if (w1 == 1) {
                $('#C2').remove();
            }
            if (w1 == 2) {
                $('#C3').remove();
            }
            if (w1 == 3) {
                $('#C4').remove();
            }
            if (w1 == 4) {
                $('#C5').remove();
            }
            if (w1 == 5) {
                $('#C6').remove();
            }
        }
    });



    // Part 1  // Inf1ants : 0-23 months
    var f1 = 0;
    $("#sinolo5 #Input5").attr('value', f1);
    $("#inc5").click(function () {
        $("#Input5").attr('value', ++f1);
        if (x == 1) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
        }
        if (x == 2) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

        }
        if (x == 3) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

        }
    });
    $("#dec5").click(function () {
        if ($("#Input5").val() != 0) {
            $("#Input5").attr('value', --f1);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }
        }
    });




    // Part 2  // Adults : 16+ years
    var y2 = 2;
    $("#sinolo6 #Input6").attr('value', y2);
    $("#inc6").click(function () {
        $("#Input6").attr('value', ++y2);
        if (x == 1) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
        }
        if (x == 2) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

        }
        if (x == 3) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

        }
    });
    $("#dec6").click(function () {
        if ($("#Input6").val() != 1) {
            $("#Input6").attr('value', --y2);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }
        }
    });


    // Part 2  // Teenagers : 12-15 y1ears
    var z2 = 0;
    $("#sinolo7 #Input7").attr('value', z2);
    $("#inc7").click(function () {
        $("#Input7").attr('value', ++z2);
        if (x == 1) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
        }
        if (x == 2) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

        }
        if (x == 3) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

        }
    });
    $("#dec7").click(function () {
        if ($("#Input7").val() != 0) {
            $("#Input7").attr('value', --z2);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }
        }
    });


    // Part 2 // Children : 2-11 y1ears
    var w2 = 0;
    $("#sinolo8 #Input8").attr('value', w2);
    $("#inc8").click(function () {
        if (w2 != 6) {
            $("#Input8").attr('value', ++w2);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }

            if (w2 == 1) {
                $('#ChildContainer2').append('<div class="row" id="C7"></div><br>');
                $('#C7').append('<div class="col-lg-6 font-weight-bold">Child <span id="a7"> ' + w2 + '</span> Age </div>');
                $('#C7').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');
            }
            if (w2 == 2) {
                $('#ChildContainer2').append('<div class="row" id="C8"></div><br>');
                $('#C8').append('<div class="col-lg-6 font-weight-bold">Child <span id="a8"> ' + w2 + ' </span> Age </div>');
                $('#C8').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }
            if (w2 == 3) {
                $('#ChildContainer2').append('<div class="row" id="C9"></div><br>');
                $('#C9').append('<div class="col-lg-6 font-weight-bold">Child <span id="a9"> ' + w2 + '</span> Age </div>');
                $('#C9').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }
            if (w2 == 4) {
                $('#ChildContainer2').append('<div class="row" id="C10"></div><br>');
                $('#C10').append('<div class="col-lg-6 font-weight-bold">Child <span id="a10"> ' + w2 + ' </span> Age </div>');
                $('#C10').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }
            if (w2 == 5) {
                $('#ChildContainer2').append('<div class="row" id="C11"></div><br>');
                $('#C11').append('<div class="col-lg-6 font-weight-bold">Child <span id="a11">' + w2 + '</span> Age </div>');
                $('#C11').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }
            if (w2 == 6) {
                $('#ChildContainer2').append('<div class="row" id="C12"></div><br>');
                $('#C12').append('<div class="col-lg-6 font-weight-bold">Child <span id="a12">' + w2 + '</span> Age </div>');
                $('#C12').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }
        }

    });
    $("#dec8").click(function () {
        if ($("#Input8").val() != 0) {
            $("#Input8").attr('value', --w2);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }
            if (w2 == 0) {
                $('#C7').remove();
                $('#ChildContainer2').empty();
            }
            if (w2 == 1) {
                $('#C8').remove();
            }
            if (w2 == 2) {
                $('#C9').remove();
            }
            if (w2 == 3) {
                $('#C10').remove();
            }
            if (w2 == 4) {
                $('#C11').remove();
            }
            if (w2 == 5) {
                $('#C12').remove();
            }
        }
    });



    // Part 2  // Inf1ants : 0-23 months
    var f2 = 0;
    $("#sinolo9 #Input9").attr('value', f2);
    $("#inc9").click(function () {
        $("#Input9").attr('value', ++f2);
        if (x == 1) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
        }
        if (x == 2) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

        }
        if (x == 3) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

        }
    });
    $("#dec9").click(function () {
        if ($("#Input9").val() != 0) {
            $("#Input9").attr('value', --f2);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }
        }
    });





    // Part 3  // Adults : 16+ years
    var y3 = 2;
    $("#sinolo10 #Input10").attr('value', y3);
    $("#inc10").click(function () {
        $("#Input10").attr('value', ++y3);
        if (x == 1) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
        }
        if (x == 2) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

        }
        if (x == 3) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

        }
    });
    $("#dec10").click(function () {
        if ($("#Input10").val() != 1) {
            $("#Input10").attr('value', --y3);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }
        }
    });


    // Part 3  // Teenagers : 12-15 y1ears
    var z3 = 0;
    $("#sinolo11 #Input11").attr('value', z3);
    $("#inc11").click(function () {
        $("#Input11").attr('value', ++z3);
        if (x == 1) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
        }
        if (x == 2) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

        }
        if (x == 3) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

        }
    });
    $("#dec11").click(function () {
        if ($("#Input11").val() != 0) {
            $("#Input11").attr('value', --z3);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }
        }
    });


    // Part 3 // Children : 2-11 y1ears
    var w3 = 0;
    $("#sinolo12 #Input12").attr('value', w3);
    $("#inc12").click(function () {
        if (w3 != 6) {
            $("#Input12").attr('value', ++w3);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }
            if (w3 == 1) {
                $('#ChildContainer3').append('<div class="row" id="C13"></div><br>');
                $('#C13').append('<div class="col-lg-6 font-weight-bold">Child <span id="a13"> ' + w3 + '</span> Age </div>');
                $('#C13').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');
            }
            if (w3 == 2) {
                $('#ChildContainer3').append('<div class="row" id="C14"></div><br>');
                $('#C14').append('<div class="col-lg-6 font-weight-bold">Child <span id="a14"> ' + w3 + ' </span> Age </div>');
                $('#C14').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }
            if (w3 == 3) {
                $('#ChildContainer3').append('<div class="row" id="C15"></div><br>');
                $('#C15').append('<div class="col-lg-6 font-weight-bold">Child <span id="a15"> ' + w3 + '</span> Age </div>');
                $('#C15').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }
            if (w3 == 4) {
                $('#ChildContainer3').append('<div class="row" id="C16"></div><br>');
                $('#C16').append('<div class="col-lg-6 font-weight-bold">Child <span id="a16"> ' + w3 + ' </span> Age </div>');
                $('#C16').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }
            if (w3 == 5) {
                $('#ChildContainer3').append('<div class="row" id="C17"></div><br>');
                $('#C17').append('<div class="col-lg-6 font-weight-bold">Child <span id="a17">' + w3 + '</span> Age </div>');
                $('#C17').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }
            if (w3 == 6) {
                $('#ChildContainer3').append('<div class="row" id="C18"></div><br>');
                $('#C18').append('<div class="col-lg-6 font-weight-bold">Child <span id="a18">' + w3 + '</span> Age </div>');
                $('#C18').append('<div class="col-lg-6"><select class="form-control"><option>Choose</option><option>2</option><option>3</option><option>4</option><option>5</option><option>6</option><option>7</option><option>8</option><option>9</option><option>10</option><option>11</option></select></div>');

            }
        }

    });
    $("#dec12").click(function () {
        if ($("#Input12").val() != 0) {
            $("#Input12").attr('value', --w3);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }
            if (w3 == 0) {
                $('#C13').remove();
                $('#ChildContainer3').empty();
            }
            if (w3 == 1) {
                $('#C14').remove();
            }
            if (w3 == 2) {
                $('#C15').remove();
            }
            if (w3 == 3) {
                $('#C16').remove();
            }
            if (w3 == 4) {
                $('#C17').remove();
            }
            if (w3 == 5) {
                $('#C18').remove();
            }
        }
    });



    // Part 3  // Inf1ants : 0-23 months
    var f3 = 0;
    $("#sinolo13 #Input13").attr('value', f3);
    $("#inc13").click(function () {
        $("#Input13").attr('value', ++f3);
        if (x == 1) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
        }
        if (x == 2) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

        }
        if (x == 3) {
            $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

        }
    });
    $("#dec13").click(function () {
        if ($("#Input13").val() != 0) {
            $("#Input13").attr('value', --f3);
            if (x == 1) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');

            }
        }
    });







    // Rooms
    var x = 1;
    $("#sinolo1 #Input1").attr('value', x);
    $("#inc1").click(function () {
        if ($("#Input1").val() != 3) {
            $("#Input1").attr('value', ++x);
            if (x == 2) {
                $('#B2').css('display', 'block');
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');

            }
            if (x == 3) {
                $('#B3').css('display', 'block');
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2 + y3 + z3 + w3 + f3) + ' Guests - ' + x + ' Rooms');
            }
        }


    });
    $("#dec1").click(function () {
        if ($("#Input1").val() != 1) {
            $("#Input1").attr('value', --x);
            if (x == 1) {
                $('#B2').css('display', 'none');
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1) + ' Guests - ' + x + ' Rooms');
            }
            if (x == 2) {
                $('#B3').css('display', 'none');
                $('#txt').attr('placeholder', ' ' + (y1 + z1 + w1 + f1 + y2 + z2 + w2 + f2) + ' Guests - ' + x + ' Rooms');
            }
        }

    });


    $('#txt').attr('placeholder', ' ' + y1 + ' Guests - ' + x + ' Rooms');

})(jQuery);

