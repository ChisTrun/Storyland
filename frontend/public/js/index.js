function directTo(url) {
    location.href = url;
};

const GetServer = async () => {
    if (!$('#staticBackdrop').is(':visible')) {
        rsp = await fetch(`${host}/extension/server`)
        data = await rsp.json()
        let modal = $('.server-modal')
        modal.empty()
        data.forEach((server, index) => {
            modal.append(`
            <div class="form-check server-modal">
                <input class="form-check-input" ${index == serverIndex ? "checked" : ""} type="radio" name="flexRadioDefault" value="${server.index}" id="flexRadioDefault1">
                <label class="form-check-label" for="flexRadioDefault1">
                    ${server.name}
                </label>
            </div>
            `)
        });
    }
};

setInterval(GetServer, 100);

let saveButton = $('#save-btn');
saveButton.click(async () => {
    await fetch(`${host}/extension/server/set`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: 'same-origin',
        body: JSON.stringify({
            index: parseInt($('input[name="flexRadioDefault"]:checked').val()),
        })
    })
    location.reload();
});

let categoryDropdown = $('.category-dropdown');
categoryDropdown.on('click', async function () {
    const dropdownContent = $('.dropdown-content');
    const dropdownList = $('.dropdown-list');

    $(this).children('i').toggleClass('fa-rotate-270');

    $(dropdownContent).toggleClass('d-none');

    const hidden = $(dropdownContent).data('hidden');
    $(dropdownContent).data('hidden', !hidden);
    if (hidden === true) {
        $.ajax({
            url: '/category/all',
            method: 'GET',
            success: function (data) {
                dropdownList.empty();
                data.map(e => {
                    dropdownList.append(`
                    <a href="/category/${e.name}/${e.id}" class="col-2 my-2" type="button">${e.name}</a>
                    `);
                });
            }
        });
    }
});

let webMode = $('.web-mode');
webMode.on('click', async function () {
    const doc = $(document.documentElement);
    const theme = doc.attr('data-theme');
    const iconMode = $(this);
    $.ajax({
        url: '/changeDarkMode',
        method: 'POST',
        data: {
            'curMode': theme,
        },
        success: function (data) {
            iconMode.toggleClass('fa-sun');
            iconMode.toggleClass('fa-moon');
            doc.attr('data-theme', theme == 'light' ? 'dark' : 'light');
        },
        error: function (xhr, status, error) {
            console.error("Error changing dark mode: ", error);
        }
    });
});