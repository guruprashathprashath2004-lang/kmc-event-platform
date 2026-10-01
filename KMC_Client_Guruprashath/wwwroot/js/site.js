// KMC Event Platform - Dusk Trail interactions
(function () {
    // Smooth-scroll for same-page anchor links (e.g. hero "Scroll Down" cue)
    document.querySelectorAll('a[href^="#"]').forEach(function (link) {
        link.addEventListener('click', function (e) {
            var targetId = link.getAttribute('href');
            if (targetId.length > 1) {
                var target = document.querySelector(targetId);
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({ behavior: 'smooth', block: 'start' });
                }
            }
        });
    });

    // Reveal cards/sections as they enter the viewport
    var revealEls = document.querySelectorAll('.reveal-on-scroll');
    if ('IntersectionObserver' in window && revealEls.length) {
        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.12 });

        revealEls.forEach(function (el) { observer.observe(el); });
    } else {
        revealEls.forEach(function (el) { el.classList.add('is-visible'); });
    }

    // Animate the seats-left progress bars from 0 to their target width
    document.querySelectorAll('.seats-bar-fill').forEach(function (bar) {
        var target = bar.getAttribute('data-fill') || '0';
        requestAnimationFrame(function () {
            setTimeout(function () { bar.style.width = target + '%'; }, 150);
        });
    });

    // Live preview for image file inputs (event poster / participant photo)
    document.querySelectorAll('input[type="file"][data-preview]').forEach(function (input) {
        input.addEventListener('change', function () {
            var previewId = input.getAttribute('data-preview');
            var img = document.getElementById(previewId);
            if (img && input.files && input.files[0]) {
                img.src = URL.createObjectURL(input.files[0]);
                img.classList.remove('d-none');
            }
        });
    });
})();
