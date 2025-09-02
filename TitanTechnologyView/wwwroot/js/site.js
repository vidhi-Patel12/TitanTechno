$(document).ready(function () {
    // Navbar scroll effect
    $(window).scroll(function () {
        if ($(this).scrollTop() > 50) {
            $('.navbar').addClass('scrolled');
        } else {
            $('.navbar').removeClass('scrolled');
        }
    });

    // Smooth scrolling for anchor links
    $('a[href*="#"]').on('click', function (e) {
        e.preventDefault();

        $('html, body').animate({
            scrollTop: $($(this).attr('href')).offset().top - 80
        }, 500);
    });

    // Dropdown hover effect
    $('.dropdown').hover(function () {
        $(this).find('.dropdown-menu').stop(true, true).delay(200).fadeIn(300);
    }, function () {
        $(this).find('.dropdown-menu').stop(true, true).delay(200).fadeOut(300);
    });

    // Form submission
    $('.contact-form').on('submit', function (e) {
        e.preventDefault();

        // Simple form validation
        var name = $(this).find('input[placeholder="Name"]').val();
        var email = $(this).find('input[placeholder="Email"]').val();
        var message = $(this).find('textarea').val();

        if (name && email && message) {
            // Show success message
            alert('Thank you for your message! We will get back to you soon.');
            $(this).trigger('reset');
        } else {
            // Show error message
            alert('Please fill in all required fields.');
        }
    });

    // Newsletter subscription
    $('.newsletter button').on('click', function (e) {
        e.preventDefault();

        var email = $(this).siblings('input').val();

        if (email) {
            // Show success message
            alert('Thank you for subscribing to our newsletter!');
            $(this).siblings('input').val('');
        } else {
            // Show error message
            alert('Please enter your email address.');
        }
    });
});

$(document).ready(function () {
    // Smooth scrolling for anchor links
    $('a[href^="#"]').on('click', function (e) {
        e.preventDefault();
        var target = $(this.getAttribute('href'));
        if (target.length) {
            $('html, body').animate({
                scrollTop: target.offset().top - 80
            }, 800);
        }
    });

    // Solution form submission
    $('.contact-form').on('submit', function (e) {
        e.preventDefault();
        var name = $(this).find('input[placeholder="Name"]').val();
        var email = $(this).find('input[placeholder="Email"]').val();
        var solution = $(this).find('select').val();
        var message = $(this).find('textarea').val();

        if (name && email && solution && message) {
            // Show success message
            alert('Thank you for your interest! Our solutions expert will contact you shortly.');
            $(this).trigger('reset');
        } else {
            // Show error message
            alert('Please fill in all required fields.');
        }
    });
});
