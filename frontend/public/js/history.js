const deleteHistory = (storyId, storyServer) => {
    $.ajax({
        url: '/history/delete',
        method: 'POST',
        data: {
            "storyId": storyId,
            "storyServer": storyServer
        },
        success: function (data) {
            location.reload();
        },
        error: function(xhr, status, error) {
            console.error('Error getting stories history: ', error);
        }
    });
};