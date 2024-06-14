const deleteHistory = (storyId) => {
    $.ajax({
        url: '/history/delete',
        method: 'POST',
        data: {
            "storyId": storyId
        },
        success: function (data) {
            location.reload();
        },
        error: function(xhr, status, error) {
            console.error('Error getting stories history: ', error);
        }
    });
};