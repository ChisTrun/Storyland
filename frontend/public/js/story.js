
const getChapters = async () => {
    const chaptersContainer = $('#chapters-container');
    $.ajax({
        url: `/story/${storyId}/s${storyServer}/chapters`,
        method: 'GET',
        success: function (data) {
            chaptersContainer.empty();
            data.map(e => {
                chaptersContainer.append(`
                <a class="truncate" href="/story/${encodeURIComponent(storyId)}/s${storyServer}/chapter/${e.index}" title="${e.name}">${e.name}</a>
                `);
            });
        },
        error: function (xhr, status, error) {
            chaptersContainer.empty();
            console.log('Error getting all chapters of the story: ', error);
        }
    });
};
getChapters();