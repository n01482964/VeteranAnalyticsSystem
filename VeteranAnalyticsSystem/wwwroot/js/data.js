document.addEventListener('DOMContentLoaded', () => {
    var googleForm = document.getElementById('google-form');
    var googleButton = document.getElementById('google-button');

    if (googleForm) {
        googleForm.addEventListener('submit', (e) => {
            e.preventDefault();
            googleButton.innerHTML = '<i class="fa fa-spinner fa-pulse"></i> Loading...';

            e.currentTarget.submit();
        });
    }
});