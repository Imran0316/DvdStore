//(function () {
//    const body = document.body;
//    const toggle = document.getElementById('sidebarToggle');

//    if (toggle) {
//        toggle.addEventListener('click', function (e) {
//            e.preventDefault();

//            if (window.innerWidth < 992) {
//                // Mobile: slide overlay
//                body.classList.toggle('sb-sidenav-show');
//            } else {
//                // Desktop: collapse width
//                body.classList.toggle('sb-sidenav-collapsed');
//                localStorage.setItem('sb|sidebar-collapsed',
//                    body.classList.contains('sb-sidenav-collapsed'));
//            }
//        });
//    }

//    // Desktop restore state
//    if (window.innerWidth >= 992 &&
//        localStorage.getItem('sb|sidebar-collapsed') === 'true') {
//        body.classList.add('sb-sidenav-collapsed');
//    }
//})();
