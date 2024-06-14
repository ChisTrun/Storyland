$('#download-btn').on('click', async () => {
    $.ajax({
        url: `/export`,
        method: 'POST',
        data: {
            'storyId': storyId,
            'storyServer': curServer.id,
            'type': $("input[name='ebookRadioDefault']:checked").val()
        },
        success: function (data) {
            if (data == null || data == undefined) return;
            window.open(data.url, '_blank');
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
        const response = await fetch(`${host}/export`)
        const resBody = await response.json()
        const container = $('.ebook-type-container')
        container.empty()
        resBody.forEach((type, index) => {
            container.append(`
            <div class="form-check mt-1">
                <input class="form-check-input" ${index == 0 ? "checked" : ""}  type="radio" name="ebookRadioDefault" value="${type.id}" id="ebookRadioDefault${type.id}">
                <label class="form-check-label" for="ebookRadioDefault${type.id}">Định dạng ${type.name}</label>
            </div>
            `)
        });
    }
};

getExportType();
setInterval(getExportType, 1000);