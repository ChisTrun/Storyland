const directTo = (url) => {
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
    const isHide = $(dropdownContent).data('hidden');
    $(dropdownContent).data('hidden', !isHide);
    if (isHide) {
        $.ajax({
            url: '/category/{{serverIndex}}/all',
            method: 'GET',
            success: function (data) {
                dropdownList.empty();
                data.map(e => {
                    dropdownList.append(`
                    <a href="/category/{{serverIndex}}/${e.name}?id=${e.id}" class="col-2 my-2" type="button">${e.name}</a>
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

const upBtn = $("#up-btn");
$(document).on("scroll", () => {
    $(window).scrollTop() > 100 ? upBtn.fadeIn() : upBtn.fadeOut();
});
upBtn.on("click", () => {
    return $("body,html").animate({
        scrollTop: 0
    }, 800), !1;
})

const content = $('.reading-font');
let curFontSize = content.css('font-size');
$('#font-size-display').html(parseFloat(curFontSize));
let curLineHeight = content.css('line-height');
$('#row-spacing-display').html(parseFloat(curLineHeight));

function togglePanel(id) {
    const panel = $(id);
    var panels = $('#chapters-list-panel, #text-format-panel');
    if (panel.css('display') === "none") {
        panels.css('display', "none");
        panel.css('display', "block");
    } else {
        panel.css('display', "none");
    }
}

function changeFont(font, button) {
    const btns = $('.text-format-button');
    btns.removeClass('active');

    console.log(content);
    content.css("font-family", font);
    $(button).addClass('active');
}

function changeColor(color, button) {
    const btns = $('.color-text-format-button');
    btns.removeClass('active');

    content.css('color', color);
    $(button).addClass('active');
}

function changeFontSize(delta) {
    curFontSize = parseFloat(curFontSize) + delta;

    if (curFontSize < 0) {
        curFontSize = 0;
    }
    $("#font-size-display").text(curFontSize);
    content.css("font-size", `${curFontSize}px`);
}

function changeBackgroundColor(color, button) {
    const btns = $('.background-color-button');
    btns.removeClass('active');

    content.parent().css('background-color', color);
    $(button).addClass('active');
}

function changeLineHeight(delta) {
    curLineHeight = parseFloat(curLineHeight) + delta;
    if (curLineHeight < 0) {
        curLineHeight = 0;
    }
    $('#row-spacing-display').text(curLineHeight);
    content.css('line-height', `${curLineHeight}px`);
}

$(document).on("keydown", function (event) {
    switch (event.key) {
        case "ArrowLeft":
            if ($('.pre-chapter-btn').css('display') != 'none') {
                location.href = $('.pre-chapter-btn').attr('href');
            };
            break;
        case "ArrowRight":
            if ($('.next-chapter-btn').css('display') != 'none') {
                location.href = $('.next-chapter-btn').attr('href');
            };
    };
});

function setChapterPanel(index) {
    const container = $('#chapters-list-panel');
    var targetElement = $(`#chapter-index-${index}`);
    if (targetElement.length) {
        container.scrollTop(targetElement.position().top + container.scrollTop());
    }
}

function deleteHistory(storyId, server) {
    $.ajax({
        url: '/history/del',
        method: 'POST',
        data: {
            "storyId": storyId,
            "server": server
        },
        success: function (data) {
            if (data.success) {
                location.reload();
            }
        }
    });
}