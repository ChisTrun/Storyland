const directTo = (url) => {
    location.href = url;
};

const getServer = async () => {
    if (!$('#staticBackdrop').is(':visible')) {
        rsp = await fetch(`${host}/extension/server`)
        data = await rsp.json()
        let modal = $('.server-modal')
        modal.empty()
        data.forEach((server, index) => {
            modal.append(`
            <div class="form-check server-modal py-1">
                <input class="form-check-input" ${index == serverIndex ? "checked" : ""} type="radio" name="flexRadioDefault" value="${server.index}" id="flexRadioDefault${server.index}">
                <label class="form-check-label" for="flexRadioDefault${server.index}">
                    ${server.name}
                </label>
            </div>
            `)
        });
    }
};
setInterval(getServer, 100);

$('#save-btn').click(async () => {
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
    if (window.location.href.includes('page=')) {
        const href = window.location.href;
        const newHref = href.replace(/([&?]page=\d+)/, (match, text) => {
            return text.startsWith('?') ? '?' : '';
        }).replace(/&{2,}/g, '&')
            .replace(/(\?&)|(&$)/, '');
        location.href = newHref;
    }
    else {
        location.reload();
    }
});

const getCategory = async () => {
    const dropdownList = $('.dropdown-list');
    $.ajax({
        url: '/category/all',
        method: 'GET',
        success: function (data) {
            dropdownList.empty();
            data.map(e => {
                dropdownList.append(`
                <a href="/category/${e.name}?id=${encodeURIComponent(e.id)}" class="col-2 my-2" type="button">${e.name}</a>
                `);
            });
        },
        error: function (xhr, status, error) {
            dropdownList.empty();
            console.log('Error get categories: ', error);
        }
    });
};
getCategory();

const toggleCtgDropdown = () => {
    $('.category-dropdown').children('i').toggleClass('fa-rotate-270');
    $('.dropdown-content').toggleClass('d-none');
}

$('.category-dropdown').on('click', function () {
    toggleCtgDropdown();
});

$('.web-mode').on('click', async function () {
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
            const isDark = data.isDark;
            doc.attr('data-theme', isDark == true ? 'dark' : 'light');
        },
        error: function (xhr, status, error) {
            console.error("Error changing dark mode: ", error);
        }
    });
});

$(document).on("scroll", () => {
    $(window).scrollTop() > 100 ? $("#up-btn").fadeIn() : $("#up-btn").fadeOut();
});

$("#up-btn").on("click", () => {
    return $("body,html").animate({
        scrollTop: 0
    }, 800), !1;
});

$('body').on('click', function (event) {
    if ($('.dropdown-content').is(':visible') && !$(event.target).closest('.dropdown-content').length && !$(event.target).closest('.category-dropdown').length) {
        toggleCtgDropdown();
    }
})