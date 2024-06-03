const content = $('.reading-font');
let curFontSize = content.css('font-size');
let curLineHeight = content.css('line-height');
$('#font-size-display').html(parseFloat(curFontSize));
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

$('body').on('click', function (event) {
    if ($('#chapters-list-panel').is(':visible') && !$(event.target).closest('.option-button').length && !$(event.target).closest('#chapters-list-panel').length) {
        $('#chapters-list-panel').css('display', "none");
    }
    if ($('#text-format-panel').is(':visible') && !$(event.target).closest('#text-format-panel').length && !$(event.target).closest('.option-button').length) {
        $('#text-format-panel').css('display', "none");
    }
})

function changeFont(font, button) {
    $('.text-format-button').removeClass('active');
    content.css("font-family", font);
    $(button).addClass('active');
}

function changeColor(color, button) {
    $('.color-text-format-button').removeClass('active');
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
    $('.background-color-button').removeClass('active');
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

function setChapterPanel(chapIndex) {
    const container = $('#chapters-list-panel');
    var targetElement = $(`#chapter-index-${chapIndex}`);
    if (targetElement.length) {
        container.scrollTop(targetElement.position().top + container.scrollTop());
    }
}
 
const getContent = async (chapterServer) => {
    content.html('<img src="/assets/loading.gif" class="load-img m-auto text-center"/>');
    $.ajax({
        url: `/story/${encodeURIComponent(storyId)}/${chapterIndex}/content`,
        method: 'POST',
        data: {
            'chapterServer': chapterServer
        },
        success: function (data) {
            content.html(data.content);
        },
        error: function (xhr, status, error) {
            content.html("<div class='text-center'>Nguồn truyện bị lỗi!</div>");
            console.log('Error getting all chapters of the story: ', error);
        }
    });
};

const getChapters = async () => {
    const container = $('#chapters-list-panel');
    $.ajax({
        url: `/story/${encodeURIComponent(storyId)}/s${storyServer}/chapters`,
        method: 'GET',
        success: function (data) {
            container.empty();
            data.map(e => {
                container.append(`
                <a class="chapter-item truncate ${e.index == chapterIndex ? `active-chapter` : ``}" id="chapter-index-${e.index}" title="${e.name}" href="/story/${encodeURIComponent(storyId)}/s${storyServer}/chapter/${e.index}">${e.name}</a>
                `);
            });
        },
        error: function (xhr, status, error) {
            container.empty();
            console.log('Error getting all chapters of the story: ', error);
        }
    });
};
getChapters();