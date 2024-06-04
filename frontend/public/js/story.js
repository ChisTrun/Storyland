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
            console.error('Error getting all chapters of the story: ', error);
        }
    });
};
getChapters();

$('#download-btn').on('click', async () => {
    $.ajax({
        url: `/export`,
        method: 'POST',
        data: {
            'storyId': storyId,
            'storyServer': storyServer,
            'type': $("input[name='ebookRadioDefault']:checked").val()
        },
        success: function (data) {
            if (data == "") return;
            window.open(data, '_blank');
            $('#download-btn').addClass('disabled');
            $('#cancel-btn').addClass('disabled');
            $('.btn-close').addClass('disabled');
            setTimeout(() => {
                $('#ebookModal .modal-content').fadeOut(200, function () {
                    $('#ebookModal').modal('hide');
                    $('#ebookModal .modal-content').show();
                    $('#download-btn').removeClass('disabled');
                    $('#cancel-btn').removeClass('disabled');
                    $('.btn-close').removeClass('disabled');
                });
            }, 2000);
        },
        error: function (xhr, status, error) {
            console.error('Error downloading ebook: ', error);
        }
    });
})

const getExportType = async () => {
    if (!$('#ebookModal').is(':visible')) {
        rsp = await fetch(`${host}/export`)
        data = await rsp.json()
        const container = $('.ebook-type-container')
        container.empty()
        data.forEach((type, index) => {
            container.append(`
            <div class="form-check mt-1">
                <input class="form-check-input" ${index == 0 ? "checked" : ""}  type="radio" name="ebookRadioDefault" value="${type.index}" id="ebookRadioDefault${type.index}">
                <label class="form-check-label" for="ebookRadioDefault${type.index}">${type.name}</label>
            </div>
            `)
        });
    }
};
getExportType();
setInterval(getExportType, 2000);