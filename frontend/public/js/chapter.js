const content = $('.reading-font');
let readingFont = {};
const firstAvailContent = chapterContents.find(e => e.chapterContent != null && e.chapterContent != "");

const rgbFromHex = (color) => {
    if (!color.includes('#')) {
        return color;
    }
    let r = 0, g = 0, b = 0;
    if (color.length === 4) {
        r = "0x" + color[1] + color[1];
        g = "0x" + color[2] + color[2];
        b = "0x" + color[3] + color[3];
    } 
    else {
        r = "0x" + color[1] + color[2];
        g = "0x" + color[3] + color[4];
        b = "0x" + color[5] + color[6];
    }
    return `rgb(${+r}, ${+g}, ${+b})`;
};

$(() => {
    if (localStorage.getItem('readingFont') === null) {
        readingFont = {
            fontFamily: "Times New Roman",
            color: "rgb(32, 32, 32)",
            fontSize: 28,
            bgColor: "rgb(255, 255, 255)",
            lineHeight: 42
        }
        content.css("font-family", readingFont.fontFamily);
        content.css("color", readingFont.color);
        content.css("font-size", `${readingFont.fontSize}px`);
        content.parent().css("background-color", readingFont.bgColor);
        content.css("line-height", `${readingFont.lineHeight}px`);
        localStorage.setItem('readingFont', JSON.stringify(readingFont));
    } else {
        readingFont = JSON.parse(localStorage.getItem('readingFont'));      
        content.css('font-family', readingFont.fontFamily);    
        content.css('color', readingFont.color);
        content.css('font-size', `${readingFont.fontSize}px`);
        content.parent().css('background-color', readingFont.bgColor);
        content.css('line-height', `${readingFont.lineHeight}px`);
    }
    $('.font-family-btn').each(function () {
        const style = $(this).attr('data-font');
        if (style == readingFont.fontFamily) {
            $(this).addClass('active');
        } else {
            $(this).removeClass('active');
        }
    });
    $('.color-btn').each(function () {
        const style = $(this).css('background-color');
        if (rgbFromHex(style) == rgbFromHex(readingFont.color)) {
            $(this).addClass('active');
        } else {
            $(this).removeClass('active');
        }
    });
    $('.bg-color-btn').each(function () {
        const style = $(this).css('background-color');
        if (rgbFromHex(style) == rgbFromHex(readingFont.bgColor)) {
            $(this).addClass('active');
        } else {
            $(this).removeClass('active');
        }
    });
    $('#font-size-display').html(readingFont.fontSize);
    $('#row-spacing-display').html(readingFont.lineHeight); 
});

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
    readingFont.fontFamily = font;
    localStorage.setItem('readingFont', JSON.stringify(readingFont));
    content.css('font-family', font);
    $(button).addClass('active');
}

function changeColor(color, button) {
    $('.color-text-format-button').removeClass('active');
    readingFont.color = color;
    localStorage.setItem('readingFont', JSON.stringify(readingFont));
    content.css('color', color);
    $(button).addClass('active');
}

function changeFontSize(delta) {
    readingFont.fontSize = parseFloat(readingFont.fontSize) + delta;
    if (readingFont.fontSize < 1) {
        readingFont.fontSize = 1;
    }
    localStorage.setItem('readingFont', JSON.stringify(readingFont));
    $("#font-size-display").text(readingFont.fontSize);
    content.css("font-size", `${readingFont.fontSize}px`);
}

function changeBackgroundColor(color, button) {
    $('.background-color-button').removeClass('active');
    readingFont.bgColor = color;
    localStorage.setItem('readingFont', JSON.stringify(readingFont));
    content.parent().css('background-color', color);
    $(button).addClass('active');
}

function changeLineHeight(delta) {
    readingFont.lineHeight = parseFloat(readingFont.lineHeight) + delta;
    if (readingFont.lineHeight < 1) {
        readingFont.lineHeight = 1;
    }
    localStorage.setItem('readingFont', JSON.stringify(readingFont));
    $('#row-spacing-display').text(readingFont.lineHeight);
    content.css('line-height', `${readingFont.lineHeight}px`);
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

const loadContent = (chapterServer) => {
    const chapterContent = chapterContents.find(e => e.serverId == chapterServer);
    $('#chapter-title').html(chapterContent.chapterName);
    $(`#btnradio${chapterServer}`).prop("checked", true);
    content.html(chapterContent.chapterContent);
};

loadContent(firstAvailContent.serverId);