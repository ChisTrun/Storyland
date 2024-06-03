function deleteHistory(storyId, storyServer) {
    $.ajax({
        url: '/history/delete',
        method: 'POST',
        data: {
            "storyId": storyId,
            "storyServer": storyServer
        },
        success: function (data) {
            if (data.isSuccess) {
                location.reload();
            }
        },
        error: function(xhr, status, error) {
            console.error('Error stories history: ', error);
        }
    });
};