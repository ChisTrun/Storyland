function deleteHistory(storyId, storyServer) {
    $.ajax({
        url: '/history/delete',
        method: 'POST',
        data: {
            "storyId": storyId,
            "storyServer": storyServer
        },
        success: function (data) {
            if (data.success) {
                location.reload();
            }
        },
        error: function(xhr, status, error) {
            console.log('Error stories history: ', error);
        }
    });
};