document.addEventListener("DOMContentLoaded", function () {
    const navItems = document.querySelectorAll(".nav-item");

    navItems.forEach(item => {
        item.addEventListener("click", function () {
            // Remove active class from all nav items
            navItems.forEach(nav => nav.classList.remove("active"));

            // Add active class to the clicked item
            this.classList.add("active");
        });
    });
});
