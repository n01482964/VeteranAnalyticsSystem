document.addEventListener('DOMContentLoaded', () => {
    const loadingState = '<i class="fa fa-spinner fa-pulse"></i> Loading...';

    var googleForm = document.getElementById('google-form');
    var googleButton = document.getElementById('google-button');

    if (googleForm) {
        googleForm.addEventListener('submit', (e) => {
            e.preventDefault();
            googleButton.innerHTML = loadingState;

            e.currentTarget.submit();
        });
    }

    var deleteSurveysButton = document.getElementById('delete-surveys-button');
    var deleteSurveysForm = document.getElementById('delete-surveys-form');

    if (deleteSurveysButton) {
        deleteSurveysForm.addEventListener('submit', (e) => {
            e.preventDefault();
            deleteSurveysButton.innerHTML = loadingState;

            e.currentTarget.submit();
        });
    }
});